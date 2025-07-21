using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.API.Models
{
    public class ViewUsuario
    {
        public int IdUsuario { get; set; }
        public string? Dni { get; set; } = null!;
        public string? NombresCompletos { get; set; } = null!;
        public string? Rol { get; set; } = null!;
        public string? Departamento { get; set; } = null!;
        public string? Provincia { get; set; } = null!;
        public string? Distrito { get; set; } = null!;
        public string? Telefono { get; set; } = null!;
        public string? NOMBRECAMPANIA { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? Estado { get; set; }
        public int? IDUSUARIOSUP { get; set; }
        public string? RESPONSABLESUP { get; set; }
        public string? REGION { get; set; }
        public string? contraseña { get; set; }
        public string? NOMBRECAMPAÑA { get; set; }
        public int? IdRol { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string? TipoDocumento { get; set; }
        public int? IdUsuarioAccion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaCese { get; set; }
        public string? Correo { get; set; }
    }
}