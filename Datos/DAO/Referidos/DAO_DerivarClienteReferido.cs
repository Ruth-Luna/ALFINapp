using ALFINapp.Datos.DAO.Derivaciones;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Referidos
{
    public class DAO_DerivarClienteReferido
    {
        private readonly MDbContext _context;
        private readonly DAO_ReferirCliente _daoReferirCliente;
        private readonly DAO_ClientesConsultas _daoClientesConsultas;
        private readonly DAO_UploadDerivacion _daoUploadDerivacion;

        public DAO_DerivarClienteReferido(
            MDbContext context,
            DAO_ReferirCliente daoReferirCliente,
            DAO_ClientesConsultas daoClientesConsultas,
            DAO_UploadDerivacion daoUploadDerivacion)
        {
            _daoReferirCliente = daoReferirCliente;
            _context = context;
            _daoClientesConsultas = daoClientesConsultas;
            _daoUploadDerivacion = daoUploadDerivacion;
        }

        public async Task<(bool IsSuccess, string Message)> DerivarClienteReferidoAsync(int idReferido)
        {
            try
            {
                var referido = await _context.clientes_referidos
                    .FirstOrDefaultAsync(r => r.IdReferido == idReferido);
                if (referido == null)
                {
                    return (false, "Cliente referido no encontrado.");
                }
                var data_cliente = await _daoClientesConsultas.GetClienteByDniAsync(referido.DniCliente ?? string.Empty);
                if (data_cliente.IsSuccess == false || data_cliente.Data == null)
                {
                    return (false, data_cliente.Message);
                }
                var derivacion = new DerivacionesAsesores
                {
                    TelefonoCliente = referido.Telefono,
                    NombreAgencia = referido.Agencia,
                    FechaVisita = referido.FechaVisita,
                    NombreCliente = referido.NombreCompletoCliente
                };
                var result_derivacion = await _daoUploadDerivacion
                    .uploadNuevaDerivacion(
                        derivacion,
                        idBase: data_cliente.Data.IdBase != null ? data_cliente.Data.IdBase.Value : 0,
                        dni_asesor_auxiliar: referido.DniAsesor ?? string.Empty);

                if (result_derivacion.success == false)
                {
                    return (false, result_derivacion.message);
                }
                var updateReferido = await ActualizarEstadoDeLaReferencia(idReferido);
                if (updateReferido.IsSuccess == false)
                {
                    return (false, updateReferido.Message);
                }
                return (true, "Cliente referido derivado correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al derivar cliente referido: {ex.Message}");
            }
        }

        public async Task<(bool IsSuccess, string Message)> ActualizarEstadoDeLaReferencia(int idReferido)
        {
            try
            {
                var referido = await _context.clientes_referidos.Where(cr => cr.IdReferido == idReferido ).FirstOrDefaultAsync();
                if (referido == null)
                {
                    return (false, "No se ha encontrado el referido");
                }

                referido.FueProcesado = true;
                referido.EstadoReferencia = "DERIVACION COMPLETADA";
                _context.Update(referido);
                await _context.SaveChangesAsync();
                return (true, "Referido procesado exitosamente");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}