# OpenSharpTrace

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/OpenSharpTrace?style=plastic)](https://www.nuget.org/packages/OpenSharpTrace)
![NuGet Downloads](https://img.shields.io/nuget/dt/OpenSharpTrace)
[![issues - dotnet-design-patterns](https://img.shields.io/github/issues/engineering87/OpenSharpTrace)](https://github.com/engineering87/OpenSharpTrace/issues)
[![Build](https://github.com/engineering87/OpenSharpTrace/actions/workflows/dotnet.yml/badge.svg)](https://github.com/engineering87/OpenSharpTrace/actions/workflows/dotnet.yml)
[![stars - dotnet-design-patterns](https://img.shields.io/github/stars/engineering87/OpenSharpTrace?style=social)](https://github.com/engineering87/OpenSharpTrace)

OpenSharpTrace is a C# .NET library that allows extending any WebApi controller to automate a custom trace and observability of REST APIs in microservices environment. 

### How it works
OpenSharpTrace implements a custom controller that overrides the `OnActionExecuting` and `OnActionExecuted` events to capture the request and response data. These are then encapsulated in a Trace object, which can be persisted specifically to SQL. All the relevant information needed for tracing both the request and response is automatically persisted, in details:

* **TransactionId**: identifier associated with the request (retrieved from the header).
* **ServerId**: name of the server.
* **ClientId**: name of the client (retrieved from the header).
* **HttpMethod**: HTTP method on the controller.
* **HttpPath**: HTTP endpoint on the controller.
* **HttpStatusCode**: HTTP result status code.
* **ActionDescriptor**: full action descriptor detail.
* **RemoteAddress**: IP address of the consumer.
* **JsonRequest**: JSON serialization of the request.
* **JsonResponse**: JSON serialization of the response.
* **TimeStamp**: UTC timestamp.
* **Exception**: possible exception message.
* **ExecutionTime**: total action execution time in milliseconds.

**TransactionId** and **ConsumerId** retrieved from the header should be enhanced by the client to allow a possible correlations between calls.
In order to enhance the two parameters, client will have to add the followuing header keys:

* `TRANSACTION` for TransactionId
* `CONSUMER` for ClientId

for example:

```csharp
HttpRequestMessage request = new HttpRequestMessage();
request.RequestUri = new Uri("API_URI");
request.Headers.Add("TRANSACTION", "123456789");
request.Headers.Add("CONSUMER", "client-name");
```

### How to use it

To use the OpenSharpTrace library, each WebApi controller must extend the **OpenSharpTraceController** controller:

```csharp
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : OpenSharpTraceController
```

each controller must implement the following constructor, for example:

```csharp
public WeatherForecastController(
	ILogger<WeatherForecastController> logger,
	ITraceQueue<Trace> transactionQueue) : base(logger, transactionQueue)
{
	_logger = logger;
}
```

for older version of the library (2.0.0 or above):

```csharp
public WeatherForecastController(
	ILoggerFactory loggerFactory, 
	ISqlTraceRepository repository) 
: base(loggerFactory, repository)
{
    _logger = loggerFactory.CreateLogger(GetType().ToString());
}
```

You also need to register the OpenSharpTrace middleware.
To do this, add the following configurations under WebApplicationBuilder:

```csharp
using OpenSharpTrace.Middleware;

services.RegisterOpenSharpTrace();
// ...
```

In the source code you can find a simple test Web Api.

### Available connectors

#### SQL

Currently the only connector available is the SQL connector on the *Trace* table using *Entity Framework Core*.
The following is the table creation script:

```tsql
CREATE TABLE [dbo].[Trace](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionId] [nvarchar](MAX) NULL,
	[ServerId] [nvarchar](MAX) NULL,
	[ClientId] [nvarchar](MAX) NULL,
	[HttpMethod] [nvarchar](7) NULL,
	[HttpPath] [nvarchar](MAX) NULL,
	[HttpStatusCode] [int] NULL,
	[ActionDescriptor] [nvarchar](MAX) NULL,
	[RemoteAddress] [nvarchar](MAX) NULL,
	[JsonRequest] [nvarchar](MAX) NULL,
	[JsonResponse] [nvarchar](MAX) NULL,
	[TimeStamp] [datetime2](7) NULL,
	[Exception] [nvarchar](MAX) NULL,
	[ExecutionTime] [numeric] NULL,
) ON [PRIMARY]
```
From version 4.1.0 onwards, table creation is handled automatically, in case it is missing on the SQL instance.

Remember to populate the **TraceDb** key within the SQL connection strings config file:

```xml
  "ConnectionStrings": {
    "TraceDb": "Server=(***;Database=***;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
```

## Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/engineering87/OpenSharpTrace/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
OpenSharpTrace source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre[at]protonmail.com for any details.
