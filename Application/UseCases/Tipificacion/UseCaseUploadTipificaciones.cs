using ALFINapp.API.DTOs;
using ALFINapp.Application.Interfaces.Tipificacion;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.UseCases.Tipificacion
{
    public class UseCaseUploadTipificaciones : IUseCaseUploadTipificaciones
    {
        private readonly IRepositoryClientes _repositoryClientes;
        private readonly IRepositoryUsuarios _repositoryUsuarios;
        private readonly IRepositoryDerivaciones _repositoryDerivaciones;
        private readonly IRepositoryTipificaciones _repositoryTipificaciones;
        private readonly IRepositoryTelefonos _repositoryTelefonos;
        public UseCaseUploadTipificaciones(
            IRepositoryClientes repositoryClientes,
            IRepositoryUsuarios repositoryUsuarios,
            IRepositoryDerivaciones repositoryDerivaciones,
            IRepositoryTipificaciones repositoryTipificaciones,
            IRepositoryTelefonos repositoryTelefonos)
        {
            _repositoryClientes = repositoryClientes;
            _repositoryUsuarios = repositoryUsuarios;
            _repositoryDerivaciones = repositoryDerivaciones;
            _repositoryTipificaciones = repositoryTipificaciones;
            _repositoryTelefonos = repositoryTelefonos;
        }
        public async Task<(bool success, string message)> execute(int idUsuario,
            List<DtoVTipificarCliente> tipificaciones,
            int IdAsignacion,
            int typeTipificacion)
        {
            try
            {
                int? usuarioId = idUsuario;
                if (usuarioId == null)
                {
                    return (false, "No se ha enviado el Id del Usuario.");
                }

                var ClienteAsignado = await _repositoryClientes.GetAsignacion(IdAsignacion);
                if (ClienteAsignado == null)
                {
                    return (false, "No se ha encontrado la asignacion.");
                }

                var usuarioInfo = await _repositoryUsuarios.GetUser(usuarioId.Value);
                if (usuarioInfo == null)
                {
                    return (false, "No se ha encontrado el usuario.");
                }

                if (tipificaciones == null || !tipificaciones.Any())
                {
                    return (false, "No se estan enviando datos para guardar.");
                }

                int? tipificacionMayorPeso = null;
                string? descripcionTipificacionMayorPeso = null;
                int? pesoMayor = 0;
                var agregado = false;
                var existeDerivacion = false;

                foreach (var tipificacion in tipificaciones)
                {
                    if (tipificacion.TipificacionId == 0)
                    {
                        Console.WriteLine("TipificacionId es 0, no se revisara este campo.");
                        continue; // Salta al siguiente registro sin hacer la inserción
                    }

                    if (string.IsNullOrEmpty(tipificacion.Telefono) || tipificacion.Telefono == "0")
                    {
                        return (false, "No modifique la entrada de los telefonos manualmente.");
                    }

                    if (tipificacion.FechaVisita == null && tipificacion.TipificacionId == 2)
                    {
                        return (false, "Debe ingresar una fecha de derivación para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).");
                    }
                    if (tipificacion.TipificacionId == 2)
                    {
                        if (tipificacion.FechaVisita == null)
                        {
                            return (false, "La fecha de derivación no puede estar vacia.");
                        }
                        if (tipificacion.AgenciaAsignada == null)
                        {
                            return (false, "La agencia asignada no puede estar vacia.");
                        }
                        var verificarDerivacion = await _repositoryDerivaciones
                            .getDerivaciones(ClienteAsignado.IdCliente, usuarioInfo.Dni ?? "");
                        if (verificarDerivacion == null || verificarDerivacion.Count() == 0)
                        {
                            return (false, "No ha enviado la derivacion correspondiente. No se guardara ninguna Tipificacion");
                        }
                        existeDerivacion = true;
                    }
                    agregado = true;
                }

                if (agregado == false)
                {
                    return (false, "No se ha llenado ningun campo o se han llenado incorrectamente.");
                }
                var clienteEnriquecido = await _repositoryClientes.GetEnriquecido(ClienteAsignado.IdCliente);
                if (clienteEnriquecido == null)
                {
                    return (false, "No se ha encontrado el cliente enriquecido.");
                }
                foreach (var tipificacion in tipificaciones)
                {
                    if (tipificacion.TipificacionId == 0)
                    {
                        Console.WriteLine("TipificacionId es 0, omitiendo inserción.");
                        continue; // Salta al siguiente registro sin hacer la inserción
                    }
                    if (tipificacion.TipificacionId == 2)
                    {
                        var verificarDerivacion = await _repositoryDerivaciones
                            .getDerivaciones(ClienteAsignado.IdCliente, usuarioInfo.Dni ?? "");
                        if (verificarDerivacion == null || verificarDerivacion.Count() == 0)
                        {
                            return (false, "No ha enviado la derivacion correspondiente. No se guardara ninguna Tipificacion");
                        }
                        if (verificarDerivacion.Count > 1)
                        {
                            continue;
                        }
                        var verificarGestion = await _repositoryDerivaciones
                            .getGestionDerivacion(verificarDerivacion[0].DniCliente ?? "", usuarioInfo.Dni ?? "");
                        if (verificarGestion != null)
                        {
                            continue;
                        }
                    }

                    var nuevaTipificacion = new ClientesTipificado
                    {
                        IdAsignacion = IdAsignacion,
                        IdTipificacion = tipificacion.TipificacionId,
                        FechaTipificacion = DateTime.Now,
                        Origen = "nuevo",
                        TelefonoTipificado = tipificacion.Telefono,
                        DerivacionFecha = tipificacion.FechaVisita
                    };

                    var resultGuardarClienteTipificado = await _repositoryTipificaciones.UploadTipificacion(nuevaTipificacion);
                    if (!resultGuardarClienteTipificado)
                    {
                        return (false, "Error al guardar la tipificación.");
                    }

                    var tipificacionActual = await _repositoryTipificaciones.GetTipificacion(tipificacion.TipificacionId);
                    if (tipificacionActual != null && tipificacionActual.Peso.HasValue)
                    {
                        int pesoActual = tipificacionActual.Peso.Value;

                        if (pesoMayor == 0 || pesoActual > pesoMayor)
                        {
                            pesoMayor = pesoActual;
                            tipificacionMayorPeso = tipificacion.TipificacionId;
                            descripcionTipificacionMayorPeso = tipificacionActual.DescripcionTipificacion;
                        }
                    }

                    if (typeTipificacion == 2)
                    {
                        var telefonos = new List<string> { clienteEnriquecido.Telefono1 ?? "", clienteEnriquecido.Telefono2 ?? "", clienteEnriquecido.Telefono3 ?? "", clienteEnriquecido.Telefono4 ?? "", clienteEnriquecido.Telefono5 ?? "" };
                        for (int i = 0; i < telefonos.Count; i++)
                        {
                            if (telefonos[i] == tipificacion.Telefono)
                            {
                                switch (i)
                                {
                                    case 0:
                                        clienteEnriquecido.FechaUltimaTipificacionTelefono1 = DateTime.Now;
                                        clienteEnriquecido.UltimaTipificacionTelefono1 = tipificacionActual?.DescripcionTipificacion ?? "Descripción no disponible";
                                        clienteEnriquecido.IdClientetipTelefono1 = nuevaTipificacion.IdClientetip;
                                        break;
                                    case 1:
                                        clienteEnriquecido.FechaUltimaTipificacionTelefono2 = DateTime.Now;
                                        clienteEnriquecido.UltimaTipificacionTelefono2 = tipificacionActual?.DescripcionTipificacion ?? "Descripción no disponible";
                                        clienteEnriquecido.IdClientetipTelefono2 = nuevaTipificacion.IdClientetip;
                                        break;
                                    case 2:
                                        clienteEnriquecido.FechaUltimaTipificacionTelefono3 = DateTime.Now;
                                        clienteEnriquecido.UltimaTipificacionTelefono3 = tipificacionActual?.DescripcionTipificacion ?? "Descripción no disponible";
                                        clienteEnriquecido.IdClientetipTelefono3 = nuevaTipificacion.IdClientetip;
                                        break;
                                    case 3:
                                        clienteEnriquecido.FechaUltimaTipificacionTelefono4 = DateTime.Now;
                                        clienteEnriquecido.UltimaTipificacionTelefono4 = tipificacionActual?.DescripcionTipificacion ?? "Descripción no disponible";
                                        clienteEnriquecido.IdClientetipTelefono4 = nuevaTipificacion.IdClientetip;
                                        break;
                                    case 4:
                                        clienteEnriquecido.FechaUltimaTipificacionTelefono5 = DateTime.Now;
                                        clienteEnriquecido.UltimaTipificacionTelefono5 = tipificacionActual?.DescripcionTipificacion ?? "Descripción no disponible";
                                        clienteEnriquecido.IdClientetipTelefono5 = nuevaTipificacion.IdClientetip;
                                        break;
                                }
                                var checkEnriquecido = await _repositoryClientes.UpdateEnriquecido(clienteEnriquecido);
                                if (!checkEnriquecido)
                                {
                                    return (false, "Error al actualizar el cliente enriquecido.");
                                }
                                break;
                            }
                        }
                    }
                    if (typeTipificacion == 1)
                    {
                        var telefonoTipificado = await _repositoryTelefonos.GetTelefono(tipificacion.Telefono ?? "", ClienteAsignado.IdCliente);
                        if (telefonoTipificado == null)
                        {
                            return (false, "El Numero de Telefono Agregado no ha sido encontrado.");
                        }
                        telefonoTipificado.UltimaTipificacion = tipificacionActual?.DescripcionTipificacion ?? "Descripción no disponible";
                        telefonoTipificado.FechaUltimaTipificacion = DateTime.Now;
                        telefonoTipificado.IdClienteTip = tipificacionActual?.IdTipificacion ?? 0;
                        var checkTelefono = await _repositoryTelefonos.UpdateTelefono(telefonoTipificado);
                        if (!checkTelefono)
                        {
                            return (false, "Error al actualizar el telefono.");
                        }
                    }
                    var guardarGestionDetalle = await _repositoryTipificaciones.UploadGestionTip(ClienteAsignado.convertToModel(), nuevaTipificacion, clienteEnriquecido, usuarioId.Value);
                    if (!guardarGestionDetalle)
                    {
                        return (false, "Error al guardar la gestión.");
                    }
                }

                if (tipificacionMayorPeso.HasValue && pesoMayor != 0)
                {
                    if (pesoMayor > (ClienteAsignado.PesoTipificacionMayor ?? 0))
                    {
                        ClienteAsignado.TipificacionMayorPeso = descripcionTipificacionMayorPeso;
                        ClienteAsignado.PesoTipificacionMayor = pesoMayor;
                        ClienteAsignado.FechaTipificacionMayorPeso = DateTime.Now;

                        var checkAsignacion = await _repositoryClientes.UpdateAsignacion(ClienteAsignado.convertToModel());
                        if (!checkAsignacion)
                        {
                            return (false, "Error al actualizar la asignación.");
                        }
                    }
                }
                var message = "No hay tipificaciones de derivacion.";
                if (existeDerivacion)
                {
                    message = "Se proceso la tipificacion de derivacion correctamente.";
                }
                return (true, "Se guardaron las Tipificaciones correctamente. " + message);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}