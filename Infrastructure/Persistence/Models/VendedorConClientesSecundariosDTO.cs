namespace ALFINapp.Infrastructure.Persistence.Models;
public class VendedorConClientesSecundariosDTO
{
    public int? IdUsuarioVendedor { get; set; }
    public string? NombresCompletosVendedor { get; set;}
    public int? NumeroClientesVendedor { get; set; }
    public int? NumeroClientesReasignadosVendedor { get; set; }
    public List<ClientesAsignadosSecundarioDTO> clientesAsignados { get; set; }
    public VendedorConClientesSecundariosDTO()
    {
        clientesAsignados = new List<ClientesAsignadosSecundarioDTO>(); // Inicializa la lista
    }
}

public class ClientesAsignadosSecundarioDTO
{
    public string? NombresCompletos { get; set; }
    public int? idCliente { get; set; }
    public bool TieneAsesorSecundarioAsignado { get; set; } = false;
    public int? NumAsesoresSecundarios { get; set;}
}