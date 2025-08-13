namespace ALFINapp.Datos.Persistence.Procedures
{
    public class SupervisoresGetDetallesAsignacionesPorAsesores
    {
        public int idUsuarioA { get; set; }
        public string nombreUsuarioA { get; set; } = string.Empty;
        public int totalClientesAsignados { get; set; }
        public int totalClientesGestionados { get; set; }
        public int totalClientesPendientes { get; set; }
        public string estaActivo { get; set; } = string.Empty;
    }
}