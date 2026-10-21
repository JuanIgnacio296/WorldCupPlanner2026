using Microsoft.EntityFrameworkCore;
using App.Dominio;

namespace DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<Equipo> Equipos { get; set; }
    public DbSet<Estadio> Estadios { get; set; }
    public DbSet<Fixture> Fixtures { get; set; }
    public DbSet<Grupo> Grupos { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<Partido> Partidos { get; set; }
    public DbSet<Semilla> Semillas { get; set; }
    public DbSet<Sorteo> Sorteos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<FechaOcupada> FechasOcupadas { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipo>().ToTable("Equipos");
        modelBuilder.Entity<Estadio>().ToTable("Estadios");
        modelBuilder.Entity<FechaOcupada>().ToTable("FechasOcupadas");
        modelBuilder.Entity<Grupo>().ToTable("Grupos");
        modelBuilder.Entity<Log>().ToTable("Logs");
        modelBuilder.Entity<Partido>().ToTable("Partidos");
        modelBuilder.Entity<Semilla>().ToTable("Semillas");
        modelBuilder.Entity<Sorteo>().ToTable("Sorteos");
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasKey(e => e.Nombre);
            
            entity.Property(e => e.Nombre).HasMaxLength(60);
            entity.Property(e => e.Confederacion).IsRequired();
            entity.Property(e => e.RankingFifa).IsRequired();
            entity.Property(e => e.Puntos).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.GolesAFavor).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.GolesEnContra).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.BanderaBase64).IsRequired();

            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Equipo_RankingFifa",
                "[RankingFifa] BETWEEN 300 AND 2500"
                ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Equipo_Confederacion",
                "[Confederacion] IN ('AFC', 'CAF', 'CONCACAF', 'CONMEBOL', 'OFC', 'UEFA')"
            ));
        });
        
        modelBuilder.Entity<Estadio>(entity =>
        {
            entity.HasKey(e => e.Nombre);
            
            entity.Property(e => e.Nombre).HasMaxLength(80);
            entity.Property(e => e.Ciudad).IsRequired().HasMaxLength(60);
            entity.Property(e => e.Descripcion).HasMaxLength(400);
            entity.Property(e => e.Capacidad).IsRequired();
            
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Estadio_Capacidad",
                "[Capacidad] >= 20000"
            ));

            entity.Navigation(e => e.FechasOcupadas)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<FechaOcupada>(entity =>
        {
            entity.Ignore(fo => fo.EstadioId);
            entity.Property(fo => fo.Fecha).IsRequired();
            entity.Property(fo => fo.FixtureToken).IsRequired();
            entity.Property<string>("EstadioNombre").IsRequired().HasMaxLength(80);

            entity.HasKey("EstadioNombre", nameof(FechaOcupada.Fecha), nameof(FechaOcupada.FixtureToken));

            entity.HasOne(fo => fo.Estadio)
                .WithMany(e => e.FechasOcupadas)
                .HasForeignKey("EstadioNombre")
                .HasPrincipalKey(e => e.Nombre)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(g => new { g.Id, g.Token });
            
            entity.Property(g => g.Id).IsRequired().HasMaxLength(1);
            entity.Property(g => g.Token).IsRequired();
            
            entity.HasOne<Sorteo>().WithMany(s => s.Grupos).HasForeignKey(g => g.Token);
            entity.HasMany<Equipo>(g => g.Equipos).WithMany();
            entity.Navigation(g => g.Equipos)
                .HasField("_listaEquipos")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Grupo_Id",
                "[Id] IN ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L')"
            ));
        });

        modelBuilder.Entity<Fixture>(entity =>
        {
            entity.HasKey(f => f.Token);
            entity.Property(f => f.Token).ValueGeneratedNever();

            entity.Property(f => f.UsaMotorProbabilistico).IsRequired();
            entity.HasOne<Semilla>(f => f.Semilla).WithMany().HasForeignKey("SemillaToken").IsRequired();
            entity.HasMany<Estadio>(f => f.Estadios).WithMany();

            entity.Ignore(f => f.Partidos);
            entity.Ignore(f => f.ListaPrimeros);
            entity.Ignore(f => f.ListaSegundos);
            entity.Ignore(f => f.ListaTerceros);
            entity.Ignore("Calendarizador");
            entity.Ignore(f => f.Grupos);
            entity.Ignore(f => f.PartidosJugados);
        });
        
        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Accion).IsRequired().HasMaxLength(200);
            entity.Property(l => l.Fecha).IsRequired();
            
            entity.HasOne<Usuario>(l => l.Usuario).WithMany().HasForeignKey("UsuarioCorreo").IsRequired();
        });

        modelBuilder.Entity<Partido>(entity =>
        {
            entity.HasKey(p => new { p.Id, p.Token });
            entity.Property(p => p.Id).ValueGeneratedNever();

            entity.Property(p => p.Fecha).IsRequired();
            entity.Property(p => p.Fase).IsRequired();
            entity.Property(p => p.Horario).IsRequired();
            entity.Property(p => p.Jornada);
            entity.Property(p => p.GolesLocal);
            entity.Property(p => p.GolesVisitante);
            entity.Property(p => p.PenalesLocal);
            entity.Property(p => p.PenalesVisitante);
            entity.Property(p => p.TarjetasAmarillasLocal);
            entity.Property(p => p.TarjetasAmarillasVisitante);
            entity.Property(p => p.TarjetasRojasLocal);
            entity.Property(p => p.TarjetasRojasVisitante);
            
            entity.HasOne<Estadio>(p => p.Estadio).WithMany().HasForeignKey("EstadioNombre");
            entity.HasOne<Equipo>(p => p.EquipoLocal).WithMany().HasForeignKey("EquipoLocalNombre");
            entity.HasOne<Equipo>(p => p.EquipoVisitante).WithMany().HasForeignKey("EquipoVisitanteNombre");
            
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_Fase",
                "[Fase] IN ('Fase de Grupos', 'Dieciseisavos de Final', 'Octavos de Final', 'Cuartos de Final', 'Semifinal', 'Tercer y Cuarto Puesto', 'Final')"
            ));

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_Jornada",
                "[Jornada] IS NULL OR [Jornada] BETWEEN 1 AND 3"
            ));
            
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_GolesLocal",
                "[GolesLocal] IS NULL OR [GolesLocal] >= 0"
            ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_GolesVisitante",
                "[GolesVisitante] IS NULL OR [GolesVisitante] >= 0"
            ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_PenalesLocal",
                "[PenalesLocal] IS NULL OR [PenalesLocal] >= 0"
            ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_PenalesVisitante",
                "[PenalesVisitante] IS NULL OR [PenalesVisitante] >= 0"
            ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_TarjetasAmarillasLocal",
                "[TarjetasAmarillasLocal] IS NULL OR [TarjetasAmarillasLocal] >= 0"
            ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_TarjetasAmarillasVisitante",
                "[TarjetasAmarillasVisitante] IS NULL OR [TarjetasAmarillasVisitante] >= 0"
            ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_TarjetasRojasLocal",
                "[TarjetasRojasLocal] IS NULL OR [TarjetasRojasLocal] >= 0"
            ));
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Partido_TarjetasRojasVisitante",
                "[TarjetasRojasVisitante] IS NULL OR [TarjetasRojasVisitante] >= 0"
            ));
        });

        modelBuilder.Entity<Semilla>(entity =>
        {
            entity.HasKey(s => s.Token);
            entity.Property(s => s.Token).ValueGeneratedNever();

            entity.Property(s => s.SemillaFixture).IsRequired();
            entity.Property(s => s.SemillaCompletar).IsRequired();
            entity.Property(s => s.SemillaCrucesFase).IsRequired();
            entity.Property(s => s.SemillaSimulacion).IsRequired();
        });

        modelBuilder.Entity<Sorteo>(entity =>
        {
            entity.HasKey(s => s.Token);
            entity.Property(s => s.Token).ValueGeneratedNever();

            entity.HasOne<Semilla>(s => s.Semilla).WithMany().HasForeignKey("SemillaToken").IsRequired();
            entity.HasMany<Equipo>(s => s.Equipos).WithMany();
            entity.HasMany<Estadio>(s => s.Estadios).WithMany();
        });
        
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Correo);

            entity.Property(u => u.Nombre).IsRequired();
            entity.Property(u => u.Apellido).IsRequired();
            entity.Property(u => u.Contrasena).IsRequired();
            entity.Property(u => u.FechaNacimiento).IsRequired();
            entity.Property(u => u.Roles).HasConversion(
                roles => string.Join(',', roles),
                texto => texto.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => Enum.Parse<Rol>(r)).ToList()
            );
            
            entity.HasIndex(u => u.Correo).IsUnique();
        });
    }
}
