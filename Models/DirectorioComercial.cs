using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("directorio_comercial")]
    public class DirectorioComercial
    {
        [Key]
        [Column("id_directorio_comercial")]
        public int IdDirectorioComercial { get; set; }

        [Column("ITEM")]
        public string? Item { get; set; }

        [Column("CECO")]
        public string? Ceco { get; set; }

        [Column("AGENCIA")]
        public string? Agencia { get; set; }

        [Column("DISTRITO")]
        public string? Distrito { get; set; }

        [Column("PROVINCIA")]
        public string? Provincia { get; set; }

        [Column("DEPARTAMENTO")]
        public string? Departamento { get; set; }

        [Column("REGION")]
        public string? Region { get; set; }

        [Column("GERENTE REGIONAL")]
        public string? GerenteRegional { get; set; }

        [Column("GERENTE DE AGENCIA/JEFE DE SUCURSAL TITULAR")]
        public string? GerenteDeAgenciaJefeDeSucursalTitular { get; set; }

        [Column("GERENTE DE AGENCIA/JEFE DE SUCURSAL/ASESOR ENCARGADO FEBRERO")]
        public string? GerenteDeAgenciaJefeDeSucursalAsesorEncargadoFebrero { get; set; }

        [Column("COMENTARIOS")]
        public string? Comentarios { get; set; }

        [Column("DIRECCION")]
        public string? Direccion { get; set; }

        [Column("CELULAR BANCO")]
        public string? CelularBanco { get; set; }

        [Column("CELULAR PROPIO")]
        public string? CelularPropio { get; set; }

        [Column("CORREO AGENCIA")]
        public string? CorreoAgencia { get; set; }

        [Column("CORREO GERENTE")]
        public string? CorreoGerente { get; set; }
    }
}