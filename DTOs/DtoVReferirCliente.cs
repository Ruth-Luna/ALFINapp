namespace ALFINapp.API.DTOs
{
    public class DtoVReferirCliente
    {
        public string dni_cliente { get; set; } = string.Empty;
        public string fuente_base { get; set; } = string.Empty;
        public string nombres_vendedor { get; set; } = string.Empty;
        public string apellidos_vendedor { get; set; } = string.Empty;
        public string nombres_clientes { get; set; } = string.Empty;
        public string dni_vendedor { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string agencia { get; set; } = string.Empty;
        public DateTime fecha_visita { get; set; } = DateTime.MinValue;
        public string celular { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public string cci { get; set; } = string.Empty;
        public string departamento { get; set; } = string.Empty;
        public string ubigeo { get; set; } = string.Empty;
        public string banco { get; set; } = string.Empty;
    }
}