# Mundial 2026

Aplicación web en .NET 8 para administrar y simular un Mundial 2026. Permite gestionar equipos, estadios y usuarios, realizar el sorteo de grupos, generar el fixture, cargar y simular resultados (incluyendo penales), auditar acciones y exportar reportes, todo desde una interfaz Blazor Server.

> [!NOTE]
> La persistencia es sobre **SQL Server** mediante Entity Framework Core. Los datos se conservan entre reinicios de la aplicación.

## Características

- Gestión de equipos con confederación, ranking FIFA y estadísticas.
- Importación de equipos desde archivos CSV (transaccional por archivo).
- Gestión de estadios (con fechas ocupadas) y de usuarios con roles.
- Sorteo de grupos A–L respetando restricciones por confederación.
- Generación del fixture para fase de grupos y fases eliminatorias.
- Carga y simulación de resultados con motor probabilístico y definición por penales.
- Notificaciones a periodistas con los resultados de los partidos.
- Auditoría de acciones relevantes con exportación a **CSV** y **XLSX**.
- Exportación del fixture final a **CSV** y **XLSX**.
- Semillas configurables para reproducir sorteos y simulaciones.
- Suite de pruebas unitarias para dominio, servicios, acceso a datos y DTOs.

## Stack

- .NET 8 / ASP.NET Core **Blazor Server** (Interactive Server Components)
- **Entity Framework Core 8** sobre **SQL Server**
- MSTest + Coverlet (cobertura)
- GitHub Actions para build, tests, cobertura y versionado automático por tags

## Estructura

```text
Mundial_319853-302420-325480/
├── App-Dominio/            # Entidades y reglas de dominio (Equipo, Sorteo, Fixture, Partido, ...)
├── App.Dominio.Tests/      # Tests unitarios del dominio
├── DTOs/                   # Objetos de transferencia de datos
├── DTOs.Tests/             # Tests de DTOs y mapeos
├── Services/               # Casos de uso, validaciones de permisos y exportadores
├── Services.Tests/         # Tests unitarios de servicios
├── DataAccess/             # DbContext, migraciones EF y repositorios SQL Server
├── DataAccess.Tests/       # Tests de repositorios
├── App_UI/                 # Aplicación Blazor Server (páginas y componentes)
└── Mundial_319853-302420-325480.sln
```

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker** (para correr SQL Server) o una instancia de SQL Server accesible

Verificar la instalación:

```bash
dotnet --version
```

## Base de datos

La aplicación se conecta usando la cadena `DefaultConnection` de `App_UI/appsettings.json`:

```text
Server=localhost,1433;Database=MundialDb;User Id=sa;Password=Pass.2026;TrustServerCertificate=True;
```

### 1. Levantar SQL Server en Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Pass.2026" \
  -p 1433:1433 --name mundial-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

> [!IMPORTANT]
> El puerto debe ser `1433` y la contraseña del usuario `sa` debe coincidir con la de la cadena de conexión (`Pass.2026`).

### 2. Crear el esquema y cargar datos

Hay dos caminos equivalentes; elegí uno.

**Opción A — Aplicar las migraciones de EF Core** (crea la base y el esquema):

```bash
dotnet tool install --global dotnet-ef        # solo la primera vez
dotnet ef database update --project DataAccess --startup-project App_UI
```

**Opción B — Ejecutar los scripts SQL incluidos** (`esquema.sql` y `datos_prueba.sql`):

> [!NOTE]
> Los scripts **no crean la base**. Creala primero y ejecutalos en orden sobre `MundialDb`.

```sql
CREATE DATABASE MundialDb;
```

Luego, conectado a `MundialDb`, ejecutar **primero** `esquema.sql` (DDL) y **después** `datos_prueba.sql` (datos de prueba). Podés hacerlo con una herramienta gráfica (Azure Data Studio, DBeaver, SSMS) o por línea de comandos:

```bash
sqlcmd -S localhost,1433 -U sa -P "Pass.2026" -C -i esquema.sql
sqlcmd -S localhost,1433 -U sa -P "Pass.2026" -C -i datos_prueba.sql
```

## Puesta en marcha

Con la base de datos lista, desde esta carpeta:

```bash
dotnet restore
dotnet build
dotnet run --project App_UI/App_UI.csproj
```

Abrir la URL que muestre la consola, normalmente `https://localhost:7159` o `http://localhost:5151`.

Para ejecutar la aplicación ya publicada (en otra PC con .NET 8 y la base configurada):

```bash
dotnet App_UI.dll
```

## Usuarios de prueba

`datos_prueba.sql` carga estos usuarios:

| Rol | Correo | Contraseña |
| --- | --- | --- |
| Administrador | `admin1@mundial.com` | `Admin#123` |
| Administrador | `admin2@mundial.com` | `Admin#456` |
| Editor | `editor1@mundial.com` | `Editor#123` |
| Editor | `editor2@mundial.com` | `Editor#456` |
| Periodista | `periodista1@mundial.com` | `Peri#1234` |
| Periodista | `periodista2@mundial.com` | `Peri#5678` |

## Importación CSV

Los editores pueden importar equipos desde un `.csv` con este encabezado:

```csv
Nombre,Confederacion,RankingFIFA
Uruguay,CONMEBOL,1645
Francia,UEFA,1837
Japon,AFC,1520
```

La importación es transaccional a nivel de archivo: si alguna fila tiene errores, no se importa ningún equipo.

## Rutas principales

| Ruta | Descripción |
| --- | --- |
| `/` | Login |
| `/home` | Inicio |
| `/equipos` | Gestión de equipos |
| `/importar-equipos` | Importación CSV (editores) |
| `/estadios` | Gestión de estadios (administradores) |
| `/usuarios` | Gestión de usuarios (administradores) |
| `/fixture` | Fixture del mundial |
| `/partidos` | Gestión y simulación de partidos |
| `/semillas` | Gestión de semillas (administradores) |
| `/auditoria` | Consulta de logs (administradores) |
| `/exportacion` | Exportación de auditoría y fixture a CSV/XLSX |
| `/notificaciones` | Notificaciones del usuario |

## Modelo funcional

El dominio se centra en estas entidades:

- `Usuario`: roles **Administrador**, **Editor** o **Periodista**.
- `Equipo`: nombre, confederación, ranking FIFA y estadísticas.
- `Estadio`: ciudad, capacidad, descripción y fechas ocupadas.
- `Sorteo`: armado de grupos A–L respetando restricciones por confederación.
- `Fixture`: generación de partidos por fase y asignación de fechas/estadios.
- `Partido`: equipos, resultado, fase, jornada y definición por penales.
- `Semilla`: valores usados para reproducir fixture, completado, cruces y simulación.
- `Notificacion`: aviso a periodistas con resultados de partidos.
- `Log`: registro de acciones auditables.

## Tests

Ejecutar toda la suite:

```bash
dotnet test
```

Ejecutar un proyecto específico:

```bash
dotnet test App.Dominio.Tests/App.Dominio.Tests.csproj
dotnet test Services.Tests/Services.Tests.csproj
```

Con la configuración de cobertura que usa CI, desde la raíz del repositorio:

```bash
dotnet test --configuration Release \
  --logger "trx;LogFileName=test-results.trx" \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage --settings .runsettings
```

## Integración continua

El workflow **Build and Test** se ejecuta en pull requests y en pushes a `main`:

- restaura dependencias y compila en `Release`;
- ejecuta los tests con cobertura;
- publica los resultados y un resumen de cobertura (comentario en el PR);
- en `main`, crea automáticamente un tag incrementando el *patch* de la versión.

## Notas de desarrollo

- `App_UI` registra servicios y repositorios mediante inyección de dependencias y configura `AppDbContext` con SQL Server (`Program.cs`).
- `Services` concentra las validaciones de permisos, los casos de uso y los exportadores (CSV/XLSX).
- `App-Dominio` contiene las reglas propias del Mundial (restricciones de grupos, avance de fases, penales).
- `DataAccess` expone los repositorios `...EFRepository` y las migraciones de EF Core.