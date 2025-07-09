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
        ca.ultima_tipificacion_general AS UltimaTipificacion,
        ca.tipificacion_mayor_peso AS TipificacionMasRelevante,
        ca.id_lista AS IdLista
    INTO #supervisorData
    FROM clientes_asignados ca
        JOIN clientes_enriquecidos ce ON ce.id_cliente = ca.id_cliente
        JOIN base_clientes bc ON bc.id_base = ce.id_base
        LEFT JOIN usuarios u ON u.id_usuario = ca.id_usuarioV
    WHERE ca.id_usuarioS = @IdUsuario
        AND (ca.destino NOT IN ('A365_AGREGADO_MANUALMENTE', 'ALFIN_AGREGADO_MANUALMENTE') OR ca.destino IS NULL)
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


CREATE PROCEDURE [dbo].[sp_supervisor_get_asignacion_de_leads_refactored]
(
    @IdUsuario INT,
    @Filter NVARCHAR(50) = NULL,
    @ValueFilter NVARCHAR(50) = NULL,
    @PageNumber INT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FechaInicio DATETIME = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0); 
    DECLARE @FechaFin DATETIME = DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, 0);
    DECLARE @Offset INT = (@PageNumber - 1) * 100;
    DECLARE @PageSize INT = 100;
    DECLARE @FilterCondition NVARCHAR(MAX) = '';

    -- Validaciones iniciales
    IF @Filter IS NULL OR @ValueFilter IS NULL
    BEGIN
        RAISERROR('No se proporcionó filtro o valor de filtro.', 16, 1);
        RETURN;
    END

    IF @Filter = 'dni_asesor'
        SET @FilterCondition = ' AND u.dni IN (' + @ValueFilter + ')';
    ELSE IF @Filter = 'lista'
        SET @FilterCondition = ' AND ca.id_lista IN (' + @ValueFilter + ')';
    ELSE IF @Filter = 'destino'
        SET @FilterCondition = ' AND ca.destino IN (' + @ValueFilter + ')';
    ELSE IF @Filter = 'fecha_asignacion'
    BEGIN
        DECLARE @FechaFiltro DATETIME = TRY_CAST(@ValueFilter AS DATETIME);
        IF @FechaFiltro IS NULL
        BEGIN
            RAISERROR('El valor de filtro para fecha no es válido.', 16, 1);
            RETURN;
        END
        SET @FilterCondition = ' AND ca.fecha_asignacion_sup = ''' + CONVERT(NVARCHAR(20), @FechaFiltro, 120) + '''';
    END
    ELSE
    BEGIN
        RAISERROR('Filtro no reconocido: %s', 16, 1, @Filter);
        RETURN;
    END

    DECLARE @SQL NVARCHAR(MAX) = '
    -- Asesores activos del supervisor
    CREATE TABLE #dniAsesores (dni NVARCHAR(50) PRIMARY KEY);
    INSERT INTO #dniAsesores
    SELECT DISTINCT u.dni
    FROM usuarios u
    WHERE ID_USUARIO_SUP = @IdUsuario AND u.estado = ''ACTIVO'';

    -- Desembolsos del mes
    CREATE TABLE #dniDesembolsos (dni_desembolso NVARCHAR(50) PRIMARY KEY);
    INSERT INTO #dniDesembolsos
    SELECT DISTINCT d.dni_desembolso
    FROM desembolsos d
    JOIN #dniAsesores da ON d.dni_desembolso = da.dni
    WHERE d.FECHA_DESEMBOLSOS >= @FechaInicio AND d.FECHA_DESEMBOLSOS < @FechaFin;

    -- Retiros del mes
    CREATE TABLE #dniRetiros (dni_retiros NVARCHAR(50) PRIMARY KEY);
    INSERT INTO #dniRetiros
    SELECT DISTINCT r.dni_retiros
    FROM retiros r
    WHERE r.fecha_retiro >= @FechaInicio AND r.fecha_retiro < @FechaFin;

    -- Datos principales con filtros de exclusión
    SELECT
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
        ca.ultima_tipificacion_general AS UltimaTipificacion,
        ca.tipificacion_mayor_peso AS TipificacionMasRelevante,
        ca.id_lista AS IdLista
    INTO #supervisorData
    FROM clientes_asignados ca
    JOIN clientes_enriquecidos ce ON ce.id_cliente = ca.id_cliente
    JOIN base_clientes bc ON bc.id_base = ce.id_base
    LEFT JOIN usuarios u ON u.id_usuario = ca.id_usuarioV
    WHERE ca.id_usuarioS = @IdUsuario
      AND ca.fecha_asignacion_sup >= @FechaInicio
      AND ca.fecha_asignacion_sup < @FechaFin
      AND (u.dni IS NULL OR u.dni NOT IN (SELECT dni_desembolso FROM #dniDesembolsos))
      AND (u.dni IS NULL OR u.dni NOT IN (SELECT dni_retiros FROM #dniRetiros))
      ' + @FilterCondition + ';

    -- Paginación
    SELECT *
    FROM #supervisorData
    ORDER BY FechaAsignacionV DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    -- Limpieza
    DROP TABLE #dniAsesores;
    DROP TABLE #dniDesembolsos;
    DROP TABLE #dniRetiros;
    DROP TABLE #supervisorData;
    ';

    EXEC sp_executesql @SQL,
        N'@IdUsuario INT, @FechaInicio DATETIME, @FechaFin DATETIME, @Offset INT, @PageSize INT',
        @IdUsuario = @IdUsuario,
        @FechaInicio = @FechaInicio,
        @FechaFin = @FechaFin,
        @Offset = @Offset,
        @PageSize = @PageSize;
END
GO



select top 150 * from desembolsos

SELECT * FROM desem


SELECT top 150 * FROM base_clientes ORDER BY id_base DESC;