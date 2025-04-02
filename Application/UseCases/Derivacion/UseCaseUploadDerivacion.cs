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
                var getClienteBase = await _repositoryClientes.getBase(idBase);
                if (getClienteBase == null)
                {
                    return (false, "Error al obtener la base");
                }

                var getAsesor = await _repositoryUsuarios.GetUser(idUsuario);
                var getEnriquecido = await _repositoryClientes.GetEnriquecidoxBase(getClienteBase.IdBase);
                if (getAsesor == null || getEnriquecido == null)
                {
                    return (false, "Error al obtener el asesor o el enriquecido");
                }
                var verificarDerivacion = await _repositoryDerivaciones
                            .getDerivaciones(getEnriquecido.IdCliente, getAsesor.Dni ?? "");
                if (verificarDerivacion == null)
                {
                    return (false, "Error al verificar la derivacion");
                }
                if (verificarDerivacion.Count > 0)
                {
                    return (false, "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones.");
                }
                var verificarGestion = await _repositoryDerivaciones
                    .getGestionDerivacion(getClienteBase.Dni ?? "", getAsesor.Dni ?? "");
                if (verificarGestion != null)
                {
                    return (false, "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones.");
                }
                var derivacion = new DerivacionesAsesores
                {
                    FechaDerivacion = DateTime.Now,
                    FechaVisita = FechaVisita,
                    DniAsesor = getAsesor.Dni,
                    DniCliente = getClienteBase.Dni,
                    IdCliente = getEnriquecido.IdCliente,
                    NombreCliente = getClienteBase.XNombre + " " + getClienteBase.XAppaterno + " " + getClienteBase.XApmaterno,
                    TelefonoCliente = Telefono,
                    NombreAgencia = agenciaComercial,
                    FueProcesado = false,
                    EstadoDerivacion = "DERIVACION PENDIENTE"
                };
                if (NombresCompletos != null)
                {
                    derivacion.NombreCliente = NombresCompletos;
                }
                var uploadDerivacion = await _repositoryDerivaciones.uploadDerivacion(derivacion);
                if (!uploadDerivacion)
                {
                    return (false, "Error al subir la derivacion");
                }
                var checkDerivacion = await _repositoryDerivaciones.verDerivacion(derivacion.DniCliente ?? string.Empty);
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