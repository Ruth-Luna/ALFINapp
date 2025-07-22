using ALFINapp.API.Models;

namespace ALFINapp.Models
{
    public class ViewClienteParaTipificacion
    {
        public ViewCliente cliente { get; set; } = new ViewCliente();
        public List<string> agenciascomerciales { get; set; } = new List<string>();
        //Propiedades de la Tabla ClientsEnriquecido
        public List<ViewTelefono> telefonos { get; set; } = new List<ViewTelefono>();
    }

    public class ViewTelefono
    {
        public string? Numero { get; set; }
        public string? Tipo { get; set; }
        public bool? EsPrincipal { get; set; }
        public string? Comentario { get; set; }
        public string? DescripcionTipificacion { get; set; }
        public DateTime? FechaTipificacion { get; set; }
    }
}