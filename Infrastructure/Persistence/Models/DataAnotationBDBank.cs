using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFINapp.Infrastructure.Persistence.Models;

public partial class CampanaGrupo
{
    [Key]
    [Column("id_campana_grupo")]
    public int IdCampanaGrupo { get; set; }

    [Column("nombre_campana")]
    public string? NombreCampana { get; set; }
}

public partial class Color
{
    [Key]
    [Column("id_color")]
    public int IdColor { get; set; }

    [Column("nombre_color")]
    public string? NombreColor { get; set; }
}

public partial class Plazo
{
    [Key]
    [Column("id_plazo")]
    public int IdPlazo { get; set; }

    [Column("num_meses")]
    public int? NumMeses { get; set; }
}

public partial class RangoDeuda
{
    [Key]
    [Column("id_rango_deuda")]
    public int IdRangoDeuda { get; set; }

    [Column("rango_deuda")]
    public string? RangoDeDeuda { get; set; }
}

public partial class UsuarioBanco
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("nombre_usuario")]
    public string? NombreUsuario { get; set; }

    [Column("descripcion_usuario")]
    public string? DescripcionUsuario { get; set; }

    [Column("tipo_usuario")]
    public string? TipoUsuario { get; set; }
}

public partial class BancoUserV3
{
    [Key]
    [Column("id_user_v3")]
    public int IdUserV3 { get; set; }

    [Column("name_user")]
    public string? NameUser { get; set; }

    [Column("user_agrupado")]
    public string? UserAgrupado { get; set; }
}