namespace ALFINapp.Datos.DAO
{
    public class Conexion
    {

        private string cadenaSQL = string.Empty;

        public Conexion()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            cadenaSQL = builder.GetSection("ConnectionStrings:DefaultConnection").Value ?? string.Empty;
        }

        public string getCadenaSQL()
        {
            return cadenaSQL;
        }
    }
}
