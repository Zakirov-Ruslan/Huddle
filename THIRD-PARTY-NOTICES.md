# Third-party notices

Huddle is released under the [MIT License](LICENSE).

It is built on open-source software. This file lists the third-party components used by the solution,
their licenses, and the obligations that come with them.

Versions are the ones pinned by the projects and `package.json` at the time of writing. The authoritative
list is always the dependency graph itself (`**/obj/project.assets.json`, `package-lock.json`).

> **Not legal advice.** If Huddle is released or sold, have the license terms confirmed by a lawyer.

## ⚠️ Components that need a decision

| Component | Version | License | What it means |
|---|---|---|---|
| **Duende IdentityServer** (+ `Duende.IdentityServer.AspNetIdentity`) | 7.2.4 | Source-available, [RPL-1.5](https://opensource.org/license/rpl-1-5/) or commercial | Free for development, testing and personal projects. **Production use requires a paid license**, or the free Community Edition for eligible companies and non-profit organizations — check the current criteria at [duendesoftware.com](https://duendesoftware.com/products/identityserver). |
| **AutoMapper** | 15.0.0 | [RPL-1.5](https://opensource.org/license/rpl-1-5/) or commercial | RPL is a strong reciprocal copyleft: distributing a product that uses it obliges you to publish your own source under RPL-1.5, unless a commercial license is bought. Versions **≤ 14.0.0 are MIT** — downgrading removes the obligation. |
| **MinIO server** (container) | `minio/minio` RELEASE.2025-04-22 | [AGPL-3.0](https://www.gnu.org/licenses/agpl-3.0.html) | Only used as a development container. AGPL obligations start when the software is distributed (for example a bundled Docker image/compose file) or modified and offered over the network. Running the unmodified upstream image is not distribution. |
| **Redis** (container) | `redis:7.4` | RSALv2 or SSPLv1 (not OSI open source) | Fine to run unmodified, including in development. Offering a modified build as a service, or redistributing it, needs a Redis license. Redis 8+ adds AGPLv3 as an option. |
| **react-icons** | 5.5.0 | MIT (wrapper) | The package is MIT, but it re-exports icon sets with their own licenses. This project imports **Font Awesome 5/6** (`fa`, `fa6`) and **IcoMoon Free** (`im`), which are **CC BY 4.0 and require attribution**. The other sets in use are MIT (`fi`, `io`, `io5`, `hi2`, `bs`, `go`, `rx`, `tb`) or Apache-2.0 (`md`, `ri`). Attribution: "Font Awesome Free by Fonticons, Inc. and IcoMoon Free by Keyamoon, licensed under CC BY 4.0". |

**MediatR** is pinned to 12.5.0, which is Apache-2.0. MediatR **13 and later are RPL-1.5/commercial** — do not
upgrade without a license.

## Permissive dependencies

Nothing beyond keeping the copyright and license text when binaries are redistributed.

### .NET / NuGet

| License | Packages |
|---|---|
| MIT | Aspire.Hosting.AppHost, Aspire.Hosting.NodeJs, Aspire.Hosting.PostgreSQL, Aspire.Hosting.RabbitMQ, Aspire.Hosting.Redis, Aspire.Npgsql.EntityFrameworkCore.PostgreSQL, Aspire.RabbitMQ.Client, CommunityToolkit.Aspire.Hosting.Minio, coverlet.collector, Microsoft.AspNetCore.Authentication.JwtBearer, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.AspNetCore.Mvc.Testing, Microsoft.AspNetCore.OpenApi, Microsoft.AspNetCore.SignalR.StackExchangeRedis, Microsoft.AspNetCore.TestHost, Microsoft.EntityFrameworkCore (+ .Design, .Tools), Microsoft.Extensions.DependencyInjection.Abstractions, Microsoft.Extensions.Hosting.Abstractions, Microsoft.Extensions.Http, Microsoft.Extensions.Http.Resilience, Microsoft.Extensions.Options (+ .ConfigurationExtensions), Microsoft.Extensions.ServiceDiscovery (+ .Yarp), Microsoft.NET.Test.Sdk, Nanoid, Yarp.ReverseProxy |
| Apache-2.0 | FluentValidation, Grpc.AspNetCore, Livekit.Server.Sdk.Dotnet, MediatR 12.5.0, Minio (SDK), Microsoft.AspNetCore.SignalR, Microsoft.Web.LibraryManager.Build, OpenTelemetry.Exporter.OpenTelemetryProtocol, OpenTelemetry.Extensions.Hosting, OpenTelemetry.Instrumentation.AspNetCore, OpenTelemetry.Instrumentation.Http, OpenTelemetry.Instrumentation.Runtime, xunit, xunit.runner.visualstudio |
| BSD-3-Clause | Polly.Core |
| PostgreSQL | Npgsql.EntityFrameworkCore.PostgreSQL |

`Microsoft.VisualStudio.Azure.Containers.Tools.Targets` is covered by a Microsoft EULA. It is build-time
tooling only and is not redistributed with the application.

### Frontend / npm

| License | Packages |
|---|---|
| MIT | @eslint/js, @headlessui/react, @tailwindcss/cli, @tanstack/react-query, @types/*, @vitejs/plugin-react, eslint, eslint-plugin-react-hooks, eslint-plugin-react-refresh, globals, install, react, react-dom, react-icons, react-intersection-observer, react-modal, react-oidc-context, react-router, react-signalr, react-tooltip, tailwindcss, typescript-eslint, usehooks-ts, uuid, vite, zustand |
| Apache-2.0 | @livekit/components-react, @livekit/components-styles, livekit-client, oidc-client-ts, typescript |
| Artistic-2.0 | npm (listed as a dependency by mistake — see below) |

`npm` and `install` appear in `dependencies` of the SPA package but are tools, not runtime libraries; they
should be dropped from `package.json`.

### Containers and services started by the AppHost

| Component | Image | License |
|---|---|---|
| PostgreSQL | `postgres:latest` | PostgreSQL License |
| RabbitMQ | `rabbitmq:4.1` | MPL-2.0 (some OCF files Apache-2.0) |
| Redis | `redis:7.4` | RSALv2 / SSPLv1 |
| MinIO | `minio/minio` | AGPL-3.0 |
| LiveKit server | `livekit/livekit-server` | Apache-2.0 |

## Obligations in practice

- **Source repository**: keeping this file is enough. Dependency sources are not vendored here, so no third-party
  license texts have to be copied into the tree.
- **Distributing builds or images** (installers, container images, a hosted bundle): include this file together
  with the license texts of the MIT/Apache-2.0/BSD components, keep upstream `NOTICE` files where they exist, and
  state if any Apache-2.0 file was modified.
- **Copyleft components** (Duende, AutoMapper if it stays at 15, MinIO, Redis) require a commercial license or
  compliance with the reciprocal terms as soon as the product is distributed or offered as a service.

## Keeping this list up to date

```powershell
# Every NuGet package the solution references
dotnet list Huddle.sln package

# License summary of the SPA dependencies
cd src/Clients/huddle.react
npx license-checker --summary
```
