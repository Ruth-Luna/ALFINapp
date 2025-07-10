using System.ComponentModel.DataAnnotations.Schema;
namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class SupervisorGetNumberOfLeads
    {
        [Column("total_clientes")]
        public int TotalClientes { get; set; }
        [Column("total_clientes_asignados")]
        public int TotalClientesAsignados { get; set; }
        [Column("total_clientes_pendientes")]
        public int TotalClientesPendientes { get; set; }
    }

    public class GestionConseguirTodasLasAsignacionesPorListas
    {
        public int id_lista { get; set; }
        public int id_usuario_supervisor { get; set; }
        public string? nombre_lista { get; set; }
        public string? dni_supervisor { get; set; }
        public string? nombre_supervisor { get; set; }
        public DateTime? fecha_creacion_lista { get; set; }
        public int total_asignaciones { get; set; }
        public int asignaciones_gestionadas { get; set; }
        public int asignadas_a_asesores { get; set; }
    }
    public class GestionConseguirODescargarAsignacionDeLeadsDeSup
    {
        [Column("dni")]
        public string? Dni { get; set; }
        [Column("x_appaterno")]
        public string? XAppaterno { get; set; }
        [Column("x_apmaterno")]
        public string? XApmaterno { get; set; }
        [Column("x_nombre")]
        public string? XNombre { get; set; }
        [Column("edad")]
        public int? Edad { get; set; }
        [Column("departamento")]
        public string? Departamento { get; set; }
        [Column("provincia")]
        public string? Provincia { get; set; }
        [Column("distrito")]
        public string? Distrito { get; set; }

        [Column("campaña")]
        public string? Campaña { get; set; }

        [Column("oferta_max")]
        public decimal? OfertaMax { get; set; }

        [Column("tasa_minima")]
        public decimal? TasaMinima { get; set; }
        [Column("sucursal_comercial")]
        public string? SucursalComercial { get; set; }

        [Column("agencia_comercial")]
        public string? AgenciaComercial { get; set; }

        [Column("plazo")]
        public int? Plazo { get; set; }

        [Column("cuota")]
        public decimal? Cuota { get; set; }

        [Column("oferta_12M")]
        public decimal? Oferta12m { get; set; }

        [Column("tasa_12M")]
        public decimal? Tasa12m { get; set; }

        [Column("cuota_12M")]
        public decimal? Cuota12m { get; set; }

        [Column("oferta_18M")]
        public decimal? Oferta18m { get; set; }

        [Column("tasa_18M")]
        public decimal? Tasa18m { get; set; }

        [Column("cuota_18M")]
        public decimal? Cuota18m { get; set; }

        [Column("oferta_24M")]
        public decimal? Oferta24m { get; set; }

        [Column("tasa_24M")]
        public decimal? Tasa24m { get; set; }

        [Column("cuota_24M")]
        public decimal? Cuota24m { get; set; }

        [Column("oferta_36M")]
        public decimal? Oferta36m { get; set; }

        [Column("tasa_36M")]
        public decimal? Tasa36m { get; set; }

        [Column("cuota_36M")]
        public decimal? Cuota36m { get; set; }

        [Column("grupo_tasa")]
        public string? GrupoTasa { get; set; }

        [Column("grupo_monto")]
        public string? GrupoMonto { get; set; }

        [Column("propension")]
        public int? Propension { get; set; }

        [Column("tipo_cliente")]
        public string? TipoCliente { get; set; }

        [Column("cliente_nuevo")]
        public string? ClienteNuevo { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("color_final")]
        public string? ColorFinal { get; set; }

        [Column("usuario")]
        public string? Usuario { get; set; }

        [Column("USER_V3")]
        public string? UserV3 { get; set; }

        [Column("flag_deuda_v_oferta")]
        public int? FlagDeudaVOferta { get; set; }

        [Column("PERFIL_RO")]
        public string? PerfilRo { get; set; }
        [Column("prioridad")]
        public string? Prioridad { get; set; }
        [Column("telefono_1")]
        public string? Telefono1 { get; set; }
        [Column("telefono_2")]

        public string? Telefono2 { get; set; }
        [Column("telefono_3")]

        public string? Telefono3 { get; set; }
        [Column("telefono_4")]

        public string? Telefono4 { get; set; }
        [Column("telefono_5")]

        public string? Telefono5 { get; set; }
        [Column("email_1")]

        public string? Email1 { get; set; }
        [Column("email_2")]

        public string? Email2 { get; set; }
        [Column("nombre_completo")]
        public string? NombreCompleto { get; set; }

        [Column("tipo_base")]
        public string? TipoBase { get; set; }

        [Column("id_asignacion")]
        public int IdAsignacion { get; set; }

        [Column("nombre_lista")]
        public string? NombreLista { get; set; }

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        [Column("fecha_asignacion_sup")]
        public DateTime? FechaAsignacionSup { get; set; }

        [Column("id_usuarioV")]
        public int? IdUsuarioV { get; set; }

        [Column("ultima_tipificacion_general")]
        public string? UltimaTipificacionGeneral { get; set; }
    }
    public class DerivacionConseguirODescargarAsignacionConDerivacionesDeSup
    {
        [Column("dni")]
        public string? Dni { get; set; }
        [Column("x_appaterno")]
        public string? XAppaterno { get; set; }
        [Column("x_apmaterno")]
        public string? XApmaterno { get; set; }
        [Column("x_nombre")]
        public string? XNombre { get; set; }
        [Column("edad")]
        public int? Edad { get; set; }
        [Column("departamento")]
        public string? Departamento { get; set; }
        [Column("provincia")]
        public string? Provincia { get; set; }
        [Column("distrito")]
        public string? Distrito { get; set; }

        [Column("campaña")]
        public string? Campaña { get; set; }

        [Column("oferta_max")]
        public decimal? OfertaMax { get; set; }

        [Column("tasa_minima")]
        public decimal? TasaMinima { get; set; }
        [Column("sucursal_comercial")]
        public string? SucursalComercial { get; set; }

        [Column("agencia_comercial")]
        public string? AgenciaComercial { get; set; }

        [Column("plazo")]
        public int? Plazo { get; set; }

        [Column("cuota")]
        public decimal? Cuota { get; set; }

        [Column("oferta_12M")]
        public decimal? Oferta12m { get; set; }

        [Column("tasa_12M")]
        public decimal? Tasa12m { get; set; }

        [Column("cuota_12M")]
        public decimal? Cuota12m { get; set; }

        [Column("oferta_18M")]
        public decimal? Oferta18m { get; set; }

        [Column("tasa_18M")]
        public decimal? Tasa18m { get; set; }

        [Column("cuota_18M")]
        public decimal? Cuota18m { get; set; }

        [Column("oferta_24M")]
        public decimal? Oferta24m { get; set; }

        [Column("tasa_24M")]
        public decimal? Tasa24m { get; set; }

        [Column("cuota_24M")]
        public decimal? Cuota24m { get; set; }

        [Column("oferta_36M")]
        public decimal? Oferta36m { get; set; }

        [Column("tasa_36M")]
        public decimal? Tasa36m { get; set; }

        [Column("cuota_36M")]
        public decimal? Cuota36m { get; set; }

        [Column("grupo_tasa")]
        public string? GrupoTasa { get; set; }

        [Column("grupo_monto")]
        public string? GrupoMonto { get; set; }

        [Column("propension")]
        public int? Propension { get; set; }

        [Column("tipo_cliente")]
        public string? TipoCliente { get; set; }

        [Column("cliente_nuevo")]
        public string? ClienteNuevo { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("color_final")]
        public string? ColorFinal { get; set; }

        [Column("usuario")]
        public string? Usuario { get; set; }

        [Column("USER_V3")]
        public string? UserV3 { get; set; }

        [Column("flag_deuda_v_oferta")]
        public int? FlagDeudaVOferta { get; set; }

        [Column("PERFIL_RO")]
        public string? PerfilRo { get; set; }
        [Column("prioridad")]
        public string? Prioridad { get; set; }
        [Column("telefono_1")]
        public string? Telefono1 { get; set; }
        [Column("telefono_2")]

        public string? Telefono2 { get; set; }
        [Column("telefono_3")]

        public string? Telefono3 { get; set; }
        [Column("telefono_4")]

        public string? Telefono4 { get; set; }
        [Column("telefono_5")]

        public string? Telefono5 { get; set; }
        [Column("email_1")]

        public string? Email1 { get; set; }
        [Column("email_2")]

        public string? Email2 { get; set; }
        [Column("nombre_completo")]
        public string? NombreCompleto { get; set; }

        [Column("tipo_base")]
        public string? TipoBase { get; set; }

        [Column("id_asignacion")]
        public int IdAsignacion { get; set; }

        [Column("nombre_lista")]
        public string? NombreLista { get; set; }

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        [Column("fecha_asignacion_sup")]
        public DateTime? FechaAsignacionSup { get; set; }

        [Column("id_usuarioV")]
        public int? IdUsuarioV { get; set; }

        [Column("ultima_tipificacion_general")]
        public string? UltimaTipificacionGeneral { get; set; }
        [Column("telefono_derivado")]
        public string? TelefonoDerivado { get; set; }
        [Column("fecha_derivacion")]
        public DateTime? FechaDerivacion { get; set; }
        [Column("nombre_agencia_derivada")]
        public string? NombreAgenciaDerivada { get; set; }
        [Column("fecha_visita")]
        public DateTime? FechaVisita { get; set; }
        [Column("oferta_max_derivada")]
        public Decimal? OfertaMaxDerivada { get; set; }
    }
}