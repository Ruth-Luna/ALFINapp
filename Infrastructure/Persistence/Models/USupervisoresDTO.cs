using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class USupervisoresDTO
    {
        [Key]
        public int id_usuario { get; set; }
        public string? NombresCompletos { get; set; }
    }
}