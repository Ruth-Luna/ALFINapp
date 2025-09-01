SELECT TOP (1000) [id_desembolsos]
      ,[dni_desembolso]
      ,[CUENTA_BT]
      ,[N_OPER]
      ,[SUCURSAL]
      ,[MONTO_FINANCIADO]
      ,[FECHA_SOL]
      ,[FECHA_DESEMBOLSOS]
      ,[FECHA_GEST]
      ,[CANAL]
      ,[TIPO_DESEM]
      ,[fecha_proporcion]
      ,[Observacion]
      ,[id_NombreBase]
      ,[doc_asesor]
      ,[doc_supervisor]
      ,[nombres_completos_supervisor]
      ,[ID_DERIVACION]
      ,[DNI_ASESOR_DERIVACION]
      ,[DOC_ASESOR_REAL]
  FROM [CORE_ALFIN].[dbo].[desembolsos]
  WHERE (MONTH(fecha_proporcion) IN (6))
    AND YEAR(fecha_proporcion) = 2025
    AND CANAL = 'A365'
order by dni_desembolso


DELETE FROM desembolsos
WHERE (MONTH(fecha_proporcion) IN (6))
  AND YEAR(fecha_proporcion) = 2025
  AND CANAL = 'A365'
  AND dni_desembolso IN (
    SELECT dni_desembolso
    FROM desembolsos
    WHERE (MONTH(fecha_proporcion) IN (5,6))
      AND YEAR(fecha_proporcion) = 2025
      AND CANAL = 'A365'
    GROUP BY dni_desembolso
    HAVING COUNT(*) > 1
  );


SELECT *
FROM desembolsos d
WHERE (MONTH(fecha_proporcion) IN (5,6))
  AND YEAR(fecha_proporcion) = 2025
  AND CANAL = 'A365'
  AND EXISTS (
    SELECT 1
    FROM desembolsos d2
    WHERE d.dni_desembolso = d2.dni_desembolso
      AND (MONTH(d2.fecha_proporcion) IN (5,6))
      AND YEAR(d2.fecha_proporcion) = 2025
      AND d2.CANAL = 'A365'
    GROUP BY d2.dni_desembolso
    HAVING COUNT(*) > 1
  )
  AND d.fecha_proporcion > (
    SELECT MIN(d3.fecha_proporcion)
    FROM desembolsos d3
    WHERE d3.dni_desembolso = d.dni_desembolso
      AND (MONTH(d3.fecha_proporcion) IN (5,6))
      AND YEAR(d3.fecha_proporcion) = 2025
      AND d3.CANAL = 'A365'
  )
ORDER BY d.dni_desembolso, d.fecha_proporcion;


DELETE FROM desembolsos
WHERE id_desembolsos IN (
  SELECT d.id_desembolsos
  FROM desembolsos d
  WHERE (MONTH(d.fecha_proporcion) IN (5,6))
    AND YEAR(d.fecha_proporcion) = 2025
    AND d.CANAL = 'A365'
    AND EXISTS (
      SELECT 1
      FROM desembolsos d2
      WHERE d.dni_desembolso = d2.dni_desembolso
        AND (MONTH(d2.fecha_proporcion) IN (5,6))
        AND YEAR(d2.fecha_proporcion) = 2025
        AND d2.CANAL = 'A365'
      GROUP BY d2.dni_desembolso
      HAVING COUNT(*) > 1
    )
    AND d.fecha_proporcion > (
      SELECT MIN(d3.fecha_proporcion)
      FROM desembolsos d3
      WHERE d3.dni_desembolso = d.dni_desembolso
        AND (MONTH(d3.fecha_proporcion) IN (5,6))
        AND YEAR(d3.fecha_proporcion) = 2025
        AND d3.CANAL = 'A365'
    )
);
