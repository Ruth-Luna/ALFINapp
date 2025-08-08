namespace ALFINapp.Datos.DAO.Derivaciones
{
    public class DAO_DerivacionesVista
    {
        private readonly MDbContext _context;
        public DAO_DerivacionesVista(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool success, string message)> getDerivacionesVista(
            int idUsuario,
            int idRol)
        {
            try
            {
                return (false, "completed");
            }
            catch (System.Exception)
            {
                return (false, "error");
            }
        }
    }
}