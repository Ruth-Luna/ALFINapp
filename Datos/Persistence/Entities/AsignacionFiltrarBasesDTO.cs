using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class AsignacionFiltrarBasesDTO
    {
        public int? id_base { get; set; }
        public string? dni { get; set; }
        public string? x_appaterno { get; set; }
        public string? x_apmaterno { get; set; }
        public string? x_nombre { get; set; }
        public int? edad { get; set; }
        public string? departamento { get; set; }
        public string? provincia { get; set; }
        public string? distrito { get; set; }
        public int? id_base_banco { get; set; }
        public int? id_detalle { get; set; }
        public string? sucursal { get; set; }
        public string? tienda { get; set; }
        public decimal? oferta_max { get; set; }
        public decimal? tasa_minima { get; set; }
        public int? tipo_verificacion { get; set; }
        public string? canal { get; set; }
        public string? tipovisita { get; set; }
        public decimal? tasa_1 { get; set; }
        public decimal? tasa_2 { get; set; }
        public decimal? tasa_3 { get; set; }
        public decimal? tasa_4 { get; set; }
        public decimal? tasa_5 { get; set; }
        public decimal? tasa_6 { get; set; }
        public decimal? tasa_7 { get; set; }
        public string? segmento { get; set; }
        public int? plazo { get; set; }
        public string? campaña { get; set; }
        public string? tem { get; set; }
        public decimal? desgravamen { get; set; }
        public decimal? cuota { get; set; }
        public decimal? oferta_12M { get; set; }
        public decimal? tasa_12M { get; set; }
        public decimal? desgravamen_12M { get; set; }
        public decimal? cuota_12M { get; set; }
        public decimal? oferta_18M { get; set; }
        public decimal? tasa_18M { get; set; }
        public decimal? desgravamen_18M { get; set; }
        public decimal? cuota_18M { get; set; }
        public decimal? oferta_24M { get; set; }
        public decimal? tasa_24M { get; set; }
        public decimal? desgravamen_24M { get; set; }
        public decimal? cuota_24M { get; set; }
        public decimal? oferta_36M { get; set; }
        public decimal? tasa_36M { get; set; }
        public decimal? desgravamen_36M { get; set; }
        public decimal? cuota_36M { get; set; }
        public string? validador_telefono { get; set; }
        public string? prioridad { get; set; }
        public string? nombre_prioridad { get; set; }
        public decimal? deuda_1 { get; set; }
        public string? entidad_1 { get; set; }
        public decimal? deuda_2 { get; set; }
        public string? entidad_2 { get; set; }
        public decimal? deuda_3 { get; set; }
        public string? entidad_3 { get; set; }
        public string? sucursal_comercial { get; set; }
        public string? agencia_comercial { get; set; }
        public string? region_comercial { get; set; }
        public string? ubicacion { get; set; }
        public decimal? oferta_maxima_sin_seguro { get; set; }
        public string? color_gestion { get; set; }
        public decimal? oferta_final { get; set; }
        public string? garantia { get; set; }
        public decimal? oferta_minima_paperless { get; set; }
        public string? rango_edad { get; set; }
        public string? rango_oferta { get; set; }
        public string? rango_sueldo { get; set; }
        public decimal? capacidad_max { get; set; }
        public string? peer { get; set; }
        public string? tipo_gest { get; set; }
        public string? tipo_cliente { get; set; }
        public string? cliente_nuevo { get; set; }
        public string? grupo_tasa { get; set; }
        public string? grupo_monto { get; set; }
        public string? tasa_vs_monto { get; set; }
        public string? grupo_tasa_reenganche { get; set; }
        public decimal? saldo_diferencial_reeng { get; set; }
        public string? flag_reeng { get; set; }
        public string? color { get; set; }
        public string? color_final { get; set; }
        public int? propension { get; set; }
        public string? marca_base { get; set; }
        public string? segmento_user { get; set; }
        public string? usuario { get; set; }
        public string? tipo_cliente_riegos { get; set; }
        public int? incremento_monto_riesgos { get; set; }
        public DateTime? fecha_carga { get; set; }
        public string? tipo_base { get; set; }
        public string? rango_tasas { get; set; }
        public int? flg_deuda_plus { get; set; }
        public int? frescura { get; set; }
        public int? id_cliente { get; set; }
    }
}