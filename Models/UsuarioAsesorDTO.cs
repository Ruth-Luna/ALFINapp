using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ALFINapp.Models;

public partial class UsuarioAsesorDTO
{
    public int IdUsuario { get; set; }
    public string? Dni { get; set; }

    public string? NombresCompletos { get; set; } = null!;
    public string? Rol { get; set; } = null!;
    public string? Departamento { get; set; } = null!;

    public string? Provincia { get; set; } = null!;

    public string? Distrito { get; set; } = null!;
    public string? Region { get; set; } = null!;

    public string? Telefono { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }


    public string? Estado { get; set; }


    public string? ApellidoPaterno { get; set; }


    public string? ApellidoMaterno { get; set; }

    public string? Nombres { get; set; }

    public int? IDUSUARIOSUP { get; set; }

    public string? RESPONSABLESUP { get; set; }

    public int? TotalClientesAsignados { get; set; }
    public int? ClientesTrabajando { get; set; }
    public int? ClientesSinTrabajar { get; set; }
}
