using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Infrastructure.Services
{
    public class DBServicesGeneral
    {
        private readonly MDbContext _context;

        public DBServicesGeneral(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, Usuario? Data)> GetUserInformation(int IdUsuario)
        {
            try
            {
                var FoundUser = await _context.usuarios
                                    .Where(u => u.IdUsuario == IdUsuario)
                                    .FirstOrDefaultAsync();

                if (FoundUser == null)
                {
                    return (false, "El usuario a buscar no se encuentra registrado", null);
                }

                return (true, "Usuario encontrado correctamente", FoundUser);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message)> UpdatePasswordGeneralFunction(int IdUsuario, string password)
        {
            try
            {
                var user = await _context.usuarios.Where(u => u.IdUsuario == IdUsuario)
                                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return (false, "El usuario a modificar no se encuentra registrado");
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE usuarios SET contraseña = {0} WHERE id_usuario = {1}",
                    password, IdUsuario);

                return (true, "La Modificación se realizo con exito");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<AgenciasDisponiblesDTO>? data)> GetUAgenciasConNumeros()
        {
            try
            {
                var agenciasDisponibles = await _context.agencias_disponibles_dto
                .FromSqlRaw("EXEC sp_U_agencias_con_numeros")
                .ToListAsync();

                if (agenciasDisponibles == null || !agenciasDisponibles.Any())
                {
                    return (false, "No se encontraron agencias disponibles", null);
                }

                return (true, "Campañas encontradas", agenciasDisponibles);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUCampanas()
        {
            try
            {
                var campanas = await _context.string_dto
                .FromSqlRaw("EXEC SP_U_campaña")
                .ToListAsync();

                if (campanas == null || !campanas.Any())
                {
                    return (false, "No se encontraron campañas disponibles", null);
                }

                return (true, "Campañas encontradas", campanas);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUClienteEstado()
        {
            try
            {
                var clienteEstado = await _context.string_dto
                .FromSqlRaw("EXEC SP_U_cliente_estado")
                .ToListAsync();

                if (clienteEstado == null || !clienteEstado.Any())
                {
                    return (false, "No se encontraron estado del cliente disponibles", null);
                }

                return (true, "Estado del cliente encontrados", clienteEstado);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUColor()
        {
            try
            {
                var colores = await _context.string_dto
                .FromSqlRaw("EXEC SP_U_color")
                .ToListAsync();

                if (colores == null || !colores.Any())
                {
                    return (false, "No se encontraron colores disponibles", null);
                }

                return (true, "Colores encontrados", colores);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUColorFinal()
        {
            try
            {
                var colorFinal = await _context.string_dto
                .FromSqlRaw("EXEC SP_U_color_final")
                .ToListAsync();

                if (colorFinal == null || !colorFinal.Any())
                {
                    return (false, "No se encontraron colores finales disponibles", null);
                }

                return (true, "Colores finales encontradps", colorFinal);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<NumerosEnterosDTO>? data)> GetUFlgDeudaPlus()
        {
            try
            {
                var flgDeudaPlus = await _context.numeros_enteros_dto
                .FromSqlRaw("EXEC SP_U_flg_deuda_plus")
                .ToListAsync();

                if (flgDeudaPlus == null || !flgDeudaPlus.Any())
                {
                    return (false, "No se encontraron flag deuda plus disponibles", null);
                }

                return (true, "Flag deuda plus encontrados", flgDeudaPlus);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<NumerosEnterosDTO>? data)> GetUFrescura()
        {
            try
            {
                var frescura = await _context.numeros_enteros_dto
                .FromSqlRaw("EXEC SP_U_frescura")
                .ToListAsync();

                if (frescura == null || !frescura.Any())
                {
                    return (false, "No se encontraron frescura disponibles", null);
                }

                return (true, "Frescura encontrados", frescura);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUGrupoMonto()
        {
            try
            {
                var grupoMonto = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_grupo_monto")
                    .ToListAsync();

                if (grupoMonto == null || !grupoMonto.Any())
                {
                    return (false, "No se encontraron grupos de monto disponibles", null);
                }

                return (true, "Grupos de monto encontrados", grupoMonto);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUGrupoTasa()
        {
            try
            {
                var grupoTasa = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_grupo_tasa")
                    .ToListAsync();

                if (grupoTasa == null || !grupoTasa.Any())
                {
                    return (false, "No se encontraron grupos de tasa disponibles", null);
                }

                return (true, "Grupos de tasa encontrados", grupoTasa);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<NumerosEnterosDTO>? data)> GetUPropension()
        {
            try
            {
                var propension = await _context.numeros_enteros_dto
                    .FromSqlRaw("EXEC SP_U_propension")
                    .ToListAsync();

                if (propension == null || !propension.Any())
                {
                    return (false, "No se encontraron propensiones disponibles", null);
                }

                return (true, "Propensiones encontradas", propension);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetURangoEdad()
        {
            try
            {
                var rangoEdad = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_rango_edad")
                    .ToListAsync();

                if (rangoEdad == null || !rangoEdad.Any())
                {
                    return (false, "No se encontraron rangos de edad disponibles", null);
                }

                return (true, "Rangos de edad encontrados", rangoEdad);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetURangoOferta()
        {
            try
            {
                var rangoOferta = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_rango_oferta")
                    .ToListAsync();

                if (rangoOferta == null || !rangoOferta.Any())
                {
                    return (false, "No se encontraron rangos de oferta disponibles", null);
                }

                return (true, "Rangos de oferta encontrados", rangoOferta);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetURangoSueldo()
        {
            try
            {
                var rangoSueldo = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_rango_sueldo")
                    .ToListAsync();

                if (rangoSueldo == null || !rangoSueldo.Any())
                {
                    return (false, "No se encontraron rangos de sueldo disponibles", null);
                }

                return (true, "Rangos de sueldo encontrados", rangoSueldo);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetURangoTasas()
        {
            try
            {
                var rangoTasas = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_rango_tasas")
                    .ToListAsync();

                if (rangoTasas == null || !rangoTasas.Any())
                {
                    return (false, "No se encontraron rangos de tasas disponibles", null);
                }

                return (true, "Rangos de tasas encontrados", rangoTasas);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUTipoBase()
        {
            try
            {
                var tipoBase = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_tipo_base")
                    .ToListAsync();

                if (tipoBase == null || !tipoBase.Any())
                {
                    return (false, "No se encontraron tipos de base disponibles", null);
                }

                return (true, "Tipos de base encontrados", tipoBase);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUTipoCliente()
        {
            try
            {
                var tipoCliente = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_tipo_cliente")
                    .ToListAsync();

                if (tipoCliente == null || !tipoCliente.Any())
                {
                    return (false, "No se encontraron tipos de cliente disponibles", null);
                }

                return (true, "Tipos de cliente encontrados", tipoCliente);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<StringDTO>? data)> GetUUsuario()
        {
            try
            {
                var usuario = await _context.string_dto
                    .FromSqlRaw("EXEC SP_U_usuario")
                    .ToListAsync();

                if (usuario == null || !usuario.Any())
                {
                    return (false, "No se encontraron usuarios disponibles", null);
                }

                return (true, "Usuarios encontrados", usuario);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}