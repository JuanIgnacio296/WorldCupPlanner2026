namespace App.Dominio.Tests;

public class NotificacionTests
{
    [TestClass]
    public class NotificacionTest
    {
        [TestMethod]
        public void Notificacion_CreacionValida_DebeSetearPropiedadesCorrectamente()
        {
            string usuarioCorreo = "usuario@mail.com";
            string mensaje = "Resultado partido: Uruguay 2 - 1 Brasil";

            Notificacion notificacion = new Notificacion(usuarioCorreo, mensaje);

            Assert.AreEqual(usuarioCorreo, notificacion.UsuarioCorreo);
            Assert.AreEqual(mensaje, notificacion.Mensaje);
            Assert.IsFalse(notificacion.Leida);
        }
    }
}