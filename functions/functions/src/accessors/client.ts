import * as c from '../common/index'

import { google } from 'googleapis'
import type {
  JWT,
  Compute,
  UserRefreshClient,
  Impersonated,
  BaseExternalAccountClient,
} from 'google-auth-library'

let client:
  | JWT
  | Compute
  | UserRefreshClient
  | Impersonated
  | BaseExternalAccountClient
export default async function getClient() {
  if (client) return client
  const auth = new google.auth.GoogleAuth({
    keyFile: `./credentials.json`,
    scopes: [`https://www.googleapis.com/auth/drive`],
  })

  // Create client instance for auth
  client = await auth.getClient()

  client.addListener(`error`, (err) => {
    c.error(err)
  })

  c.log(`Client initialized.`)
  return client
}
