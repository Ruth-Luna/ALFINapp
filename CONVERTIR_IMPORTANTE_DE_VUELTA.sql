SELECT *
  FROM [CORE_ALFIN].[dbo].[detalle_base]
where CAST(fecha_carga AS DATE) = '2025-05-28'

SELECT *
  FROM base_clientes_banco
WHERE CAST(fecha_subida AS DATE) = '2025-05-23'

INSERT INTO [CORE_ALFIN].[dbo].[detalle_base] (
    [id_base]
      ,[sucursal]
      ,[tienda]
      ,[oferta_max]
      ,[tasa_minima]
      ,[tipo_verificacion]
      ,[canal]
      ,[tipovisita]
      ,[tasa_1]
      ,[tasa_2]
      ,[tasa_3]
      ,[tasa_4]
      ,[tasa_5]
      ,[tasa_6]
      ,[tasa_7]
      ,[segmento]
      ,[plazo]
      ,[campaña]
      ,[tem]
      ,[desgravamen]
      ,[cuota]
      ,[oferta_12M]
      ,[tasa_12M]
      ,[desgravamen_12M]
      ,[cuota_12M]
      ,[oferta_18M]
      ,[tasa_18M]
      ,[desgravamen_18M]
      ,[cuota_18M]
      ,[oferta_24M]
      ,[tasa_24M]
      ,[desgravamen_24M]
      ,[cuota_24M]
      ,[oferta_36M]
      ,[tasa_36M]
      ,[desgravamen_36M]
      ,[cuota_36M]
      ,[validador_telefono]
      ,[prioridad]
      ,[nombre_prioridad]
      ,[deuda_1]
      ,[entidad_1]
      ,[deuda_2]
      ,[entidad_2]
      ,[deuda_3]
      ,[entidad_3]
      ,[sucursal_comercial]
      ,[agencia_comercial]
      ,[region_comercial]
      ,[ubicacion]
      ,[oferta_maxima_sin_seguro]
      ,[color_gestion]
      ,[oferta_final]
      ,[garantia]
      ,[oferta_minima_paperless]
      ,[rango_edad]
      ,[rango_oferta]
      ,[rango_sueldo]
      ,[capacidad_max]
      ,[peer]
      ,[tipo_gest]
      ,[tipo_cliente]
      ,[cliente_nuevo]
      ,[grupo_tasa]
      ,[grupo_monto]
      ,[tasa_vs_monto]
      ,[grupo_tasa_reenganche]
      ,[saldo_diferencial_reeng]
      ,[flag_reeng]
      ,[color]
      ,[color_final]
      ,[propension]
      ,[marca_base]
      ,[segmento_user]
      ,[usuario]
      ,[tipo_cliente_riegos]
      ,[incremento_monto_riesgos]
      ,[tipo_base]
      ,[rango_tasas]
      ,[flg_deuda_plus]
      ,[frescura]
      ,[id_NombreBase]
      ,[LEAD_CALIDAD]
      ,[USER_V3]
      ,[base_banco]
      ,[flag_deuda_v_oferta]
      ,[PERFIL_RO]
      ,[PROPENSIONV2]
      ,[MGNEG]
      ,[PROPENSION_IC]
      ,[PRIORIDAD_SISTEMA]
      ,[ofertamaximasinseguro]
      ,[VARIACION_OFERTA_CAMPAÑA_ANTERIOR]
      ,[VARIACION_TASA_CAMPAÑA_ANTERIOR]
      ,[VAR_TASA_CREDITO_ANTERIOR]
      ,[FLG_CET_6M]
      ,[BLOQUE], fecha_carga
)
SELECT
    -- Las mismas columnas, pero modificamos fecha_carga
    [id_base]
      ,[sucursal]
      ,[tienda]
      ,[oferta_max]
      ,[tasa_minima]
      ,[tipo_verificacion]
      ,[canal]
      ,[tipovisita]
      ,[tasa_1]
      ,[tasa_2]
      ,[tasa_3]
      ,[tasa_4]
      ,[tasa_5]
      ,[tasa_6]
      ,[tasa_7]
      ,[segmento]
      ,[plazo]
      ,[campaña]
      ,[tem]
      ,[desgravamen]
      ,[cuota]
      ,[oferta_12M]
      ,[tasa_12M]
      ,[desgravamen_12M]
      ,[cuota_12M]
      ,[oferta_18M]
      ,[tasa_18M]
      ,[desgravamen_18M]
      ,[cuota_18M]
      ,[oferta_24M]
      ,[tasa_24M]
      ,[desgravamen_24M]
      ,[cuota_24M]
      ,[oferta_36M]
      ,[tasa_36M]
      ,[desgravamen_36M]
      ,[cuota_36M]
      ,[validador_telefono]
      ,[prioridad]
      ,[nombre_prioridad]
      ,[deuda_1]
      ,[entidad_1]
      ,[deuda_2]
      ,[entidad_2]
      ,[deuda_3]
      ,[entidad_3]
      ,[sucursal_comercial]
      ,[agencia_comercial]
      ,[region_comercial]
      ,[ubicacion]
      ,[oferta_maxima_sin_seguro]
      ,[color_gestion]
      ,[oferta_final]
      ,[garantia]
      ,[oferta_minima_paperless]
      ,[rango_edad]
      ,[rango_oferta]
      ,[rango_sueldo]
      ,[capacidad_max]
      ,[peer]
      ,[tipo_gest]
      ,[tipo_cliente]
      ,[cliente_nuevo]
      ,[grupo_tasa]
      ,[grupo_monto]
      ,[tasa_vs_monto]
      ,[grupo_tasa_reenganche]
      ,[saldo_diferencial_reeng]
      ,[flag_reeng]
      ,[color]
      ,[color_final]
      ,[propension]
      ,[marca_base]
      ,[segmento_user]
      ,[usuario]
      ,[tipo_cliente_riegos]
      ,[incremento_monto_riesgos]
      ,[tipo_base]
      ,[rango_tasas]
      ,[flg_deuda_plus]
      ,[frescura]
      ,[id_NombreBase]
      ,[LEAD_CALIDAD]
      ,[USER_V3]
      ,[base_banco]
      ,[flag_deuda_v_oferta]
      ,[PERFIL_RO]
      ,[PROPENSIONV2]
      ,[MGNEG]
      ,[PROPENSION_IC]
      ,[PRIORIDAD_SISTEMA]
      ,[ofertamaximasinseguro]
      ,[VARIACION_OFERTA_CAMPAÑA_ANTERIOR]
      ,[VARIACION_TASA_CAMPAÑA_ANTERIOR]
      ,[VAR_TASA_CREDITO_ANTERIOR]
      ,[FLG_CET_6M]
      ,[BLOQUE], CAST('2025-06-01' AS DATE) AS fecha_carga
FROM [CORE_ALFIN].[dbo].[detalle_base]
WHERE CAST(fecha_carga AS DATE) = '2025-05-28';


INSERT INTO base_clientes_banco (
    -- Lista explícita de columnas aquí, excepto fecha_subida
    -- Ejemplo:
    [dni]
      ,[tasa_1]
      ,[tasa_2]
      ,[tasa_3]
      ,[tasa_4]
      ,[tasa_5]
      ,[tasa_6]
      ,[tasa_7]
      ,[oferta_max]
      ,[id_plazo_banco]
      ,[CAPACIDAD_PAGO_MEN]
      ,[id_campana_grupo_banco]
      ,[id_color_banco]
      ,[id_usuario_banco]
      ,[id_rango_deuda]
      ,[num_entidades]
      ,[frescura]
      ,[reenganche]
      ,[tasas_especiales]
      ,[PATERNO]
      ,[NOMBRES]
      ,[MATERNO]
      ,[Numero1]
      ,[Numero2]
      ,[Numero3]
      ,[Numero4]
      ,[Numero5]
      ,[id_user_v3]
      ,[deuda_entidades]
      ,[perfil_ro]
      ,[PRIORIDAD_SISTEMA]
      ,[AUTORIZACION_DATOS]
      ,[mgneg]
      ,[MARCA_PD], fecha_subida
)
SELECT
    [dni]
      ,[tasa_1]
      ,[tasa_2]
      ,[tasa_3]
      ,[tasa_4]
      ,[tasa_5]
      ,[tasa_6]
      ,[tasa_7]
      ,[oferta_max]
      ,[id_plazo_banco]
      ,[CAPACIDAD_PAGO_MEN]
      ,[id_campana_grupo_banco]
      ,[id_color_banco]
      ,[id_usuario_banco]
      ,[id_rango_deuda]
      ,[num_entidades]
      ,[frescura]
      ,[reenganche]
      ,[tasas_especiales]
      ,[PATERNO]
      ,[NOMBRES]
      ,[MATERNO]
      ,[Numero1]
      ,[Numero2]
      ,[Numero3]
      ,[Numero4]
      ,[Numero5]
      ,[id_user_v3]
      ,[deuda_entidades]
      ,[perfil_ro]
      ,[PRIORIDAD_SISTEMA]
      ,[AUTORIZACION_DATOS]
      ,[mgneg]
      ,[MARCA_PD], CAST('2025-06-01' AS DATE) AS fecha_subida
FROM base_clientes_banco
WHERE CAST(fecha_subida AS DATE) = '2025-05-23';








update [CORE_ALFIN].[dbo].[detalle_base]
set fecha_carga = '2025-05-27'
where CAST(fecha_carga AS DATE) = '2025-06-01'

update [CORE_ALFIN].[dbo].[detalle_base]
set fecha_carga = '2025-05-28'
where CAST(fecha_carga AS DATE) = '2025-06-02'



UPDATE base_clientes_banco
SET fecha_subida = '2025-05-23'
WHERE CAST(fecha_subida AS DATE) = '2025-06-02';


SELECT * FROM derivaciones_asesores ORDER BY fecha_derivacion DESC;


SELECT * FROM usuarios WHERE dni IN (07979277
,44981021
,09962759
,09493623
,15607669
)


SELECT * FROM Asesores_ocultos


INSERT INTO Asesores_ocultos (DNI_VICIDIAL, NOMBRE_REAL_ASESOR, DNI_AL_BANCO, NOMBRE_CAMBIO)
VALUES
('75780432', 'BRIGGITTE DANIELLA ROJAS JACINTO', '07979277', 'DANIEL ALEJANDRO ESCALONA ALVAREZ'),
('46496384', 'BEATRIZ ESTHER AGREDA CARHUAS', '44981021', 'JULIANA YANET CORONADO GONZALEZ'),
('42899476', 'MIRIAM JIMENEZ SANTOS', '09962759', 'MARCO MACO ALVAREZ'),
('09824726', 'JAKELYNE LINO VILLACORTA', '09493623', 'TOMAS MONTESINOS GARAY'),
('76091066', 'LESLIE ALICE MACO JIMENEZ', '15607669', 'VILMA SANTOS GARAY')
