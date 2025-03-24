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

        public UseCaseUploadTipificaciones(
            IRepositoryClientes repositoryClientes, 
            IRepositoryUsuarios repositoryUsuarios,
            IRepositoryDerivaciones repositoryDerivaciones)
        {
            _repositoryClientes = repositoryClientes;
            _repositoryUsuarios = repositoryUsuarios;
            _repositoryDerivaciones = repositoryDerivaciones;
        }
        public async Task<(bool, string)> execute(int idUsuario, List<DtoVTipificarCliente> tipificaciones, int IdAsignacion)
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
                int? pesoMayor = 0;
                string? descripcionTipificacionMayorPeso = null;
                var agregado = false;

                foreach (var tipificacion in tipificaciones)
                {
                    if (tipificacion.TipificacionId == 0)
                    {
                        Console.WriteLine("TipificacionId es 0, no se revisara este campo.");
                        continue; // Salta al siguiente registro sin hacer la inserción
                    }

                    if (tipificacion.FechaVisita == null && tipificacion.TipificacionId == 2)
                    {
                        return (false, "Debe ingresar una fecha de derivación para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).");
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
                            return (false, "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.");
                        }

                        var verificarGestion = await _repositoryDerivaciones
                            .getGestionDerivacion(verificarDerivacion[0].DniCliente ?? "", usuarioInfo.Dni ?? "");
                        if (verificarGestion != null)
                        {
                            return (false, "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.");
                        }
                    }
                    agregado = true;
                }

                if (agregado == false)
                {
                    return (false, "No se ha llenado ningun campo o se han llenado incorrectamente.");
                }

                foreach (var tipificacion in tipificaciones)
                {
                    if (tipificacion.TipificacionId == 0)
                    {
                        Console.WriteLine("TipificacionId es 0, omitiendo inserción.");
                        continue; // Salta al siguiente registro sin hacer la inserción
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

                    var resultGuardarClienteTipificado = await _dbServicesTipificaciones.GuardarNuevaTipificacion(nuevaTipificacion);
                    if (!resultGuardarClienteTipificado.IsSuccess)
                    {
                        TempData["MessageError"] = resultGuardarClienteTipificado.Message;
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    // Verificar si esta tipificación tiene el mayor peso
                    var tipificacionActual = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.TipificacionId);
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
                    var telefonoTipificado = _context.telefonos_agregados.FirstOrDefault(ta => ta.Telefono == tipificacion.Telefono && ta.IdCliente == ClienteAsignado.Data.IdCliente);
                    if (telefonoTipificado == null)
                    {
                        TempData["MessageError"] = "El Numero de Telefono Agregado no ha sido encontrado.";
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    else
                    {
                        var tipificacionUltima = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.TipificacionId);
                        if (tipificacionUltima == null)
                        {
                            TempData["MessageError"] = "La tipificacion no ha sido encontrada.";
                            return RedirectToAction("Redireccionar", "Error");
                        }
                        telefonoTipificado.UltimaTipificacion = tipificacionUltima.DescripcionTipificacion;
                        telefonoTipificado.FechaUltimaTipificacion = DateTime.Now;
                        telefonoTipificado.IdClienteTip = nuevaTipificacion.IdClientetip;
                        _context.telefonos_agregados.Update(telefonoTipificado);
                    }

                    var clienteEnriquecido = await _dbServicesAsesores.ObtenerEnriquecido(ClienteAsignado.Data.IdCliente);
                    if (!clienteEnriquecido.IsSuccess || clienteEnriquecido.Data == null)
                    {
                        TempData["MessageError"] = clienteEnriquecido.Message;
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    var guardarGestionDetalle = await _dbServicesTipificaciones.GuardarGestionDetalle(ClienteAsignado.Data, nuevaTipificacion, clienteEnriquecido.Data, usuarioId.Value);
                    if (!guardarGestionDetalle.IsSuccess)
                    {
                        TempData["MessageError"] = guardarGestionDetalle.Message;
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    await _context.SaveChangesAsync();
                }

                if (tipificacionMayorPeso.HasValue && pesoMayor != 0)
                {
                    if (pesoMayor > (ClienteAsignado.Data.PesoTipificacionMayor ?? 0))
                    {
                        ClienteAsignado.Data.TipificacionMayorPeso = descripcionTipificacionMayorPeso;
                        ClienteAsignado.Data.PesoTipificacionMayor = pesoMayor;
                        ClienteAsignado.Data.FechaTipificacionMayorPeso = DateTime.Now;
                        _context.clientes_asignados.Update(ClienteAsignado.Data);
                    }
                }
                _context.SaveChanges();
                TempData["Message"] = "Las tipificaciones se han guardado correctamente (Se han Obviado los campos Vacios y los campos que fueron llenados con datos incorrectos).";
                return RedirectToAction("Redireccionar", "Error");
                return (true, "Non Implemented");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}