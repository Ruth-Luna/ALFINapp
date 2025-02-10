using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ALFINapp.Services;
using System.Threading.Tasks;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class ReagregacionesController : Controller
    {
        private readonly MDbContext _context;
        private readonly DBServicesAsignacionesAsesores _dbServicesAsignacionesAsores;
        private readonly DBServicesConsultasClientes _dbServicesConsultasClientes;

        public ReagregacionesController(MDbContext context, DBServicesAsignacionesAsesores dbServicesAsignacionesAsores, DBServicesConsultasClientes dbServicesConsultasClientes)
        {
            _context = context;
            _dbServicesAsignacionesAsores = dbServicesAsignacionesAsores;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
        }
    }
}