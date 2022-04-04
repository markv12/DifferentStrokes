import * as c from '../common/index'

import { File, Storage } from '@google-cloud/storage'
const storage = new Storage()
export const bucketName = `ld50-game`

const perPage = 100

const cachedFiles: {
  cache: UsefulFileData[]
  lastUpdated: number
} = {
  cache: [],
  lastUpdated: 0,
}
const cacheExpirationTime = 1000 * 60 // 1 minute // 1000 * 60 * 60 // 1 hour

export async function getFiles(
  force = false,
): Promise<ResponseOrError<UsefulFileData[]>> {
  if (
    !force &&
    cachedFiles.lastUpdated + cacheExpirationTime >
      Date.now() &&
    cachedFiles.cache.length > 0
  ) {
    console.log(
      `using cached files`,
      cachedFiles.cache.length,
    )
    return cachedFiles.cache
  }

  try {
    let [files]: any = await storage
      .bucket(bucketName)
      .getFiles()
    files = filesToUsefulData(files).sort(
      (a, b) => b.date - a.date,
    )
    cachedFiles.cache = files
    cachedFiles.lastUpdated = Date.now()
    console.log(`cached files`, files.length)
    return files
  } catch (error) {
    cachedFiles.cache = []
    cachedFiles.lastUpdated = Date.now()
    return { error }
  }
}

const topFilesCount = 6
export async function getTopFiles(): Promise<
  UsefulFileData[]
> {
  let filesRes = await getFiles()
  if (`error` in filesRes) return []
  let files = filesRes as UsefulFileData[]
  files = files
    .filter((file) => file.iteration === 2)
    .sort((a, b) => {
      const aLikes = a.likes
      const bLikes = b.likes
      const aDisLikes = a.dislikes
      const bDisLikes = b.dislikes
      const aTotalRatings = aLikes + aDisLikes
      const bTotalRatings = bLikes + bDisLikes
      const aScoreRatio =
        (aLikes / (aDisLikes || 1)) * (aTotalRatings / 100)
      const bScoreRatio =
        (bLikes / (bDisLikes || 1)) * (bTotalRatings / 100)
      const aAgeModifier =
        (a.date - Date.now()) /
        (1000 * 60 * 60 * 24 * 30 * 12)
      const bAgeModifier =
        (b.date - Date.now()) /
        (1000 * 60 * 60 * 24 * 30 * 12)
      return (
        bScoreRatio +
        bAgeModifier -
        (aScoreRatio + aAgeModifier)
      )
    })
    .slice(0, topFilesCount)
  return files
}

export async function getRelevantFiles(
  page: number = 0,
): Promise<{ step1: File[]; step2: File[] }> {
  if (!page) page = 0
  const start = page * perPage

  let filesRes = await getFiles()
  if (`error` in filesRes) return { step1: [], step2: [] }

  let files: UsefulFileData[] = [...filesRes]

  const fileResponse: any = { step1: [], step2: [] }
  try {
    let step1Files = files
      .filter((f) => f.iteration === 1)
      .sort((a, b) => String(b.id).localeCompare(a.id))
      .slice(start, start + perPage)
    // c.log(
    //   `step1Files`,
    //   step1Files,
    //   files.filter((f) => f.iteration === 1),
    //   start,
    //   start + perPage,
    // )
    fileResponse.step1.push(...shuffle(step1Files))
  } catch (error) {
    console.log(error)
  }

  try {
    let step2Files = files.filter((f) => f.iteration === 2)

    // c.log(
    //   `step2Files`,
    //   step2Files,
    //   files.filter((f) => f.iteration === 2),
    // )

    step2Files = step2Files
      .sort((a, b) => {
        const aLikes = a.likes
        const bLikes = b.likes
        const aDisLikes = a.dislikes
        const bDisLikes = b.dislikes
        const aScoreRatio = aLikes / (aDisLikes || 1)
        const bScoreRatio = bLikes / (bDisLikes || 1)
        const aAgeModifier =
          (a.date - Date.now()) /
          (1000 * 60 * 60 * 24 * 30 * 12)
        const bAgeModifier =
          (b.date - Date.now()) /
          (1000 * 60 * 60 * 24 * 30 * 12)
        // console.log({ aAgeModifier, bAgeModifier })
        return (
          bScoreRatio +
          bAgeModifier -
          (aScoreRatio + aAgeModifier)
        )
      })
      // take the best 50 and shuffle them
      .slice(start, start + perPage)
    step2Files = shuffle(step2Files)
    fileResponse.step2.push(...step2Files)
  } catch (error) {
    console.log(error)
  }
  return fileResponse
}

export async function upload(
  objectId: string,
  iterationCount: string | number,
  image: Buffer,
  metadata: {
    [key: string]: string
  } = {},
): Promise<ResponseOrError<string>> {
  const fileName = `${iterationCount}_${objectId}.png`
  const exists = await fileExists(fileName)
  if (exists) {
    updateFileMetadata(
      await storage.bucket(bucketName).file(fileName),
      metadata,
    )

    c.log(
      `${objectId} already exists, returning existing paths`,
    )
    return `https://storage.googleapis.com/${bucketName}/${fileName}`
  }

  const publicPath = await uploadFile(
    fileName,
    image,
    metadata,
  )
  if (typeof publicPath !== `string`) {
    c.error(publicPath.error)
    return { error: publicPath.error }
  }

  // update our cache
  cachedFiles.cache = cachedFiles.cache.filter(
    (f) => f.id !== objectId,
  )
  cachedFiles.cache.push({
    id: objectId,
    iteration: parseInt(`${iterationCount}`) || 1,
    date: Date.now(),
    likes: 0,
    dislikes: 0,
    path: publicPath,
    originalPath: publicPath.replace(`2_`, `1_`),
  })
  console.log(`cached new upload`)

  return publicPath
}

async function fileExists(path: string): Promise<boolean> {
  return new Promise(async (resolve) => {
    try {
      const [exists] = await storage
        .bucket(bucketName)
        .file(path)
        .exists()
      resolve(exists)
    } catch (error) {
      resolve(false)
    }
  })
}

async function uploadFile(
  path: string,
  data: Buffer,
  metadata: {
    [key: string]: string
  } = {},
): Promise<ResponseOrError<string>> {
  return new Promise(async (resolve) => {
    try {
      const file = await storage
        .bucket(bucketName)
        .file(path)

      const blobStream = file.createWriteStream({
        resumable: false,
        metadata: {
          contentType: `image/png`,
          metadata,
        },
      })
      blobStream.on(`error`, (err) =>
        resolve({ error: err.message || err }),
      )
      blobStream.on(`finish`, () => {
        const publicUrl = `https://storage.googleapis.com/${bucketName}/${encodeURI(
          file.name,
        )}`
        c.log(`image ${path} uploaded to ${bucketName}`)
        resolve(publicUrl)
      })
      blobStream.end(data)
    } catch (error) {
      resolve({
        error: `Error, could not upload file: ${error}`,
      })
    }
  })
}

export async function deleteAllVersionsOfFile(
  id: string,
): Promise<void> {
  const pathsToCheck = [`0_` + id, `1_` + id, `2_` + id]
  for (let path of pathsToCheck) {
    const exists = await fileExists(bucketName + `/` + path)
    if (exists) {
      c.log(`deleting ${path}`)
      try {
        const [file] = await storage
          .bucket(bucketName)
          .file(path)
          .delete()
        console.log(`${(file as any).name} deleted`)
      } catch (error) {
        console.log(error)
      }
    }
    cachedFiles.cache = cachedFiles.cache.filter(
      (f) => f.id !== id,
    )
  }
}

export async function updateFilePrefix(
  file: File | string,
  newPrefix: string,
) {
  try {
    if (typeof file === `string`) {
      console.log(`updating ${file} to ${newPrefix}`)
      file = await storage.bucket(bucketName).file(file)
    }
    const existingName = file.name.split(`_`)[1]
    const newName = `${newPrefix}_${existingName}`
    await file.copy(newName)
    await file.delete()
    // reload our cache
    getFiles(true)
    console.log(`${file.name} copied to ${newName}`)
  } catch (error) {
    console.log(error)
  }
}

export async function incrementLikes(file: string) {
  try {
    const [fileObject] = await storage
      .bucket(bucketName)
      .file(file)
      .get()
    const metadata = fileObject.metadata
    const likes = parseInt(metadata.metadata?.likes || `0`)
    const dislikes = parseInt(
      metadata.metadata?.dislikes || `0`,
    )
    const newMetadata = {
      likes: `${likes + 1}`,
      dislikes: `${dislikes}`,
    }
    const found = cachedFiles.cache.find(
      (f) => f.path === file,
    )
    if (found) found.likes = likes + 1
    await updateFileMetadata(fileObject, newMetadata)
  } catch (error) {
    console.log(error)
  }
}

export async function incrementDislikes(file: string) {
  try {
    const [fileObject] = await storage
      .bucket(bucketName)
      .file(file)
      .get()
    const metadata = fileObject.metadata
    const likes = parseInt(metadata.metadata?.likes || `0`)
    const dislikes = parseInt(
      metadata.metadata?.dislikes || `0`,
    )
    const newMetadata = {
      likes: `${likes}`,
      dislikes: `${dislikes + 1}`,
    }
    const found = cachedFiles.cache.find(
      (f) => f.path === file,
    )
    if (found) found.likes = likes + 1
    await updateFileMetadata(fileObject, newMetadata)
  } catch (error) {
    console.log(error)
  }
}

export async function updateFileMetadata(
  file: File | string,
  metadataToSet: {
    [key: string]: string
  },
) {
  try {
    if (typeof file === `string`) {
      console.log(`updating ${file} metadata`)
      file = await storage.bucket(bucketName).file(file)
    }
    const existingMetadata = await file.getMetadata()
    // console.log(`existingMetadata`, existingMetadata)
    const newMetadata = {
      ...existingMetadata,
      metadata: {
        ...((existingMetadata as any).metadata || {}),
        ...metadataToSet,
      },
    }
    const [metadata] = await file.setMetadata(newMetadata)
    console.log(metadata, metadataToSet)
  } catch (error) {
    console.log(error)
  }
}

function shuffle(array: any[]): any[] {
  let currentIndex = array.length,
    randomIndex

  // While there remain elements to shuffle...
  while (currentIndex != 0) {
    // Pick a remaining element...
    randomIndex = Math.floor(Math.random() * currentIndex)
    currentIndex--

    // And swap it with the current element.
    ;[array[currentIndex], array[randomIndex]] = [
      array[randomIndex],
      array[currentIndex],
    ]
  }

  return array
}

interface UsefulFileData {
  id: string
  iteration: number
  date: number
  likes: number
  dislikes: number
  path: string
  originalPath: string
}
function filesToUsefulData(
  files: File[],
): UsefulFileData[] {
  const existingIds = new Set<string>()
  for (let file of files) {
    const id = file.name.split(`_`)[1]?.split(`.`)[0]
    existingIds.add(id)
  }

  const filesWithData: UsefulFileData[] = []
  for (let id of existingIds) {
    let found: File | undefined
    let foundFiles = files.filter((file) =>
      file.name.includes(id),
    )
    if (foundFiles.length === 0) continue
    if (foundFiles.length === 1) {
      found = foundFiles[0]
    }
    if (foundFiles.length > 1) {
      found = foundFiles.find((file) =>
        file.name.startsWith(`2_`),
      )
    }
    // we get the 2nd iteration version because that holds the like/dislike metatada
    if (!found) continue

    let iteration: any = found.name.split(`_`)[0]
    if (iteration === undefined) continue
    if (iteration === `0`) iteration = 1
    else iteration = 2

    const data: UsefulFileData = {
      id,
      iteration,
      path: `https://${bucketName}.storage.googleapis.com/${found.name}`,
      originalPath: `https://${bucketName}.storage.googleapis.com/${found.name}`,
      likes: found.metadata?.metadata?.likes || 0,
      dislikes: found.metadata?.metadata?.dislikes || 0,
      date: new Date(
        found.metadata?.updated ||
          found.metadata?.timeCreated ||
          0,
      ).getTime(),
    }
    if (iteration === 2)
      data.originalPath = `https://storage.googleapis.com/${bucketName}/1_${id}.png`
    filesWithData.push(data)
  }

  return filesWithData
}

async function configureBucketCors() {
  await storage.bucket(bucketName).setCorsConfiguration([
    {
      maxAgeSeconds: 3600,
      method: [`GET`, `POST`],
      origin: [`*`],
      responseHeader: [
        `Content-Type`,
        `X-Goog-Upload-Status`,
      ],
    },
  ])

  console.log(`Bucket ${bucketName} CORS config updated`)
}

configureBucketCors().catch(console.error)
