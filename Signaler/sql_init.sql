use [master]
GO

DROP DATABASE IF EXISTS signaler;
GO

CREATE DATABASE signaler;
GO

use signaler;
GO

CREATE TABLE [dbo].[User]
(
	[username] VARCHAR(255) NOT NULL PRIMARY KEY,
	[password] VARCHAR(255) NOT NULL
);
GO

INSERT INTO [dbo].[User]
VALUES ('Mama', '123'),
	   ('Sara', '123'),
       ('Jesa', '123');
GO

CREATE TABLE [dbo].[Group] (
    [groupID] INT IDENTITY (1, 1) NOT NULL,
	[type] varchar(20) NOT NULL CHECK ([type] IN ('Group', 'Chat')),
	[creator] varchar(255), -- Used onlt when type is GROUP
	[groupName] varchar(255), -- Used onlt when type is GROUP
    PRIMARY KEY CLUSTERED ([groupID] ASC),
	FOREIGN KEY ([creator]) REFERENCES [dbo].[User]([username])
);
GO

CREATE TABLE [dbo].[BelongsTo]
(
	[username] VARCHAR(255) NOT NULL,
	[groupID] INT NOT NULL,
	PRIMARY KEY([username], [groupID]), 
    CONSTRAINT [FK_USER_USERNAME] FOREIGN KEY ([username]) REFERENCES [dbo].[User]([username]),
    CONSTRAINT [FK_GROUP_GROUPID] FOREIGN KEY ([groupID]) REFERENCES [dbo].[Group]([groupId]),
);
GO

CREATE TABLE [dbo].[Message] (
    [messageID]      INT           NOT NULL IDENTITY(1, 1),
    [messageText]        VARCHAR (255) NOT NULL,
    [senderUsername] VARCHAR (255) NOT NULL,
    [groupID]        INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([messageID] ASC),
    CONSTRAINT [FK_BELONGSTO_ID] FOREIGN KEY ([senderUsername], [groupID]) REFERENCES [dbo].[BelongsTo] ([username], [groupID])
);