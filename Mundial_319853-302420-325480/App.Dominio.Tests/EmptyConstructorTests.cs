using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Dominio;

namespace App.Dominio.Tests;

[TestClass]
public class EmptyConstructorTests
{
    [TestMethod] public void Equipo_ConstructorVacio() => Activator.CreateInstance(typeof(Equipo), true);
    [TestMethod] public void Estadio_ConstructorVacio() => Activator.CreateInstance(typeof(Estadio), true);
    [TestMethod] public void Fixture_ConstructorVacio() => Activator.CreateInstance(typeof(Fixture), true);
    [TestMethod] public void Grupo_ConstructorVacio() => Activator.CreateInstance(typeof(Grupo), true);
    [TestMethod] public void Log_ConstructorVacio() => Activator.CreateInstance(typeof(Log), true);
    [TestMethod] public void Notificacion_ConstructorVacio() => Activator.CreateInstance(typeof(Notificacion), true);
    [TestMethod] public void Partido_ConstructorVacio() => Activator.CreateInstance(typeof(Partido), true);
    [TestMethod] public void Semilla_ConstructorVacio() => Activator.CreateInstance(typeof(Semilla), true);
    [TestMethod] public void Sorteo_ConstructorVacio() => Activator.CreateInstance(typeof(Sorteo), true);
    [TestMethod] public void Usuario_ConstructorVacio() => Activator.CreateInstance(typeof(Usuario), true);
    [TestMethod] public void FechaOcupada_ConstructorVacio() => Activator.CreateInstance(typeof(FechaOcupada), true);
}
