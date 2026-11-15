# 🎤 Huddle
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)


**Huddle** is a real-time chat application (a Discord alternative) built on .NET using a service-oriented architecture and **.NET Aspire**. This project demonstrates the practical application of DDD, Clean Architecture, and patterns for building resilient distributed systems.

## 🚀 Features
- 🎥 **WebRTC** for real-time voice and video communication.
- 🏗️ **Clean Architecture + DDD**: strict separation into Domain, Application, Infrastructure, and API layers.
- 📨 **CQRS with MediatR**: clear separation of commands and queries.
- 📦 **Outbox Pattern**: guaranteed event delivery via RabbitMQ.
- 🔍 **Observability**: OpenTelemetry, structured logging (Serilog), and the .NET Aspire dashboard out of the box.
- 📎 **File storage**: chat attachments and server avatars are stored in **MinIO** (S3-compatible) through a dedicated file service.

## 📎 File storage (MinIO)
The AppHost starts a MinIO container and a `filedb` database; every upload goes through **`Huddle.FileService`**, which is exposed through the gateway under `/files/**`.

Endpoints:
- `POST /api/files?kind=attachment` or `?kind=avatar` — multipart upload (`file` field), returns the file id and the relative content URL.
- `GET /api/files/{id}/content` — streams the file; add `?download=true` to force a download.
- `GET /api/files/{id}/url` — short-lived presigned MinIO URL (owner only).
- `DELETE /api/files/{id}` — removes the object and its metadata (owner only).
- `POST /api/files/claim` — internal endpoint the channel service uses to verify uploaded files before they are attached to a message or a server avatar.

Limits and validation: up to 10 files per message, 25 MB per attachment, 5 MB per avatar; `png`, `jpeg`, `gif`, `webp`, `pdf`, `txt`, `csv`, `json`, `zip`, `docx`, `xlsx` (avatars must be images). The channel service stores attachment metadata (`message_attachments`) and the server avatar file id, and resolves the URLs itself, so the storage host is never persisted.


## 🎥 Voice and video (LiveKit)
The AppHost runs a `livekit/livekit-server` container and asks for host networking, so the RTC ports are reachable directly: `7880/tcp` (signalling), `7881/tcp` (ICE over TCP) and `50000-60000/udp`. Clients outside the machine need those ports forwarded to the developer machine by the router. Docker Desktop provides the "host" network inside its own VM, so there the RTC ports have to be published or forwarded with the container instead.

LiveKit accepts settings such as `rtc.node_ip` only through its configuration body, so the AppHost **generates** the configuration on every run and passes it in the `LIVEKIT_CONFIG` environment variable (see [LiveKitConfigBuilder](src/Huddle.AppHost/LiveKit/LiveKitConfigBuilder.cs)) — there is no machine specific config file in the repository.

`rtc.node_ip` is the public address clients connect to, so it changes with the network:

- by default the AppHost detects the public IP address of the machine at startup and logs it as `[livekit] rtc.node_ip = ...`;
- override it when detection does not fit (VPN, no internet, fixed address), either in `appsettings.Development.json`, with a user secret or with an environment variable:

```
dotnet user-secrets set "LiveKit:NodeIp" "203.0.113.7" --project src/Huddle.AppHost
```

```
$env:LiveKit__NodeIp = "203.0.113.7"
```

- set `LiveKit:AutoDetectNodeIp` to `false` (and leave `NodeIp` empty) to fall back to LiveKit's own STUN lookup.

The LiveKit API key pair and the internal key between the channel and file services are generated on every AppHost run, so no credentials are stored in the repository. Pin them with `LiveKit:ApiKey`, `LiveKit:ApiSecret` or `Files:InternalApiKey` (user secrets, environment variables or `appsettings.Development.json`) when a value has to survive a restart, for example to check the server with https://livekit.io/connection-test. The values the container actually received are visible with `docker inspect <livekit container> --format '{{json .Config.Env}}'`.


## 🏛️ Architecture
![Architecture Diagram](img/huddle-architecture-diagram.svg)


## 🛠️ Getting Started
- Ensure you have the .NET 8 SDK (or later) and Docker Desktop installed.
- Trust the development HTTPS certificates:
```dotnet dev-certs https --trust ```
- Run the AppHost project:
 ```cd src/Huddle.AppHost ```
 ``` dotnet run ```

## 📚 Inspiration & References
- https://github.com/dotnet/eShop
- https://github.com/dotnet-architecture/eShopOnContainers/tree/dev
- https://github.com/ardalis/CleanArchitecture
- https://learn.microsoft.com/ru-ru/dotnet/architecture/microservices/
