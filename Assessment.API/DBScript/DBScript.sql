CREATE DATABASE WebApiDB

CREATE TABLE [dbo].[ApiUser] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [UserName]             VARCHAR(256) NOT NULL,
    [NormalizedUserName]   VARCHAR(256) NOT NULL,
    [Email]                VARCHAR(256) NULL,
    [NormalizedEmail]      VARCHAR(256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         VARCHAR(MAX) NULL,
    [PhoneNumber]          VARCHAR(50)  NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [FirstName] VARCHAR(256) NULL, 
    [LastName] VARCHAR(256) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ApiUser_NormalizedUserName]
    ON [dbo].[ApiUser]([NormalizedUserName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApiUser_NormalizedEmail]
    ON [dbo].[ApiUser]([NormalizedEmail] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_ApiUser_FirstName]
    ON [dbo].[ApiUser]([FirstName] ASC);
	GO

	CREATE NONCLUSTERED INDEX [IX_ApiUser_LastName]
    ON [dbo].[ApiUser]([LastName] ASC);


GO

CREATE TABLE [dbo].[ApiUserRoles] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (256) NOT NULL,
    [NormalizedName] NVARCHAR (256) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ApiUserRoles_NormalizedName]
    ON [dbo].[ApiUserRoles]([NormalizedName] ASC);

