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
    CAST(bcb_destino.fecha_subida AS DATE) = '2025-05-14'
    AND (
        (bcb_destino.NOMBRES IS NULL AND bcb_origen.NOMBRES IS NOT NULL) OR
        (bcb_destino.MATERNO IS NULL AND bcb_origen.MATERNO IS NOT NULL) OR
        (bcb_destino.PATERNO IS NULL AND bcb_origen.PATERNO IS NOT NULL)
    )






SELECT * FROM base_clientes WHERE dni = '41043657'