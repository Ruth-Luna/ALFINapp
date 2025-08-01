using ALFINapp.Infrastructure.Persistence.Procedures;

namespace ALFINapp.API.Models
{
    public class ViewCliente
    {
        public string dni { get; set; } = string.Empty;
        public string xappaterno { get; set; } = string.Empty;
        public string xapmaterno { get; set; } = string.Empty;
        public string xnombre { get; set; } = string.Empty;
        public int? edad { get; set; } = 0;

        public decimal ofertaMax { get; set; } = 0;
        public string campaña { get; set; } = string.Empty;
        public int idbase { get; set; } = 0;
        public int? idasignacion { get; set; } = 0;
        public DateTime fechaasignacion { get; set; } = DateTime.MinValue;
        public string comentario { get; set; } = string.Empty;
        public string tipificacion { get; set; } = string.Empty;
        public int? PesoTipificacionMayor { get; set; } = 0;
        public string tipificacionMasRelevante { get; set; } = string.Empty;
        public DateTime? fechaTipificacionDeMayorPeso { get; set; } = null;
        public int? idtipificacion { get; set; } = 0;
        public DateTime fechaultimatipificacion { get; set; } = DateTime.MinValue;
        public string prioridad { get; set; } = string.Empty;
        public string TraidoDe { get; set; } = string.Empty;

        // Columnas que estaban ausentes
        public int idcliente { get; set; } = 0;
        public DateTime fechacarga { get; set; } = DateTime.MinValue;

        // Columnas de para gestion de leads de Supervisores
        public int? idUsuarioV { get; set; } = 0;
        public DateTime? fechaAsignacionV { get; set; } = null;
        public string? dnivendedor { get; set; } = String.Empty;
        public string? destino { get; set; } = String.Empty;
        public string? nombresCompletos { get; set; } = String.Empty;
        public string? nombresCompletosV { get; set; } = String.Empty;
        public int? idLista { get; set; } = 0;
        public string? nombreLista { get; set; } = String.Empty;
        public DateTime? fechaCreacionLista { get; set; } = null;
        public ViewCliente() { }

        public ViewCliente(LeadsGetClientesAsignadosGestionLeads model)
        {
            dni = model.Dni ?? string.Empty;
            xappaterno = model.XAppaterno ?? string.Empty;
            xapmaterno = model.XApmaterno ?? string.Empty;
            xnombre = model.XNombre ?? string.Empty;
            ofertaMax = model.OfertaMax ?? 0;
            campaña = model.Campaña ?? string.Empty;
            idbase = model.IdBase;
            idasignacion = model.IdAsignacion ?? 0;
            fechaasignacion = model.FechaAsignacionVendedor ?? DateTime.MinValue;
            comentario = model.ComentarioGeneral ?? string.Empty;
            tipificacion = model.TipificacionDeMayorPeso ?? string.Empty;
            PesoTipificacionMayor = model.PesoTipificacionMayor ?? 0;
            fechaTipificacionDeMayorPeso = model.FechaTipificacionDeMayorPeso ?? DateTime.MinValue;
            tipificacionMasRelevante = model.TipificacionDeMayorPeso ?? string.Empty;
            fechaultimatipificacion = model.FechaTipificacionDeMayorPeso ?? DateTime.MinValue;
            prioridad = model.PrioridadSistema ?? string.Empty;
            TraidoDe = model.TraidoDe ?? string.Empty;
            fechaAsignacionV = model.FechaAsignacionVendedor ?? DateTime.MinValue;
        }

        public ViewCliente(SupervisorGetAsignacionLeads model)
        {
            dni = model.Dni ?? string.Empty;
            xappaterno = model.XAppaterno ?? string.Empty;
            xapmaterno = model.XApmaterno ?? string.Empty;
            xnombre = model.XNombre ?? string.Empty;
            idbase = model.IdCliente ?? 0;
            idasignacion = model.IdAsignacion ?? 0;
            idUsuarioV = model.idUsuarioV ?? 0;
            fechaAsignacionV = model.FechaAsignacionV ?? DateTime.MinValue;
            dnivendedor = model.DniVendedor ?? string.Empty;
            destino = model.Destino ?? string.Empty;
            nombresCompletosV = model.NombresCompletos ?? string.Empty;
            idLista = model.IdLista ?? 0;
            nombreLista = model.NombreLista ?? string.Empty;
            fechaCreacionLista = model.FechaCreacionLista ?? DateTime.MinValue;

            // Las demás propiedades se dejan con sus valores por defecto
            dnivendedor = model.DniVendedor ?? string.Empty;
            destino = model.Destino ?? string.Empty;
            nombresCompletos = xappaterno + " " + xapmaterno + " " + xnombre;
            tipificacion = model.UltimaTipificacion ?? string.Empty;
            tipificacionMasRelevante = model.TipificacionMasRelevante ?? string.Empty;

        }
    }
}
