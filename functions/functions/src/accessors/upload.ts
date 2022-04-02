import formidable from 'formidable-serverless'

export default async function parseBufferFromUpload(
  req,
): Promise<ResponseOrError<Buffer>> {
  return new Promise(async (resolve) => {
    console.log(
      `parseBufferFromUpload`,
      req.rawBody,
      typeof req.rawBody,
    )
    resolve(req.rawBody)

    // const form = new formidable.IncomingForm()
    // form.parse(req, async (err, fields, files) => {
    //   console.log(`done parsing`)
    //   if (err) {
    //     console.log(err)
    //     resolve({
    //       error: err,
    //     })
    //     return
    //   }
    //   const file = files[Object.keys(files)[0]]
    //   const buffer = fs.readFileSync(file.path)
    //   resolve(
    //     // file.name
    //     //   .split(`.`)[0]
    //     //   .replace(/(_|\s)*/g, ``)
    //     //   .replace(/^\d+_/g, ``),
    //     buffer,
    //   )
    // })
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
