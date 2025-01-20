namespace ALFINapp.Models;

public class DetalleTipificarClienteDTO
{

    // Propiedades de la tabla BaseCliente
    public string? Dni { get; set; }
    public string? XAppaterno { get; set; }
    public string? XApmaterno { get; set; }
    public string? XNombre { get; set; }
    public int? Edad { get; set; }
    public string? Departamento { get; set; }
    public string? Provincia { get; set; }
    public string? Distrito { get; set; }
    public int? IdBase { get; set; }

    //Propiedades de la tabla DetalleBase
    public string? Campaña { get; set; }
    public string? Sucursal { get; set; }
    public string? AgenciaComercial { get; set; }
    public int? Plazo { get; set; }
    public decimal? Cuota { get; set; }
    public string? GrupoTasa { get; set; }
    public string? GrupoMonto { get; set; }
    public int? Propension { get; set; }
    public string? TipoCliente { get; set; }
    public string? ClienteNuevo { get; set; }
    public string? Color { get; set; }
    public string? ColorFinal { get; set; }
    public string? Usuario { get; set; }
    public string? SegmentoUser { get; set; }

    //Propiedades de la Tabla ClientsEnriquecido
    public List<TelefonoDTO> Telefonos { get; set; }
    
    //Propiedas de Tasa
    public decimal? OfertaMax { get; set; }
    public decimal? TasaMinima { get; set; }
    public decimal? Oferta12m { get; set; }
    public decimal? Tasa12m { get; set; }
    public decimal? Cuota12m { get; set; }
    public decimal? Oferta18m { get; set; }
    public decimal? Tasa18m { get; set; }
    public decimal? Cuota18m { get; set; }
    public decimal? Oferta24m { get; set; }
    public decimal? Tasa24m { get; set; }
    public decimal? Cuota24m { get; set; }
    public decimal? Oferta36m { get; set; }
    public decimal? Tasa36m { get; set; }
    public decimal? Cuota36m { get; set; }

    public DetalleTipificarClienteDTO()
    {
        Telefonos = new List<TelefonoDTO>(); // Inicializa la lista
    }
}

public class TelefonoDTO
{
    public string? Numero { get; set; }
    public string? Comentario { get; set; }
    public string? DescripcionTipificacion { get; set; }
    public DateTime? FechaTipificacion { get; set; }
}