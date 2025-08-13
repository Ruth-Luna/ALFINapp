SELECT 
    dni
FROM [CORE_ALFIN].[dbo].[base_clientes_banco]
WHERE CAST(fecha_subida AS DATE) = '2025-08-12'
AND (PATERNO IS NULL OR NOMBRES IS NULL OR MATERNO IS NULL)

UPDATE bcb_destino
SET 
    bcb_destino.PATERNO = ISNULL(bcb_destino.PATERNO, bcb_origen.PATERNO),
    bcb_destino.NOMBRES = ISNULL(bcb_destino.NOMBRES, bcb_origen.NOMBRES),
    bcb_destino.MATERNO = ISNULL(bcb_destino.MATERNO, bcb_origen.MATERNO),
    bcb_destino.Numero1 = ISNULL(bcb_destino.Numero1, bcb_origen.Numero1),
    bcb_destino.Numero2 = ISNULL(bcb_destino.Numero2, bcb_origen.Numero2),
    bcb_destino.Numero3 = ISNULL(bcb_destino.Numero3, bcb_origen.Numero3),
    bcb_destino.Numero4 = ISNULL(bcb_destino.Numero4, bcb_origen.Numero4),
    bcb_destino.Numero5 = ISNULL(bcb_destino.Numero5, bcb_origen.Numero5)
FROM base_clientes_banco bcb_destino
INNER JOIN base_clientes_banco bcb_origen
    ON bcb_destino.dni = bcb_origen.dni
WHERE 
    CAST(bcb_destino.fecha_subida AS DATE) = '2025-08-12'
    AND (
        (bcb_destino.NOMBRES IS NULL AND bcb_origen.NOMBRES IS NOT NULL) OR
        (bcb_destino.MATERNO IS NULL AND bcb_origen.MATERNO IS NOT NULL) OR
        (bcb_destino.PATERNO IS NULL AND bcb_origen.PATERNO IS NOT NULL)
    )



WITH bcb_origen AS (
    SELECT 
        dni,
        x_appaterno as PATERNO,
        x_apmaterno as MATERNO,
        x_nombre as NOMBRES,
        ce.telefono_1 as Numero1,
        ce.telefono_2 as Numero2,
        ce.telefono_3 as Numero3,
        ce.telefono_4 as Numero4,
        ce.telefono_5 as Numero5
    FROM base_clientes bc
    JOIN clientes_enriquecidos ce ON bc.id_base = ce.id_base
)
UPDATE bcb_destino
SET 
    bcb_destino.PATERNO = ISNULL(bcb_destino.PATERNO, bcb_origen.PATERNO),
    bcb_destino.NOMBRES = ISNULL(bcb_destino.NOMBRES, bcb_origen.NOMBRES),
    bcb_destino.MATERNO = ISNULL(bcb_destino.MATERNO, bcb_origen.MATERNO),
    bcb_destino.Numero1 = ISNULL(bcb_destino.Numero1, bcb_origen.Numero1),
    bcb_destino.Numero2 = ISNULL(bcb_destino.Numero2, bcb_origen.Numero2),
    bcb_destino.Numero3 = ISNULL(bcb_destino.Numero3, bcb_origen.Numero3),
    bcb_destino.Numero4 = ISNULL(bcb_destino.Numero4, bcb_origen.Numero4),
    bcb_destino.Numero5 = ISNULL(bcb_destino.Numero5, bcb_origen.Numero5)
FROM base_clientes_banco bcb_destino
INNER JOIN bcb_origen ON bcb_destino.dni = bcb_origen.dni
WHERE 
    CAST(bcb_destino.fecha_subida AS DATE) = '2025-08-12'
    AND (
        (bcb_destino.NOMBRES IS NULL AND bcb_origen.NOMBRES IS NOT NULL) OR
        (bcb_destino.MATERNO IS NULL AND bcb_origen.MATERNO IS NOT NULL) OR
        (bcb_destino.PATERNO IS NULL AND bcb_origen.PATERNO IS NOT NULL)
    );

select top 150 * from clientes_enriquecidos

select COUNT(*) from base_clientes_banco WHERE CAST(fecha_subida as date) = '2025-08-12'
AND (PATERNO IS NULL OR NOMBRES IS NULL OR MATERNO IS NULL)

select dni from base_clientes_banco WHERE CAST(fecha_subida as date) = '2025-08-12'
AND (PATERNO IS NULL OR NOMBRES IS NULL OR MATERNO IS NULL)


UPDATE base_clientes_banco
SET
    dni = RIGHT('00000000' + dni, 8)
WHERE LEN(dni) < 8
AND CAST(fecha_subida AS DATE) = '2025-08-12';

INSERT INTO base_clientes_banco (
    dni,
    tasa_1,
    tasa_2,
    tasa_3,
    tasa_4,
    tasa_5,
    tasa_6,
    tasa_7,
    oferta_max,
    id_plazo_banco,
    CAPACIDAD_PAGO_MEN,
    id_campana_grupo_banco,
    id_color_banco,
    id_usuario_banco,
    id_rango_deuda,
    num_entidades,
    frescura,
    tasas_especiales,
    fecha_subida,
    id_user_v3,
    deuda_entidades,
    perfil_ro,
    AUTORIZACION_DATOS,
    mgneg,
    MARCA_PD
)
SELECT
    dni,
    tasa_1,
    tasa_2,
    tasa_3,
    tasa_4,
    tasa_5,
    tasa_6,
    tasa_7,
    oferta_max,
    plazo,                    -- ← Mapeado a id_plazo_banco
    CAPACIDAD_PAGO_MEN,
    campaña_grupo,            -- ← Mapeado a id_campana_grupo_banco
    color_id,                 -- ← Mapeado a id_color_banco
    usuario,                  -- ← Mapeado a id_usuario_banco
    RANGO_DEUDA,              -- ← Mapeado a id_rango_deuda
    NumEntidades,
    fresco,                   -- ← Mapeado a frescura
    NULL,                     -- ← tasas_especiales: no existe en origen
    GETDATE(),                -- ← fecha_subida: valor actual
    user_v3,                  -- ← Mapeado a id_user_v3
    deuda_entidades,
    perfil_ro,
    AUTORIZACION_DATOS,
    Mgneg,
    MARCA_PD
FROM [dbo].[CAMPANASJULIO18];






UPDATE base_clientes_banco
SET 
    PATERNO = ISNULL(base_clientes_banco.PATERNO, ENRIQUECERAGOSTO.PATERNO),
    MATERNO = ISNULL(base_clientes_banco.MATERNO, ENRIQUECERAGOSTO.MATERNO),
    NOMBRES = ISNULL(base_clientes_banco.NOMBRES, ENRIQUECERAGOSTO.NOMBRES),
    
    Numero1 = ISNULL(base_clientes_banco.Numero1, ENRIQUECERAGOSTO.CELULAR_1),
    Numero2 = ISNULL(base_clientes_banco.Numero2, ENRIQUECERAGOSTO.CELULAR_2),
    Numero3 = ISNULL(base_clientes_banco.Numero3, ENRIQUECERAGOSTO.CELULAR_3),
    Numero4 = ISNULL(base_clientes_banco.Numero4, ENRIQUECERAGOSTO.CELULAR_4),
    Numero5 = ISNULL(base_clientes_banco.Numero5, ENRIQUECERAGOSTO.CELULAR_5)

FROM base_clientes_banco
JOIN ENRIQUECERAGOSTO ON base_clientes_banco.dni = ENRIQUECERAGOSTO.dni;

SELECT top 150 * FROM base_clientes order by id_base DESC

UPDATE clientes_enriquecidos
SET 
    telefono_1 = ISNULL(clientes_enriquecidos.telefono_1, ENRIQUECERAGOSTO.CELULAR_1),
    telefono_2 = ISNULL(clientes_enriquecidos.telefono_2, ENRIQUECERAGOSTO.CELULAR_2),
    telefono_3 = ISNULL(clientes_enriquecidos.telefono_3, ENRIQUECERAGOSTO.CELULAR_3),
    telefono_4 = ISNULL(clientes_enriquecidos.telefono_4, ENRIQUECERAGOSTO.CELULAR_4),
    telefono_5 = ISNULL(clientes_enriquecidos.telefono_5, ENRIQUECERAGOSTO.CELULAR_5)
FROM clientes_enriquecidos
join base_clientes ON clientes_enriquecidos.id_base = base_clientes.id_base
JOIN ENRIQUECERAGOSTO ON base_clientes.dni = ENRIQUECERAGOSTO.dni;



select top 150 * from base_clientes_banco WHERE CAST(fecha_subida AS DATE) = CAST(GETDATE() AS DATE)



SELECT COUNT(*) from ENRIQUECERAGOSTO


SELECT COUNT(*) FROM detalle_base WHERE fecha_carga = '2025-08-12' 