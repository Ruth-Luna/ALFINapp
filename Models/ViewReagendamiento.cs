using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.API.Models
{
    public class ViewClienteReagendado
    {
        public int IdDerivacion { get; set; } = 0;
        public string? Dni { get; set; } = String.Empty;
        public string? NombresCompletos { get; set; } = String.Empty;
        public decimal? OfertaMax { get; set; } = 0;
        public string? AgenciaAsignada { get; set; } = String.Empty;
        public string? Telefono { get; set; } = String.Empty;
        public DateTime? FechaVisitaPrevia { get; set; } = DateTime.MinValue;
        public ViewClienteReagendado()
        {
        }
        public ViewClienteReagendado(DerivacionesAsesores model)
        {
            IdDerivacion = model.IdDerivacion;
            Dni = model.DniCliente;
            NombresCompletos = model.NombreCliente;
            OfertaMax = model.OfertaMax < 1000 ? model.OfertaMax * 100 : model.OfertaMax;
            AgenciaAsignada = model.NumAgencia;
            Telefono = model.TelefonoCliente;
            FechaVisitaPrevia = model.FechaVisita;
        }
    }
}