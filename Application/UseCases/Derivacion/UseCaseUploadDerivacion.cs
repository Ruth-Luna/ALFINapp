using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Derivacion;
using ALFINapp.Application.Interfaces.Tipificacion;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.UseCases.Derivacion
{
    public class UseCaseUploadDerivacion : IUseCaseUploadDerivacion
    {
        private readonly IRepositoryClientes _repositoryClientes;
        private readonly IRepositoryDerivaciones _repositoryDerivaciones;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        private readonly IUseCaseUploadTipificaciones _useCaseUploadTipificaciones;
        public UseCaseUploadDerivacion(IRepositoryClientes repositoryClientes,
            IRepositoryDerivaciones repositoryDerivaciones,
            IRepositoryUsuarios repositoryUsuarios,
            IUseCaseUploadTipificaciones useCaseUploadTipificaciones)
        {
            _repositoryClientes = repositoryClientes;
            _repositoryDerivaciones = repositoryDerivaciones;
            _repositoryUsuarios = repositoryUsuarios;
            _useCaseUploadTipificaciones = useCaseUploadTipificaciones;
        }
        public async Task<(bool success, string message)> Execute(
            string agenciaComercial,
            DateTime FechaVisita,
            string Telefono,
            int idBase,
            int idUsuario,
            int idAsignacion,
            int type,
            string? NombresCompletos)
        {
            try
            {
                var verificarDisponibilidad = await _repositoryDerivaciones.verDisponibilidad(idBase);
                if (!verificarDisponibilidad.success)
                {
                    return (false, verificarDisponibilidad.message);
                }
                var derivacion = new DerivacionesAsesores
                {
                    FechaDerivacion = DateTime.Now,
                    FechaVisita = FechaVisita,
                    TelefonoCliente = Telefono,
                    NombreAgencia = agenciaComercial
                };
                var getcliente = await _repositoryClientes.getBase(idBase);
                if (getcliente == null)
                {
                    return (false, "No se encontro el cliente");
                }
                if (NombresCompletos != null)
                {
                    derivacion.NombreCliente = NombresCompletos;
                }
                var uploadDerivacion = await _repositoryDerivaciones.uploadNuevaDerivacion(
                    derivacion,
                    idBase,
                    idUsuario);
                if (!uploadDerivacion.success)
                {
                    return (false, uploadDerivacion.message);
                }
                var checkDerivacion = await _repositoryDerivaciones.verDerivacion(getcliente.Dni ?? string.Empty);
                if (!checkDerivacion.success)
                {
                    return (false, checkDerivacion.message);
                }
                var createTipificacion = new List<DtoVTipificarCliente>();
                var tipificacion = new DtoVTipificarCliente
                {
                    Telefono = Telefono,
                    TipificacionId = 2,
                    FechaVisita = FechaVisita,
                    AgenciaAsignada = agenciaComercial
                };
                createTipificacion.Add(tipificacion);
                var uploadTipificacion = await _useCaseUploadTipificaciones.execute(
                    idUsuario,
                    createTipificacion,
                    idAsignacion,
                    type
                    );
                if (!uploadTipificacion.success)
                {
                    return (false, uploadTipificacion.message);
                }
                return (true, "Derivacion subida correctamente. Si tiene mas tipificaciones puede guardarlas usando el boton de Guardar Tipificaciones. Esto no modificara ni procesara la Tipificacion de Derivacion");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}