SELECT 
    dni
FROM [CORE_ALFIN].[dbo].[base_clientes_banco]
WHERE fecha_subida = '2025-05-14'
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
    CAST(bcb_destino.fecha_subida AS DATE) = '2025-07-01'
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
    CAST(bcb_destino.fecha_subida AS DATE) = '2025-07-01'
    AND (
        (bcb_destino.NOMBRES IS NULL AND bcb_origen.NOMBRES IS NOT NULL) OR
        (bcb_destino.MATERNO IS NULL AND bcb_origen.MATERNO IS NOT NULL) OR
        (bcb_destino.PATERNO IS NULL AND bcb_origen.PATERNO IS NOT NULL)
    );

select top 150 * from clientes_enriquecidos

select dni from base_clientes_banco WHERE CAST(fecha_subida as date) = '2025-07-01'
AND (PATERNO IS NULL OR NOMBRES IS NULL OR MATERNO IS NULL)