/*
    Script de inicialización completo - MundialDb
*/

USE [master];
GO

IF DB_ID(N'MundialDb') IS NULL
BEGIN
    CREATE DATABASE [MundialDb];
END
GO

USE [MundialDb];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -----------------------------------------------------------------------------
    -- 00_Estructura: MundialDB.sql
    -----------------------------------------------------------------------------
-- DROP SCHEMA dbo;
-- CREATE SCHEMA dbo;
-- MundialDb.dbo.Equipos definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Equipos;

CREATE TABLE MundialDb.dbo.Equipos (
	Nombre nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Confederacion nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RankingFifa int NOT NULL,
	Puntos int DEFAULT 0 NOT NULL,
	GolesAFavor int DEFAULT 0 NOT NULL,
	GolesEnContra int DEFAULT 0 NOT NULL,
	BanderaBase64 nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Equipos PRIMARY KEY (Nombre)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Equipos_Nombre ON
MundialDb.dbo.Equipos ( Nombre ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

ALTER TABLE MundialDb.dbo.Equipos WITH NOCHECK ADD CONSTRAINT CK_Equipo_Confederacion CHECK (([Confederacion] = 'UEFA'
    OR [Confederacion] = 'OFC'
    OR [Confederacion] = 'CONMEBOL'
    OR [Confederacion] = 'CONCACAF'
    OR [Confederacion] = 'CAF'
    OR [Confederacion] = 'AFC'));

ALTER TABLE MundialDb.dbo.Equipos WITH NOCHECK ADD CONSTRAINT CK_Equipo_RankingFifa CHECK (([RankingFifa] >=(300)
    AND [RankingFifa] <=(2500)));
-- MundialDb.dbo.Notificaciones definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Notificaciones;

CREATE TABLE MundialDb.dbo.Notificaciones (
	Id int IDENTITY(1, 1) NOT NULL,
	UsuarioCorreo nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Mensaje nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Leida bit NOT NULL,
	Fecha datetime2 NOT NULL,
	CONSTRAINT PK_Notificaciones PRIMARY KEY (Id)
);
-- MundialDb.dbo.Semillas definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Semillas;

CREATE TABLE MundialDb.dbo.Semillas (
	Token int NOT NULL,
	SemillaFixture int NOT NULL,
	SemillaCompletar int NOT NULL,
	SemillaCrucesFase int NOT NULL,
	SemillaSimulacion int NOT NULL,
	CONSTRAINT PK_Semillas PRIMARY KEY (Token)
);
-- MundialDb.dbo.Usuarios definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Usuarios;

CREATE TABLE MundialDb.dbo.Usuarios (
	Correo nvarchar(900) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Nombre nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Apellido nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Contrasena nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FechaNacimiento datetime2 NOT NULL,
	Roles nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Usuarios PRIMARY KEY (Correo)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Usuarios_Correo ON
MundialDb.dbo.Usuarios ( Correo ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;
-- MundialDb.dbo.[__EFMigrationsHistory] definition
-- Drop table
-- DROP TABLE MundialDb.dbo.[__EFMigrationsHistory];

CREATE TABLE MundialDb.dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);
-- MundialDb.dbo.Fixtures definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Fixtures;

CREATE TABLE MundialDb.dbo.Fixtures (
	Token int NOT NULL,
	SemillaToken int NOT NULL,
	UsaMotorProbabilistico bit DEFAULT CONVERT([bit],(0)) NOT NULL,
	CONSTRAINT PK_Fixtures PRIMARY KEY (Token),
	CONSTRAINT FK_Fixtures_Semillas_SemillaToken FOREIGN KEY (SemillaToken) REFERENCES MundialDb.dbo.Semillas(Token) ON
DELETE
    CASCADE
);

CREATE NONCLUSTERED INDEX IX_Fixtures_SemillaToken ON
MundialDb.dbo.Fixtures ( SemillaToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;
-- MundialDb.dbo.Logs definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Logs;

CREATE TABLE MundialDb.dbo.Logs (
	Id int IDENTITY(1, 1) NOT NULL,
	UsuarioCorreo nvarchar(900) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Accion nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Fecha datetime2 NOT NULL,
	CONSTRAINT PK_Logs PRIMARY KEY (Id),
	CONSTRAINT FK_Logs_Usuarios_UsuarioCorreo FOREIGN KEY (UsuarioCorreo) REFERENCES MundialDb.dbo.Usuarios(Correo) ON
DELETE
    CASCADE
);

CREATE NONCLUSTERED INDEX IX_Logs_UsuarioCorreo ON
MundialDb.dbo.Logs ( UsuarioCorreo ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;
-- MundialDb.dbo.Sorteos definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Sorteos;

CREATE TABLE MundialDb.dbo.Sorteos (
	Token int NOT NULL,
	SemillaToken int NOT NULL,
	CONSTRAINT PK_Sorteos PRIMARY KEY (Token),
	CONSTRAINT FK_Sorteos_Semillas_SemillaToken FOREIGN KEY (SemillaToken) REFERENCES MundialDb.dbo.Semillas(Token) ON
DELETE
    CASCADE
);

CREATE NONCLUSTERED INDEX IX_Sorteos_SemillaToken ON
MundialDb.dbo.Sorteos ( SemillaToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;
-- MundialDb.dbo.EquipoSorteo definition
-- Drop table
-- DROP TABLE MundialDb.dbo.EquipoSorteo;

CREATE TABLE MundialDb.dbo.EquipoSorteo (
	EquiposNombre nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	SorteoToken int NOT NULL,
	CONSTRAINT PK_EquipoSorteo PRIMARY KEY (EquiposNombre,
SorteoToken),
	CONSTRAINT FK_EquipoSorteo_Equipos_EquiposNombre FOREIGN KEY (EquiposNombre) REFERENCES MundialDb.dbo.Equipos(Nombre) ON
DELETE
    CASCADE,
    CONSTRAINT FK_EquipoSorteo_Sorteos_SorteoToken FOREIGN KEY (SorteoToken) REFERENCES MundialDb.dbo.Sorteos(Token) ON
    DELETE
        CASCADE
);

CREATE NONCLUSTERED INDEX IX_EquipoSorteo_SorteoToken ON
MundialDb.dbo.EquipoSorteo ( SorteoToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;
-- MundialDb.dbo.Grupos definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Grupos;

CREATE TABLE MundialDb.dbo.Grupos (
	Id nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Token int NOT NULL,
	CONSTRAINT PK_Grupos PRIMARY KEY (Id,
Token),
	CONSTRAINT FK_Grupos_Sorteos_Token FOREIGN KEY (Token) REFERENCES MundialDb.dbo.Sorteos(Token) ON
DELETE
    CASCADE
);

CREATE NONCLUSTERED INDEX IX_Grupos_Token ON
MundialDb.dbo.Grupos ( Token ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

ALTER TABLE MundialDb.dbo.Grupos WITH NOCHECK ADD CONSTRAINT CK_Grupo_Id CHECK (([Id] = 'L'
    OR [Id] = 'K'
    OR [Id] = 'J'
    OR [Id] = 'I'
    OR [Id] = 'H'
    OR [Id] = 'G'
    OR [Id] = 'F'
    OR [Id] = 'E'
    OR [Id] = 'D'
    OR [Id] = 'C'
    OR [Id] = 'B'
    OR [Id] = 'A'));
-- MundialDb.dbo.EquipoGrupo definition
-- Drop table
-- DROP TABLE MundialDb.dbo.EquipoGrupo;

CREATE TABLE MundialDb.dbo.EquipoGrupo (
	EquiposNombre nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	GrupoId nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	GrupoToken int NOT NULL,
	CONSTRAINT PK_EquipoGrupo PRIMARY KEY (EquiposNombre,
GrupoId,
GrupoToken),
	CONSTRAINT FK_EquipoGrupo_Equipos_EquiposNombre FOREIGN KEY (EquiposNombre) REFERENCES MundialDb.dbo.Equipos(Nombre) ON
DELETE
    CASCADE,
    CONSTRAINT FK_EquipoGrupo_Grupos_GrupoId_GrupoToken FOREIGN KEY (GrupoId,
    GrupoToken) REFERENCES MundialDb.dbo.Grupos(Id,
    Token) ON
    DELETE
        CASCADE
);

CREATE NONCLUSTERED INDEX IX_EquipoGrupo_GrupoId_GrupoToken ON
MundialDb.dbo.EquipoGrupo ( GrupoId ASC ,
GrupoToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;
-- MundialDb.dbo.Estadios definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Estadios;

CREATE TABLE MundialDb.dbo.Estadios (
	Nombre nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Id int NOT NULL,
	Ciudad nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Descripcion nvarchar(800) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Capacidad int NOT NULL,
	GrupoId nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	GrupoToken int NULL,
	CONSTRAINT PK_Estadios PRIMARY KEY (Nombre),
	CONSTRAINT FK_Estadios_Grupos_GrupoId_GrupoToken FOREIGN KEY (GrupoId,
GrupoToken) REFERENCES MundialDb.dbo.Grupos(Id,
Token)
);

CREATE NONCLUSTERED INDEX IX_Estadios_GrupoId_GrupoToken ON
MundialDb.dbo.Estadios ( GrupoId ASC ,
GrupoToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

CREATE UNIQUE NONCLUSTERED INDEX IX_Estadios_Nombre ON
MundialDb.dbo.Estadios ( Nombre ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

ALTER TABLE MundialDb.dbo.Estadios WITH NOCHECK ADD CONSTRAINT CK_Estadio_Capacidad CHECK (([Capacidad] >=(20000)));
-- MundialDb.dbo.FechasOcupadas definition
-- Drop table
-- DROP TABLE MundialDb.dbo.FechasOcupadas;

CREATE TABLE MundialDb.dbo.FechasOcupadas (
	Fecha datetime2 NOT NULL,
	EstadioNombre nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FixtureToken int DEFAULT 0 NOT NULL,
	CONSTRAINT PK_FechasOcupadas PRIMARY KEY (EstadioNombre,
Fecha,
FixtureToken),
	CONSTRAINT FK_FechasOcupadas_Estadios_EstadioNombre FOREIGN KEY (EstadioNombre) REFERENCES MundialDb.dbo.Estadios(Nombre) ON
DELETE
    CASCADE
);
-- MundialDb.dbo.Partidos definition
-- Drop table
-- DROP TABLE MundialDb.dbo.Partidos;

CREATE TABLE MundialDb.dbo.Partidos (
	Id int NOT NULL,
	Token int NOT NULL,
	GrupoID nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Fecha datetime2 NOT NULL,
	EstadioNombre nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EquipoLocalNombre nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EquipoVisitanteNombre nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	GolesLocal int NULL,
	GolesVisitante int NULL,
	Fase nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Horario nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Jornada int NULL,
	PenalesLocal int NULL,
	PenalesVisitante int NULL,
	TarjetasAmarillasLocal int NOT NULL,
	TarjetasRojasLocal int NOT NULL,
	TarjetasAmarillasVisitante int NOT NULL,
	TarjetasRojasVisitante int NOT NULL,
	GrupoToken int NULL,
	CONSTRAINT PK_Partidos PRIMARY KEY (Id,
Token),
	CONSTRAINT FK_Partidos_Equipos_EquipoLocalNombre FOREIGN KEY (EquipoLocalNombre) REFERENCES MundialDb.dbo.Equipos(Nombre),
	CONSTRAINT FK_Partidos_Equipos_EquipoVisitanteNombre FOREIGN KEY (EquipoVisitanteNombre) REFERENCES MundialDb.dbo.Equipos(Nombre),
	CONSTRAINT FK_Partidos_Estadios_EstadioNombre FOREIGN KEY (EstadioNombre) REFERENCES MundialDb.dbo.Estadios(Nombre),
	CONSTRAINT FK_Partidos_Grupos_GrupoID_GrupoToken FOREIGN KEY (GrupoID,
GrupoToken) REFERENCES MundialDb.dbo.Grupos(Id,
Token)
);

CREATE NONCLUSTERED INDEX IX_Partidos_EquipoLocalNombre ON
MundialDb.dbo.Partidos ( EquipoLocalNombre ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

CREATE NONCLUSTERED INDEX IX_Partidos_EquipoVisitanteNombre ON
MundialDb.dbo.Partidos ( EquipoVisitanteNombre ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

CREATE NONCLUSTERED INDEX IX_Partidos_EstadioNombre ON
MundialDb.dbo.Partidos ( EstadioNombre ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

CREATE NONCLUSTERED INDEX IX_Partidos_GrupoID_GrupoToken ON
MundialDb.dbo.Partidos ( GrupoID ASC ,
GrupoToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_TarjetasRojasVisitante CHECK (([TarjetasRojasVisitante] IS NULL
    OR [TarjetasRojasVisitante] >=(0)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_Fase CHECK (([Fase] = 'Final'
    OR [Fase] = 'Tercer y Cuarto Puesto'
    OR [Fase] = 'Semifinal'
    OR [Fase] = 'Cuartos de Final'
    OR [Fase] = 'Octavos de Final'
    OR [Fase] = 'Dieciseisavos de Final'
    OR [Fase] = 'Fase de Grupos'));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_GolesLocal CHECK (([GolesLocal] IS NULL
    OR [GolesLocal] >=(0)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_GolesVisitante CHECK (([GolesVisitante] IS NULL
    OR [GolesVisitante] >=(0)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_Jornada CHECK (([Jornada] IS NULL
    OR [Jornada] >=(1)
        AND [Jornada] <=(3)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_PenalesLocal CHECK (([PenalesLocal] IS NULL
    OR [PenalesLocal] >=(0)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_PenalesVisitante CHECK (([PenalesVisitante] IS NULL
    OR [PenalesVisitante] >=(0)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_TarjetasAmarillasLocal CHECK (([TarjetasAmarillasLocal] IS NULL
    OR [TarjetasAmarillasLocal] >=(0)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_TarjetasAmarillasVisitante CHECK (([TarjetasAmarillasVisitante] IS NULL
    OR [TarjetasAmarillasVisitante] >=(0)));

ALTER TABLE MundialDb.dbo.Partidos WITH NOCHECK ADD CONSTRAINT CK_Partido_TarjetasRojasLocal CHECK (([TarjetasRojasLocal] IS NULL
    OR [TarjetasRojasLocal] >=(0)));
-- MundialDb.dbo.EstadioFixture definition
-- Drop table
-- DROP TABLE MundialDb.dbo.EstadioFixture;

CREATE TABLE MundialDb.dbo.EstadioFixture (
	EstadiosNombre nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FixtureToken int NOT NULL,
	CONSTRAINT PK_EstadioFixture PRIMARY KEY (EstadiosNombre,
FixtureToken),
	CONSTRAINT FK_EstadioFixture_Estadios_EstadiosNombre FOREIGN KEY (EstadiosNombre) REFERENCES MundialDb.dbo.Estadios(Nombre) ON
DELETE
    CASCADE,
    CONSTRAINT FK_EstadioFixture_Fixtures_FixtureToken FOREIGN KEY (FixtureToken) REFERENCES MundialDb.dbo.Fixtures(Token) ON
    DELETE
        CASCADE
);

CREATE NONCLUSTERED INDEX IX_EstadioFixture_FixtureToken ON
MundialDb.dbo.EstadioFixture ( FixtureToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;
-- MundialDb.dbo.EstadioSorteo definition
-- Drop table
-- DROP TABLE MundialDb.dbo.EstadioSorteo;

CREATE TABLE MundialDb.dbo.EstadioSorteo (
	EstadiosNombre nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	SorteoToken int NOT NULL,
	CONSTRAINT PK_EstadioSorteo PRIMARY KEY (EstadiosNombre,
SorteoToken),
	CONSTRAINT FK_EstadioSorteo_Estadios_EstadiosNombre FOREIGN KEY (EstadiosNombre) REFERENCES MundialDb.dbo.Estadios(Nombre) ON
DELETE
    CASCADE,
    CONSTRAINT FK_EstadioSorteo_Sorteos_SorteoToken FOREIGN KEY (SorteoToken) REFERENCES MundialDb.dbo.Sorteos(Token) ON
    DELETE
        CASCADE
);

CREATE NONCLUSTERED INDEX IX_EstadioSorteo_SorteoToken ON
MundialDb.dbo.EstadioSorteo ( SorteoToken ASC )  
	 WITH ( PAD_INDEX = OFF ,
FILLFACTOR = 100 ,
SORT_IN_TEMPDB = OFF ,
IGNORE_DUP_KEY = OFF ,
STATISTICS_NORECOMPUTE = OFF ,
ONLINE = OFF ,
ALLOW_ROW_LOCKS = ON
,
ALLOW_PAGE_LOCKS = ON
)
	 ON
[PRIMARY] ;

    -----------------------------------------------------------------------------
    -- 01_Historial de migraciones EF: ___EFMigrationsHistory__202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.[__EFMigrationsHistory] (MigrationId,ProductVersion) VALUES
	 (N'20260615144438_InitialCreate',N'8.0.27'),
	 (N'20260615151450_FixtureNoCascadePartidos',N'8.0.27'),
	 (N'20260615203125_MotorProbabilisticoYEstadioFixture',N'8.0.27'),
	 (N'20260616125649_FechaOcupadaPorFixture',N'8.0.27');

    -----------------------------------------------------------------------------
    -- 02_Semillas: Semillas_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Semillas (Token,SemillaFixture,SemillaCompletar,SemillaCrucesFase,SemillaSimulacion) VALUES
	 (1,1,1,1,1);

    -----------------------------------------------------------------------------
    -- 03_Usuarios: Usuarios_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Usuarios (Correo,Nombre,Apellido,Contrasena,FechaNacimiento,Roles) VALUES
	 (N'admin1@mundial.com',N'Ana',N'Lopez',N'Admin#123','1985-03-12 00:00:00.0000000',N'Administrador'),
	 (N'admin2@mundial.com',N'Bruno',N'Diaz',N'Admin#456','1988-07-04 00:00:00.0000000',N'Administrador'),
	 (N'editor1@mundial.com',N'Carla',N'Mendez',N'Editor#123','1990-01-20 00:00:00.0000000',N'Editor'),
	 (N'editor2@mundial.com',N'Diego',N'Rossi',N'Editor#456','1992-11-09 00:00:00.0000000',N'Editor'),
	 (N'periodista1@mundial.com',N'Elena',N'Soto',N'Peri#1234','1991-05-15 00:00:00.0000000',N'Periodista'),
	 (N'periodista2@mundial.com',N'Felipe',N'Vega',N'Peri#5678','1993-09-27 00:00:00.0000000',N'Periodista');

    -----------------------------------------------------------------------------
    -- 04_Equipos: Equipos_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Equipos (Nombre,Confederacion,RankingFifa,Puntos,GolesAFavor,GolesEnContra,BanderaBase64) VALUES
	 (N'AFC_01',N'AFC',2050,3,4,7,N''),
	 (N'AFC_02',N'AFC',1572,4,3,6,N''),
	 (N'AFC_03',N'AFC',1451,4,6,6,N''),
	 (N'AFC_04',N'AFC',1124,1,3,8,N''),
	 (N'AFC_05',N'AFC',2186,6,8,6,N''),
	 (N'AFC_06',N'AFC',1689,0,7,12,N''),
	 (N'AFC_07',N'AFC',858,4,4,6,N''),
	 (N'AFC_08',N'AFC',1423,1,4,6,N''),
	 (N'Argentina',N'CONMEBOL',1900,6,5,2,N'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'),
	 (N'CAF_01',N'CAF',1452,3,9,11,N'');
INSERT INTO MundialDb.dbo.Equipos (Nombre,Confederacion,RankingFifa,Puntos,GolesAFavor,GolesEnContra,BanderaBase64) VALUES
	 (N'CAF_02',N'CAF',399,0,2,10,N''),
	 (N'CAF_03',N'CAF',482,6,8,6,N''),
	 (N'CAF_04',N'CAF',1501,9,8,3,N''),
	 (N'CAF_05',N'CAF',1890,4,7,8,N''),
	 (N'CAF_06',N'CAF',2356,2,1,2,N''),
	 (N'CAF_07',N'CAF',2095,7,7,2,N''),
	 (N'CAF_08',N'CAF',902,6,8,5,N''),
	 (N'CAF_09',N'CAF',1768,6,8,6,N''),
	 (N'CONCACAF_01',N'CONCACAF',1312,4,5,6,N''),
	 (N'CONCACAF_02',N'CONCACAF',1454,4,5,4,N'');
INSERT INTO MundialDb.dbo.Equipos (Nombre,Confederacion,RankingFifa,Puntos,GolesAFavor,GolesEnContra,BanderaBase64) VALUES
	 (N'CONCACAF_03',N'CONCACAF',1059,6,6,3,N''),
	 (N'CONCACAF_04',N'CONCACAF',2224,9,10,3,N''),
	 (N'CONCACAF_05',N'CONCACAF',382,7,5,0,N''),
	 (N'CONCACAF_06',N'CONCACAF',1177,1,2,8,N''),
	 (N'CONCACAF_07',N'CONCACAF',1032,3,3,5,N''),
	 (N'CONMEBOL_01',N'CONMEBOL',1040,2,3,4,N''),
	 (N'CONMEBOL_02',N'CONMEBOL',651,1,3,5,N''),
	 (N'CONMEBOL_03',N'CONMEBOL',2055,6,8,6,N''),
	 (N'CONMEBOL_04',N'CONMEBOL',1603,0,5,12,N''),
	 (N'CONMEBOL_05',N'CONMEBOL',618,5,5,4,N'');
INSERT INTO MundialDb.dbo.Equipos (Nombre,Confederacion,RankingFifa,Puntos,GolesAFavor,GolesEnContra,BanderaBase64) VALUES
	 (N'Espana',N'UEFA',1875,9,9,1,N'UklGRl4bAABXRUJQVlA4IFIbAAAQiACdASr0AXcBPm02mEkkIyKhIphI+IANiWNu4XaxDOMf0n8gNon6H+VXsn1T+ofgv+4/tT0SdXeVJyn/sP7P+T3y8/rX96/p3uc/Lv+6/ufwEfqd/pv7t/c/bS9Uv7NeoL+df3T/1f4j3af8z/yP8R7nf7b/pvyA+QL+kf4f/0dhf+7vsFft5///XZ/a74TP7L/wv299pz/8dnv00/VD+pdtHff44vcsjlvo/wPQ36vIs/+j/gvFn4dagvrz/XeKHsjra+gF7E/XP+14a2ov4W9gDyz74T8N6gXinZ6PrD2C/2L61fpGi6L3Fnfn2uXf9xZ359rl3/cWd+fa5d/3Fnfn2uXf9xZ359rl3/cWd+fa5d/3Fnfn2uXf9xZ359rl3/cWd+fa5d/3Fnfn2uXf9xZ31/B2mew+hEnIavvudkZW60txhhF2TsPoRJyGr77nZGVutLcYYRdk7D6ESchq++52RlbrS3GGEXZOw+hEnIavwWTxv+ahh/zUMP+ahh/zUMP+ahh/zTrrUFM9wc0bvkhmDj++qXfPp/a5d/3Fnfn2uXf9qYDo+l1Maxup3zfgiY4YjJZTjWAupPtPsTCU/QhvvWkgN89dnnRd/3Fnfn2uXf9xX0jYdI6ybw/BxGMaLbeGc2sD3l18wQmmCp4ew0aaSH4T/+XQWelkpgxwYTSZFEOLO/Ptcu/7USfy1Pdw+TGKT7TTvx5Zoue6SMyrKdDYC4xEj+fCHNzGLO6g+Gt1fvRUJmPepM5xTTwcJE/tcu/7izvz7XLXZPZqOIzb8qaajxfsNJHdLnr0i4inTJaGrSCLciWLi7NqG/xBh2jeWfIhiXOYawohxZ359rl3/bKPSbNu394SrDHb7Jk4xVxIdGZbFDMU6xKpYpxHst5IDxCubl2fTGAgo3zmSZ27K0nLDbCe0ZFtAOn/HPPGETIO9xZ359rl3/cWhRspAkU37587zrl1uztFjUA4gu5uabeBXNmrhufZFO343GA06zq235Yk+LH5//omcjYg92YE6EsbuyKBozjUE5/3Fnfn2uXf9xXRVfDbYXko1pws2Q2NdRy6t1CErWZ+NrQYlIitZV21CU/aq9SozDJVIlZcKrXJo3Fnfn2uXf9xZ29EUPZzDwoAzoVE6zJOeP0K2aRGz2rHdrFjwBYcCIRNgVtzrm0gDnI5vIFqFNEkZz0W8d+fa5d/3Fnfn2OigW8YtnplxKMKZQNgh8GKTUC0Afe+lN7FE+SUv+4CGjYYGMuaXMyP7fJ1+bI7KeLO/Ptcu/45oa5JWlNIsZ5RabI/MUP1Y9VUeR/WVa135IZ5VrXfkhnlWtd+SGeZ8XFAPArzfG/5qGH/NQw/5qGH/NQw7L3Fnfn2uXf9xZ359rl3/cWd+fa5d/3Fnfn2uXf9xZ359rl3/cWd+fa5d/3Fnfn2uXf9xZ359rl3/cWd+fa5d/3Fnfn2uXf9xZ31AAD+/3ax6IxFq1GU/uy7+Vz/5XP8k4AAAAAAAAAABFaC/sOg/dI5Zla6kz+TYCdzK45NK7xs2dqlfGqQasNTBqeSgDtFE6vc7KD8qkmUH5VJMoPyqSZQflUkyg/KpJlB+VSTKD8qkmUH5VJMoPyqSZQflUkyg/KpJlB+VSTKzFmdxJ/F+TVWp0gAAN2OcQ2AAAAAAAOROHZ7FTJ+OC6ngBNVGulizChlpwpbhGxMBJimy4sJEkK5HTnGtE937MZFp3KzmCTvmHcfi0o3JVxZz9ZqXy/9KcI6Ytjt2m83aGVWiSW9nsCUmusRA7zkmGlNTdDeefPx11jFMqgZKhiwcs2X2L1SyqblImqCWzG2zw+VAA4MyklpU6TLyDfs3TFk4sfFhPkloqagUIefbAlA0h8bZqLNRnMpxTASlCj4DONiU2xK8IEFm5E5YF8+xnqXVtKjr6f2vGNPSpjUcQupvq3k21msBWj7TtR7tbL+n3vQ5sbp80OFmxrzeAAyHG6+BswkOqvoFYfcEx81eV7jw4CvHG6oW9TgdpdRnjjdJIOfIgIYSb9eJHAUdy2mU1se/vGlc1dADzKi4/0tqRILqABejXVTUfa5CF9bSfu5XKCO+20kqXBv72FEC006KQqP0WqX3pRSIVNiQQZ41GgSyfW5zjLfxOLsX14fjYeKh7ug2BKSn/KdZzzUeUt6Wh9MAq4KWstrpLUbPcMiVzrCimwZGd7G1v8yLPRNxjiG0ulJayAHoBe2oQi9rByyxa2jjXncvlr29VxE6c6MQztunAHPjHmWX9V/U+AJTHtTaNx33/y0AUjvIbwpDB/vvehR1yB8lEhcc8VBVWuFHcJUQWN7Kxr0kuXu/jQMNP+fIHQJzL9LfMa57oSrpfFn4eAN4aU44WjjPrLtA74hRGzXvMStJFblO8BAH5xSEOzrasJ+Hy7wmflOK7jkEHCSdNQScQJewXj1OBZpzw2Wni7dv+haGolAJFcX2FcwnzOONApoGG3gC4oHiuxGBZBaoAfhSO0jkq8WAGZckAXwUl+Z9dbc4YvumEEw0wnYoauVr4krEhantClgqMLrovapmtUTtoJJbuLSLZFB1aZz3D178dPyvK7flF7FyyEBVvQIsf2LxFHUYVi4OLpbLQx9mIzVfECLQH/sUwKqyVTiDTNAZga4Y8vtt57+Flo29LRncDn1jPkjB26Jq/uIUINomm/yeeqCSiCqs9tOS8yxVhUkDBHdoatljkVEsOknckGBalVnABjxnajSmIwLiZztdJB//ns8AK1fQzOGK3c1t+GKmChuJ3oMmzUGs0xyRqkFnWcGz2bSFg4FGDCUqxH2CfG+jea+KD12GHkWVIfRga0mI4/xx+CJFkV39RqWzmrD2o59UYiK7ZUkMGELE4X/1yqmxoytplL0axneuMPfBWXeqo+6yncfvqNoarauBXlZo5fpjRlnFFSMQMngwUNzW+U+EPz5haOTF/u0X7SI40nvs4pRurHR1y0wh++slS51WXLZXNbz2WoNDWa4gRStwt7napKExzncg4cg2QWBKlz/07nAtRa6+UZ7pT+q5aiNiB1/Ib7eEv/gdFQSqhOq4cw1vZZpz6rKLQUcrbIKl+3Mot6FZu5/bCPE3dOVll2hFYrSKxqViVSzTnoR+j4womDtQdlJ0MUH4CA6jrSyYlVx8SHQftxwzSkX/scE480djdirnJy0PQeLHsrpUcoWbgBbZi5j4Wo1MnQamyiceYcLsAU+AFkyog29YBdV+S9H8SC8cB7Xtu52AMdt9xDn94AFOOOc/xpsrHHgkRbEehEtHQiWkX/08MtobOTEK79Xdj4UhuSboHrOo7obCjDUU4FbTHHAPKSNqII+JR7TwfGkl/C0pXx7rGtS2+MjdcvtLdfu161QbR/KIiDgIgtewHsprJPbjXer7HqYmd6oVxQEN1++JBol4O4JaXZ/ATQW5b5OHv1vIIB+6rV5NE0IcOT3KeRQD3ZBCzE8ax6a09E3BnU0XgcnthMKzPPx5DiQVnmEW4x2VTxCMJld6tT1ww/G74Ho358vz9l108C0U7emcDLZonMrqFWfrHb4K/l0xMDwfE58rybVEL0IPILkN3J/zAanY5PjPK4GTJTuMPvEUU6Ftb1maXTlrkHoP0Ht0ocJj4RyyFGwUn6JUGuytonJrT78sNHI0yDqqSxkwhvn0QfPy6YxzeXZRr+nUBmyoBjCETS7J5u08cd8HOQuuRtT5h/X2++CvzumAZpmgK3+dOai70yadIsnHtVtyOh/rtRWSkcPOk2qK4F90W/zfV/O14s2iiQWK+xOc0XAetk/sTXe5NdWEWkFp78rLSmCmcBmTQUJX6ilN33in/Ndfgc5AIdDI/NytyWlP/QORrFkRHtZGLdcKZwdjiIh3s7HeNe3VmOXLoiZaHLoekGhlaPjNkDX2H+1Hxdcdzvy4FQb2m4ayNdilnAg0sZwbzFzSdiZYMSVuL7X4DEWx01kgZa8H1tWRDbPxSmXaB1b5jT/nf+0RPBRBBodBTsd5zL7pom6CmKZbwuPHXFjEXdS00MflDTs7o+KK9YMhi9Oronc2z8wW1S0LX/yHNbha8tkvYgxzsvPzKUffYO3xyjFFm4qine/cpDPVrTDyXNpgGbzRaSr1E0ZDElaeuc+JmsFbdk5zR/e1Z8tkfCVb4Bm9x2B3bNlDWuFwuulUfuvy9t+lyf5mwYUKZ4mYm3DYhd8Z62U0K8DytwwfGG3HM0FaBKez55fF+F2yZC4h8jwC/BvXkY9mVVxVA94lYayostMUDZ6nd1pDI9ObOEvw3XE0GikFH9HZ/tw7qn6Z2YUp33NP1w8szeDp5u93uTyYejtYJKsTIWdD3H2EE+XBfszpJtpl5LT2Lwm0VJZ1TlIts4RmD/sbzLctiDXSIGDaTfp1oTQrrcBDFfC41gt5P9zLl+TDH9nfAWR40MN9Y6x3uu0Mg767eJ6LtHzw3Ut4XlPzf147suPtufBz/emmnasN3jByJdGNe8SK5p3/2s5IdDaElueNRDZtdTbpknr9j36jlvFUP8FK0M3nFCT5QSEyHgOt0QsZ60YEkjpA1yJRobOOLiP3XqdiIdbfXtbCmqz6n+sGccGi7EDLJvHTlIPwj4JZMS74YjBd26j2+ybkE4fYFP4F3QMWxVBDrLEe0Z2Hz2xUdTpXQEC8c7MB7sEerW7ItUkndZNRFL6SSiWr7JXto+UQJt5CPippMnvrIXc6h5QO0zKIKz22mKkrJ2khwpP/Jagt3+xsh4j/Eih6IP4yaG/uRbJukokSVQbhHLje/mt8ihXiC1BVmrD0ChRCKuQIAyHI7GNeSTHRRwsw0YcC/AKifVlKGw3nwnAODnul78SlnefDFdG8g3JBA5vrgPKxeDLIjri287bYwpwv6Cc53IqK3g37QAoEay+pWYtTlilvDITNBVnEbf4ng1tTAGNy24Xj81dkrIGVPJNFFpytHzkMSATGl3udoNkrgagA1ognDgImNpa81Q4eXUn/YJTN6PzNcFL4uBtaw1+KWYOZ5KsvO/lWPwCynTL37wg0J1UQEiWJGXxCmLWFX1T5pzZ54xMKr5u+HDwIzLMLLMQk7N5hd4YIurkO9+AxGp6GjBs8y4KTrljcc1ZPYVjHRlfGBkG/xBKX9AKnX5oAFKMAqWhXV0J4xXuTLdrteeLaZTk0PrifE7h0tSAZjBHUUs7CYBRqvtrVqHtX4N6x1+CydVQCpX+1f+Slz8Wa0e25ERpW7aXe8NAwqOBNq4gFrgGVU+mF0p41pApDEqCwqlHSW6GpavN+s/mNkI2pffPWtyfTg9CCFVH9kHCrKxobyQ1vdWGc9Nre3Jr3mlPcP+IhSa0Q0JZ/tj7pPi4cwOV3ltvdIetJD68n5XoidQGcvvrVhPX5N/QxRuvh6aH61XzeE2Ll8pDLekEWgIExFEv42M/g0TriTZyFBel38/9J6Jd3B+UqXxwmCrjTiYxQ8C/ow0RgmYNLLxqpjFKe3nci25WwJldPtzacbLDQwTuUFq3IcxX3+pmuvHJkGrBaSCz+ujdmh5sjN29c891xWlbeG2+nI6URoc+hnOm+mycE9oetW9fNF7N8DUGeAJHsCGxZmfSCa+5YRSxYMuE1//gvINW99NxBb4SUQ3N0n6b4cPdOkHTyOCgx/z3HxoH2EzVwsK7ui5Ta907I8G90fDpn4o1c5y7FsDwpcNuuiboGuwxxN6pruWFLPixXGh5baci42cy7EGYKXUnBIWVk0LBGuhR9opfs7DE0nmPwJCsZy1+7vGikKZ8FxU93nNplEVfolqSQj9dc22uW8dIUvWaavkNLAGfphQebbUcvQNWJQce0tchDX6ZW0f4xetXi+EA+eO/B2hj9hfxfiARnsCoEZUYf/omB93ExLBJbCkWDL4ER15v6GbRjCeKCezwO5UNY33Tycxgr/WfjCx+wSuJ1pGNYAsyR8zX+8z4wJgk6c3U9YlOD0ljRODQMz2uDl4OPlA8fdrds/yScssTCE65O4qoardlXShPZFbJF2IczZS3LL0+2z+USBZCh6sk38rqwJ0yeNZtcZuKkC6MRPB6i00qwwFAjbohLeZYLlNWuKhq6M5Kc3HAxoOs76+6oE8ZcZ+sT+WPMnQvSN5+/Eb5sgiSRgd2WRcxBcfhqiaLwptB5/f1wrGbQWDDT3vnm8Rt/fWHwIlbXJgDm5KTW4KD3uBDNHyW2lHZi7hYD04R0/+X5OY+6MGX2lfV8KKy7nIQbcgouHELBTHtDahZmy8RIbCwss4Tinq+8IFNOe8BhgJiLHmepfmZbtWt+OTPLCPWZXpXrsZF2ghcYY8GyA7BGtFWvFiYWXza+quzjckn90E2PUzn9b9/DXC4RDHJf+RBUi4zPnn2I661Srmqe149UMIDIO5DtBivBiLhHVj/J+8W++J/BEJ8DtqXlb5E0RAZKrTQ11nUaMt5IEOzRZWGYTPwoeJ+uEu7lE/cxP/+WuzumUWwPaLfCHGbpAjsNUa/0qHDwt1WbMDLyXideqAzChJufr1GOpxvuEusSwF8SkwbDawIDy/0pdKQErvbDD96Y4LbdM9nxmVHBL9NwDg9SLvlQyGWLEIgEViis5XifCVLf/WEMo6WWqtq8j67Q/s5T4d2GUbXPZoIcLU8ujW4Yy7NsEt7KoGmRPvB+IAn6IN13NVKrj/z7hMumgbHf9L18trtzvOua7eHEShAQ8P0zFS4ewyus+IQWM8cjj2gdqc1I7EEQFzTtfvnO32lV1IRtxhpLE/pLf5WAaY0qRu2F/8JDqEs069zKFnS01iBTsI705OuNZyALaNZDxJVgpCeJp+AQRrp7TNyxkvCsgg0sBfsj/QiNgYR5MAXh2eDw9MmuoegkD//YvKz4ehukh3hk/ABiBCDrRyVZo3B1Lv/z+e4MASYEKf64ZrWOz0eIgr9Fen0r3BEH2WaWPKToLqSXHE4l02gkHBQUEucwqn+vJqon6BexM6r1vj6KIx2kMUuaASpFFoEjkXbKJP4yO6lu6Pt+uNTif+/yzow19xKe4EKOVHzbX03l1ytxCA+Wksmc+xsVpzp8nUzwmEmMDmqgcKE3xj1VHoWJZgX/rOABYdPJvae77K9xU4DGi7Xr5GPYPq1b/kYK+ps4uf0hxy90/sII8bCU12aq3XzcRvVRDPW4RlQztEjvYxFhmGz/XeCCl1MKGqVZvRo3lO40gY8+xa/BDrrTaH2aJCAtuL+/4wUf1MWTQJy4xsiu7BXuqp7GZhO4erGTPp4HIMm9koOXfu5lnPdwqN1f7u6nyBLrjIecoWL3JWASfB+UX6b9M1COXKBwNJ6wfUqcNm4XspNsv7ls/2ZdebrXMxBhDTM1Lt6blflJCYRJHGAqKr650taInYEiGEec6IX95g5WFTxRAP6ikUZn8GNvbpmkYn8IGgvgyugtn5TbS0fqLwEhws0Zv8zIJp7LXb8yOTG+vxi592WRF3NPjtNDUyJvyAnebOiVMbn5WWIuczRweID4U0FaNFizoW7LEm3v+do4JWS0EN5n5NhtntUHh24AcuDbZSH4/5vsKIWiFYKseJ7XDm70/yRl5Ii9OnXtyfqVUlLEUpSMSqpNnhyGUhwHNCOPaQz3cFWX8CZIbS8HHTgILeDbfScyXWo5qzmcuyTySTNwQ0pYZqOfsV6WW1BPSOfrhF7/Hr8J3MT9x55AdSdFHcS2Y0tL6uTsFWbNv8H1YF5qf3YgAw1kHW0JRmvnjC5qQimLvmmygDmIOoWzz6arv++U7fqAPRFmqaeAWpMaUxxdp9IJqKya5Ren/G8h3TSUWI2LHdSll198uKMwMMFDZue0CpSh3I+dvNb1LBL9cyo7Tj3YKT5J0qYi1VGb7eG23z/86AL5BFiKzlbPbzIIgZHQZ+0Ox8Xa01ycuTq6Ek9Vbkm7gm+bn4xXTGegAHWJ7AoLmJyUvvCbEyIHKSiT/l1WQPr8jOWoJU6N4+JMiVO9W4AbYrCsycxuDuSC4DZXNbtUfxKK9+mezvHRcSe1jvYsyldrxymRFlEnTUIHGQvf0ZkQcittps02hEmTl+39h3wsa9vV7NbL7L7LVfhemofEw1FXyNj6LiiRz8Wxiuah4Q6UdVgii5QxFiOFOW7gX6MnAS27stcvL2Qyc4o23WM17AyJj0r3m+hbOyS67IbPHWw56TPN0/Hy0Xy1J8qQiuqvRDTNZDIEeRocLBmhqzVvGqtY8EQn/lA+ru8gGrISD/hKejsclkcbGLMyyxsdB098s/ZF4vICaaAXP2xrx7FasP6TRjnFYrjbVPP0BP0v+6dw5Xy+eLvIhJPNtnlr2dDUx5lYsiXmXO+G5aGEl4mu4hhNk1VnhiZL9zL3jvrhnS7y4R6jAKbvsQFs+LXt1sM0VljKOtYVjWOg4meUwY5pcj4N4NhRN8iZsTrxk0Jkg6gK1rlHmtMbQDlq0VF8vcTnYLpGdp5e5s/EARKeWU7nO1CNt1sMZ8oAJLks/aBq3K/AjSuqXT4K6Utix25lhXwa3rIz/NlmiOAwHuZrvD5TGTpSyxct85FLpNYge8hrKgP1F2Bg4RPfASFYltllLwcLQPR7z1n5K6FagM17EPBG0NUJaOyzrhX8SuGUhY66O/Xm0WAolhFzk+VlA2sEqc8+Nof3YG3XZURU541hw7Lhq0lXPHcag653jR4cL8rfK2Kk46l9+6ig6tcyc51FGV6XXcec9E613h9dJkQ9D27Af7aqD6mkpjdadoOrlrpW99ASYcCtipkOo30HgT3i2tlHOn2rsNBBVknc6f3Bog4HNgyIiYZ3mAIMwqhYhb4alyoQ0wyol8AfW6LASsRgOL/lcHUWceUMsMZ9L777ZiV4uTOnDj3YfrCH/Hkb3ybNsE2BCoNSBMwzXazw63lRXDZGqZZRK2sM3MeoeV4VGiUSjBlHs0vr1DezSr0R9q4io4aPzpvl0oaZol0KiF6Sx+/i/VcHopM9xuEjEG9GfGLAOf39MDM2eWpoKn7dU/ekrho46lUTMsDSkfgXl6dWDM3Kfd+COs6Du+qxPtlCo88xtPhxD0Kkhtl3FuoYA+Zfy7rLje8ijDiFJzUgTtdokQfTOrNXmwxb+0Cx4f6DVkCyMtX1cMLfF07aDQA9n/f9uKcrpZQ0Tv1QwOlkVX8fTqPpMu9cs8qiM48WobJxcni4/gCOUKC9vYFWC8VlKg6VKwjGLY4XA/4fyx5vn+gKTHgJWSPq4JbfDUA2QhtpR0J+Mz/G0BFn4hZ0NjgoAc9zVu/hr4pr7iZHHThiEZWZbs3DX7g/sBYWwU9t+7qNvMsNdKzzcdG9hAABDgf1g0wKTlR6Vb8fyyn7mratUiDdAAAAAAAAAAAAAAA'),
	 (N'Francia',N'UEFA',1850,1,2,8,N'UklGRlAAAABXRUJQVlA4TEMAAAAvScE2ABdAkG3jsO3+qNcQCCRO+TuMtFgwwdmXv13zH//EBAraNnJuj+f3JJ9pAbwa0f8JeMgjyItM+o/+o//ov08QAA=='),
	 (N'OFC_01',N'OFC',2455,3,6,7,N''),
	 (N'UEFA_01',N'UEFA',1617,7,7,3,N''),
	 (N'UEFA_02',N'UEFA',1952,4,2,2,N''),
	 (N'UEFA_03',N'UEFA',744,6,9,5,N''),
	 (N'UEFA_04',N'UEFA',1287,3,5,5,N''),
	 (N'UEFA_05',N'UEFA',701,5,4,3,N''),
	 (N'UEFA_06',N'UEFA',1913,4,6,6,N''),
	 (N'UEFA_07',N'UEFA',2333,4,6,9,N'');
INSERT INTO MundialDb.dbo.Equipos (Nombre,Confederacion,RankingFifa,Puntos,GolesAFavor,GolesEnContra,BanderaBase64) VALUES
	 (N'UEFA_08',N'UEFA',2214,0,6,9,N''),
	 (N'UEFA_09',N'UEFA',2470,6,9,7,N''),
	 (N'UEFA_10',N'UEFA',1216,4,3,6,N''),
	 (N'UEFA_11',N'UEFA',457,9,11,4,N''),
	 (N'UEFA_12',N'UEFA',703,5,9,8,N''),
	 (N'UEFA_13',N'UEFA',1840,4,4,5,N''),
	 (N'UEFA_14',N'UEFA',1229,4,5,5,N''),
	 (N'Uruguay',N'CONMEBOL',1800,3,8,8,N'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');

    -----------------------------------------------------------------------------
    -- 05_Sorteos: Sorteos_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Sorteos (Token,SemillaToken) VALUES
	 (1,1);

    -----------------------------------------------------------------------------
    -- 06_Fixtures: Fixtures_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Fixtures (Token,SemillaToken,UsaMotorProbabilistico) VALUES
	 (1,1,0);

    -----------------------------------------------------------------------------
    -- 07_Grupos: Grupos_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Grupos (Id,Token) VALUES
	 (N'A',1),
	 (N'B',1),
	 (N'C',1),
	 (N'D',1),
	 (N'E',1),
	 (N'F',1),
	 (N'G',1),
	 (N'H',1),
	 (N'I',1),
	 (N'J',1);
INSERT INTO MundialDb.dbo.Grupos (Id,Token) VALUES
	 (N'K',1),
	 (N'L',1);

    -----------------------------------------------------------------------------
    -- 08_Estadios: Estadios_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Estadios (Nombre,Id,Ciudad,Descripcion,Capacidad,GrupoId,GrupoToken) VALUES
	 (N'BC Place',0,N'Vancouver',N'Sede norte',54000,NULL,NULL),
	 (N'Estadio Azteca',0,N'Ciudad de Mexico',N'Sede principal',87000,NULL,NULL),
	 (N'Estadio Centenario',0,N'Montevideo',N'Estadio historico',60000,NULL,NULL),
	 (N'MetLife Stadium',0,N'Nueva Jersey',N'Sede de la final',82500,NULL,NULL),
	 (N'SoFi Stadium',0,N'Los Angeles',N'Sede oeste',70000,NULL,NULL);

    -----------------------------------------------------------------------------
    -- 09_EquipoSorteo: EquipoSorteo_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.EquipoSorteo (EquiposNombre,SorteoToken) VALUES
	 (N'AFC_01',1),
	 (N'AFC_02',1),
	 (N'AFC_03',1),
	 (N'AFC_04',1),
	 (N'AFC_05',1),
	 (N'AFC_06',1),
	 (N'AFC_07',1),
	 (N'AFC_08',1),
	 (N'Argentina',1),
	 (N'CAF_01',1);
INSERT INTO MundialDb.dbo.EquipoSorteo (EquiposNombre,SorteoToken) VALUES
	 (N'CAF_02',1),
	 (N'CAF_03',1),
	 (N'CAF_04',1),
	 (N'CAF_05',1),
	 (N'CAF_06',1),
	 (N'CAF_07',1),
	 (N'CAF_08',1),
	 (N'CAF_09',1),
	 (N'CONCACAF_01',1),
	 (N'CONCACAF_02',1);
INSERT INTO MundialDb.dbo.EquipoSorteo (EquiposNombre,SorteoToken) VALUES
	 (N'CONCACAF_03',1),
	 (N'CONCACAF_04',1),
	 (N'CONCACAF_05',1),
	 (N'CONCACAF_06',1),
	 (N'CONCACAF_07',1),
	 (N'CONMEBOL_01',1),
	 (N'CONMEBOL_02',1),
	 (N'CONMEBOL_03',1),
	 (N'CONMEBOL_04',1),
	 (N'CONMEBOL_05',1);
INSERT INTO MundialDb.dbo.EquipoSorteo (EquiposNombre,SorteoToken) VALUES
	 (N'Espana',1),
	 (N'Francia',1),
	 (N'OFC_01',1),
	 (N'UEFA_01',1),
	 (N'UEFA_02',1),
	 (N'UEFA_03',1),
	 (N'UEFA_04',1),
	 (N'UEFA_05',1),
	 (N'UEFA_06',1),
	 (N'UEFA_07',1);
INSERT INTO MundialDb.dbo.EquipoSorteo (EquiposNombre,SorteoToken) VALUES
	 (N'UEFA_08',1),
	 (N'UEFA_09',1),
	 (N'UEFA_10',1),
	 (N'UEFA_11',1),
	 (N'UEFA_12',1),
	 (N'UEFA_13',1),
	 (N'UEFA_14',1),
	 (N'Uruguay',1);

    -----------------------------------------------------------------------------
    -- 10_EquipoGrupo: EquipoGrupo_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.EquipoGrupo (EquiposNombre,GrupoId,GrupoToken) VALUES
	 (N'CAF_05',N'A',1),
	 (N'CONMEBOL_05',N'A',1),
	 (N'UEFA_02',N'A',1),
	 (N'UEFA_04',N'A',1),
	 (N'CAF_08',N'B',1),
	 (N'CONCACAF_06',N'B',1),
	 (N'OFC_01',N'B',1),
	 (N'UEFA_01',N'B',1),
	 (N'CAF_01',N'C',1),
	 (N'CONMEBOL_04',N'C',1);
INSERT INTO MundialDb.dbo.EquipoGrupo (EquiposNombre,GrupoId,GrupoToken) VALUES
	 (N'UEFA_09',N'C',1),
	 (N'UEFA_11',N'C',1),
	 (N'AFC_08',N'D',1),
	 (N'CONMEBOL_03',N'D',1),
	 (N'UEFA_12',N'D',1),
	 (N'UEFA_13',N'D',1),
	 (N'AFC_05',N'E',1),
	 (N'Argentina',N'E',1),
	 (N'CAF_02',N'E',1),
	 (N'CONCACAF_03',N'E',1);
INSERT INTO MundialDb.dbo.EquipoGrupo (EquiposNombre,GrupoId,GrupoToken) VALUES
	 (N'AFC_02',N'F',1),
	 (N'CAF_06',N'F',1),
	 (N'CONCACAF_05',N'F',1),
	 (N'CONMEBOL_01',N'F',1),
	 (N'AFC_07',N'G',1),
	 (N'CAF_04',N'G',1),
	 (N'UEFA_08',N'G',1),
	 (N'UEFA_14',N'G',1),
	 (N'AFC_04',N'H',1),
	 (N'CONCACAF_04',N'H',1);
INSERT INTO MundialDb.dbo.EquipoGrupo (EquiposNombre,GrupoId,GrupoToken) VALUES
	 (N'Francia',N'H',1),
	 (N'UEFA_03',N'H',1),
	 (N'AFC_01',N'I',1),
	 (N'CONMEBOL_02',N'I',1),
	 (N'Espana',N'I',1),
	 (N'UEFA_10',N'I',1),
	 (N'CAF_03',N'J',1),
	 (N'CONCACAF_02',N'J',1),
	 (N'UEFA_07',N'J',1),
	 (N'Uruguay',N'J',1);
INSERT INTO MundialDb.dbo.EquipoGrupo (EquiposNombre,GrupoId,GrupoToken) VALUES
	 (N'AFC_03',N'K',1),
	 (N'CAF_09',N'K',1),
	 (N'CONCACAF_07',N'K',1),
	 (N'UEFA_06',N'K',1),
	 (N'AFC_06',N'L',1),
	 (N'CAF_07',N'L',1),
	 (N'CONCACAF_01',N'L',1),
	 (N'UEFA_05',N'L',1);

    -----------------------------------------------------------------------------
    -- 11_EstadioSorteo: EstadioSorteo_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.EstadioSorteo (EstadiosNombre,SorteoToken) VALUES
	 (N'BC Place',1),
	 (N'Estadio Azteca',1),
	 (N'Estadio Centenario',1),
	 (N'MetLife Stadium',1),
	 (N'SoFi Stadium',1);

    -----------------------------------------------------------------------------
    -- 12_EstadioFixture: EstadioFixture_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.EstadioFixture (EstadiosNombre,FixtureToken) VALUES
	 (N'BC Place',1),
	 (N'Estadio Azteca',1),
	 (N'Estadio Centenario',1),
	 (N'MetLife Stadium',1),
	 (N'SoFi Stadium',1);

    -----------------------------------------------------------------------------
    -- 13_FechasOcupadas: FechasOcupadas_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-01 14:00:00.0000000',N'BC Place',1),
	 ('2026-06-01 18:00:00.0000000',N'BC Place',1),
	 ('2026-06-01 22:00:00.0000000',N'BC Place',1),
	 ('2026-06-02 14:00:00.0000000',N'BC Place',1),
	 ('2026-06-02 18:00:00.0000000',N'BC Place',1),
	 ('2026-06-04 14:00:00.0000000',N'BC Place',1),
	 ('2026-06-04 18:00:00.0000000',N'BC Place',1),
	 ('2026-06-05 14:00:00.0000000',N'BC Place',1),
	 ('2026-06-05 18:00:00.0000000',N'BC Place',1),
	 ('2026-06-07 14:00:00.0000000',N'BC Place',1);
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-07 18:00:00.0000000',N'BC Place',1),
	 ('2026-06-09 18:00:00.0000000',N'BC Place',1),
	 ('2026-06-10 14:00:00.0000000',N'BC Place',1),
	 ('2026-06-12 14:00:00.0000000',N'BC Place',1),
	 ('2026-06-12 18:00:00.0000000',N'BC Place',1),
	 ('2026-06-01 14:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-01 18:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-01 22:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-02 14:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-02 18:00:00.0000000',N'Estadio Azteca',1);
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-02 22:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-04 14:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-04 18:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-05 14:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-05 18:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-07 18:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-08 14:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-10 14:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-10 18:00:00.0000000',N'Estadio Azteca',1),
	 ('2026-06-12 18:00:00.0000000',N'Estadio Azteca',1);
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-01 14:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-01 18:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-01 22:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-02 14:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-02 18:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-04 14:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-04 18:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-04 22:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-05 14:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-05 18:00:00.0000000',N'Estadio Centenario',1);
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-08 14:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-08 18:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-10 18:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-11 14:00:00.0000000',N'Estadio Centenario',1),
	 ('2026-06-01 14:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-01 18:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-01 22:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-02 14:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-04 14:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-04 18:00:00.0000000',N'MetLife Stadium',1);
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-04 22:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-05 14:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-05 18:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-05 22:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-08 18:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-09 14:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-11 14:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-11 18:00:00.0000000',N'MetLife Stadium',1),
	 ('2026-06-01 14:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-01 18:00:00.0000000',N'SoFi Stadium',1);
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-01 22:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-02 14:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-04 14:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-04 18:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-05 14:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-05 18:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-05 22:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-07 14:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-09 14:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-09 18:00:00.0000000',N'SoFi Stadium',1);
INSERT INTO MundialDb.dbo.FechasOcupadas (Fecha,EstadioNombre,FixtureToken) VALUES
	 ('2026-06-11 18:00:00.0000000',N'SoFi Stadium',1),
	 ('2026-06-12 14:00:00.0000000',N'SoFi Stadium',1);

    -----------------------------------------------------------------------------
    -- 14_Partidos: Partidos_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (1,1,N'A','2026-06-01 14:00:00.0000000',N'BC Place',N'UEFA_02',N'CONMEBOL_05',1,1,N'Fase de Grupos',N'14:00',1,NULL,NULL,5,1,5,0,NULL),
	 (2,1,N'A','2026-06-01 14:00:00.0000000',N'Estadio Azteca',N'CAF_05',N'UEFA_04',3,5,N'Fase de Grupos',N'14:00',1,NULL,NULL,2,0,5,0,NULL),
	 (3,1,N'A','2026-06-04 14:00:00.0000000',N'Estadio Centenario',N'UEFA_02',N'UEFA_04',1,0,N'Fase de Grupos',N'14:00',2,NULL,NULL,3,1,0,1,NULL),
	 (4,1,N'A','2026-06-04 14:00:00.0000000',N'MetLife Stadium',N'CAF_05',N'CONMEBOL_05',3,3,N'Fase de Grupos',N'14:00',2,NULL,NULL,2,0,4,1,NULL),
	 (5,1,N'A','2026-06-07 14:00:00.0000000',N'SoFi Stadium',N'UEFA_02',N'CAF_05',0,1,N'Fase de Grupos',N'14:00',3,NULL,NULL,4,0,0,0,NULL),
	 (6,1,N'A','2026-06-07 14:00:00.0000000',N'BC Place',N'UEFA_04',N'CONMEBOL_05',0,1,N'Fase de Grupos',N'14:00',3,NULL,NULL,1,0,0,0,NULL),
	 (7,1,N'B','2026-06-01 18:00:00.0000000',N'Estadio Azteca',N'OFC_01',N'CAF_08',0,3,N'Fase de Grupos',N'18:00',1,NULL,NULL,3,1,0,1,NULL),
	 (8,1,N'B','2026-06-01 14:00:00.0000000',N'Estadio Centenario',N'UEFA_01',N'CONCACAF_06',0,0,N'Fase de Grupos',N'14:00',1,NULL,NULL,5,1,4,0,NULL),
	 (9,1,N'B','2026-06-04 18:00:00.0000000',N'MetLife Stadium',N'OFC_01',N'CONCACAF_06',5,1,N'Fase de Grupos',N'18:00',2,NULL,NULL,5,1,0,0,NULL),
	 (10,1,N'B','2026-06-04 14:00:00.0000000',N'SoFi Stadium',N'UEFA_01',N'CAF_08',4,2,N'Fase de Grupos',N'14:00',2,NULL,NULL,5,1,1,0,NULL);
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (11,1,N'B','2026-06-07 18:00:00.0000000',N'BC Place',N'OFC_01',N'UEFA_01',1,3,N'Fase de Grupos',N'18:00',3,NULL,NULL,0,1,2,0,NULL),
	 (12,1,N'B','2026-06-07 18:00:00.0000000',N'Estadio Azteca',N'CONCACAF_06',N'CAF_08',1,3,N'Fase de Grupos',N'18:00',3,NULL,NULL,5,0,4,1,NULL),
	 (13,1,N'C','2026-06-01 18:00:00.0000000',N'Estadio Centenario',N'UEFA_09',N'UEFA_11',0,5,N'Fase de Grupos',N'18:00',1,NULL,NULL,5,0,1,1,NULL),
	 (14,1,N'C','2026-06-01 14:00:00.0000000',N'MetLife Stadium',N'CONMEBOL_04',N'CAF_01',3,5,N'Fase de Grupos',N'14:00',1,NULL,NULL,0,1,2,0,NULL),
	 (15,1,N'C','2026-06-04 18:00:00.0000000',N'SoFi Stadium',N'UEFA_09',N'CAF_01',5,2,N'Fase de Grupos',N'18:00',2,NULL,NULL,1,1,1,1,NULL),
	 (16,1,N'C','2026-06-04 14:00:00.0000000',N'BC Place',N'CONMEBOL_04',N'UEFA_11',2,3,N'Fase de Grupos',N'14:00',2,NULL,NULL,2,1,0,0,NULL),
	 (17,1,N'C','2026-06-08 14:00:00.0000000',N'Estadio Azteca',N'UEFA_09',N'CONMEBOL_04',4,0,N'Fase de Grupos',N'14:00',3,NULL,NULL,2,0,2,1,NULL),
	 (18,1,N'C','2026-06-08 14:00:00.0000000',N'Estadio Centenario',N'CAF_01',N'UEFA_11',2,3,N'Fase de Grupos',N'14:00',3,NULL,NULL,5,0,4,1,NULL),
	 (19,1,N'D','2026-06-01 18:00:00.0000000',N'MetLife Stadium',N'CONMEBOL_03',N'UEFA_12',4,5,N'Fase de Grupos',N'18:00',1,NULL,NULL,0,0,4,1,NULL),
	 (20,1,N'D','2026-06-01 14:00:00.0000000',N'SoFi Stadium',N'UEFA_13',N'AFC_08',2,1,N'Fase de Grupos',N'14:00',1,NULL,NULL,2,1,3,1,NULL);
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (21,1,N'D','2026-06-04 18:00:00.0000000',N'BC Place',N'CONMEBOL_03',N'AFC_08',2,1,N'Fase de Grupos',N'18:00',2,NULL,NULL,1,0,1,0,NULL),
	 (22,1,N'D','2026-06-04 14:00:00.0000000',N'Estadio Azteca',N'UEFA_13',N'UEFA_12',2,2,N'Fase de Grupos',N'14:00',2,NULL,NULL,4,1,5,0,NULL),
	 (23,1,N'D','2026-06-08 18:00:00.0000000',N'Estadio Centenario',N'CONMEBOL_03',N'UEFA_13',2,0,N'Fase de Grupos',N'18:00',3,NULL,NULL,2,0,3,0,NULL),
	 (24,1,N'D','2026-06-08 18:00:00.0000000',N'MetLife Stadium',N'AFC_08',N'UEFA_12',2,2,N'Fase de Grupos',N'18:00',3,NULL,NULL,0,0,1,1,NULL),
	 (25,1,N'E','2026-06-01 18:00:00.0000000',N'SoFi Stadium',N'AFC_05',N'CAF_02',5,2,N'Fase de Grupos',N'18:00',1,NULL,NULL,3,1,5,0,NULL),
	 (26,1,N'E','2026-06-01 18:00:00.0000000',N'BC Place',N'Argentina',N'CONCACAF_03',0,2,N'Fase de Grupos',N'18:00',1,NULL,NULL,2,0,3,1,NULL),
	 (27,1,N'E','2026-06-04 18:00:00.0000000',N'Estadio Azteca',N'AFC_05',N'CONCACAF_03',3,0,N'Fase de Grupos',N'18:00',2,NULL,NULL,0,0,0,1,NULL),
	 (28,1,N'E','2026-06-04 18:00:00.0000000',N'Estadio Centenario',N'Argentina',N'CAF_02',1,0,N'Fase de Grupos',N'18:00',2,NULL,NULL,0,0,5,1,NULL),
	 (29,1,N'E','2026-06-09 14:00:00.0000000',N'MetLife Stadium',N'AFC_05',N'Argentina',0,4,N'Fase de Grupos',N'14:00',3,NULL,NULL,1,1,0,1,NULL),
	 (30,1,N'E','2026-06-09 14:00:00.0000000',N'SoFi Stadium',N'CONCACAF_03',N'CAF_02',4,0,N'Fase de Grupos',N'14:00',3,NULL,NULL,4,0,5,1,NULL);
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (31,1,N'F','2026-06-01 22:00:00.0000000',N'BC Place',N'CAF_06',N'CONCACAF_05',0,0,N'Fase de Grupos',N'22:00',1,NULL,NULL,0,0,4,0,NULL),
	 (32,1,N'F','2026-06-01 22:00:00.0000000',N'Estadio Azteca',N'AFC_02',N'CONMEBOL_01',2,2,N'Fase de Grupos',N'22:00',1,NULL,NULL,2,0,5,0,NULL),
	 (33,1,N'F','2026-06-04 22:00:00.0000000',N'Estadio Centenario',N'CAF_06',N'CONMEBOL_01',1,1,N'Fase de Grupos',N'22:00',2,NULL,NULL,4,0,4,0,NULL),
	 (34,1,N'F','2026-06-04 22:00:00.0000000',N'MetLife Stadium',N'AFC_02',N'CONCACAF_05',0,4,N'Fase de Grupos',N'22:00',2,NULL,NULL,3,0,1,1,NULL),
	 (35,1,N'F','2026-06-09 18:00:00.0000000',N'SoFi Stadium',N'CAF_06',N'AFC_02',0,1,N'Fase de Grupos',N'18:00',3,NULL,NULL,4,0,0,0,NULL),
	 (36,1,N'F','2026-06-09 18:00:00.0000000',N'BC Place',N'CONMEBOL_01',N'CONCACAF_05',0,1,N'Fase de Grupos',N'18:00',3,NULL,NULL,1,0,3,1,NULL),
	 (37,1,N'G','2026-06-02 14:00:00.0000000',N'Estadio Azteca',N'UEFA_08',N'AFC_07',3,4,N'Fase de Grupos',N'14:00',1,NULL,NULL,2,0,1,1,NULL),
	 (38,1,N'G','2026-06-01 22:00:00.0000000',N'Estadio Centenario',N'CAF_04',N'UEFA_14',3,2,N'Fase de Grupos',N'22:00',1,NULL,NULL,0,1,4,0,NULL),
	 (39,1,N'G','2026-06-05 14:00:00.0000000',N'MetLife Stadium',N'UEFA_08',N'UEFA_14',2,3,N'Fase de Grupos',N'14:00',2,NULL,NULL,4,0,5,0,NULL),
	 (40,1,N'G','2026-06-05 14:00:00.0000000',N'SoFi Stadium',N'CAF_04',N'AFC_07',3,0,N'Fase de Grupos',N'14:00',2,NULL,NULL,1,0,3,0,NULL);
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (41,1,N'G','2026-06-10 14:00:00.0000000',N'BC Place',N'UEFA_08',N'CAF_04',1,2,N'Fase de Grupos',N'14:00',3,NULL,NULL,0,0,0,1,NULL),
	 (42,1,N'G','2026-06-10 14:00:00.0000000',N'Estadio Azteca',N'UEFA_14',N'AFC_07',0,0,N'Fase de Grupos',N'14:00',3,NULL,NULL,0,0,0,1,NULL),
	 (43,1,N'H','2026-06-02 14:00:00.0000000',N'Estadio Centenario',N'CONCACAF_04',N'UEFA_03',3,2,N'Fase de Grupos',N'14:00',1,NULL,NULL,3,1,2,1,NULL),
	 (44,1,N'H','2026-06-01 22:00:00.0000000',N'MetLife Stadium',N'Francia',N'AFC_04',1,1,N'Fase de Grupos',N'22:00',1,NULL,NULL,2,1,2,0,NULL),
	 (45,1,N'H','2026-06-05 18:00:00.0000000',N'SoFi Stadium',N'CONCACAF_04',N'AFC_04',3,0,N'Fase de Grupos',N'18:00',2,NULL,NULL,5,1,0,1,NULL),
	 (46,1,N'H','2026-06-05 14:00:00.0000000',N'BC Place',N'Francia',N'UEFA_03',0,3,N'Fase de Grupos',N'14:00',2,NULL,NULL,2,1,0,0,NULL),
	 (47,1,N'H','2026-06-10 18:00:00.0000000',N'Estadio Azteca',N'CONCACAF_04',N'Francia',4,1,N'Fase de Grupos',N'18:00',3,NULL,NULL,3,1,4,1,NULL),
	 (48,1,N'H','2026-06-10 18:00:00.0000000',N'Estadio Centenario',N'AFC_04',N'UEFA_03',2,4,N'Fase de Grupos',N'18:00',3,NULL,NULL,2,1,5,0,NULL),
	 (49,1,N'I','2026-06-02 14:00:00.0000000',N'MetLife Stadium',N'AFC_01',N'CONMEBOL_02',3,2,N'Fase de Grupos',N'14:00',1,NULL,NULL,0,1,2,1,NULL),
	 (50,1,N'I','2026-06-01 22:00:00.0000000',N'SoFi Stadium',N'Espana',N'UEFA_10',5,0,N'Fase de Grupos',N'22:00',1,NULL,NULL,0,0,2,1,NULL);
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (51,1,N'I','2026-06-05 18:00:00.0000000',N'BC Place',N'AFC_01',N'UEFA_10',0,2,N'Fase de Grupos',N'18:00',2,NULL,NULL,3,0,3,1,NULL),
	 (52,1,N'I','2026-06-05 14:00:00.0000000',N'Estadio Azteca',N'Espana',N'CONMEBOL_02',1,0,N'Fase de Grupos',N'14:00',2,NULL,NULL,3,1,4,1,NULL),
	 (53,1,N'I','2026-06-11 14:00:00.0000000',N'Estadio Centenario',N'AFC_01',N'Espana',1,3,N'Fase de Grupos',N'14:00',3,NULL,NULL,0,1,3,0,NULL),
	 (54,1,N'I','2026-06-11 14:00:00.0000000',N'MetLife Stadium',N'UEFA_10',N'CONMEBOL_02',1,1,N'Fase de Grupos',N'14:00',3,NULL,NULL,1,1,3,0,NULL),
	 (55,1,N'J','2026-06-02 14:00:00.0000000',N'SoFi Stadium',N'UEFA_07',N'CAF_03',1,5,N'Fase de Grupos',N'14:00',1,NULL,NULL,3,1,2,1,NULL),
	 (56,1,N'J','2026-06-02 14:00:00.0000000',N'BC Place',N'Uruguay',N'CONCACAF_02',1,3,N'Fase de Grupos',N'14:00',1,NULL,NULL,4,1,2,0,NULL),
	 (57,1,N'J','2026-06-05 18:00:00.0000000',N'Estadio Azteca',N'UEFA_07',N'CONCACAF_02',1,1,N'Fase de Grupos',N'18:00',2,NULL,NULL,0,1,3,0,NULL),
	 (58,1,N'J','2026-06-05 14:00:00.0000000',N'Estadio Centenario',N'Uruguay',N'CAF_03',4,1,N'Fase de Grupos',N'14:00',2,NULL,NULL,3,1,5,0,NULL),
	 (59,1,N'J','2026-06-11 18:00:00.0000000',N'MetLife Stadium',N'UEFA_07',N'Uruguay',4,3,N'Fase de Grupos',N'18:00',3,NULL,NULL,0,1,5,0,NULL),
	 (60,1,N'J','2026-06-11 18:00:00.0000000',N'SoFi Stadium',N'CONCACAF_02',N'CAF_03',1,2,N'Fase de Grupos',N'18:00',3,NULL,NULL,1,1,1,1,NULL);
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (61,1,N'K','2026-06-02 18:00:00.0000000',N'BC Place',N'UEFA_06',N'CONCACAF_07',0,2,N'Fase de Grupos',N'18:00',1,NULL,NULL,5,0,4,0,NULL),
	 (62,1,N'K','2026-06-02 18:00:00.0000000',N'Estadio Azteca',N'CAF_09',N'AFC_03',4,1,N'Fase de Grupos',N'18:00',1,NULL,NULL,4,1,0,0,NULL),
	 (63,1,N'K','2026-06-05 18:00:00.0000000',N'Estadio Centenario',N'UEFA_06',N'AFC_03',2,2,N'Fase de Grupos',N'18:00',2,NULL,NULL,4,1,4,1,NULL),
	 (64,1,N'K','2026-06-05 18:00:00.0000000',N'MetLife Stadium',N'CAF_09',N'CONCACAF_07',2,1,N'Fase de Grupos',N'18:00',2,NULL,NULL,1,1,4,1,NULL),
	 (65,1,N'K','2026-06-12 14:00:00.0000000',N'SoFi Stadium',N'UEFA_06',N'CAF_09',4,2,N'Fase de Grupos',N'14:00',3,NULL,NULL,5,1,2,0,NULL),
	 (66,1,N'K','2026-06-12 14:00:00.0000000',N'BC Place',N'AFC_03',N'CONCACAF_07',3,0,N'Fase de Grupos',N'14:00',3,NULL,NULL,4,1,0,1,NULL),
	 (67,1,N'L','2026-06-02 22:00:00.0000000',N'Estadio Azteca',N'CAF_07',N'UEFA_05',0,0,N'Fase de Grupos',N'22:00',1,NULL,NULL,3,1,2,0,NULL),
	 (68,1,N'L','2026-06-02 18:00:00.0000000',N'Estadio Centenario',N'AFC_06',N'CONCACAF_01',3,4,N'Fase de Grupos',N'18:00',1,NULL,NULL,0,1,3,0,NULL),
	 (69,1,N'L','2026-06-05 22:00:00.0000000',N'MetLife Stadium',N'CAF_07',N'CONCACAF_01',3,1,N'Fase de Grupos',N'22:00',2,NULL,NULL,5,0,0,0,NULL),
	 (70,1,N'L','2026-06-05 22:00:00.0000000',N'SoFi Stadium',N'AFC_06',N'UEFA_05',3,4,N'Fase de Grupos',N'22:00',2,NULL,NULL,2,0,1,0,NULL);
INSERT INTO MundialDb.dbo.Partidos (Id,Token,GrupoID,Fecha,EstadioNombre,EquipoLocalNombre,EquipoVisitanteNombre,GolesLocal,GolesVisitante,Fase,Horario,Jornada,PenalesLocal,PenalesVisitante,TarjetasAmarillasLocal,TarjetasRojasLocal,TarjetasAmarillasVisitante,TarjetasRojasVisitante,GrupoToken) VALUES
	 (71,1,N'L','2026-06-12 18:00:00.0000000',N'BC Place',N'CAF_07',N'AFC_06',4,1,N'Fase de Grupos',N'18:00',3,NULL,NULL,0,1,3,1,NULL),
	 (72,1,N'L','2026-06-12 18:00:00.0000000',N'Estadio Azteca',N'CONCACAF_01',N'UEFA_05',0,0,N'Fase de Grupos',N'18:00',3,NULL,NULL,0,1,4,1,NULL);

    -----------------------------------------------------------------------------
    -- 15_Notificaciones: Notificaciones_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_02 1 - 1 CONMEBOL_05',0,'2026-06-17 00:00:18.5556730'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_02 1 - 1 CONMEBOL_05',0,'2026-06-17 00:00:18.5651610'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_05 3 - 5 UEFA_04',0,'2026-06-17 00:00:18.6138470'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_05 3 - 5 UEFA_04',0,'2026-06-17 00:00:18.6139170'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_02 1 - 0 UEFA_04',0,'2026-06-17 00:00:18.6440900'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_02 1 - 0 UEFA_04',0,'2026-06-17 00:00:18.6441580'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_05 3 - 3 CONMEBOL_05',0,'2026-06-17 00:00:18.6766290'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_05 3 - 3 CONMEBOL_05',0,'2026-06-17 00:00:18.6767030'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_02 0 - 1 CAF_05',0,'2026-06-17 00:00:18.7218100'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_02 0 - 1 CAF_05',0,'2026-06-17 00:00:18.7218830');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_04 0 - 1 CONMEBOL_05',0,'2026-06-17 00:00:18.7414930'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_04 0 - 1 CONMEBOL_05',0,'2026-06-17 00:00:18.7415560'),
	 (N'periodista1@mundial.com',N'Resultado partido: OFC_01 0 - 3 CAF_08',0,'2026-06-17 00:00:18.7589770'),
	 (N'periodista2@mundial.com',N'Resultado partido: OFC_01 0 - 3 CAF_08',0,'2026-06-17 00:00:18.7590290'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_01 0 - 0 CONCACAF_06',0,'2026-06-17 00:00:18.7872930'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_01 0 - 0 CONCACAF_06',0,'2026-06-17 00:00:18.7873670'),
	 (N'periodista1@mundial.com',N'Resultado partido: OFC_01 5 - 1 CONCACAF_06',0,'2026-06-17 00:00:18.8182090'),
	 (N'periodista2@mundial.com',N'Resultado partido: OFC_01 5 - 1 CONCACAF_06',0,'2026-06-17 00:00:18.8182890'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_01 4 - 2 CAF_08',0,'2026-06-17 00:00:18.8480580'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_01 4 - 2 CAF_08',0,'2026-06-17 00:00:18.8481530');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: OFC_01 1 - 3 UEFA_01',0,'2026-06-17 00:00:18.8912590'),
	 (N'periodista2@mundial.com',N'Resultado partido: OFC_01 1 - 3 UEFA_01',0,'2026-06-17 00:00:18.8913540'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONCACAF_06 1 - 3 CAF_08',0,'2026-06-17 00:00:18.9199370'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONCACAF_06 1 - 3 CAF_08',0,'2026-06-17 00:00:18.9200010'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_09 0 - 5 UEFA_11',0,'2026-06-17 00:00:18.9461900'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_09 0 - 5 UEFA_11',0,'2026-06-17 00:00:18.9462730'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONMEBOL_04 3 - 5 CAF_01',0,'2026-06-17 00:00:18.9759430'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONMEBOL_04 3 - 5 CAF_01',0,'2026-06-17 00:00:18.9760040'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_09 5 - 2 CAF_01',0,'2026-06-17 00:00:19.0096620'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_09 5 - 2 CAF_01',0,'2026-06-17 00:00:19.0097570');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: CONMEBOL_04 2 - 3 UEFA_11',0,'2026-06-17 00:00:19.0326460'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONMEBOL_04 2 - 3 UEFA_11',0,'2026-06-17 00:00:19.0327440'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_09 4 - 0 CONMEBOL_04',0,'2026-06-17 00:00:19.0539780'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_09 4 - 0 CONMEBOL_04',0,'2026-06-17 00:00:19.0540450'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_01 2 - 3 UEFA_11',0,'2026-06-17 00:00:19.0816310'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_01 2 - 3 UEFA_11',0,'2026-06-17 00:00:19.0816930'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONMEBOL_03 4 - 5 UEFA_12',0,'2026-06-17 00:00:19.1110040'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONMEBOL_03 4 - 5 UEFA_12',0,'2026-06-17 00:00:19.1110820'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_13 2 - 1 AFC_08',0,'2026-06-17 00:00:19.1371300'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_13 2 - 1 AFC_08',0,'2026-06-17 00:00:19.1372060');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: CONMEBOL_03 2 - 1 AFC_08',0,'2026-06-17 00:00:19.1614920'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONMEBOL_03 2 - 1 AFC_08',0,'2026-06-17 00:00:19.1615690'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_13 2 - 2 UEFA_12',0,'2026-06-17 00:00:19.1859240'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_13 2 - 2 UEFA_12',0,'2026-06-17 00:00:19.1859950'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONMEBOL_03 2 - 0 UEFA_13',0,'2026-06-17 00:00:19.2175350'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONMEBOL_03 2 - 0 UEFA_13',0,'2026-06-17 00:00:19.2176070'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_08 2 - 2 UEFA_12',0,'2026-06-17 00:00:19.2667650'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_08 2 - 2 UEFA_12',0,'2026-06-17 00:00:19.2668360'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_05 5 - 2 CAF_02',0,'2026-06-17 00:00:19.3315840'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_05 5 - 2 CAF_02',0,'2026-06-17 00:00:19.3316440');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: Argentina 0 - 2 CONCACAF_03',0,'2026-06-17 00:00:19.3558900'),
	 (N'periodista2@mundial.com',N'Resultado partido: Argentina 0 - 2 CONCACAF_03',0,'2026-06-17 00:00:19.3559580'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_05 3 - 0 CONCACAF_03',0,'2026-06-17 00:00:19.3810720'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_05 3 - 0 CONCACAF_03',0,'2026-06-17 00:00:19.3811330'),
	 (N'periodista1@mundial.com',N'Resultado partido: Argentina 1 - 0 CAF_02',0,'2026-06-17 00:00:19.4137420'),
	 (N'periodista2@mundial.com',N'Resultado partido: Argentina 1 - 0 CAF_02',0,'2026-06-17 00:00:19.4138090'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_05 0 - 4 Argentina',0,'2026-06-17 00:00:19.4588060'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_05 0 - 4 Argentina',0,'2026-06-17 00:00:19.4588730'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONCACAF_03 4 - 0 CAF_02',0,'2026-06-17 00:00:19.4837680'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONCACAF_03 4 - 0 CAF_02',0,'2026-06-17 00:00:19.4838280');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_06 0 - 0 CONCACAF_05',0,'2026-06-17 00:00:19.5684460'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_06 0 - 0 CONCACAF_05',0,'2026-06-17 00:00:19.5685830'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_02 2 - 2 CONMEBOL_01',0,'2026-06-17 00:00:19.6686540'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_02 2 - 2 CONMEBOL_01',0,'2026-06-17 00:00:19.6688380'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_06 1 - 1 CONMEBOL_01',0,'2026-06-17 00:00:19.7227390'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_06 1 - 1 CONMEBOL_01',0,'2026-06-17 00:00:19.7228190'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_02 0 - 4 CONCACAF_05',0,'2026-06-17 00:00:19.7620630'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_02 0 - 4 CONCACAF_05',0,'2026-06-17 00:00:19.7621870'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_06 0 - 1 AFC_02',0,'2026-06-17 00:00:19.8058760'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_06 0 - 1 AFC_02',0,'2026-06-17 00:00:19.8059600');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: CONMEBOL_01 0 - 1 CONCACAF_05',0,'2026-06-17 00:00:19.8492730'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONMEBOL_01 0 - 1 CONCACAF_05',0,'2026-06-17 00:00:19.8493550'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_08 3 - 4 AFC_07',0,'2026-06-17 00:00:19.8769000'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_08 3 - 4 AFC_07',0,'2026-06-17 00:00:19.8770090'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_04 3 - 2 UEFA_14',0,'2026-06-17 00:00:19.9037290'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_04 3 - 2 UEFA_14',0,'2026-06-17 00:00:19.9037930'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_08 2 - 3 UEFA_14',0,'2026-06-17 00:00:19.9350960'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_08 2 - 3 UEFA_14',0,'2026-06-17 00:00:19.9351600'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_04 3 - 0 AFC_07',0,'2026-06-17 00:00:19.9741200'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_04 3 - 0 AFC_07',0,'2026-06-17 00:00:19.9742040');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_08 1 - 2 CAF_04',0,'2026-06-17 00:00:20.0133150'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_08 1 - 2 CAF_04',0,'2026-06-17 00:00:20.0133990'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_14 0 - 0 AFC_07',0,'2026-06-17 00:00:20.0535800'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_14 0 - 0 AFC_07',0,'2026-06-17 00:00:20.0536550'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONCACAF_04 3 - 2 UEFA_03',0,'2026-06-17 00:00:20.0935390'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONCACAF_04 3 - 2 UEFA_03',0,'2026-06-17 00:00:20.0936220'),
	 (N'periodista1@mundial.com',N'Resultado partido: Francia 1 - 1 AFC_04',0,'2026-06-17 00:00:20.1244170'),
	 (N'periodista2@mundial.com',N'Resultado partido: Francia 1 - 1 AFC_04',0,'2026-06-17 00:00:20.1245130'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONCACAF_04 3 - 0 AFC_04',0,'2026-06-17 00:00:20.1595580'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONCACAF_04 3 - 0 AFC_04',0,'2026-06-17 00:00:20.1596760');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: Francia 0 - 3 UEFA_03',0,'2026-06-17 00:00:20.2012250'),
	 (N'periodista2@mundial.com',N'Resultado partido: Francia 0 - 3 UEFA_03',0,'2026-06-17 00:00:20.2012990'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONCACAF_04 4 - 1 Francia',0,'2026-06-17 00:00:20.2320930'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONCACAF_04 4 - 1 Francia',0,'2026-06-17 00:00:20.2321650'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_04 2 - 4 UEFA_03',0,'2026-06-17 00:00:20.2607320'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_04 2 - 4 UEFA_03',0,'2026-06-17 00:00:20.2607970'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_01 3 - 2 CONMEBOL_02',0,'2026-06-17 00:00:20.2942310'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_01 3 - 2 CONMEBOL_02',0,'2026-06-17 00:00:20.2943120'),
	 (N'periodista1@mundial.com',N'Resultado partido: Espana 5 - 0 UEFA_10',0,'2026-06-17 00:00:20.3214890'),
	 (N'periodista2@mundial.com',N'Resultado partido: Espana 5 - 0 UEFA_10',0,'2026-06-17 00:00:20.3215610');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_01 0 - 2 UEFA_10',0,'2026-06-17 00:00:20.3498200'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_01 0 - 2 UEFA_10',0,'2026-06-17 00:00:20.3498810'),
	 (N'periodista1@mundial.com',N'Resultado partido: Espana 1 - 0 CONMEBOL_02',0,'2026-06-17 00:00:20.3923130'),
	 (N'periodista2@mundial.com',N'Resultado partido: Espana 1 - 0 CONMEBOL_02',0,'2026-06-17 00:00:20.3923970'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_01 1 - 3 Espana',0,'2026-06-17 00:00:20.4256890'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_01 1 - 3 Espana',0,'2026-06-17 00:00:20.4257720'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_10 1 - 1 CONMEBOL_02',0,'2026-06-17 00:00:20.4586220'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_10 1 - 1 CONMEBOL_02',0,'2026-06-17 00:00:20.4587000'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_07 1 - 5 CAF_03',0,'2026-06-17 00:00:20.4909410'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_07 1 - 5 CAF_03',0,'2026-06-17 00:00:20.4910190');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: Uruguay 1 - 3 CONCACAF_02',0,'2026-06-17 00:00:20.5221230'),
	 (N'periodista2@mundial.com',N'Resultado partido: Uruguay 1 - 3 CONCACAF_02',0,'2026-06-17 00:00:20.5221980'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_07 1 - 1 CONCACAF_02',0,'2026-06-17 00:00:20.5587390'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_07 1 - 1 CONCACAF_02',0,'2026-06-17 00:00:20.5588120'),
	 (N'periodista1@mundial.com',N'Resultado partido: Uruguay 4 - 1 CAF_03',0,'2026-06-17 00:00:20.5897100'),
	 (N'periodista2@mundial.com',N'Resultado partido: Uruguay 4 - 1 CAF_03',0,'2026-06-17 00:00:20.5897770'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_07 4 - 3 Uruguay',0,'2026-06-17 00:00:20.6208800'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_07 4 - 3 Uruguay',0,'2026-06-17 00:00:20.6209490'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONCACAF_02 1 - 2 CAF_03',0,'2026-06-17 00:00:20.6459630'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONCACAF_02 1 - 2 CAF_03',0,'2026-06-17 00:00:20.6460100');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_06 0 - 2 CONCACAF_07',0,'2026-06-17 00:00:20.6707130'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_06 0 - 2 CONCACAF_07',0,'2026-06-17 00:00:20.6707740'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_09 4 - 1 AFC_03',0,'2026-06-17 00:00:20.6951540'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_09 4 - 1 AFC_03',0,'2026-06-17 00:00:20.6952200'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_06 2 - 2 AFC_03',0,'2026-06-17 00:00:20.7273610'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_06 2 - 2 AFC_03',0,'2026-06-17 00:00:20.7274240'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_09 2 - 1 CONCACAF_07',0,'2026-06-17 00:00:20.7530660'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_09 2 - 1 CONCACAF_07',0,'2026-06-17 00:00:20.7531250'),
	 (N'periodista1@mundial.com',N'Resultado partido: UEFA_06 4 - 2 CAF_09',0,'2026-06-17 00:00:20.7762930'),
	 (N'periodista2@mundial.com',N'Resultado partido: UEFA_06 4 - 2 CAF_09',0,'2026-06-17 00:00:20.7763470');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_03 3 - 0 CONCACAF_07',0,'2026-06-17 00:00:20.7998980'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_03 3 - 0 CONCACAF_07',0,'2026-06-17 00:00:20.7999560'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_07 0 - 0 UEFA_05',0,'2026-06-17 00:00:20.8237910'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_07 0 - 0 UEFA_05',0,'2026-06-17 00:00:20.8238480'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_06 3 - 4 CONCACAF_01',0,'2026-06-17 00:00:20.8473420'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_06 3 - 4 CONCACAF_01',0,'2026-06-17 00:00:20.8473940'),
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_07 3 - 1 CONCACAF_01',0,'2026-06-17 00:00:20.8804960'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_07 3 - 1 CONCACAF_01',0,'2026-06-17 00:00:20.8805660'),
	 (N'periodista1@mundial.com',N'Resultado partido: AFC_06 3 - 4 UEFA_05',0,'2026-06-17 00:00:20.9034790'),
	 (N'periodista2@mundial.com',N'Resultado partido: AFC_06 3 - 4 UEFA_05',0,'2026-06-17 00:00:20.9035310');
INSERT INTO MundialDb.dbo.Notificaciones (UsuarioCorreo,Mensaje,Leida,Fecha) VALUES
	 (N'periodista1@mundial.com',N'Resultado partido: CAF_07 4 - 1 AFC_06',0,'2026-06-17 00:00:20.9422660'),
	 (N'periodista2@mundial.com',N'Resultado partido: CAF_07 4 - 1 AFC_06',0,'2026-06-17 00:00:20.9423240'),
	 (N'periodista1@mundial.com',N'Resultado partido: CONCACAF_01 0 - 0 UEFA_05',0,'2026-06-17 00:00:20.9642500'),
	 (N'periodista2@mundial.com',N'Resultado partido: CONCACAF_01 0 - 0 UEFA_05',0,'2026-06-17 00:00:20.9643090');

    -----------------------------------------------------------------------------
    -- 16_Logs: Logs_202606170024.sql
    -----------------------------------------------------------------------------
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'admin1@mundial.com',N'Carga inicial del usuario administrador admin1@mundial.com','2026-06-17 00:00:16.0022120'),
	 (N'admin1@mundial.com',N'Creación de usuario: admin2@mundial.com','2026-06-17 00:00:16.3439040'),
	 (N'admin1@mundial.com',N'Creación de usuario: editor1@mundial.com','2026-06-17 00:00:16.3582420'),
	 (N'admin1@mundial.com',N'Creación de usuario: editor2@mundial.com','2026-06-17 00:00:16.3674280'),
	 (N'admin1@mundial.com',N'Creación de usuario: periodista1@mundial.com','2026-06-17 00:00:16.3761890'),
	 (N'admin1@mundial.com',N'Creación de usuario: periodista2@mundial.com','2026-06-17 00:00:16.3846500'),
	 (N'admin1@mundial.com',N'Creado el estadio Estadio Centenario','2026-06-17 00:00:16.4394840'),
	 (N'admin1@mundial.com',N'Creado el estadio Estadio Azteca','2026-06-17 00:00:16.4483990'),
	 (N'admin1@mundial.com',N'Creado el estadio MetLife Stadium','2026-06-17 00:00:16.4557920'),
	 (N'admin1@mundial.com',N'Creado el estadio SoFi Stadium','2026-06-17 00:00:16.4637580');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'admin1@mundial.com',N'Creado el estadio BC Place','2026-06-17 00:00:16.4717220'),
	 (N'admin1@mundial.com',N'Creado el equipo Uruguay','2026-06-17 00:00:16.5278720'),
	 (N'admin1@mundial.com',N'Creado el equipo Argentina','2026-06-17 00:00:16.5395090'),
	 (N'admin1@mundial.com',N'Creado el equipo Francia','2026-06-17 00:00:16.5492870'),
	 (N'admin1@mundial.com',N'Creado el equipo Espana','2026-06-17 00:00:16.5594360'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_01','2026-06-17 00:00:16.5734080'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_02','2026-06-17 00:00:16.5846620'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_03','2026-06-17 00:00:16.5959540'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_04','2026-06-17 00:00:16.6074710'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_05','2026-06-17 00:00:16.6195240');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_06','2026-06-17 00:00:16.6298270'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_07','2026-06-17 00:00:16.6401520'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_08','2026-06-17 00:00:16.6516750'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_09','2026-06-17 00:00:16.6643380'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_10','2026-06-17 00:00:16.6763160'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_11','2026-06-17 00:00:16.6874330'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_12','2026-06-17 00:00:16.6992710'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_13','2026-06-17 00:00:16.7106870'),
	 (N'admin1@mundial.com',N'Creado el equipo UEFA_14','2026-06-17 00:00:16.7223320'),
	 (N'admin1@mundial.com',N'Creado el equipo CONMEBOL_01','2026-06-17 00:00:16.7356770');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'admin1@mundial.com',N'Creado el equipo CONMEBOL_02','2026-06-17 00:00:16.7481290'),
	 (N'admin1@mundial.com',N'Creado el equipo CONMEBOL_03','2026-06-17 00:00:16.7790580'),
	 (N'admin1@mundial.com',N'Creado el equipo CONMEBOL_04','2026-06-17 00:00:16.7921860'),
	 (N'admin1@mundial.com',N'Creado el equipo CONMEBOL_05','2026-06-17 00:00:16.8049910'),
	 (N'admin1@mundial.com',N'Creado el equipo CONCACAF_01','2026-06-17 00:00:16.8201800'),
	 (N'admin1@mundial.com',N'Creado el equipo CONCACAF_02','2026-06-17 00:00:16.8324200'),
	 (N'admin1@mundial.com',N'Creado el equipo CONCACAF_03','2026-06-17 00:00:16.8445660'),
	 (N'admin1@mundial.com',N'Creado el equipo CONCACAF_04','2026-06-17 00:00:16.8596340'),
	 (N'admin1@mundial.com',N'Creado el equipo CONCACAF_05','2026-06-17 00:00:16.8706030'),
	 (N'admin1@mundial.com',N'Creado el equipo CONCACAF_06','2026-06-17 00:00:16.8803360');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'admin1@mundial.com',N'Creado el equipo CONCACAF_07','2026-06-17 00:00:16.8908540'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_01','2026-06-17 00:00:16.9044330'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_02','2026-06-17 00:00:16.9160670'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_03','2026-06-17 00:00:16.9265800'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_04','2026-06-17 00:00:16.9361990'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_05','2026-06-17 00:00:16.9461100'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_06','2026-06-17 00:00:16.9569440'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_07','2026-06-17 00:00:16.9669500'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_08','2026-06-17 00:00:16.9794840'),
	 (N'admin1@mundial.com',N'Creado el equipo CAF_09','2026-06-17 00:00:16.9918710');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'admin1@mundial.com',N'Creado el equipo AFC_01','2026-06-17 00:00:17.0053320'),
	 (N'admin1@mundial.com',N'Creado el equipo AFC_02','2026-06-17 00:00:17.0181700'),
	 (N'admin1@mundial.com',N'Creado el equipo AFC_03','2026-06-17 00:00:17.0293340'),
	 (N'admin1@mundial.com',N'Creado el equipo AFC_04','2026-06-17 00:00:17.0394380'),
	 (N'admin1@mundial.com',N'Creado el equipo AFC_05','2026-06-17 00:00:17.0503320'),
	 (N'admin1@mundial.com',N'Creado el equipo AFC_06','2026-06-17 00:00:17.0620290'),
	 (N'admin1@mundial.com',N'Creado el equipo AFC_07','2026-06-17 00:00:17.0730380'),
	 (N'admin1@mundial.com',N'Creado el equipo AFC_08','2026-06-17 00:00:17.0839500'),
	 (N'admin1@mundial.com',N'Creado el equipo OFC_01','2026-06-17 00:00:17.0957230'),
	 (N'editor1@mundial.com',N'Creación de sorteo con token: 1. Semilla: 1','2026-06-17 00:00:17.2710540');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Sortear equipos con token : 1','2026-06-17 00:00:17.3328350'),
	 (N'editor1@mundial.com',N'Creacion de fixture con token : 1','2026-06-17 00:00:18.3147330'),
	 (N'editor1@mundial.com',N'Partido ID 1 simulado automáticamente. Resultado: 1 - 1','2026-06-17 00:00:18.5207510'),
	 (N'editor1@mundial.com',N'Partido ID 2 simulado automáticamente. Resultado: 3 - 5','2026-06-17 00:00:18.5887570'),
	 (N'editor1@mundial.com',N'Partido ID 3 simulado automáticamente. Resultado: 1 - 0','2026-06-17 00:00:18.6206830'),
	 (N'editor1@mundial.com',N'Partido ID 4 simulado automáticamente. Resultado: 3 - 3','2026-06-17 00:00:18.6506490'),
	 (N'editor1@mundial.com',N'Partido ID 5 simulado automáticamente. Resultado: 0 - 1','2026-06-17 00:00:18.6839190'),
	 (N'editor1@mundial.com',N'Partido ID 6 simulado automáticamente. Resultado: 0 - 1','2026-06-17 00:00:18.7286740'),
	 (N'editor1@mundial.com',N'Partido ID 7 simulado automáticamente. Resultado: 0 - 3','2026-06-17 00:00:18.7488630'),
	 (N'editor1@mundial.com',N'Partido ID 8 simulado automáticamente. Resultado: 0 - 0','2026-06-17 00:00:18.7650900');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Partido ID 9 simulado automáticamente. Resultado: 5 - 1','2026-06-17 00:00:18.7947690'),
	 (N'editor1@mundial.com',N'Partido ID 10 simulado automáticamente. Resultado: 4 - 2','2026-06-17 00:00:18.8259670'),
	 (N'editor1@mundial.com',N'Partido ID 11 simulado automáticamente. Resultado: 1 - 3','2026-06-17 00:00:18.8555110'),
	 (N'editor1@mundial.com',N'Partido ID 12 simulado automáticamente. Resultado: 1 - 3','2026-06-17 00:00:18.8989490'),
	 (N'editor1@mundial.com',N'Partido ID 13 simulado automáticamente. Resultado: 0 - 5','2026-06-17 00:00:18.9262070'),
	 (N'editor1@mundial.com',N'Partido ID 14 simulado automáticamente. Resultado: 3 - 5','2026-06-17 00:00:18.9534890'),
	 (N'editor1@mundial.com',N'Partido ID 15 simulado automáticamente. Resultado: 5 - 2','2026-06-17 00:00:18.9829650'),
	 (N'editor1@mundial.com',N'Partido ID 16 simulado automáticamente. Resultado: 2 - 3','2026-06-17 00:00:19.0186280'),
	 (N'editor1@mundial.com',N'Partido ID 17 simulado automáticamente. Resultado: 4 - 0','2026-06-17 00:00:19.0407580'),
	 (N'editor1@mundial.com',N'Partido ID 18 simulado automáticamente. Resultado: 2 - 3','2026-06-17 00:00:19.0612500');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Partido ID 19 simulado automáticamente. Resultado: 4 - 5','2026-06-17 00:00:19.0892780'),
	 (N'editor1@mundial.com',N'Partido ID 20 simulado automáticamente. Resultado: 2 - 1','2026-06-17 00:00:19.1207530'),
	 (N'editor1@mundial.com',N'Partido ID 21 simulado automáticamente. Resultado: 2 - 1','2026-06-17 00:00:19.1458490'),
	 (N'editor1@mundial.com',N'Partido ID 22 simulado automáticamente. Resultado: 2 - 2','2026-06-17 00:00:19.1713200'),
	 (N'editor1@mundial.com',N'Partido ID 23 simulado automáticamente. Resultado: 2 - 0','2026-06-17 00:00:19.1941460'),
	 (N'editor1@mundial.com',N'Partido ID 24 simulado automáticamente. Resultado: 2 - 2','2026-06-17 00:00:19.2257890'),
	 (N'editor1@mundial.com',N'Partido ID 25 simulado automáticamente. Resultado: 5 - 2','2026-06-17 00:00:19.3098310'),
	 (N'editor1@mundial.com',N'Partido ID 26 simulado automáticamente. Resultado: 0 - 2','2026-06-17 00:00:19.3391920'),
	 (N'editor1@mundial.com',N'Partido ID 27 simulado automáticamente. Resultado: 3 - 0','2026-06-17 00:00:19.3637790'),
	 (N'editor1@mundial.com',N'Partido ID 28 simulado automáticamente. Resultado: 1 - 0','2026-06-17 00:00:19.3887880');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Partido ID 29 simulado automáticamente. Resultado: 0 - 4','2026-06-17 00:00:19.4220240'),
	 (N'editor1@mundial.com',N'Partido ID 30 simulado automáticamente. Resultado: 4 - 0','2026-06-17 00:00:19.4691740'),
	 (N'editor1@mundial.com',N'Partido ID 31 simulado automáticamente. Resultado: 0 - 0','2026-06-17 00:00:19.5041450'),
	 (N'editor1@mundial.com',N'Partido ID 32 simulado automáticamente. Resultado: 2 - 2','2026-06-17 00:00:19.6130910'),
	 (N'editor1@mundial.com',N'Partido ID 33 simulado automáticamente. Resultado: 1 - 1','2026-06-17 00:00:19.6874610'),
	 (N'editor1@mundial.com',N'Partido ID 34 simulado automáticamente. Resultado: 0 - 4','2026-06-17 00:00:19.7373510'),
	 (N'editor1@mundial.com',N'Partido ID 35 simulado automáticamente. Resultado: 0 - 1','2026-06-17 00:00:19.7799280'),
	 (N'editor1@mundial.com',N'Partido ID 36 simulado automáticamente. Resultado: 0 - 1','2026-06-17 00:00:19.8218030'),
	 (N'editor1@mundial.com',N'Partido ID 37 simulado automáticamente. Resultado: 3 - 4','2026-06-17 00:00:19.8600970'),
	 (N'editor1@mundial.com',N'Partido ID 38 simulado automáticamente. Resultado: 3 - 2','2026-06-17 00:00:19.8870930');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Partido ID 39 simulado automáticamente. Resultado: 2 - 3','2026-06-17 00:00:19.9161530'),
	 (N'editor1@mundial.com',N'Partido ID 40 simulado automáticamente. Resultado: 3 - 0','2026-06-17 00:00:19.9451130'),
	 (N'editor1@mundial.com',N'Partido ID 41 simulado automáticamente. Resultado: 1 - 2','2026-06-17 00:00:19.9854530'),
	 (N'editor1@mundial.com',N'Partido ID 42 simulado automáticamente. Resultado: 0 - 0','2026-06-17 00:00:20.0253760'),
	 (N'editor1@mundial.com',N'Partido ID 43 simulado automáticamente. Resultado: 3 - 2','2026-06-17 00:00:20.0654480'),
	 (N'editor1@mundial.com',N'Partido ID 44 simulado automáticamente. Resultado: 1 - 1','2026-06-17 00:00:20.1046800'),
	 (N'editor1@mundial.com',N'Partido ID 45 simulado automáticamente. Resultado: 3 - 0','2026-06-17 00:00:20.1369200'),
	 (N'editor1@mundial.com',N'Partido ID 46 simulado automáticamente. Resultado: 0 - 3','2026-06-17 00:00:20.1718580'),
	 (N'editor1@mundial.com',N'Partido ID 47 simulado automáticamente. Resultado: 4 - 1','2026-06-17 00:00:20.2127720'),
	 (N'editor1@mundial.com',N'Partido ID 48 simulado automáticamente. Resultado: 2 - 4','2026-06-17 00:00:20.2427490');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Partido ID 49 simulado automáticamente. Resultado: 3 - 2','2026-06-17 00:00:20.2714170'),
	 (N'editor1@mundial.com',N'Partido ID 50 simulado automáticamente. Resultado: 5 - 0','2026-06-17 00:00:20.3041390'),
	 (N'editor1@mundial.com',N'Partido ID 51 simulado automáticamente. Resultado: 0 - 2','2026-06-17 00:00:20.3317570'),
	 (N'editor1@mundial.com',N'Partido ID 52 simulado automáticamente. Resultado: 1 - 0','2026-06-17 00:00:20.3601870'),
	 (N'editor1@mundial.com',N'Partido ID 53 simulado automáticamente. Resultado: 1 - 3','2026-06-17 00:00:20.4044440'),
	 (N'editor1@mundial.com',N'Partido ID 54 simulado automáticamente. Resultado: 1 - 1','2026-06-17 00:00:20.4380140'),
	 (N'editor1@mundial.com',N'Partido ID 55 simulado automáticamente. Resultado: 1 - 5','2026-06-17 00:00:20.4712300'),
	 (N'editor1@mundial.com',N'Partido ID 56 simulado automáticamente. Resultado: 1 - 3','2026-06-17 00:00:20.5017710'),
	 (N'editor1@mundial.com',N'Partido ID 57 simulado automáticamente. Resultado: 1 - 1','2026-06-17 00:00:20.5343550'),
	 (N'editor1@mundial.com',N'Partido ID 58 simulado automáticamente. Resultado: 4 - 1','2026-06-17 00:00:20.5696870');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Partido ID 59 simulado automáticamente. Resultado: 4 - 3','2026-06-17 00:00:20.6013180'),
	 (N'editor1@mundial.com',N'Partido ID 60 simulado automáticamente. Resultado: 1 - 2','2026-06-17 00:00:20.6305530'),
	 (N'editor1@mundial.com',N'Partido ID 61 simulado automáticamente. Resultado: 0 - 2','2026-06-17 00:00:20.6545180'),
	 (N'editor1@mundial.com',N'Partido ID 62 simulado automáticamente. Resultado: 4 - 1','2026-06-17 00:00:20.6792240'),
	 (N'editor1@mundial.com',N'Partido ID 63 simulado automáticamente. Resultado: 2 - 2','2026-06-17 00:00:20.7035610'),
	 (N'editor1@mundial.com',N'Partido ID 64 simulado automáticamente. Resultado: 2 - 1','2026-06-17 00:00:20.7371200'),
	 (N'editor1@mundial.com',N'Partido ID 65 simulado automáticamente. Resultado: 4 - 2','2026-06-17 00:00:20.7615580'),
	 (N'editor1@mundial.com',N'Partido ID 66 simulado automáticamente. Resultado: 3 - 0','2026-06-17 00:00:20.7845480'),
	 (N'editor1@mundial.com',N'Partido ID 67 simulado automáticamente. Resultado: 0 - 0','2026-06-17 00:00:20.8085200'),
	 (N'editor1@mundial.com',N'Partido ID 68 simulado automáticamente. Resultado: 3 - 4','2026-06-17 00:00:20.8318840');
INSERT INTO MundialDb.dbo.Logs (UsuarioCorreo,Accion,Fecha) VALUES
	 (N'editor1@mundial.com',N'Partido ID 69 simulado automáticamente. Resultado: 3 - 1','2026-06-17 00:00:20.8554670'),
	 (N'editor1@mundial.com',N'Partido ID 70 simulado automáticamente. Resultado: 3 - 4','2026-06-17 00:00:20.8891720'),
	 (N'editor1@mundial.com',N'Partido ID 71 simulado automáticamente. Resultado: 4 - 1','2026-06-17 00:00:20.9128000'),
	 (N'editor1@mundial.com',N'Partido ID 72 simulado automáticamente. Resultado: 0 - 0','2026-06-17 00:00:20.9479730'),
	 (N'editor1@mundial.com',N'Se realizó una simulación con la semilla de token: 1','2026-06-17 00:00:20.9690380'),
	 (N'admin1@mundial.com',N'Login exitoso del usuario admin1@mundial.com','2026-06-17 00:14:27.1247100'),
	 (N'admin1@mundial.com',N'Modificacion del equipo Argentina','2026-06-17 00:15:16.5482320'),
	 (N'admin1@mundial.com',N'Modificacion del equipo Espana','2026-06-17 00:15:41.5345960'),
	 (N'admin1@mundial.com',N'Modificacion del equipo Francia','2026-06-17 00:16:06.9178520'),
	 (N'admin1@mundial.com',N'Modificacion del equipo Uruguay','2026-06-17 00:16:34.2856400');



    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO
