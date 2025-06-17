namespace ALFINapp.Infrastructure.Persistence.Procedures
{
    public class SupervisorGetAsignacionLeads
    {
        public int? IdAsignacion { get; set; }
        public int? IdCliente { get; set; }
        public int? idUsuarioV { get; set; }
        public DateTime? FechaAsignacionV { get; set; }
        //Propiedades de la tabla base_clientes
        public string? Dni { get; set; }
        public string? XAppaterno { get; set; }
        public string? XApmaterno { get; set; }
        public string? XNombre { get; set; }
        public string? DniVendedor { get; set; }
        public string? Destino { get; set; }
        //Propiedades de la tabla usuarios
        public string? NombresCompletos { get; set; }
        public string? UltimaTipificacion { get; set; }
        public string? TipificacionMasRelevante { get; set; }
        // Propiedades de la tabla de listas
        public int? IdLista { get; set; }
        public string? NombreLista { get; set; }
        public DateTime? FechaCreacionLista { get; set; }
    }
}