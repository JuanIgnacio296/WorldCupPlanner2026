using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

internal static class EfExistentesExtensions
{
    public static void MarcarExistente(this AppDbContext ctx, object? entidad)
    {
        if (entidad is null)
        {
            return;
        }

        var entry = ctx.Entry(entidad);
        if (entry.State == EntityState.Added)
        {
            entry.State = EntityState.Unchanged;
        }
    }

    public static void MarcarExistentes(this AppDbContext ctx, IEnumerable? entidades)
    {
        if (entidades is null)
        {
            return;
        }

        foreach (var entidad in entidades)
        {
            ctx.MarcarExistente(entidad);
        }
    }

    public static void MarcarTipoExistente<T>(this AppDbContext ctx) where T : class
    {
        foreach (var entry in ctx.ChangeTracker.Entries<T>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.State = EntityState.Unchanged;
            }
        }
    }

    public static void DesadjuntarDuplicados<T>(this AppDbContext ctx, T? entidad) where T : class
    {
        if (entidad is null)
        {
            return;
        }

        var clave = ctx.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
        if (clave is null)
        {
            return;
        }

        var propiedadesClave = clave.Properties
            .Select(p => typeof(T).GetProperty(p.Name))
            .Where(p => p is not null)
            .ToList();

        if (propiedadesClave.Count == 0)
        {
            return;
        }

        foreach (var entry in ctx.ChangeTracker.Entries<T>().ToList())
        {
            if (ReferenceEquals(entry.Entity, entidad))
            {
                continue;
            }

            bool mismaClave = propiedadesClave.All(p => Equals(p!.GetValue(entry.Entity), p!.GetValue(entidad)));
            if (mismaClave)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}