using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class BoolDTO
    {
        [Key]
        bool ValueBool { get; set; }
    }
}