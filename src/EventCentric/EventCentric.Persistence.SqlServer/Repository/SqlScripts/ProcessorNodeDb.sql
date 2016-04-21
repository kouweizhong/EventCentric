﻿--=====================================================
-- README: This script can drop and create the schema. 
-- To achieve this, you need to specify the desired db 
-- name in 2 places bellow
--====================================================

USE [master]
GO


declare @dbName varchar(max);
--==========================
-- 1/2 CHANGE DB NAME HERE
--==========================
set @dbName = N'{dbName}';


-- Create database
-- More info: http://dba.stackexchange.com/questions/30349/how-to-drop-database-in-single-user-mode?rq=1
declare @createDbSql varchar(max);
set @createDbSql = N'

IF EXISTS (SELECT name FROM sys.databases WHERE name = N''' + @dbName + N''') 
ALTER DATABASE ' + @dbName  + ' SET MULTI_USER WITH ROLLBACK IMMEDIATE;

IF EXISTS (SELECT name FROM sys.databases WHERE name = N''' + @dbName + N''') 
ALTER DATABASE ' + @dbName  + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

IF EXISTS (SELECT name FROM sys.databases WHERE name = N''' + @dbName + N''')
DROP DATABASE [' + @dbName + '];

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N''' + @dbName + N''') 
CREATE DATABASE [' + @dbName +'];'


EXEC(@createDbSql);
GO

--==========================
-- 2/2 CHANGE DB NAME HERE
--==========================
USE [{dbName}]
GO

-- Create EventStore schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'EventStore')
EXECUTE sp_executesql N'CREATE SCHEMA [EventStore] AUTHORIZATION [dbo]';

-- Create EventStore.Events
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[EventStore].[Events]') AND type in (N'U'))
CREATE TABLE [EventStore].[Events](
	[StreamType] [nvarchar](255) NOT NULL,
	[StreamId] [uniqueidentifier] NOT NULL,
	[Version] [bigint] NOT NULL,
    [TransactionId] [uniqueidentifier] NOT NULL,
	[EventId] [uniqueidentifier] NOT NULL,
	CONSTRAINT EventStore_Events_EventId UNIQUE(EventId),
	[EventType] [nvarchar] (255) NOT NULL,
	[CorrelationId] [uniqueidentifier] NULL,
    [EventCollectionVersion] [bigint] NOT NULL,
    [LocalTime] [datetime] NOT NULL,
	[UtcTime] [datetime] NOT NULL,
	[RowVersion] [rowversion] NOT NULL,
	[Payload] [nvarchar] (max) NOT NULL

PRIMARY KEY CLUSTERED 
(
	[StreamType] ASC,
	[StreamId] ASC, 
    [Version] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY];

-- Create EventStore.Snapshots
IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[EventStore].[Snapshots]') AND type in (N'U'))
CREATE TABLE[EventStore].[Snapshots](
	[StreamType] [nvarchar](255) NOT NULL,
    [StreamId] [uniqueidentifier] NOT NULL,
    [Version] [bigint] NOT NULL,
    [Payload] [nvarchar](max) NULL,
    [CreationLocalTime] [datetime] NOT NULL,
    [UpdateLocalTime] [datetime] NOT NULL
PRIMARY KEY CLUSTERED
(
	[StreamType] ASC,
    [StreamId] ASC
)WITH(PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON[PRIMARY]
) ON[PRIMARY];

-- Create EventStore.Subscriptions
IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[EventStore].[SubscribedSources]') AND type in (N'U'))
CREATE TABLE[EventStore].[Subscriptions](
	[SubscriberStreamType] [nvarchar](128) NOT NULL,
	[StreamType] [nvarchar] (255) NOT NULL,
    [Url] [nvarchar] (500) NOT NULL,
	[Token] [nvarchar] (max) NOT NULL,
    [ProcessorBufferVersion] [bigint] NOT NULL,
    [IsPoisoned] [bit] NOT NULL,
	[WasCanceled] [bit] NOT NULL,
    [PoisonEventCollectionVersion] [bigint] NULL,
    [DeadLetterPayload] [nvarchar] (max) NULL,
    [ExceptionMessage] [nvarchar] (max) NULL,
    [CreationLocalTime] [datetime] NOT NULL,
    [UpdateLocalTime] [datetime] NOT NULL
PRIMARY KEY CLUSTERED
(
	[SubscriberStreamType] ASC,
	[StreamType] ASC
)WITH(PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON[PRIMARY]
) ON[PRIMARY];

-- Create EventStore.Inbox
IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[EventStore].[Inbox]') AND type in (N'U'))
CREATE TABLE[EventStore].[Inbox](
	[InboxId] [bigint] IDENTITY(1,1) NOT NULL,
	[InboxStreamType] [nvarchar](128) NOT NULL,
    [EventId] [uniqueidentifier] NOT NULL,
	CONSTRAINT EventStore_Inbox_EventId UNIQUE(EventId),
    [TransactionId] [uniqueidentifier] NOT NULL,
	[StreamType] [nvarchar] (255) NOT NULL,
    [StreamId] [uniqueidentifier] NOT NULL,
    [Version] [bigint] NOT NULL,
    [EventType] [nvarchar] (255) NOT NULL,
    [EventCollectionVersion] [bigint] NOT NULL,
    [Ignored] [bit] NULL,
    [CreationLocalTime] [datetime] NOT NULL,
	[Payload] [nvarchar] (max) NOT NULL

PRIMARY KEY CLUSTERED
(
    [InboxId] ASC
)WITH(PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON[PRIMARY]
) ON[PRIMARY];



