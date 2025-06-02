SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trg_gestion_detalles_re_after_insert_update]
ON [dbo].[derivaciones_asesores]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TempCampana TABLE (
        dni NVARCHAR(20),
        cod_campana NVARCHAR(35)
    );

    DECLARE @TempTelefono TABLE (
        dni NVARCHAR(20),
        origen_telefono NVARCHAR(10)
    );

    WITH CTE_Telefono_T1 AS
    (
        SELECT
            bc.dni as dni,
            'T1' AS origen_telefono
        FROM
            base_clientes bc
        JOIN 
            clientes_enriquecidos ce ON bc.id_base = ce.id_base
        WHERE
            bc.dni IN (SELECT dni_cliente FROM inserted)
            AND ce.telefono_1 IN (SELECT telefono_cliente FROM inserted)
    ), CTE_Telefono_T2 AS
    (
        SELECT
            bc.dni as dni,
            'T2' AS origen_telefono
        FROM
            base_clientes bc
        JOIN 
            clientes_enriquecidos ce ON bc.id_base = ce.id_base
        WHERE
            bc.dni IN (SELECT dni_cliente FROM inserted)
            AND ce.telefono_2 IN (SELECT telefono_cliente FROM inserted)
    ), CTE_Telefono_T3 AS
    (
        SELECT
            bc.dni as dni,
            'T3' AS origen_telefono
        FROM
            base_clientes bc
        JOIN 
            clientes_enriquecidos ce ON bc.id_base = ce.id_base
        WHERE
            bc.dni IN (SELECT dni_cliente FROM inserted)
            AND ce.telefono_3 IN (SELECT telefono_cliente FROM inserted)
    ), CTE_Telefono_T4 AS
    (
        SELECT
            bc.dni as dni,
            'T4' AS origen_telefono
        FROM
            base_clientes bc
        JOIN 
            clientes_enriquecidos ce ON bc.id_base = ce.id_base
        WHERE
            bc.dni IN (SELECT dni_cliente FROM inserted)
            AND ce.telefono_4 IN (SELECT telefono_cliente FROM inserted)
    ), CTE_Telefono_T5 AS
    (
        SELECT
            bc.dni as dni,
            'T5' AS origen_telefono
        FROM
            base_clientes bc
        JOIN 
            clientes_enriquecidos ce ON bc.id_base = ce.id_base
        WHERE
            bc.dni IN (SELECT dni_cliente FROM inserted)
            AND ce.telefono_5 IN (SELECT telefono_cliente FROM inserted)
    ), CTE_Telefono_TA AS
    (
        SELECT
            bc.dni as dni,
            'TA' AS origen_telefono
        FROM
            base_clientes bc
        JOIN 
            clientes_enriquecidos ce ON bc.id_base = ce.id_base
        JOIN telefonos_agregados ta ON ce.id_cliente = ta.id_cliente
        WHERE
            bc.dni IN (SELECT dni_cliente FROM inserted)
            AND ta.telefono IN (SELECT telefono_cliente FROM inserted)
    )

    INSERT INTO @TempTelefono (dni, origen_telefono)
    SELECT dni, origen_telefono FROM CTE_Telefono_T1
    UNION ALL
    SELECT dni, origen_telefono FROM CTE_Telefono_T2
    UNION ALL
    SELECT dni, origen_telefono FROM CTE_Telefono_T3
    UNION ALL
    SELECT dni, origen_telefono FROM CTE_Telefono_T4
    UNION ALL
    SELECT dni, origen_telefono FROM CTE_Telefono_T5
    UNION ALL
    SELECT dni, origen_telefono FROM CTE_Telefono_TA;

    WITH CTE_Campana_Ordenada AS (
        SELECT
            bc.dni,
            db.campaña AS cod_campana,
            ROW_NUMBER() OVER (PARTITION BY bc.dni ORDER BY db.fecha_carga DESC) AS rn
        FROM
            base_clientes bc
        JOIN detalle_base db ON bc.id_base = db.id_base
        WHERE
            bc.dni IN (SELECT dni_cliente FROM inserted)
            AND db.campaña IS NOT NULL
    )

    INSERT INTO @TempCampana (dni, cod_campana)
    SELECT dni, cod_campana FROM CTE_Campana_Ordenada
    WHERE cod_campana IS NOT NULL;

    INSERT INTO GESTION_DETALLE
    (
        [id_asignacion],
        cod_canal,
        canal,
        doc_cliente,
        fecha_envio,
        fecha_gestion,
        hora_gestion,
        telefono,
        origen_telefono,
        cod_campaña,
        cod_tip,
        oferta,
        doc_asesor,
        origen,
        archivo_origen,
        fecha_carga,
        id_derivacion
    )
    SELECT
        ca.id_asignacion AS id_asignacion,
        'SYSTEMA365' AS cod_canal,
        'A365' AS canal,
        i.dni_cliente AS doc_cliente,
        GETDATE() AS fecha_envio,
        GETDATE() AS fecha_gestion,
        CAST(GETDATE() AS TIME) AS hora_gestion,
        i.telefono_cliente AS telefono,
        ISNULL(tt.origen_telefono, 'ND') AS origen_telefono,
        ISNULL(tc.cod_campana, 'ND') AS cod_campaña,
        2 as cod_tip,
        i.oferta_max as oferta,
        i.dni_asesor as doc_asesor,
        'nuevo' as origen,
        'BD PROPIA' as archivo_origen,
        GETDATE() as fecha_carga,
        i.id_derivacion as id_derivacion
    FROM inserted i
    LEFT JOIN @TempCampana tc ON i.dni_cliente = tc.dni
    LEFT JOIN @TempTelefono tt ON i.dni_cliente = tt.dni
    LEFT JOIN clientes_asignados ca ON i.id_cliente = ca.id_cliente
    where
    NOT EXISTS (
            SELECT 1 FROM GESTION_DETALLE gd
            WHERE gd.id_derivacion = i.id_derivacion
        );
END;

DROP TRIGGER [dbo].[trg_gestion_detalles_re_after_insert_update]

ALTER TABLE [dbo].[derivaciones_asesores] DISABLE TRIGGER [trg_gestion_detalles_re_after_insert_update]
ALTER TABLE [dbo].[derivaciones_asesores] ENABLE TRIGGER [trg_gestion_detalles_re_after_insert_update]

SELECT TOP 120 * FROM derivaciones_asesores 
ORDER BY id_derivacion DESC;

SELECT * FROM GESTION_DETALLE 
WHERE id_derivacion = 14380;

SELECT top 150 * FROM GESTION_DETALLE 
order by id_feedback desc;


INSERT INTO [dbo].[derivaciones_asesores] 
    (dni_asesor, dni_cliente, fue_procesado, fue_enviado_email, fue_reagendado)
VALUES 
    ('12354658', '12134567', 1, 1, 0)


DELETE FROM [dbo].[derivaciones_asesores] 
WHERE id_derivacion = 14380;


DELETE FROM [dbo].[GESTION_DETALLE] 
WHERE id_derivacion = 14380;


SELECT TOP (1000) [id_derivacion]
      ,[fecha_derivacion]
      ,[dni_asesor]
      ,[dni_cliente]
      ,[id_cliente]
      ,[nombre_cliente]
      ,[telefono_cliente]
      ,[nombre_agencia]
      ,[num_agencia]
      ,[fue_procesado]
      ,[fecha_visita]
      ,[estado_derivacion]
      ,[id_asignacion]
      ,[observacion_derivacion]
      ,[fue_enviado_email]
      ,[ID_DESEMBOLSO]
      ,[doc_supervisor]
      ,[oferta_max]
      ,[supervisor]
      ,[monto_desembolso]
      ,[real_error]
      ,[fue_reagendado]
      ,[fue_reprocesado]
      ,[evaluate]
      ,[fecha_reagendamiento]
  FROM [CORE_ALFIN].[dbo].[derivaciones_asesores]
order by id_derivacion desc;


SELECT top 120 * FROM clientes_asignados