using ALFINapp.API.Models;

namespace ALFINapp.Datos.DAO.Asesores
{
    public class DAO_AsesorInicioView
    {
        private readonly MDbContext _context;
        private DA_Usuario _da_usuario = new DA_Usuario();
        public DAO_AsesorInicioView(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, ViewInicioVendedor? Data)> Execute(int idUsuario)
        {
            var usuario = _da_usuario.getUsuario(idUsuario);
            if (usuario == null)
            {
                return (false, "No se encontró el usuario", null);
            }
            var usuarioVendedor = new ViewUsuario(usuario);
            var vendedor = new ViewInicioVendedor();
            vendedor.Vendedor = usuarioVendedor;
            return (true, "Clientes encontrados", vendedor);
        }
    }
}