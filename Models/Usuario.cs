using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ALFINapp.Models;

public partial class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }
    [Column("dni")]

    public string? Dni { get; set; } = null!;
    [Column("Nombres_Completos")]
    public string? NombresCompletos { get; set; } = null!;
    [Column("rol")]
    public string? Rol { get; set; } = null!;
    [Column("departamento")]
    public string? Departamento { get; set; } = null!;
    [Column("provincia")]
    public string? Provincia { get; set; } = null!;
    [Column("distrito")]
    public string? Distrito { get; set; } = null!;
    [Column("telefono")]
    public string? Telefono { get; set; } = null!;
    [Column("fecha_registro")]
    public DateTime? FechaRegistro { get; set; }
    [Column("estado")]

    public string? Estado { get; set; }
    /*[Column("Apellido_Paterno")]

    public string? ApellidoPaterno { get; set; }
    [Column("Apellido_Materno")]

    public string? ApellidoMaterno { get; set; }
    [Column("Nombres")]
    public string? Nombres { get; set; }*/
    [Column("ID_USUARIO_SUP")]
    public int? IDUSUARIOSUP { get; set; }
    [Column("RESPONSABLE_SUP")]
    public string? RESPONSABLESUP { get; set; }
    [Column("REGION")]
    public string? REGION { get; set; }
    [Column("contraseña")]
    public string? contraseña { get; set; }
    [Column("NOMBRE_CAMPAÑA")]
    public string? NOMBRECAMPAÑA { get; set; }
    [Column("id_rol")]
    public int? IdRol { get; set; }
}
