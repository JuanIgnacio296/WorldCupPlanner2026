using App_UI.Components;
using Services;
using Services.Interfaces;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IinMemoryLog, LogEFRepository>();
builder.Services.AddScoped<IinMemoryUsuario, UsuarioEFRepository>();
builder.Services.AddScoped<IinMemoryEquipo, EquipoEFRepository>();
builder.Services.AddScoped<IinMemoryEstadio, EstadioEFRepository>();
builder.Services.AddScoped<IinMemoryPartido, PartidoEFRepository>();
builder.Services.AddScoped<IinMemorySorteo, SorteoEFRepository>();
builder.Services.AddScoped<IinMemoryFixture, FixtureEFRepository>();
builder.Services.AddScoped<IinMemorySemilla, SemillaEFRepository>();

builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IEstadioService, EstadioService>();
builder.Services.AddScoped<IPartidoService, PartidoService>();
builder.Services.AddScoped<ISesionService, SesionService>();
builder.Services.AddScoped<ISorteoService, SorteoService>();
builder.Services.AddScoped<IFixtureService, FixtureService>();
builder.Services.AddScoped<IImportacionService, ImportacionService>();
builder.Services.AddScoped<ISemillaService, SemillaService>();
builder.Services.AddScoped<IExportacionService, ExportacionService>();

builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App_UI.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
