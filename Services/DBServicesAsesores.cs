using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Services
{
    public class DBServicesAsesores
    {
        private readonly MDbContext _context;

        public DBServicesAsesores(MDbContext context)
        {
            _context = context;
        }
        
    }
}