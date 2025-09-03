-- Create the Trace table
CREATE TABLE [dbo].[Trace](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionId] [nvarchar](MAX) NULL,
	[ServerId] [nvarchar](MAX) NULL,
	[ClientId] [nvarchar](MAX) NULL,
	[HttpMethod] [nvarchar](16) NULL,
	[HttpPath] [nvarchar](MAX) NULL,
	[HttpStatusCode] [int] NULL,
	[ActionDescriptor] [nvarchar](MAX) NULL,
	[RemoteAddress] [nvarchar](MAX) NULL,
	[JsonRequest] [nvarchar](MAX) NULL,
	[JsonResponse] [nvarchar](MAX) NULL,
	[TimeStamp] [datetime2](7) NULL,
	[Exception] [nvarchar](MAX) NULL,
	[ExecutionTime] [decimal](18,3) NULL,
	CONSTRAINT [PK_Trace] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY];

-- Index for chronological queries (sorting/filtering by timestamp)
CREATE NONCLUSTERED INDEX [IX_Trace_TimeStamp]
ON [dbo].[Trace]([TimeStamp]);
GO

-- Index for client-based queries
CREATE NONCLUSTERED INDEX [IX_Trace_ClientId]
ON [dbo].[Trace]([ClientId]);
GO

-- Index for transaction-based queries
CREATE NONCLUSTERED INDEX [IX_Trace_TransactionId]
ON [dbo].[Trace]([TransactionId]);
GO

-- Index for HTTP status code analysis
CREATE NONCLUSTERED INDEX [IX_Trace_HttpStatusCode]
ON [dbo].[Trace]([HttpStatusCode]);
GO