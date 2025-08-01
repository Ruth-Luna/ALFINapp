using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.API.Models
{
    public class ViewUsuario
    {
        public int IdUsuario { get; set; } = 0;
        public string Dni { get; set; } = string.Empty;
        public string NombresCompletos { get; set; } = string.Empty;
      
        public string? Apellido_Paterno { get; set; } = string.Empty;
        public string? Apellido_Materno { get; set; } = string.Empty;
        public string? Nombres { get; set; } = string.Empty;
      
        public string? Usuario { get; set; }
        public string? Contrasenia { get; set; }
              
        public string Rol { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.MinValue;
        public string Estado { get; set; } = string.Empty;
        public int IDUSUARIOSUP { get; set; } = 0;
        public string RESPONSABLESUP { get; set; } = string.Empty;
        public string REGION { get; set; } = string.Empty;
        public string contraseña { get; set; } = string.Empty;
        public string NOMBRECAMPAÑA { get; set; } = string.Empty;
        public int IdRol { get; set; } = 0;
        public DateTime FechaActualizacion { get; set; } = DateTime.MinValue;
        public string TipoDocumento { get; set; } = string.Empty;
        public int IdUsuarioAccion { get; set; } = 0;
        public DateTime FechaInicio { get; set; } = DateTime.MinValue;
        public DateTime FechaCese { get; set; } = DateTime.MinValue;
        public string Correo { get; set; } = string.Empty;
        public string Cci { get; set; } = string.Empty;

        public int Resultado { get; set; }
        public string Mensaje { get; set; }
      
        public ViewUsuario() { }

        public ViewUsuario(Usuario model)
        {
            IdUsuario = model.IdUsuario;
            Dni = model.Dni ?? string.Empty;
            NombresCompletos = model.NombresCompletos ?? string.Empty;
            Rol = model.Rol ?? string.Empty;
            Departamento = model.Departamento ?? string.Empty;
            Provincia = model.Provincia ?? string.Empty;
            Distrito = model.Distrito ?? string.Empty;
            Telefono = model.Telefono ?? string.Empty;
            FechaRegistro = model.FechaRegistro ?? DateTime.MinValue;
            Estado = model.Estado ?? string.Empty;
            IDUSUARIOSUP = model.IDUSUARIOSUP ?? 0;
            RESPONSABLESUP = model.RESPONSABLESUP ?? string.Empty;
            REGION = model.REGION ?? string.Empty;
            contraseña = model.contraseña ?? string.Empty;
            NOMBRECAMPAÑA = model.NOMBRECAMPAÑA ?? string.Empty;
            IdRol = model.IdRol ?? 0;
            FechaActualizacion = model.FechaActualizacion ?? DateTime.MinValue;
            TipoDocumento = model.TipoDocumento ?? string.Empty;
            IdUsuarioAccion = model.IdUsuarioAccion ?? 0;
            FechaInicio = model.FechaInicio ?? DateTime.MinValue;
            FechaCese = model.FechaCese ?? DateTime.MinValue;
            Correo = model.Correo ?? string.Empty;

            // Cci no está en Usuario, se deja con su valor por defecto
        }
    }
    public class ViewRol
    {
        public int IdRol { get; set; } 
        public string? Rol { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ViewCorreoRecuperacion
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Codigo { get; set; }
        public bool? Estado { get; set; }
        public int IdSolicitud { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}