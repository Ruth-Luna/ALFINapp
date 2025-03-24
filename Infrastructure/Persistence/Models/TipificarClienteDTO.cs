namespace ALFINapp.Infrastructure.Persistence.Models;

public class TipificarClienteDTO
{
    public string? Telefono { get; set;}
    public int TipificacionId { get; set;}
    public DateTime? FechaVisita { get; set;}
    public string? AgenciaAsignada { get; set;}
}
