using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Domain.Entities;
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
            NOMBRECAMPAÑA = model.NOMBRECAMPANIA;
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
                Dni = Dni ?? string.Empty,
                NombresCompletos = NombresCompletos ?? string.Empty,
                Rol = Rol ?? string.Empty,
                Departamento = Departamento ?? string.Empty,
                Provincia = Provincia ?? string.Empty,
                Distrito = Distrito ?? string.Empty,
                Telefono = Telefono ?? string.Empty,
                FechaRegistro = FechaRegistro == null ? DateTime.Now : FechaRegistro.Value,
                Estado = Estado ?? string.Empty,
                IDUSUARIOSUP = IDUSUARIOSUP == null ? 0 : IDUSUARIOSUP.Value,
                RESPONSABLESUP = RESPONSABLESUP ?? string.Empty,
                REGION = REGION ?? string.Empty,
                NOMBRECAMPAÑA = NOMBRECAMPAÑA ?? string.Empty,
                IdRol = IdRol == null ? 0 : IdRol.Value,
                FechaActualizacion = FechaActualizacion == null ? DateTime.Now : FechaActualizacion.Value,
                TipoDocumento = TipoDocumento ?? string.Empty,
                IdUsuarioAccion = IdUsuarioAccion == null ? 0 : IdUsuarioAccion.Value,
                FechaInicio = FechaInicio == null ? DateTime.Now : FechaInicio.Value,
                FechaCese = FechaCese == null ? DateTime.Now : FechaCese.Value,
                Correo = Correo ?? string.Empty
            };
        }
        public Vendedor ToEntityVendedor ()
        {
            return new Vendedor
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
        public Supervisor ToEntitySupervisor ()
        {
            return new Supervisor
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