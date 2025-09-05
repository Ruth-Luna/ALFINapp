namespace ALFINapp.Datos.DAO
{
    public class Conexion
    {

        private string cadenaSQL = string.Empty;

        public Conexion()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            cadenaSQL = builder.GetSection("ConnectionStrings:DefaultConnection").Value ?? throw new Exception("Connection string 'DefaultConnection' not found in appsettings.json.");
        }

        public string getCadenaSQL()
        {
            return cadenaSQL;
        }
    }
}
