using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.API.Models
{
    public class ViewInicioVendedor
    {
        public Vendedor Vendedor { get; set; } = new Vendedor();
    }
}