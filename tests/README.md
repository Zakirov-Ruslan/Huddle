# Tests

| Project | Kind | Requires Docker |
|---|---|---|
| `Huddle.Channel.Domain.UnitTests` | unit — aggregates, value objects, domain events | no |
| `Huddle.Channel.Application.UnitTests` | unit — command handlers, access service, idempotency, validators (NSubstitute) | no |
| `Huddle.Channel.FunctionalTests` | functional — the real WebApi over PostgreSQL in a container | yes |

```
dotnet test                                            # everything, needs Docker
dotnet test tests/Huddle.Channel.Domain.UnitTests       # fast loop, no Docker
```

## Functional tests

`ChannelApiFixture` starts a `postgres:latest` container with Testcontainers, boots the API through
`WebApplicationFactory<Program>` and lets EF Core apply the migrations on startup. Everything that would need
another service is replaced:

- **identity** — the JWT bearer scheme is swapped for `TestAuthHandler`; the caller is taken from the
  `X-Test-User` header, so a test can act as the server owner, a member or a stranger;
- **RabbitMQ** — `RabbitMQEventBus` is not started and `IEventBus` is a no-op, so integration events reach the
  outbox but are never published;
- **files** — `IFilesClient` is a stub that claims every requested file.

The database is shared by all tests of the collection, so tests create their own servers and members and never
truncate: isolation comes from unique names and identities. Both test classes share one fixture through the
`channel-api` collection, so the container is started once per run.
