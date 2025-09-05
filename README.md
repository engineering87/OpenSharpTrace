# OpenSharpTrace

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/OpenSharpTrace?style=plastic)](https://www.nuget.org/packages/OpenSharpTrace)
![NuGet Downloads](https://img.shields.io/nuget/dt/OpenSharpTrace)
[![issues - opensharptrace](https://img.shields.io/github/issues/engineering87/OpenSharpTrace)](https://github.com/engineering87/OpenSharpTrace/issues)
[![Build](https://github.com/engineering87/OpenSharpTrace/actions/workflows/dotnet.yml/badge.svg)](https://github.com/engineering87/OpenSharpTrace/actions/workflows/dotnet.yml)
[![stars - opensharptrace](https://img.shields.io/github/stars/engineering87/OpenSharpTrace?style=social)](https://github.com/engineering87/OpenSharpTrace)

**OpenSharpTrace** is a .NET/C# library that augments Web API controllers to automatically trace requests/responses and key metadata for observability in microservices.

✅ **Persistence supported:** **Microsoft SQL Server** via **Entity Framework Core**  

## Features
OpenSharpTrace offers the following features to enhance tracing and logging capabilities in .NET applications:

- **Controller-level tracing** – hooks into `OnActionExecuting` / `OnActionExecuted`.
- **Batch persistence** – traces are queued in memory and **flushed periodically** in a single DB transaction (default: every 60s).
- **SQL Server ready** – EF Core `ExecutionStrategy` and `EnableRetryOnFailure`.
- **Rich trace data** – status code, path (+ query string), client/server IDs, remote IP (with `X-Forwarded-For`), timings, exceptions, JSON payloads.
- **Simple DI registration** – one extension method to wire everything up.

These features make OpenSharpTrace a powerful and flexible choice for implementing robust tracing solutions in your .NET ecosystem.

## Installation

```bash
dotnet add package OpenSharpTrace
```

## How it works
OpenSharpTrace provides a base controller that hooks into the ASP.NET Core action pipeline by overriding `OnActionExecuting` and `OnActionExecute`.
At the start of each request it snapshots the input and start time; at the end it computes the elapsed time, gathers HTTP/context metadata, serializes request/response payloads, and packages everything into a `Trace`.
Traces are enqueued in memory and periodically flushed to SQL Server in a single transaction (default interval: 60s) with EF Core’s execution strategy and transient-retry.

The following fields are captured and persisted:

- **TransactionId** – correlation identifier for the request (read from the `TRANSACTION` header if present).
- **ServerId** – server/host handling the request.
- **ClientId** – caller identity (read from the `CONSUMER` header if present).
- **HttpMethod** – HTTP method.
- **HttpPath** – request path including the query string.
- **HttpStatusCode** – response status code.
- **ActionDescriptor** – fully qualified action identifier.
- **RemoteAddress** – client IP (prefers `X-Forwarded-For`, falls back to `RemoteIpAddress`; IPv6-mapped addresses are normalized).
- **JsonRequest** – JSON serialization of the request arguments.
- **JsonResponse** – JSON serialization of the action result (when available).
- **TimeStamp** – UTC timestamp at trace creation.
- **Exception** – exception message, if any occurred during execution.
- **ExecutionTime** – total action execution time in milliseconds.

To enable end-to-end correlation across services, clients should set the following headers on outbound requests:

- `TRANSACTION` → mapped to TransactionId
- `CONSUMER` → mapped to ClientId

for example:

```csharp
HttpRequestMessage request = new HttpRequestMessage();
request.RequestUri = new Uri("API_URI");
request.Headers.Add("TRANSACTION", "123456789");
request.Headers.Add("CONSUMER", "client-name");
```

## How to use it
1) **Inherit from the base controller**

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Controllers;
using OpenSharpTrace.TransactionQueue;
using OpenSharpTrace.Persistence.SQL.Entities;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : OpenSharpTraceController
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        ITraceQueue<Trace> transactionQueue)
        : base(logger, transactionQueue)
    {
        _logger = logger;
    }

    // your actions...
}
```

2) **Register services (Dependency Injection)**
Option A – default connection key TraceDb from appsettings.json:

```csharp
using OpenSharpTrace.Middleware;

builder.Services.RegisterOpenSharpTrace();
```

Option B – specify a connection key:

```csharp
builder.Services.RegisterOpenSharpTrace("MyTraceDb");
```

Option C – pass IConfiguration (recommended for hosted apps):

```csharp
builder.Services.RegisterOpenSharpTrace(builder.Configuration);                // uses "TraceDb"
builder.Services.RegisterOpenSharpTrace(builder.Configuration, "MyTraceDb");   // custom key
```

## Available connectors

#### SQL
Currently the only connector available is the SQL connector on the *Trace* table using *Entity Framework Core*.
The following is the table creation script:

```tsql
CREATE TABLE [dbo].[Trace](
    [Id]            BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TransactionId] NVARCHAR(128) NULL,
    [ServerId]      NVARCHAR(128) NULL,
    [ClientId]      NVARCHAR(128) NULL,
    [HttpMethod]    NVARCHAR(16) NULL,
    [HttpPath]      NVARCHAR(128) NULL,
    [HttpStatusCode] INT NULL,
    [ActionDescriptor] NVARCHAR(MAX) NULL,
    [RemoteAddress] NVARCHAR(64) NULL,
    [JsonRequest]   NVARCHAR(MAX) NULL,
    [JsonResponse]  NVARCHAR(MAX) NULL,
    [TimeStamp]     DATETIME2(7) NULL,
    [Exception]     NVARCHAR(MAX) NULL,
    [ExecutionTime] FLOAT NULL
);

CREATE INDEX IX_Trace_TimeStamp     ON [dbo].[Trace]([TimeStamp]);
CREATE INDEX IX_Trace_ClientId      ON [dbo].[Trace]([ClientId]);
CREATE INDEX IX_Trace_TransactionId ON [dbo].[Trace]([TransactionId]);
CREATE INDEX IX_Trace_HttpStatusCode ON [dbo].[Trace]([HttpStatusCode]);
```
The library ensures the trace table exists on startup (SQL Server).

## Configuration
**appsettings.json**

```json
{
  "ConnectionStrings": {
    "TraceDb": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

## Sample usage

```csharp
// Program.cs (.NET 6+)
var builder = WebApplication.CreateBuilder(args);

// Register OpenSharpTrace with SQL Server persistence
builder.Services.RegisterOpenSharpTrace(builder.Configuration);

var app = builder.Build();
app.MapControllers();
app.Run();
```

## Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/engineering87/OpenSharpTrace/issues) if you encounter a bug or have a suggestion for improvements/features

## License
OpenSharpTrace source code is available under MIT License, see license in the source.

## Contact
Please contact at francesco.delre[at]protonmail.com for any details.
