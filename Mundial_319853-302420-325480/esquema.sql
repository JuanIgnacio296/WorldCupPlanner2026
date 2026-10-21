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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Equipos] (
        [Nombre] nvarchar(60) NOT NULL,
        [Confederacion] nvarchar(max) NOT NULL,
        [RankingFifa] int NOT NULL,
        [Puntos] int NOT NULL DEFAULT 0,
        [GolesAFavor] int NOT NULL DEFAULT 0,
        [GolesEnContra] int NOT NULL DEFAULT 0,
        [BanderaBase64] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Equipos] PRIMARY KEY ([Nombre]),
        CONSTRAINT [CK_Equipo_Confederacion] CHECK ([Confederacion] IN ('AFC', 'CAF', 'CONCACAF', 'CONMEBOL', 'OFC', 'UEFA')),
        CONSTRAINT [CK_Equipo_RankingFifa] CHECK ([RankingFifa] BETWEEN 300 AND 2500)
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Notificaciones] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioCorreo] nvarchar(max) NOT NULL,
        [Mensaje] nvarchar(max) NOT NULL,
        [Leida] bit NOT NULL,
        [Fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_Notificaciones] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Semillas] (
        [Token] int NOT NULL,
        [SemillaFixture] int NOT NULL,
        [SemillaCompletar] int NOT NULL,
        [SemillaCrucesFase] int NOT NULL,
        [SemillaSimulacion] int NOT NULL,
        CONSTRAINT [PK_Semillas] PRIMARY KEY ([Token])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Correo] nvarchar(450) NOT NULL,
        [Nombre] nvarchar(max) NOT NULL,
        [Apellido] nvarchar(max) NOT NULL,
        [Contrasena] nvarchar(max) NOT NULL,
        [FechaNacimiento] datetime2 NOT NULL,
        [Roles] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Correo])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Fixtures] (
        [Token] int NOT NULL,
        [SemillaToken] int NOT NULL,
        CONSTRAINT [PK_Fixtures] PRIMARY KEY ([Token]),
        CONSTRAINT [FK_Fixtures_Semillas_SemillaToken] FOREIGN KEY ([SemillaToken]) REFERENCES [Semillas] ([Token]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Sorteos] (
        [Token] int NOT NULL,
        [SemillaToken] int NOT NULL,
        CONSTRAINT [PK_Sorteos] PRIMARY KEY ([Token]),
        CONSTRAINT [FK_Sorteos_Semillas_SemillaToken] FOREIGN KEY ([SemillaToken]) REFERENCES [Semillas] ([Token]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Logs] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioCorreo] nvarchar(450) NOT NULL,
        [Accion] nvarchar(200) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_Logs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Logs_Usuarios_UsuarioCorreo] FOREIGN KEY ([UsuarioCorreo]) REFERENCES [Usuarios] ([Correo]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [EquipoSorteo] (
        [EquiposNombre] nvarchar(60) NOT NULL,
        [SorteoToken] int NOT NULL,
        CONSTRAINT [PK_EquipoSorteo] PRIMARY KEY ([EquiposNombre], [SorteoToken]),
        CONSTRAINT [FK_EquipoSorteo_Equipos_EquiposNombre] FOREIGN KEY ([EquiposNombre]) REFERENCES [Equipos] ([Nombre]) ON DELETE CASCADE,
        CONSTRAINT [FK_EquipoSorteo_Sorteos_SorteoToken] FOREIGN KEY ([SorteoToken]) REFERENCES [Sorteos] ([Token]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Grupos] (
        [Id] nvarchar(1) NOT NULL,
        [Token] int NOT NULL,
        CONSTRAINT [PK_Grupos] PRIMARY KEY ([Id], [Token]),
        CONSTRAINT [CK_Grupo_Id] CHECK ([Id] IN ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L')),
        CONSTRAINT [FK_Grupos_Sorteos_Token] FOREIGN KEY ([Token]) REFERENCES [Sorteos] ([Token]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [EquipoGrupo] (
        [EquiposNombre] nvarchar(60) NOT NULL,
        [GrupoId] nvarchar(1) NOT NULL,
        [GrupoToken] int NOT NULL,
        CONSTRAINT [PK_EquipoGrupo] PRIMARY KEY ([EquiposNombre], [GrupoId], [GrupoToken]),
        CONSTRAINT [FK_EquipoGrupo_Equipos_EquiposNombre] FOREIGN KEY ([EquiposNombre]) REFERENCES [Equipos] ([Nombre]) ON DELETE CASCADE,
        CONSTRAINT [FK_EquipoGrupo_Grupos_GrupoId_GrupoToken] FOREIGN KEY ([GrupoId], [GrupoToken]) REFERENCES [Grupos] ([Id], [Token]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Estadios] (
        [Nombre] nvarchar(80) NOT NULL,
        [Id] int NOT NULL,
        [Ciudad] nvarchar(60) NOT NULL,
        [Descripcion] nvarchar(400) NOT NULL,
        [Capacidad] int NOT NULL,
        [FixtureToken] int NULL,
        [GrupoId] nvarchar(1) NULL,
        [GrupoToken] int NULL,
        CONSTRAINT [PK_Estadios] PRIMARY KEY ([Nombre]),
        CONSTRAINT [CK_Estadio_Capacidad] CHECK ([Capacidad] >= 20000),
        CONSTRAINT [FK_Estadios_Fixtures_FixtureToken] FOREIGN KEY ([FixtureToken]) REFERENCES [Fixtures] ([Token]),
        CONSTRAINT [FK_Estadios_Grupos_GrupoId_GrupoToken] FOREIGN KEY ([GrupoId], [GrupoToken]) REFERENCES [Grupos] ([Id], [Token])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [EstadioSorteo] (
        [EstadiosNombre] nvarchar(80) NOT NULL,
        [SorteoToken] int NOT NULL,
        CONSTRAINT [PK_EstadioSorteo] PRIMARY KEY ([EstadiosNombre], [SorteoToken]),
        CONSTRAINT [FK_EstadioSorteo_Estadios_EstadiosNombre] FOREIGN KEY ([EstadiosNombre]) REFERENCES [Estadios] ([Nombre]) ON DELETE CASCADE,
        CONSTRAINT [FK_EstadioSorteo_Sorteos_SorteoToken] FOREIGN KEY ([SorteoToken]) REFERENCES [Sorteos] ([Token]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [FechasOcupadas] (
        [Fecha] datetime2 NOT NULL,
        [EstadioNombre] nvarchar(80) NOT NULL,
        CONSTRAINT [PK_FechasOcupadas] PRIMARY KEY ([EstadioNombre], [Fecha]),
        CONSTRAINT [FK_FechasOcupadas_Estadios_EstadioNombre] FOREIGN KEY ([EstadioNombre]) REFERENCES [Estadios] ([Nombre]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE TABLE [Partidos] (
        [Id] int NOT NULL,
        [Token] int NOT NULL,
        [GrupoID] nvarchar(1) NULL,
        [Fecha] datetime2 NOT NULL,
        [EstadioNombre] nvarchar(80) NULL,
        [EquipoLocalNombre] nvarchar(60) NULL,
        [EquipoVisitanteNombre] nvarchar(60) NULL,
        [GolesLocal] int NULL,
        [GolesVisitante] int NULL,
        [Fase] nvarchar(max) NOT NULL,
        [Horario] nvarchar(max) NOT NULL,
        [Jornada] int NULL,
        [PenalesLocal] int NULL,
        [PenalesVisitante] int NULL,
        [TarjetasAmarillasLocal] int NOT NULL,
        [TarjetasRojasLocal] int NOT NULL,
        [TarjetasAmarillasVisitante] int NOT NULL,
        [TarjetasRojasVisitante] int NOT NULL,
        [GrupoToken] int NULL,
        CONSTRAINT [PK_Partidos] PRIMARY KEY ([Id], [Token]),
        CONSTRAINT [CK_Partido_Fase] CHECK ([Fase] IN ('Fase de Grupos', 'Dieciseisavos de Final', 'Octavos de Final', 'Cuartos de Final', 'Semifinal', 'Tercer y Cuarto Puesto', 'Final')),
        CONSTRAINT [CK_Partido_GolesLocal] CHECK ([GolesLocal] IS NULL OR [GolesLocal] >= 0),
        CONSTRAINT [CK_Partido_GolesVisitante] CHECK ([GolesVisitante] IS NULL OR [GolesVisitante] >= 0),
        CONSTRAINT [CK_Partido_Jornada] CHECK ([Jornada] IS NULL OR [Jornada] BETWEEN 1 AND 3),
        CONSTRAINT [CK_Partido_PenalesLocal] CHECK ([PenalesLocal] IS NULL OR [PenalesLocal] >= 0),
        CONSTRAINT [CK_Partido_PenalesVisitante] CHECK ([PenalesVisitante] IS NULL OR [PenalesVisitante] >= 0),
        CONSTRAINT [CK_Partido_TarjetasAmarillasLocal] CHECK ([TarjetasAmarillasLocal] IS NULL OR [TarjetasAmarillasLocal] >= 0),
        CONSTRAINT [CK_Partido_TarjetasAmarillasVisitante] CHECK ([TarjetasAmarillasVisitante] IS NULL OR [TarjetasAmarillasVisitante] >= 0),
        CONSTRAINT [CK_Partido_TarjetasRojasLocal] CHECK ([TarjetasRojasLocal] IS NULL OR [TarjetasRojasLocal] >= 0),
        CONSTRAINT [CK_Partido_TarjetasRojasVisitante] CHECK ([TarjetasRojasVisitante] IS NULL OR [TarjetasRojasVisitante] >= 0),
        CONSTRAINT [FK_Partidos_Equipos_EquipoLocalNombre] FOREIGN KEY ([EquipoLocalNombre]) REFERENCES [Equipos] ([Nombre]),
        CONSTRAINT [FK_Partidos_Equipos_EquipoVisitanteNombre] FOREIGN KEY ([EquipoVisitanteNombre]) REFERENCES [Equipos] ([Nombre]),
        CONSTRAINT [FK_Partidos_Estadios_EstadioNombre] FOREIGN KEY ([EstadioNombre]) REFERENCES [Estadios] ([Nombre]),
        CONSTRAINT [FK_Partidos_Fixtures_Token] FOREIGN KEY ([Token]) REFERENCES [Fixtures] ([Token]) ON DELETE CASCADE,
        CONSTRAINT [FK_Partidos_Grupos_GrupoID_GrupoToken] FOREIGN KEY ([GrupoID], [GrupoToken]) REFERENCES [Grupos] ([Id], [Token])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EquipoGrupo_GrupoId_GrupoToken] ON [EquipoGrupo] ([GrupoId], [GrupoToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Equipos_Nombre] ON [Equipos] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EquipoSorteo_SorteoToken] ON [EquipoSorteo] ([SorteoToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Estadios_FixtureToken] ON [Estadios] ([FixtureToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Estadios_GrupoId_GrupoToken] ON [Estadios] ([GrupoId], [GrupoToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Estadios_Nombre] ON [Estadios] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EstadioSorteo_SorteoToken] ON [EstadioSorteo] ([SorteoToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Fixtures_SemillaToken] ON [Fixtures] ([SemillaToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Grupos_Token] ON [Grupos] ([Token]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Logs_UsuarioCorreo] ON [Logs] ([UsuarioCorreo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Partidos_EquipoLocalNombre] ON [Partidos] ([EquipoLocalNombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Partidos_EquipoVisitanteNombre] ON [Partidos] ([EquipoVisitanteNombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Partidos_EstadioNombre] ON [Partidos] ([EstadioNombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Partidos_GrupoID_GrupoToken] ON [Partidos] ([GrupoID], [GrupoToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Partidos_Token] ON [Partidos] ([Token]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sorteos_SemillaToken] ON [Sorteos] ([SemillaToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615144438_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260615144438_InitialCreate', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615151450_FixtureNoCascadePartidos'
)
BEGIN
    ALTER TABLE [Partidos] DROP CONSTRAINT [FK_Partidos_Fixtures_Token];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615151450_FixtureNoCascadePartidos'
)
BEGIN
    DROP INDEX [IX_Partidos_Token] ON [Partidos];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615151450_FixtureNoCascadePartidos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260615151450_FixtureNoCascadePartidos', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615203125_MotorProbabilisticoYEstadioFixture'
)
BEGIN
    ALTER TABLE [Estadios] DROP CONSTRAINT [FK_Estadios_Fixtures_FixtureToken];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615203125_MotorProbabilisticoYEstadioFixture'
)
BEGIN
    DROP INDEX [IX_Estadios_FixtureToken] ON [Estadios];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615203125_MotorProbabilisticoYEstadioFixture'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Estadios]') AND [c].[name] = N'FixtureToken');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Estadios] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Estadios] DROP COLUMN [FixtureToken];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615203125_MotorProbabilisticoYEstadioFixture'
)
BEGIN
    ALTER TABLE [Fixtures] ADD [UsaMotorProbabilistico] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615203125_MotorProbabilisticoYEstadioFixture'
)
BEGIN
    CREATE TABLE [EstadioFixture] (
        [EstadiosNombre] nvarchar(80) NOT NULL,
        [FixtureToken] int NOT NULL,
        CONSTRAINT [PK_EstadioFixture] PRIMARY KEY ([EstadiosNombre], [FixtureToken]),
        CONSTRAINT [FK_EstadioFixture_Estadios_EstadiosNombre] FOREIGN KEY ([EstadiosNombre]) REFERENCES [Estadios] ([Nombre]) ON DELETE CASCADE,
        CONSTRAINT [FK_EstadioFixture_Fixtures_FixtureToken] FOREIGN KEY ([FixtureToken]) REFERENCES [Fixtures] ([Token]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615203125_MotorProbabilisticoYEstadioFixture'
)
BEGIN
    CREATE INDEX [IX_EstadioFixture_FixtureToken] ON [EstadioFixture] ([FixtureToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615203125_MotorProbabilisticoYEstadioFixture'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260615203125_MotorProbabilisticoYEstadioFixture', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616125649_FechaOcupadaPorFixture'
)
BEGIN
    ALTER TABLE [FechasOcupadas] DROP CONSTRAINT [PK_FechasOcupadas];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616125649_FechaOcupadaPorFixture'
)
BEGIN
    ALTER TABLE [FechasOcupadas] ADD [FixtureToken] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616125649_FechaOcupadaPorFixture'
)
BEGIN
    ALTER TABLE [FechasOcupadas] ADD CONSTRAINT [PK_FechasOcupadas] PRIMARY KEY ([EstadioNombre], [Fecha], [FixtureToken]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616125649_FechaOcupadaPorFixture'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616125649_FechaOcupadaPorFixture', N'8.0.27');
END;
GO

COMMIT;
GO

