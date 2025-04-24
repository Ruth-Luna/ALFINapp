using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Reagendamiento;

namespace ALFINapp.Application.UseCases.Reagendamiento
{
    public class UseCaseReagendar : IUseCaseReagendar
    {
        public async Task<(bool IsSuccess, string Message)>  Reagendar(int idCliente, DateTime nuevaFechaVisita, string motivoReagendamiento, string nuevaAgencia, string nuevaOferta, List<(string Filtro, string Dato)> filtros)
        {
            try
            {
                return (true, "Reagendamiento exitoso");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return (false, "Error al reagendar");
            }
        }
    }
}