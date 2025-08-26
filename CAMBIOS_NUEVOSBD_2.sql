SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_Derivacion_consulta_derivaciones_x_asesor_por_dni_con_reagendacion]
    @Dni DniTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @fecha_disponible DATE,
    @fecha_disponible2 DATE,
    @fecha_reagendacion1 DATE,
    @fecha_reagendacion2 DATE;

    SELECT @fecha_disponible = FECHA
    FROM (
        SELECT FECHA,
            ROW_NUMBER() OVER (ORDER BY FECHA DESC) AS rn
        FROM dbo.CALENDARIO
        WHERE FECHA <= CAST(GETDATE() AS DATE)
            AND ISNULL(FERIADO, 0) = 0
            AND NOM_DIA_SEMANA <> 'DOMINGO'
    ) AS dias_habiles
    WHERE dias_habiles.rn = 5;
    SELECT @fecha_disponible2 = FECHA
    FROM (
        SELECT FECHA,
            ROW_NUMBER() OVER (ORDER BY FECHA DESC) AS rn
        FROM dbo.CALENDARIO
        WHERE FECHA <= CAST(GETDATE() AS DATE)
            AND ISNULL(FERIADO, 0) = 0
            AND NOM_DIA_SEMANA <> 'DOMINGO'
    ) AS dias_habiles
    WHERE dias_habiles.rn = 6;

    SELECT @fecha_reagendacion1 = FECHA
    FROM (
        SELECT FECHA,
            ROW_NUMBER() OVER (ORDER BY FECHA DESC) AS rn
        FROM dbo.CALENDARIO
        WHERE FECHA <= CAST(GETDATE() AS DATE)
            AND ISNULL(FERIADO, 0) = 0
            AND NOM_DIA_SEMANA <> 'DOMINGO'
    ) AS dias_habiles
    WHERE dias_habiles.rn = 2;
    SELECT @fecha_reagendacion2 = FECHA
    FROM (
        SELECT FECHA,
            ROW_NUMBER() OVER (ORDER BY FECHA DESC) AS rn
        FROM dbo.CALENDARIO
        WHERE FECHA <= CAST(GETDATE() AS DATE)
            AND ISNULL(FERIADO, 0) = 0
            AND NOM_DIA_SEMANA <> 'DOMINGO'
    ) AS dias_habiles
    WHERE dias_habiles.rn = 3;

    ;WITH
        DER
        AS
        (
            SELECT
                d.fecha_reagendamiento AS FechaVisitaNN,
                d.*,
                CASE
                    WHEN de.id_desembolsos IS NOT NULL THEN (cast(1 as bit))
                    ELSE (cast(0 as bit))
                END AS fue_desembolsado,
                de.MONTO_FINANCIADO AS monto_desembolso_financiado,
                de.FECHA_DESEMBOLSOS AS fecha_desembolsos,
                de.doc_asesor as doc_asesor_desembolso,
                de.doc_supervisor as doc_supervisor_desembolso
            FROM derivaciones_asesores d
            LEFT JOIN desembolsos de ON d.dni_cliente = de.dni_desembolso
            AND YEAR(d.fecha_derivacion) = YEAR(de.FECHA_DESEMBOLSOS)
            AND MONTH(d.fecha_derivacion) = MONTH(de.FECHA_DESEMBOLSOS)
            WHERE
        YEAR(d.fecha_derivacion) = YEAR(GETDATE())
                AND MONTH(d.fecha_derivacion) = MONTH(GETDATE())
                AND d.dni_asesor IN (SELECT dni
                FROM @Dni) AND fue_reprocesado = 0
        ),
        FINAL
        AS
        (
            SELECT
                *,
                CASE
            WHEN (
                    (fue_reprocesado = 1 AND
                    (CAST(FechaVisitaNN AS date) <= @fecha_reagendacion2))
                    OR
                    (CAST(FechaVisitaNN AS date) <= @fecha_disponible)
                 )
            THEN 0
            ELSE 1
                END AS PuedeSerReagendado
            FROM DER
        )
    SELECT
        id_derivacion,
        fecha_derivacion,
        dni_asesor,
        dni_cliente,
        id_cliente,
        nombre_cliente,
        telefono_cliente,
        nombre_agencia,
        num_agencia,
        fue_procesado,
        fecha_visita,
        estado_derivacion,
        id_asignacion,
        observacion_derivacion,
        fue_enviado_email, ID_DESEMBOLSO,
        doc_supervisor,
        oferta_max,
        supervisor,
        monto_desembolso,
        real_error,
        fue_reagendado,
        fue_reprocesado,
        cast(PuedeSerReagendado as bit) as PuedeSerReagendado,
        fecha_evidencia,
        hay_evidencias,
        fue_desembolsado,
        fecha_desembolsos,
        doc_asesor_desembolso,
        doc_supervisor_desembolso,
        monto_desembolso_financiado
    FROM FINAL
    WHERE
        YEAR(fecha_derivacion) = YEAR(GETDATE())
        AND MONTH(fecha_derivacion) = MONTH(GETDATE())
        AND dni_asesor IN (SELECT dni
        FROM @Dni)
END;
GO



SELECT top 150  * FROM derivaciones_asesores
order by id_derivacion desc