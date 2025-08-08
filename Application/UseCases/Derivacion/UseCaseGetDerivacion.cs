using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Derivacion;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;

namespace ALFINapp.Application.UseCases.Derivacion
{
    public class UseCaseGetDerivacion : IUseCaseGetDerivacion
    {
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        private readonly IRepositorySupervisor _repositorySupervisor;
        private readonly IRepositoryDerivaciones _repositoryDerivaciones;
        public UseCaseGetDerivacion(
            IRepositoryUsuarios repositoryUsuarios,
            IRepositorySupervisor repositorySupervisor,
            IRepositoryDerivaciones repositoryDerivaciones)
        {
            _repositoryUsuarios = repositoryUsuarios;
            _repositorySupervisor = repositorySupervisor;
            _repositoryDerivaciones = repositoryDerivaciones;
        }
        public async Task<(bool success, string message, ViewDerivacionesVistaGeneral data)> Execute(
            int idUsuario, int idRol)
        {
            try
            {
                var getdatosUsuario = await _repositoryUsuarios.GetUser(idUsuario);
                if (getdatosUsuario == null)
                {
                    return (false, "No se encontró el usuario", new ViewDerivacionesVistaGeneral());
                }
                var viewDerivaciones = new ViewDerivacionesVistaGeneral();
                var asesores = new List<Vendedor>();
                var supervisores = new List<ALFINapp.Domain.Entities.Supervisor>();
                if (idRol == 1 || idRol == 4)
                {
                    var getAllAsesores = await _repositoryUsuarios.GetAllAsesores();
                    var GetAllSupervisores = await _repositoryUsuarios.GetAllSupervisores();
                    foreach (var item in GetAllSupervisores)
                    {
                        supervisores.Add(item.ToEntitySupervisor());
                    }
                    foreach (var item in getAllAsesores)
                    {
                        var supAsesor = supervisores.FirstOrDefault(x => x.IdUsuario == item.IDUSUARIOSUP);
                        if (supAsesor != null)
                        {
                            supAsesor.Vendedores.Add(item.ToEntityVendedor());
                        }
                        asesores.Add(item.ToEntityVendedor());
                    }
                }
                else if (idRol == 2)
                {
                    var getAllAsesores = await _repositoryUsuarios.GetAllAsesoresBySupervisor(idUsuario);
                    foreach (var item in getAllAsesores)
                    {
                        asesores.Add(item.ToEntityVendedor());
                    }

                }
                else if (idRol == 3)
                {
                    var asesorDTO = new DetallesUsuarioDTO(getdatosUsuario);
                    asesores.Add(asesorDTO.ToEntityVendedor());
                }
                var getAllDerivaciones = await _repositoryDerivaciones.getDerivaciones(asesores);
                getAllDerivaciones = getAllDerivaciones.OrderByDescending(x => x.FechaDerivacion).ToList();
                viewDerivaciones.Asesores = asesores;
                viewDerivaciones.Supervisores = supervisores;
                viewDerivaciones.RolUsuario = idRol;
                viewDerivaciones.DniUsuario = getdatosUsuario.Dni ?? string.Empty;
                
                foreach (var item in getAllDerivaciones)
                {
                    viewDerivaciones.Derivaciones.Add(item.ToViewDerivaciones());
                }
                return (true, "Se realizo la busqueda de Derivaciones con exito", viewDerivaciones);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewDerivacionesVistaGeneral());
            }
        }
    }
}