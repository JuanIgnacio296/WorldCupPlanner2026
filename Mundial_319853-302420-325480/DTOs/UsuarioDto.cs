namespace DTOs;

public class UsuarioCreateDto
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Correo { get; set; } = "";
    public DateTime FechaNacimiento { get; set; }
    public string Contrasena { get; set; } = "";
    public List<string> Roles { get; set; } = [];
}

public class UsuarioUpdateDto
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Correo { get; set; } = "";
    public DateTime FechaNacimiento { get; set; }
    public List<string> Roles { get; set; } = [];
    public string? Contrasena { get; set; }
}

public class UsuarioReadDto
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Correo { get; set; } = "";
    public DateTime FechaNacimiento { get; set; }
    public List<string> Roles { get; set; } = [];

    public bool EsAdministrador => Roles.Contains("Administrador");
    public bool EsEditor => Roles.Contains("Editor");
    public bool EsPeriodista => Roles.Contains("Periodista");
}

public class LoginDto
{
    public string Correo { get; set; } = "";
    public string Contrasena { get; set; } = "";
}

public class CambiarPasswordDto
{
    public string Correo { get; set; } = "";
    public string NuevaContrasena { get; set; } = "";
    public string ConfirmacionNuevaContrasena { get; set; } = "";
}
