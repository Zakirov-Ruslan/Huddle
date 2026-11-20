# Voice service

Owns everything around LiveKit: it issues the access tokens the SPA uses to join a channel room, receives
the LiveKit webhooks and keeps track of who sits in the voice channels of a server.

| Endpoint | Description |
|---|---|
| `GET api/livekit/token?serverId&channelId` | Access token for the LiveKit room of a channel, issued after a gRPC access check. |
| `GET api/livekit/servers/{serverId}/participants` | Voice presence snapshot of a whole server, grouped by channel. |
| `POST api/livekit/webhook` | LiveKit callbacks: participant joined/left, room finished. |

A room is named `server-{serverId}-channel-{channelId}`; everything that maps a room back to a channel goes
through `VoiceRoomName`.

## How voice presence travels

1. LiveKit reports joins, leaves and finished rooms to the webhook endpoint, signed with the LiveKit API
   secret (see [Webhook authentication](#webhook-authentication)).
2. The service updates its Redis projection of the server (`voice:server:{serverId}`) and publishes an
   integration event that carries the version of the resulting state.
3. The realtime service pushes `VoiceParticipantJoined` / `VoiceParticipantLeft` / `VoiceChannelClosed` to
   the server group, together with that version.
4. The SPA fetches the snapshot with `GET .../participants` and applies the pushes to it, ignoring anything at
   or below the version of the snapshot it already holds.

LiveKit stays the source of truth. The projection expires after two minutes and is rebuilt from the LiveKit
RoomService on the next read, so a missed webhook or a wiped Redis cannot leave a stale list behind for
longer than that. The version is incremented in the same Redis transaction as the change itself, which is
what lets a client tell a delta it has already applied from a newer one.

The snapshot request is per server instead of per channel: one access check per server page and one Redis
round-trip, while the participants stay grouped by channel on the server side.

## Webhook authentication

The webhook is the only endpoint here that does not use the standard JWT bearer scheme. LiveKit signs its
callbacks with a JWT issued for its own API key, not for a Huddle user, and the identity server would reject
it. `WebhookReceiver.Receive` verifies that signature and the sha256 digest of the body instead, and an
unsigned request (or one with a modified body) is answered with `401`.

The endpoint is also excluded from the HTTPS redirection: LiveKit delivers webhooks over plain HTTP in the
development environment and does not follow redirects.

## Configuration

| Setting | Description |
|---|---|
| `LIVEKIT_URL` | LiveKit HTTP endpoint, used by the RoomService client. |
| `LIVEKIT_API_KEY`, `LIVEKIT_API_SECRET` | API key pair, used for room grants and webhook verification. |
| `IDENTITY_URL` | Identity server behind the JWT bearer scheme. |
| `CHANNELS_URL` | Channel service behind the gRPC access checks. |
| `ConnectionStrings:redis` | Cache of the presence projection. |

The AppHost provides all of them, including the LiveKit webhook URL it renders into the LiveKit configuration
(see the root README).
