import formidable from 'formidable-serverless'
import fs from 'fs'

export default async function parseBufferFromUpload(
  req,
  res,
): Promise<
  ResponseOrError<{ id: string; buffer: Buffer }>
> {
  return new Promise(async (resolve) => {
    const form = new formidable.IncomingForm()
    form.parse(req, async (err, fields, files) => {
      if (err) {
        resolve({
          error: err,
        })
        return
      }
      const file = files[Object.keys(files)[0]]
      const buffer = fs.readFileSync(file.path)
      resolve({
        id: file.name
          .split(`.`)[0]
          .replace(/(_|\s)*/g, ``)
          .replace(/^\d+_/g, ``),
        buffer,
      })
    })
  })
}

export async function parseBody(
  req,
  res,
): Promise<ResponseOrError<{ [key: string]: any }>> {
  return new Promise(async (resolve) => {
    const form = new formidable.IncomingForm()
    form.parse(req, async (err, fields, files) => {
      if (err) {
        resolve({
          error: err,
        })
        return
      }
      resolve(fields)
    })
  })
}
