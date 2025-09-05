namespace ALFINapp.Datos.DAO
{
    public class Conexion
    {

        private string cadenaSQL = string.Empty;

        public Conexion()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            cadenaSQL = builder.GetSection("ConnectionStrings:DefaultConnection").Value ?? throw new Exception("Connection string 'DefaultConnection' not found in appsettings.Development.json.");
        }

        public string getCadenaSQL()
        {
            return cadenaSQL;
        }
    }
}
