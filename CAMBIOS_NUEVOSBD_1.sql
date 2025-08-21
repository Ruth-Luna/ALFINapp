GO 
ALTER PROCEDURE [dbo].[SP_REAGENDAMIENTOS_GET_REAGENDAMIENTOS_VIEW]
(
    @id_usuario INT = NULL,
    @month INT = NULL,
    @year INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @dnis_table TABLE (dni VARCHAR(20));
    IF @id_usuario IS NULL
    BEGIN
        INSERT INTO @dnis_table (dni)
        SELECT dni 
        FROM [CORE_ALFIN].[dbo].[usuarios] 
        WHERE id_rol = 3;
    END
    ELSE
    BEGIN
        DECLARE @id_rol INT;
        SELECT TOP 1 @id_rol = id_rol
        FROM [CORE_ALFIN].[dbo].[usuarios]
        WHERE id_usuario = @id_usuario;
        IF @id_rol IS NULL
        BEGIN
            RAISERROR('El usuario no existe o no tiene un rol asignado.', 16, 1);
            RETURN;
        END
        IF @id_rol = 3
        BEGIN
            INSERT INTO @dnis_table (dni)
            SELECT dni 
            FROM [CORE_ALFIN].[dbo].[usuarios] 
            WHERE id_usuario = @id_usuario;
            -- DEBUGGING WITH LOG TABLE
            INSERT INTO debug_log (mensaje)
            VALUES ('Usuario con id ' + CAST(@id_usuario AS VARCHAR(10)) + ' tiene rol 3, DNI: ' + (SELECT dni FROM [CORE_ALFIN].[dbo].[usuarios] WHERE id_usuario = @id_usuario));
        END
        ELSE IF @id_rol = 2
        BEGIN
            INSERT INTO @dnis_table (dni)
            SELECT dni 
            FROM [CORE_ALFIN].[dbo].[usuarios] 
            WHERE ID_USUARIO_SUP = @id_usuario
                AND id_rol IN (3);
        END
        ELSE IF @id_rol = 1 OR @id_rol = 4
        BEGIN
            INSERT INTO @dnis_table (dni)
            SELECT dni 
            FROM [CORE_ALFIN].[dbo].[usuarios]
            WHERE id_rol IN (3);
        END
        ELSE
        BEGIN
            RAISERROR('El usuario no tiene permisos para acceder a esta información.', 16, 1);
            RETURN;
        END
    END
    
    IF @month IS NULL
    BEGIN
        SET @month = MONTH(GETDATE());
    END
    IF @year IS NULL
    BEGIN
        SET @year = YEAR(GETDATE());
    END

    DECLARE @fecha_reagendacion1 DATE;
    DECLARE @fecha_reagendacion2 DATE;

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

    -- DEBUGGING WITH LOG TABLE
    INSERT INTO debug_log (mensaje)
    VALUES ('Fechas de reagendación: ' + CAST(@fecha_reagendacion1 AS VARCHAR(10)) + ' - ' + CAST(@fecha_reagendacion2 AS VARCHAR(10)));

    ;WITH CTE_REAGENDAMIENTOS AS (
    SELECT 
        ar.*, 
        CASE
            WHEN ar.fecha_agendamiento BETWEEN @fecha_reagendacion1 AND @fecha_reagendacion2 
            THEN 1
            ELSE 0
        END AS [puede_ser_reagendado],
        ROW_NUMBER() OVER (PARTITION BY ar.id_derivacion ORDER BY ar.id_agendamientos_re DESC) AS rn,
        ROW_NUMBER() OVER (PARTITION BY ar.id_derivacion ORDER BY ar.id_agendamientos_re ASC) AS rn_asc
    FROM [CORE_ALFIN].[dbo].[agendamientos_reagendamientos] ar
)

    SELECT 
        R.id_derivacion,
        R.id_agendamientos_re,
        R.oferta,
        R.fecha_visita,
        R.telefono,
        R.agencia,
        R.fecha_agendamiento,
        R.fecha_derivacion,
        R.dni_asesor,
        R.dni_cliente,
        puede_ser_reagendado,
        U.Nombres_Completos AS NOMBRE_ASESOR,
        da.estado_derivacion,
        da.fecha_derivacion AS fecha_derivacion_original,
        da.doc_supervisor,
        R.rn_asc AS numero_reagendamiento
    FROM CTE_REAGENDAMIENTOS R
    LEFT JOIN [CORE_ALFIN].[dbo].[usuarios] U ON R.dni_asesor = U.dni
    LEFT JOIN derivaciones_asesores da ON R.id_derivacion = da.id_derivacion 
        AND YEAR(da.fecha_derivacion) = YEAR(R.fecha_derivacion)
        AND MONTH(da.fecha_derivacion) = MONTH(R.fecha_derivacion)
    WHERE (MONTH(R.fecha_agendamiento) = @month 
        AND YEAR(R.fecha_agendamiento) = @year)
        AND R.dni_asesor IN (SELECT dni FROM @dnis_table)
        AND R.rn = 1
        AND R.rn_asc <> 1
    ORDER BY R.fecha_visita DESC, R.fecha_agendamiento DESC;
END



GO 
CREATE PROCEDURE [dbo].[SP_REAGENDAMIENTOS_GET_REAGENDAMIENTOS_HISTORICO]
(
    @id_derivacion INT
)
AS
BEGIN
    SELECT 
        R.id_derivacion,
        R.id_agendamientos_re,
        R.oferta,
        R.fecha_visita,
        R.telefono,
        R.agencia,
        R.fecha_agendamiento,
        R.fecha_derivacion,
        R.dni_asesor,
        R.dni_cliente,
        COUNT(*) OVER (PARTITION BY R.id_derivacion) AS total_reagendamientos
    FROM agendamientos_reagendamientos R
    LEFT JOIN [CORE_ALFIN].[dbo].[usuarios] U ON R.dni_asesor = U.dni
    LEFT JOIN derivaciones_asesores da ON R.id_derivacion = da.id_derivacion 
        AND YEAR(da.fecha_derivacion) = YEAR(R.fecha_derivacion)
        AND MONTH(da.fecha_derivacion) = MONTH(R.fecha_derivacion)
    WHERE R.id_derivacion = @id_derivacion
    ORDER BY R.fecha_visita DESC, R.fecha_agendamiento DESC;
END


SELECT * FROM debug_log
DELETE FROM debug_log 

EXEC [dbo].[SP_REAGENDAMIENTOS_GET_REAGENDAMIENTOS_VIEW]
    @id_usuario = 2108,
    @month = 8,
    @year = 2025;

    SELECT TOP (1000) [id_agendamientos_re]
      ,[id_derivacion]
      ,[oferta]
      ,[fecha_visita]
      ,[telefono]
      ,[agencia]
      ,[estado_derivacion]
      ,[fue_enviado_email]
      ,[fue_enviado_form]
      ,[dni_supervisor]
      ,[fecha_agendamiento]
      ,[fecha_derivacion]
      ,[dni_asesor]
      ,[dni_cliente]
  FROM [CORE_ALFIN].[dbo].[agendamientos_reagendamientos]


SELECT * FROM directorio_comercial

UPDATE directorio_comercial
SET AGENCIA = 'CAÑETE'
WHERE AGENCIA = 'CAÃ?ETE'


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_supervisor_get_asignacion_de_leads]
    @IdUsuario INT
AS
BEGIN
    -- Definir rangos de fecha para evitar cálculos repetidos
    DECLARE @FechaInicio DATETIME = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0);
    DECLARE @FechaFin DATETIME = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, 0);

    -- Obtener DNI de asesores en una tabla temporal (mejor que una CTE)
    CREATE TABLE #dniAsesores
    (
        dni NVARCHAR(50) PRIMARY KEY
    );
    INSERT INTO #dniAsesores
    SELECT DISTINCT u.dni
    FROM usuarios u
    WHERE ID_USUARIO_SUP = @IdUsuario;

    -- Obtener datos principales
    SELECT
        TOP 500
        ca.id_asignacion AS IdAsignacion,
        ca.id_cliente AS IdCliente,
        ca.id_usuarioV AS idUsuarioV,
        ca.fecha_asignacion_vendedor AS FechaAsignacionV,
        bc.dni AS Dni,
        bc.x_appaterno AS XAppaterno,
        bc.x_apmaterno AS XApmaterno,
        bc.x_nombre AS XNombre,
        u.Nombres_Completos AS NombresCompletos,
        u.dni AS DniVendedor,
        ca.destino AS Destino,
        CASE 
            WHEN ce.ultima_tipificacion_telefono_1 IS NOT NULL THEN ce.ultima_tipificacion_telefono_1
            WHEN ce.ultima_tipificacion_telefono_2 IS NOT NULL THEN ce.ultima_tipificacion_telefono_2
            WHEN ce.ultima_tipificacion_telefono_3 IS NOT NULL THEN ce.ultima_tipificacion_telefono_3
            WHEN ce.ultima_tipificacion_telefono_4 IS NOT NULL THEN ce.ultima_tipificacion_telefono_4
            WHEN ce.ultima_tipificacion_telefono_5 IS NOT NULL THEN ce.ultima_tipificacion_telefono_5
        END AS UltimaTipificacion,
        ca.tipificacion_mayor_peso AS TipificacionMasRelevante,
        ca.id_lista AS IdLista
    INTO #supervisorData
    FROM clientes_asignados ca
        JOIN clientes_enriquecidos ce ON ce.id_cliente = ca.id_cliente
        JOIN base_clientes bc ON bc.id_base = ce.id_base
        LEFT JOIN usuarios u ON u.id_usuario = ca.id_usuarioV
    WHERE ca.id_usuarioS = @IdUsuario
        -- AND (ca.destino NOT IN ('A365_AGREGADO_MANUALMENTE', 'ALFIN_AGREGADO_MANUALMENTE') OR ca.destino IS NULL)
        AND ca.fecha_asignacion_sup >= @FechaInicio
        AND ca.fecha_asignacion_sup < @FechaFin;

    -- Obtener DNIs con desembolsos
    CREATE TABLE #dniDesembolsos
    (
        dni_desembolso NVARCHAR(50) PRIMARY KEY
    );
    INSERT INTO #dniDesembolsos
    SELECT DISTINCT d.dni_desembolso
    FROM desembolsos d
        JOIN #dniAsesores da ON d.dni_desembolso = da.dni
    WHERE d.FECHA_DESEMBOLSOS >= @FechaInicio
        AND d.FECHA_DESEMBOLSOS < @FechaFin;

    -- Obtener DNIs con retiros
    CREATE TABLE #dniRetiros
    (
        dni_retiros NVARCHAR(50) PRIMARY KEY
    );
    INSERT INTO #dniRetiros
    SELECT DISTINCT r.dni_retiros
    FROM retiros r
    WHERE r.fecha_retiro >= @FechaInicio
        AND r.fecha_retiro < @FechaFin;

    -- Obtener la información final con JOINs
    SELECT sd.*, la.nombre_lista as NombreLista, la.fecha_creacion as FechaCreacionLista
    FROM #supervisorData sd
        LEFT JOIN #dniDesembolsos dd ON sd.Dni = dd.dni_desembolso
        LEFT JOIN #dniRetiros dr ON sd.Dni = dr.dni_retiros
        LEFT JOIN listas_asignacion la ON sd.IdLista = la.id_lista
    WHERE dd.dni_desembolso IS NULL
        AND dr.dni_retiros IS NULL;

    -- Eliminar tablas temporales
    DROP TABLE #dniAsesores;
    DROP TABLE #dniDesembolsos;
    DROP TABLE #dniRetiros;
    DROP TABLE #supervisorData;
END;
GO