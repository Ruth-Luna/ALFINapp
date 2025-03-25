using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.DTOs
{
    public class DetallesUsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string? Dni { get; set; } = null!;
        public string? NombresCompletos { get; set; } = null!;
        public string? Rol { get; set; } = null!;
        public string? Departamento { get; set; } = null!;
        public string? Provincia { get; set; } = null!;
        public string? Distrito { get; set; } = null!;
        public string? Telefono { get; set; } = null!;
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
        public DetallesUsuarioDTO (Usuario model)
        {
            IdUsuario = model.IdUsuario;
            Dni = model.Dni;
            NombresCompletos = model.NombresCompletos;
            Rol = model.Rol;
            Departamento = model.Departamento;
            Provincia = model.Provincia;
            Distrito = model.Distrito;
            Telefono = model.Telefono;
            FechaRegistro = model.FechaRegistro;
            Estado = model.Estado;
            IDUSUARIOSUP = model.IDUSUARIOSUP;
            RESPONSABLESUP = model.RESPONSABLESUP;
            REGION = model.REGION;
            contraseña = model.contraseña;
            NOMBRECAMPAÑA = model.NOMBRECAMPAÑA;
            IdRol = model.IdRol;
            FechaActualizacion = model.FechaActualizacion;
            TipoDocumento = model.TipoDocumento;
            IdUsuarioAccion = model.IdUsuarioAccion;
            FechaInicio = model.FechaInicio;
            FechaCese = model.FechaCese;
            Correo = model.Correo;
        }
        public ViewUsuario ToView ()
        {
            return new ViewUsuario
            {
                IdUsuario = IdUsuario,
                Dni = Dni,
                NombresCompletos = NombresCompletos,
                Rol = Rol,
                Departamento = Departamento,
                Provincia = Provincia,
                Distrito = Distrito,
                Telefono = Telefono,
                FechaRegistro = FechaRegistro,
                Estado = Estado,
                IDUSUARIOSUP = IDUSUARIOSUP,
                RESPONSABLESUP = RESPONSABLESUP,
                REGION = REGION,
                contraseña = contraseña,
                NOMBRECAMPAÑA = NOMBRECAMPAÑA,
                IdRol = IdRol,
                FechaActualizacion = FechaActualizacion,
                TipoDocumento = TipoDocumento,
                IdUsuarioAccion = IdUsuarioAccion,
                FechaInicio = FechaInicio,
                FechaCese = FechaCese,
                Correo = Correo
            };
        }
    }
}