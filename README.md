# OpenSharpTrace

OpenSharpTrace is a C# .NET 6 library that allows extending any WebApi controller to automate a custom trace and observability of REST APIs in microservices environment. 

### How it works
OpenSharpTrace implements a custom controller which overrides the *OnActionExecuting* and *OnActionExecuted* events to retrieve request and response and encapsulates them in a **Trace** object which will persist through different connectors, such as SQL or MongoDB.
All the information useful for tracing both request and response will be automatically persisted, in details:

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

* TRANSACTION for TransactionId
* CONSUMER for ClientId

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

**.NET 5**:

```csharp
using OpenSharpTrace.Middleware;

// in Startup.cs ConfigureServices
services.RegisterOpenSharpTrace();
// ...
```

**.NET 6**:

```csharp
using OpenSharpTrace.Middleware;

// in Program.cs
builder.Services.RegisterOpenSharpTrace();
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
	[TransactionId] [nvarchar](36) NULL,
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

Remember to populate the **TraceDb** key within the connection strings config file.

#### MongoDB (not available)

Integration with MongoDB is work in progress.

### NuGet

The library is available on NuGet packetmanager.

https://www.nuget.org/packages/OpenSharpTrace/

### Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
 * [Open an issue](https://github.com/engineering87/OpenSharpTrace/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
OpenSharpTrace source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre[at]protonmail.com for any details.
