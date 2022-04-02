import { File } from '@google-cloud/storage'
import * as functions from 'firebase-functions'
import {
  bucketName,
  getRelevantFiles,
  incrementDislikes,
  incrementLikes,
  updateFileMetadata,
  updateFilePrefix,
  upload,
} from './accessors/storage'
import parseBufferFromUpload, {
  parseBody,
} from './accessors/upload'

// export const allFiles = functions.https.onRequest(
//   async (req, res): Promise<void> => {
//     const files = await listFiles()
//     res.send(files)
//   },
// )

export const files = functions.https.onRequest(
  async (req, res): Promise<void> => {
    let page = 0
    try {
      page = parseInt(req.params[0].split(`/`).slice(1)[0])
    } catch (e) {
      page = 0
    }
    if (!page) page = 0
    const files = await getRelevantFiles(page)
    res.send(files)
  },
)

// export const unfinishedFiles = functions.https.onRequest(
//   async (req, res): Promise<void> => {
//     const [startFrom] = req.params[0].split(`/`).slice(1)
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
//     const [startFrom] = req.params[0].split(`/`).slice(1)
//     const files = await getBestDoneFiles(startFrom)
//     if (`error` in files) {
//       res.send([])
//       return
//     }
//     res.send(files.map((f) => fileToUsefulData(f)))
//   },
// )

export const upload1 = functions.https.onRequest(
  async (req, res): Promise<void> => {
    const bufferRes = await parseBufferFromUpload(req, res)
    if (`error` in bufferRes) {
      res.send(bufferRes)
      return
    }
    const uploadRes = await upload(
      bufferRes.id,
      0,
      bufferRes.buffer,
    )
    if (typeof uploadRes === `object`) {
      res.send(uploadRes)
      return
    }
    res.send(`ok`)
  },
)

export const upload2 = functions.https.onRequest(
  async (req, res): Promise<void> => {
    const bufferRes = await parseBufferFromUpload(req, res)
    if (`error` in bufferRes) {
      res.send(bufferRes)
      return
    }
    const uploadRes = await upload(
      bufferRes.id,
      2,
      bufferRes.buffer,
    )
    if (typeof uploadRes === `object`) {
      res.send(uploadRes)
      return
    }

    // update the initial file to have the 1_ prefix
    console.log(`updating initial version name`)
    updateFilePrefix(`0_${bufferRes.id}.png`, `1`)

    res.send(`ok`)
  },
)

export const like = functions.https.onRequest(
  async (req, res): Promise<void> => {
    const [id] = req.params[0].split(`/`).slice(1)
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
    const [id] = req.params[0].split(`/`).slice(1)
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
    res.send(`test`)
  },
)
