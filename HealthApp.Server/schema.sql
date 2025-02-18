IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [ReferenceValue] nvarchar(max) NOT NULL,
    [Global] bit NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Conclusions] (
    [Id] int NOT NULL IDENTITY,
    [IssueDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Conclusions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [People] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Surname] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Birthday] datetime2 NOT NULL,
    CONSTRAINT [PK_People] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Practices] (
    [Id] int NOT NULL IDENTITY,
    [Practice] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Practices] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [PersonId] int NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUsers_People_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [People] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Doctors] (
    [Id] int NOT NULL IDENTITY,
    [LicenseExpiration] datetime2 NOT NULL,
    [PersonId] int NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_People_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [People] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Patients] (
    [PersonId] int NOT NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([PersonId]),
    CONSTRAINT [FK_Patients_People_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [People] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Licences] (
    [Id] int NOT NULL IDENTITY,
    [IssueDate] datetime2 NOT NULL,
    [ExpirationDate] datetime2 NOT NULL,
    [PracticesId] int NOT NULL,
    CONSTRAINT [PK_Licences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Licences_Practices_PracticesId] FOREIGN KEY ([PracticesId]) REFERENCES [Practices] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    [Discriminator] nvarchar(34) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Issues] (
    [Id] int NOT NULL IDENTITY,
    [Patient] int NOT NULL,
    [LeadDoctor] int NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Issues] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Issues_Doctors_LeadDoctor] FOREIGN KEY ([LeadDoctor]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Issues_Patients_Patient] FOREIGN KEY ([Patient]) REFERENCES [Patients] ([PersonId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [DoctorsLicences] (
    [Id] int NOT NULL IDENTITY,
    [DoctorId] int NOT NULL,
    [LicenceId] int NOT NULL,
    CONSTRAINT [PK_DoctorsLicences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DoctorsLicences_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DoctorsLicences_Licences_LicenceId] FOREIGN KEY ([LicenceId]) REFERENCES [Licences] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Analyses] (
    [Id] int NOT NULL IDENTITY,
    [Issue] int NOT NULL,
    [Category] int NOT NULL,
    [Value] float NOT NULL,
    CONSTRAINT [PK_Analyses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Analyses_Categories_Category] FOREIGN KEY ([Category]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Analyses_Issues_Issue] FOREIGN KEY ([Issue]) REFERENCES [Issues] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Appointments] (
    [Id] int NOT NULL IDENTITY,
    [Issue] int NOT NULL,
    [Doctor] int NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NULL,
    [Status] nvarchar(max) NOT NULL,
    [PreviousAppointmentId] int NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_Appointments_PreviousAppointmentId] FOREIGN KEY ([PreviousAppointmentId]) REFERENCES [Appointments] ([Id]),
    CONSTRAINT [FK_Appointments_Doctors_Doctor] FOREIGN KEY ([Doctor]) REFERENCES [Doctors] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Appointments_Issues_Issue] FOREIGN KEY ([Issue]) REFERENCES [Issues] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Diagnoses] (
    [Id] int NOT NULL IDENTITY,
    [Issue] int NOT NULL,
    [Doctor] int NOT NULL,
    CONSTRAINT [PK_Diagnoses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Diagnoses_Doctors_Doctor] FOREIGN KEY ([Doctor]) REFERENCES [Doctors] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Diagnoses_Issues_Issue] FOREIGN KEY ([Issue]) REFERENCES [Issues] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AnalysisConclusions] (
    [Id] int NOT NULL IDENTITY,
    [Analysis] int NOT NULL,
    [Conclusion] int NOT NULL,
    [Doctor] int NOT NULL,
    CONSTRAINT [PK_AnalysisConclusions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AnalysisConclusions_Analyses_Analysis] FOREIGN KEY ([Analysis]) REFERENCES [Analyses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AnalysisConclusions_Conclusions_Conclusion] FOREIGN KEY ([Conclusion]) REFERENCES [Conclusions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AnalysisConclusions_Doctors_Doctor] FOREIGN KEY ([Doctor]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AppointmentComments] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [CommentText] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentComments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentComments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [Diagnosis] int NOT NULL,
    [Dose] nvarchar(max) NULL,
    [Medicine] nvarchar(max) NOT NULL,
    [ExpirationDate] datetime2 NULL,
    [CreatedDate] datetime2 NOT NULL,
    [Overturned] bit NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_Diagnoses_Diagnosis] FOREIGN KEY ([Diagnosis]) REFERENCES [Diagnoses] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Global', N'Name', N'ReferenceValue') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [Global], [Name], [ReferenceValue])
VALUES (1, CAST(1 AS bit), N'Infection', N'Bacterial/Viral'),
(2, CAST(1 AS bit), N'Chronic', N'Diabetes, Hypertension'),
(3, CAST(0 AS bit), N'Acute', N'Surgery Required');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Global', N'Name', N'ReferenceValue') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IssueDate') AND [object_id] = OBJECT_ID(N'[Conclusions]'))
    SET IDENTITY_INSERT [Conclusions] ON;
INSERT INTO [Conclusions] ([Id], [IssueDate])
VALUES (1, '2023-11-01T00:00:00.0000000'),
(2, '2023-12-05T00:00:00.0000000');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IssueDate') AND [object_id] = OBJECT_ID(N'[Conclusions]'))
    SET IDENTITY_INSERT [Conclusions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Birthday', N'Gender', N'Name', N'Surname') AND [object_id] = OBJECT_ID(N'[People]'))
    SET IDENTITY_INSERT [People] ON;
INSERT INTO [People] ([Id], [Birthday], [Gender], [Name], [Surname])
VALUES (1, '0001-01-01T00:00:00.0000000', N'Male', N'John', N'Doe'),
(2, '0001-01-01T00:00:00.0000000', N'Female', N'Jane', N'Smith');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Birthday', N'Gender', N'Name', N'Surname') AND [object_id] = OBJECT_ID(N'[People]'))
    SET IDENTITY_INSERT [People] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Practice') AND [object_id] = OBJECT_ID(N'[Practices]'))
    SET IDENTITY_INSERT [Practices] ON;
INSERT INTO [Practices] ([Id], [Practice])
VALUES (1, N'General Medicine'),
(2, N'Pediatrics'),
(3, N'Cardiology'),
(4, N'Neurology'),
(5, N'Orthopedics'),
(6, N'Dermatology'),
(7, N'Psychiatry');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Practice') AND [object_id] = OBJECT_ID(N'[Practices]'))
    SET IDENTITY_INSERT [Practices] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'LicenseExpiration', N'PersonId') AND [object_id] = OBJECT_ID(N'[Doctors]'))
    SET IDENTITY_INSERT [Doctors] ON;
INSERT INTO [Doctors] ([Id], [LicenseExpiration], [PersonId])
VALUES (1, '0001-01-01T00:00:00.0000000', 1),
(2, '0001-01-01T00:00:00.0000000', 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'LicenseExpiration', N'PersonId') AND [object_id] = OBJECT_ID(N'[Doctors]'))
    SET IDENTITY_INSERT [Doctors] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ExpirationDate', N'IssueDate', N'PracticesId') AND [object_id] = OBJECT_ID(N'[Licences]'))
    SET IDENTITY_INSERT [Licences] ON;
INSERT INTO [Licences] ([Id], [ExpirationDate], [IssueDate], [PracticesId])
VALUES (1, '2029-01-01T00:00:00.0000000', '2019-01-01T00:00:00.0000000', 1),
(2, '2031-05-15T00:00:00.0000000', '2021-05-15T00:00:00.0000000', 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ExpirationDate', N'IssueDate', N'PracticesId') AND [object_id] = OBJECT_ID(N'[Licences]'))
    SET IDENTITY_INSERT [Licences] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PersonId') AND [object_id] = OBJECT_ID(N'[Patients]'))
    SET IDENTITY_INSERT [Patients] ON;
INSERT INTO [Patients] ([PersonId])
VALUES (1),
(2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PersonId') AND [object_id] = OBJECT_ID(N'[Patients]'))
    SET IDENTITY_INSERT [Patients] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DoctorId', N'LicenceId') AND [object_id] = OBJECT_ID(N'[DoctorsLicences]'))
    SET IDENTITY_INSERT [DoctorsLicences] ON;
INSERT INTO [DoctorsLicences] ([Id], [DoctorId], [LicenceId])
VALUES (1, 1, 1),
(2, 2, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DoctorId', N'LicenceId') AND [object_id] = OBJECT_ID(N'[DoctorsLicences]'))
    SET IDENTITY_INSERT [DoctorsLicences] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'LeadDoctor', N'Patient', N'Status') AND [object_id] = OBJECT_ID(N'[Issues]'))
    SET IDENTITY_INSERT [Issues] ON;
INSERT INTO [Issues] ([Id], [LeadDoctor], [Patient], [Status])
VALUES (1, 1, 1, N'Open'),
(2, 2, 2, N'In Progress');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'LeadDoctor', N'Patient', N'Status') AND [object_id] = OBJECT_ID(N'[Issues]'))
    SET IDENTITY_INSERT [Issues] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Category', N'Issue', N'Value') AND [object_id] = OBJECT_ID(N'[Analyses]'))
    SET IDENTITY_INSERT [Analyses] ON;
INSERT INTO [Analyses] ([Id], [Category], [Issue], [Value])
VALUES (1, 1, 1, 100.0E0),
(2, 2, 2, 200.0E0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Category', N'Issue', N'Value') AND [object_id] = OBJECT_ID(N'[Analyses]'))
    SET IDENTITY_INSERT [Analyses] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Doctor', N'EndTime', N'Issue', N'PreviousAppointmentId', N'StartTime', N'Status') AND [object_id] = OBJECT_ID(N'[Appointments]'))
    SET IDENTITY_INSERT [Appointments] ON;
INSERT INTO [Appointments] ([Id], [Doctor], [EndTime], [Issue], [PreviousAppointmentId], [StartTime], [Status])
VALUES (1, 1, '2024-01-10T10:00:00.0000000', 1, NULL, '2024-01-10T09:00:00.0000000', N'Completed'),
(2, 2, '2024-02-15T15:00:00.0000000', 2, NULL, '2024-02-15T14:00:00.0000000', N'Scheduled');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Doctor', N'EndTime', N'Issue', N'PreviousAppointmentId', N'StartTime', N'Status') AND [object_id] = OBJECT_ID(N'[Appointments]'))
    SET IDENTITY_INSERT [Appointments] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Doctor', N'Issue') AND [object_id] = OBJECT_ID(N'[Diagnoses]'))
    SET IDENTITY_INSERT [Diagnoses] ON;
INSERT INTO [Diagnoses] ([Id], [Doctor], [Issue])
VALUES (1, 1, 1),
(2, 2, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Doctor', N'Issue') AND [object_id] = OBJECT_ID(N'[Diagnoses]'))
    SET IDENTITY_INSERT [Diagnoses] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Analysis', N'Conclusion', N'Doctor') AND [object_id] = OBJECT_ID(N'[AnalysisConclusions]'))
    SET IDENTITY_INSERT [AnalysisConclusions] ON;
INSERT INTO [AnalysisConclusions] ([Id], [Analysis], [Conclusion], [Doctor])
VALUES (1, 1, 1, 1),
(2, 2, 2, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Analysis', N'Conclusion', N'Doctor') AND [object_id] = OBJECT_ID(N'[AnalysisConclusions]'))
    SET IDENTITY_INSERT [AnalysisConclusions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AppointmentId', N'CommentText', N'CreatedAt') AND [object_id] = OBJECT_ID(N'[AppointmentComments]'))
    SET IDENTITY_INSERT [AppointmentComments] ON;
INSERT INTO [AppointmentComments] ([Id], [AppointmentId], [CommentText], [CreatedAt])
VALUES (1, 1, N'Patient is responding well to treatment.', '2024-02-01T10:00:00.0000000'),
(2, 2, N'Follow-up required in 2 weeks.', '2024-02-01T11:00:00.0000000'),
(3, 2, N'Blood test results pending.', '2024-02-02T09:30:00.0000000');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AppointmentId', N'CommentText', N'CreatedAt') AND [object_id] = OBJECT_ID(N'[AppointmentComments]'))
    SET IDENTITY_INSERT [AppointmentComments] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Diagnosis', N'Dose', N'ExpirationDate', N'Medicine', N'Overturned') AND [object_id] = OBJECT_ID(N'[Prescriptions]'))
    SET IDENTITY_INSERT [Prescriptions] ON;
INSERT INTO [Prescriptions] ([Id], [CreatedDate], [Diagnosis], [Dose], [ExpirationDate], [Medicine], [Overturned])
VALUES (1, '2024-01-15T00:00:00.0000000', 1, NULL, NULL, N'Antibiotics', CAST(0 AS bit)),
(2, '2024-02-18T00:00:00.0000000', 2, NULL, NULL, N'Painkillers', CAST(0 AS bit));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Diagnosis', N'Dose', N'ExpirationDate', N'Medicine', N'Overturned') AND [object_id] = OBJECT_ID(N'[Prescriptions]'))
    SET IDENTITY_INSERT [Prescriptions] OFF;
GO

CREATE INDEX [IX_Analyses_Category] ON [Analyses] ([Category]);
GO

CREATE INDEX [IX_Analyses_Issue] ON [Analyses] ([Issue]);
GO

CREATE INDEX [IX_AnalysisConclusions_Analysis] ON [AnalysisConclusions] ([Analysis]);
GO

CREATE INDEX [IX_AnalysisConclusions_Conclusion] ON [AnalysisConclusions] ([Conclusion]);
GO

CREATE INDEX [IX_AnalysisConclusions_Doctor] ON [AnalysisConclusions] ([Doctor]);
GO

CREATE INDEX [IX_AppointmentComments_AppointmentId] ON [AppointmentComments] ([AppointmentId]);
GO

CREATE INDEX [IX_Appointments_Doctor] ON [Appointments] ([Doctor]);
GO

CREATE INDEX [IX_Appointments_Issue] ON [Appointments] ([Issue]);
GO

CREATE INDEX [IX_Appointments_PreviousAppointmentId] ON [Appointments] ([PreviousAppointmentId]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [IX_AspNetUsers_PersonId] ON [AspNetUsers] ([PersonId]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Diagnoses_Doctor] ON [Diagnoses] ([Doctor]);
GO

CREATE INDEX [IX_Diagnoses_Issue] ON [Diagnoses] ([Issue]);
GO

CREATE UNIQUE INDEX [IX_Doctors_PersonId] ON [Doctors] ([PersonId]);
GO

CREATE INDEX [IX_DoctorsLicences_DoctorId] ON [DoctorsLicences] ([DoctorId]);
GO

CREATE INDEX [IX_DoctorsLicences_LicenceId] ON [DoctorsLicences] ([LicenceId]);
GO

CREATE INDEX [IX_Issues_LeadDoctor] ON [Issues] ([LeadDoctor]);
GO

CREATE INDEX [IX_Issues_Patient] ON [Issues] ([Patient]);
GO

CREATE INDEX [IX_Licences_PracticesId] ON [Licences] ([PracticesId]);
GO

CREATE INDEX [IX_Prescriptions_Diagnosis] ON [Prescriptions] ([Diagnosis]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250208024221_InitialCreate', N'8.0.2');
GO

COMMIT;
GO

