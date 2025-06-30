using ALFINapp.DTOs;

namespace ALFINapp.API.DTOs
{
    public class DtoVRegisterEmail
    {
        public string? email_update_users { get; set; }
    }
    public class DtoVReagendar
    {
        public DateTime? FechaReagendamiento { get; set; }
        public int? IdDerivacion { get; set; }
        public List<DtoVUploadFiles>? evidencias { get; set; } = new List<DtoVUploadFiles>();
    }
}