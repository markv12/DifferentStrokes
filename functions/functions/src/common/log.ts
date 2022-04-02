const fillCharacter = `.`

let longest = 0

const reset = `\x1b[0m`,
  dim = `\x1b[2m`,
  bright = `\x1b[1m`

const colors: any = {
  gray: `\x1b[2m`,
  red: `\x1b[31m`,
  green: `\x1b[32m`,
  yellow: `\x1b[33m`,
  blue: `\x1b[34m`,
  pink: `\x1b[35m`,
  cyan: `\x1b[36m`,
  white: `\x1b[37m`,
}

export const log = (...args: any[]): void => {
  const regexResult =
    /log\.[jt]s[^\n]*\n([^\n\r]*\/([^/\n\r]+\/[^/\n\r]+\/[^/:\n\r]+))\.[^:\n\r]+:(\d+)/gi.exec(
      `${new Error().stack}`,
    )
  const fullPath: string = regexResult?.[1] || ``
  const lineNumber: string = regexResult?.[3] || ``
  const pathName: string =
    regexResult?.[2]?.replace(/(dist\/|src\/)/gi, ``) || ``

  for (let index = 0; index < args.length; index++) {
    const arg = args[index]
    if (arg in colors) {
      if (!args[index + 1]) continue
      if (typeof args[index + 1] === `object`) {
        // args[index] = colors[arg]
        // args = [
        //   ...args.slice(0, index + 2),
        //   reset,
        //   ...args.slice(index + 2),
        // ]
        args.splice(index, 1)
      } else {
        args[index + 1] =
          colors[arg] + `${args[index + 1]}` + reset
        args.splice(index, 1)
      }
    }
  }

  let prefix = String(
    reset +
      dim +
      `[${new Date().toLocaleTimeString(undefined, {
        hour12: false,
        hour: `2-digit`,
        minute: `2-digit`,
      })}]` +
      pathName,
    // +
    // `:` +
    // lineNumber,
  )

  if (prefix.length > longest) longest = prefix.length
  while (
    prefix.length < Math.min(45, Math.max(25, longest))
  )
    prefix += fillCharacter
  prefix += reset

  console.log(prefix, ...args)
}

export function error(...args: any[]): void {
  log(`red`, ...arguments)
}

export function trace() {
  console.trace()
}
