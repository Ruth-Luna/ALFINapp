using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ALFINapp.Models
{
    public class ViewTipificaciones
    {
        public List<string> lista_tipificaciones_disponibles { get; set; } = new List<string>();
        public List<string> lista_agencias_disponibles { get; set; } = new List<string>();
        public ViewClienteDetallado cliente { get; set; } = new ViewClienteDetallado();
        public ViewTipificaciones() { }
        public ViewTipificaciones(
            DetalleClienteA365TipificarDTO datos_cliente,
            List<string> lista_tipificaciones_disponibles
            )
        {
            this.lista_tipificaciones_disponibles = lista_tipificaciones_disponibles;
            this.cliente = new ViewClienteDetallado(datos_cliente);
        }
    }

    public class ViewClienteDetallado
    {
        // Lista de telefonos del cliente
        // Estos datos son obtenidos de la base de datos y se deberan llenar manualmente se creara una funcion simple para agregar telefonos
        public List<ViewTelefonoDetallado> lista_telefonos_del_cliente_bd { get; set; } = new List<ViewTelefonoDetallado>();
        public List<ViewTelefonoDetallado> lista_telefonos_del_cliente_manual { get; set; } = new List<ViewTelefonoDetallado>();
        // Datos del cliente
        public string? dni { get; set; }
        public string? xappaterno { get; set; }
        public string? xapmaterno { get; set; }
        public string? xnombre { get; set; }
        public int? edad { get; set; }
        public string? departamento { get; set; }
        public string? provincia { get; set; }
        public string? distrito { get; set; }
        public int? id_base { get; set; }

        //Propiedades de la tabla DetalleBase
        public string? campaña { get; set; }
        public string? sucursal { get; set; }
        public string? agencia_comercial { get; set; }
        public int? plazo { get; set; }
        public decimal? cuota { get; set; }
        public string? grupo_tasa { get; set; }
        public string? grupo_monto { get; set; }
        public int? propension { get; set; }
        public string? tipo_cliente { get; set; }
        public string? cliente_nuevo { get; set; }
        public string? color { get; set; }
        public string? color_final { get; set; }
        public string? usuario { get; set; }
        public string? segmento_user { get; set; }
        public string? perfil_ro { get; set; }

        //Propiedades de la Tabla ClientsEnriquecido
        public int? id_cliente { get; set; }
        // public string? telefono1 { get; set; }
        // public string? telefono2 { get; set; }
        // public string? telefono3 { get; set; }
        // public string? telefono4 { get; set; }
        // public string? telefono5 { get; set; }
        // public string? comentario_telefono1 { get; set; }
        // public string? comentario_telefono2 { get; set; }
        // public string? comentario_telefono3 { get; set; }
        // public string? comentario_telefono4 { get; set; }
        // public string? comentario_telefono5 { get; set; }
        // public string? ultima_tipificacion_telefono1 { get; set; }
        // public string? ultima_tipificacion_telefono2 { get; set; }
        // public string? ultima_tipificacion_telefono3 { get; set; }
        // public string? ultima_tipificacion_telefono4 { get; set; }
        // public string? ultima_tipificacion_telefono5 { get; set; }
        // public DateTime? fecha_ultima_tipificacion_telefono1 { get; set; }
        // public DateTime? fecha_ultima_tipificacion_telefono2 { get; set; }
        // public DateTime? fecha_ultima_tipificacion_telefono3 { get; set; }
        // public DateTime? fecha_ultima_tipificacion_telefono4 { get; set; }
        // public DateTime? fecha_ultima_tipificacion_telefono5 { get; set; }

        //Propiedas de Tasa
        public decimal? oferta_max { get; set; }
        public decimal? tasa_minima { get; set; }
        public decimal? oferta_12m { get; set; }
        public decimal? tasa_12m { get; set; }
        public decimal? cuota_12m { get; set; }
        public decimal? oferta_18m { get; set; }
        public decimal? tasa_18m { get; set; }
        public decimal? cuota_18m { get; set; }
        public decimal? oferta_24m { get; set; }
        public decimal? tasa_24m { get; set; }
        public decimal? cuota_24m { get; set; }
        public decimal? oferta_36m { get; set; }
        public decimal? tasa_36m { get; set; }
        public decimal? cuota_36m { get; set; }

        //Propiedades de la tabla ClientesAsignados
        public int? id_asignacion { get; set; }
        public string? fuente_base { get; set; }

        //Propiedades de la tabla TelefonoAgregadosManualmente
        // public string? telefono_manual { get; set; }
        // public string? comentario_telefono_manual { get; set; }
        // public string? ultima_tipificacion_telefono_manual { get; set; }
        // public DateTime? fecha_ultima_tipificacion_telefono_manual { get; set; }

        public ViewClienteDetallado() { }
        public ViewClienteDetallado(DetalleClienteA365TipificarDTO model)
        {
            dni = model.Dni ?? string.Empty;
            xappaterno = model.XAppaterno ?? string.Empty;
            xapmaterno = model.XApmaterno ?? string.Empty;
            xnombre = model.XNombre ?? string.Empty;
            edad = model.Edad ?? 0;
            departamento = model.Departamento ?? string.Empty;
            provincia = model.Provincia ?? string.Empty;
            distrito = model.Distrito ?? string.Empty;
            id_base = model.IdBase;

            campaña = model.Campaña ?? string.Empty;
            sucursal = model.Sucursal ?? string.Empty;
            agencia_comercial = model.AgenciaComercial ?? string.Empty;
            plazo = model.Plazo ?? 0;
            cuota = model.Cuota ?? 0;
            grupo_tasa = model.GrupoTasa ?? string.Empty;
            grupo_monto = model.GrupoMonto ?? string.Empty;
            propension = model.Propension ?? 0;
            tipo_cliente = model.TipoCliente ?? string.Empty;
            cliente_nuevo = model.ClienteNuevo ?? string.Empty;
            color = model.Color ?? string.Empty;
            color_final = model.ColorFinal ?? string.Empty;
            usuario = model.Usuario ?? string.Empty;
            segmento_user = model.SegmentoUser ?? string.Empty;
            perfil_ro = model.PerfilRo ?? string.Empty;

            id_cliente = model.IdCliente ?? 0;
            oferta_max = model.OfertaMax ?? 0;
            tasa_minima = model.TasaMinima ?? 0;
            oferta_12m = model.Oferta12m ?? 0;
            tasa_12m = model.Tasa12m ?? 0;
            cuota_12m = model.Cuota12m ?? 0;
            oferta_18m = model.Oferta18m ?? 0;
            tasa_18m = model.Tasa18m ?? 0;
            cuota_18m = model.Cuota18m ?? 0;
            oferta_24m = model.Oferta24m ?? 0;
            tasa_24m = model.Tasa24m ?? 0;
            cuota_24m = model.Cuota24m ?? 0;
            oferta_36m = model.Oferta36m ?? 0;
            tasa_36m = model.Tasa36m ?? 0;
            cuota_36m = model.Cuota36m ?? 0;

            id_asignacion = model.IdAsignacion ?? 0;
            fuente_base = model.FuenteBase ?? string.Empty;
        }
        public void llenarTelefonosBd(DetalleClienteA365TipificarDTO model)
        {
            lista_telefonos_del_cliente_bd.Clear();

            for (int i = 1; i <= 5; i++)
            {
                var telefono = model.GetType().GetProperty($"Telefono{i}")?.GetValue(model)?.ToString();
                var comentario = model.GetType().GetProperty($"ComentarioTelefono{i}")?.GetValue(model)?.ToString();
                var ultimaTipificacion = model.GetType().GetProperty($"UltimaTipificacionTelefono{i}")?.GetValue(model)?.ToString();
                var fechaUltimaTipificacion = model.GetType().GetProperty($"FechaUltimaTipificacionTelefono{i}")?.GetValue(model);

                lista_telefonos_del_cliente_bd.Add(new ViewTelefonoDetallado
                {
                    telefono = telefono,
                    comentario = comentario,
                    ultima_tipificacion = ultimaTipificacion,
                    fecha_ultima_tipificacion = (DateTime?)fechaUltimaTipificacion
                });
            }
        }

        public void llenarTelefonosManual(List<DetalleClienteA365TipificarDTO> telefonos)
        {
            foreach (var telefono in telefonos)
            {
                lista_telefonos_del_cliente_manual.Add(new ViewTelefonoDetallado
                {
                    telefono = telefono.TelefonoManual,
                    comentario = telefono.ComentarioTelefonoManual,
                    ultima_tipificacion = telefono.UltimaTipificacionTelefonoManual,
                    fecha_ultima_tipificacion = telefono.FechaUltimaTipificacionTelefonoManual
                });
            }
        }
    }
    public class ViewTelefonoDetallado
    {
        public string? telefono { get; set; }
        public string? comentario { get; set; }
        public string? ultima_tipificacion { get; set; }
        public DateTime? fecha_ultima_tipificacion { get; set; }
        public ViewTelefonoDetallado() { }
        public ViewTelefonoDetallado(string telefono, string comentario, string ultima_tipificacion, DateTime? fecha_ultima_tipificacion)
        {
            this.telefono = telefono;
            this.comentario = comentario;
            this.ultima_tipificacion = ultima_tipificacion;
            this.fecha_ultima_tipificacion = fecha_ultima_tipificacion;
        }
    }
}