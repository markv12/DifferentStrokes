import { File } from '@google-cloud/storage'
import * as functions from 'firebase-functions'
import {
  bucketName,
  getRelevantFiles,
  getTopFiles,
  incrementDislikes,
  incrementLikes,
  updateFileMetadata,
  updateFilePrefix,
  upload,
} from './accessors/storage'
import parseBufferFromUpload, {
  parseBody,
} from './accessors/upload'

export const files = functions.https.onRequest(
  async (req, res): Promise<void> => {
    res.set(`Access-Control-Allow-Origin`, `*`)

    let page = 0
    try {
      page = parseInt(req.params[`0`].replace(`/`, ``))
    } catch (e) {
      page = 0
    }
    if (!page) page = 0
    const files = await getRelevantFiles(page)
    res.send(files)
  },
)

export const top = functions.https.onRequest(
  async (req, res): Promise<void> => {
    res.set(`Access-Control-Allow-Origin`, `*`)

    const files = await getTopFiles()
    res.send(files)
  },
)

// export const allFiles = functions.https.onRequest(
//   async (req, res): Promise<void> => {
//     const files = await listFiles()
//     res.send(files)
//   },
// )

// export const unfinishedFiles = functions.https.onRequest(
//   async (req, res): Promise<void> => {
//     const [startFrom] = req.params[(req.params.length) - 1].split(`/`).slice(1)
//     const files = await getFilesByIteration(0, startFrom)
//     if (`error` in files) {
//       res.send([])
//       return
//     }
//     res.send(files.map((f) => fileToUsefulData(f)))
//   },
// )

// export const doneFiles = functions.https.onRequest(
//   async (req, res): Promise<void> => {
//     const [startFrom] = req.params[(req.params.length) - 1].split(`/`).slice(1)
//     const files = await getBestDoneFiles(startFrom)
//     if (`error` in files) {
//       res.send([])
//       return
//     }
//     res.send(files.map((f) => fileToUsefulData(f)))
//   },
// )

export const uploadv1 = functions.https.onRequest(
  async (req, res): Promise<void> => {
    res.set(`Access-Control-Allow-Origin`, `*`)

    console.log(req.params)
    const id = req.params[`0`].replace(`/`, ``)
    if (!id) {
      console.log(`no id`)
      res.send(`No file name`)
      return
    }

    const bufferRes = await parseBufferFromUpload(req)
    if (!bufferRes || (bufferRes as any).error) {
      console.log(
        `bufferRes`,
        bufferRes,
        !bufferRes,
        (bufferRes as any).error,
      )
      res.send(bufferRes || `No buffer`)
      return
    }
    const uploadRes = await upload(
      id,
      0,
      bufferRes as Buffer,
    )
    if (!uploadRes || typeof uploadRes === `object`) {
      console.log(uploadRes)
      res.send(uploadRes || `No upload`)
      return
    }
    console.log(`ok`)
    res.send(`ok`)
  },
)

export const uploadv2 = functions.https.onRequest(
  async (req, res): Promise<void> => {
    res.set(`Access-Control-Allow-Origin`, `*`)

    const id = req.params[`0`].replace(`/`, ``)
    if (!id) {
      console.log(`no id`)
      res.send(`No file name`)
      return
    }

    const bufferRes = await parseBufferFromUpload(req)
    if (!bufferRes || (bufferRes as any).error) {
      console.log(`bufferRes`, bufferRes)
      res.send(bufferRes || `No buffer`)
      return
    }
    const uploadRes = await upload(
      id,
      2,
      bufferRes as Buffer,
    )
    if (!uploadRes || typeof uploadRes === `object`) {
      console.log(uploadRes)
      res.send(uploadRes || `No upload`)
      return
    }

    // update the initial file to have the 1_ prefix
    console.log(`updating initial version name`)
    updateFilePrefix(`0_${id}.png`, `1`)

    console.log(`ok`)
    res.send(`ok`)
  },
)

export const like = functions.https.onRequest(
  async (req, res): Promise<void> => {
    res.set(`Access-Control-Allow-Origin`, `*`)

    console.log(req.params)
    const id = req.params[`0`]
      .replace(`/`, ``)
      .replace(/^\d_/g, ``)
    if (!id) {
      res.send({ error: `no id` })
      return
    }
    await incrementLikes(`2_${id}.png`)
    res.send(`ok`)
  },
)

export const dislike = functions.https.onRequest(
  async (req, res): Promise<void> => {
    res.set(`Access-Control-Allow-Origin`, `*`)

    const id = req.params[`0`]
      .replace(`/`, ``)
      .replace(/^\d_/g, ``)
    if (!id) {
      res.send({ error: `no id` })
      return
    }
    await incrementDislikes(`2_${id}.png`)
    res.send(`ok`)
  },
)

// export const updateMetadata = functions.https.onRequest(
//   async (req, res): Promise<void> => {
//     const body = await parseBody(req, res)
//     if (`error` in body) {
//       res.send(body)
//       return
//     }
//     try {
//       const metadata = JSON.parse(body.metadata)
//       const fileName = body.fileName
//       console.log(`received metadata`, metadata, fileName)
//       await updateFileMetadata(fileName, metadata)
//       res.send(`ok`)
//     } catch (error) {
//       console.log(`error`, error, body)
//       res.status(400).send(error)
//     }
//   },
// )

export const test = functions.https.onRequest(
  async (req, res): Promise<void> => {
    res.set(`Access-Control-Allow-Origin`, `*`)

    res.send(`test` + JSON.stringify(req.params))
  },
)
