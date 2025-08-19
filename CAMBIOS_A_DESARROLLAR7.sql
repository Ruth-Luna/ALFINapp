SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_REPORTES_DERIVACIONES_ASESOR]
    @DniAsesor NVARCHAR(50),
    @mes INT = NULL,
    @anio INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @mes IS NULL
    BEGIN
        SET @mes = MONTH(GETDATE());
    END
    IF @anio IS NULL
    BEGIN
        SET @anio = YEAR(GETDATE());
    END
    SELECT 
        COUNT(*) AS NumeroEntero
    from derivaciones_asesores da 
    WHERE
        da.dni_asesor = @DniAsesor
        AND YEAR(da.fecha_derivacion) = @anio
        AND MONTH(da.fecha_derivacion) = @mes
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_REPORTES_ASIGNACIONES_ASESOR]
(
    @DniAsesor NVARCHAR(50),
    @mes INT = NULL,
    @anio INT = NULL
)
    
AS
BEGIN
    SET NOCOUNT ON;

    IF @mes IS NULL AND @anio IS NULL
    BEGIN
        SET @mes = MONTH(GETDATE());
        SET @anio = YEAR(GETDATE());
    END
    
    ;WITH idAsesor AS (
        SELECT TOP (1) usuarios.id_usuario AS id_usuario
        FROM usuarios
        WHERE dni = @DniAsesor
    )

    SELECT 
        COUNT(*) as NumeroEntero
    FROM clientes_asignados ca
    WHERE ca.id_usuarioV = (SELECT id_usuario FROM idAsesor)
    AND YEAR(ca.fecha_asignacion_sup) = @anio
    AND MONTH(ca.fecha_asignacion_sup) = @mes
END;
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_REPORTES_GESTION_ASESOR]
(
    @DniAsesor NVARCHAR(50),
    @mes INT = NULL,
    @anio INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @mes IS NULL OR @anio IS NULL
    BEGIN
        SET @mes = MONTH(GETDATE());
        SET @anio = YEAR(GETDATE());
    END

    SELECT 
        COUNT(*) AS NumeroEntero
    FROM GESTION_DETALLE gd
    WHERE gd.doc_asesor = @DniAsesor
    AND YEAR(gd.fecha_gestion) = @anio
    AND MONTH(gd.fecha_gestion) = @mes;
END
GO

sp_help GESTION_DETALLE

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_REPORTES_DESEMBOLSOS_ASESOR]
(
    @DniAsesor NVARCHAR(50),
    @mes INT = NULL,
    @anio INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the month and year are provided
    IF @mes IS NULL OR @anio IS NULL
    BEGIN
        SET @mes = MONTH(GETDATE());
        SET @anio = YEAR(GETDATE());
    END
    
    SELECT 
        COUNT(*) AS NumeroEntero
    FROM desembolsos d 
    WHERE d.DNI_ASESOR_DERIVACION = @DniAsesor
    AND YEAR(d.FECHA_DESEMBOLSOS) = @anio
    AND MONTH(d.FECHA_DESEMBOLSOS) = @mes;
END;
GO







CREATE PROCEDURE SP_REPORTES_ASESOR_TIPIFICACIONES_TOP
    @Dni NVARCHAR(50),
    @Mes INT,
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        gd.cod_tip AS IdTipificacion,
        ISNULL(t.descripcion_tipificacion, '') AS DescripcionTipificaciones,
        COUNT(*) AS ContadorTipificaciones
    FROM GESTION_DETALLE gd
    LEFT JOIN tipificaciones t
        ON gd.cod_tip = t.id_tipificacion
    WHERE gd.doc_asesor = @Dni
      AND MONTH(gd.fecha_gestion) = @Mes
      AND YEAR(gd.fecha_gestion) = @Anio
    GROUP BY gd.cod_tip, t.descripcion_tipificacion;
END;
GO



exec SP_ASIGNACION_CRUCE_DNIS @Page = 1


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_REPORTES_SUPERVISOR_GESTION_ULTIMA]
(
    @DniSupervisor NVARCHAR(50),
    @mes INT = NULL,
    @anio INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @idSupervisor INT;

    IF @mes IS NULL AND @anio IS NULL
    BEGIN
        SET @mes = MONTH(GETDATE());
        SET @anio = YEAR(GETDATE());
    END;

    -- Get the supervisor's ID based on the provided DNI
    SELECT TOP 1 @idSupervisor = u.id_usuario
    FROM usuarios u
    WHERE u.dni = @DniSupervisor
      AND u.dni <> '000673385'; -- Exclude the specific DNI

    ;WITH 
    Gestiones AS (
        SELECT 
            gd.doc_cliente,
            gd.doc_asesor,
            gd.fecha_gestion,
            gd.cod_tip,
            gd.id_feedback,
            ROW_NUMBER() OVER (
                PARTITION BY gd.doc_cliente, gd.doc_asesor
                ORDER BY 
                    CASE WHEN gd.cod_tip = 2 THEN 1 ELSE 0 END DESC,
                    gd.fecha_gestion DESC
            ) AS rn_asesor
        FROM GESTION_DETALLE gd
        WHERE gd.id_supervisor = @idSupervisor
          AND YEAR(gd.fecha_gestion) = @anio
          AND MONTH(gd.fecha_gestion) = @mes
    ),
    GestionCliente AS (
        SELECT 
            g.doc_cliente,
            g.doc_asesor,
            g.fecha_gestion,
            g.cod_tip,
            ROW_NUMBER() OVER (
                PARTITION BY g.doc_cliente
                ORDER BY g.fecha_gestion DESC
            ) AS rn_cliente
        FROM Gestiones g
        WHERE g.rn_asesor = 1
    )
    SELECT 
        gc.doc_cliente,
        gc.doc_asesor,
        gc.fecha_gestion,
        gc.cod_tip
    FROM GestionCliente gc
    WHERE gc.rn_cliente = 1
    ORDER BY gc.doc_asesor;
END;
GO





GO
ALTER PROCEDURE [dbo].[SP_REPORTES_SUPERVISOR_ASESORES_DEL_SUPERVISOR]
(
    @idSupervisor NVARCHAR(50),
    @mes INT = NULL,
    @anio INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @mes IS NULL OR @anio IS NULL
    BEGIN
        SET @mes = MONTH(GETDATE());
        SET @anio = YEAR(GETDATE());
    END;

    ;WITH datos_asesores AS (
        SELECT 
            u.id_usuario,
            u.dni,
            u.nombres_completos
        FROM usuarios u
        WHERE u.ID_USUARIO_SUP = @idSupervisor
    ), 
    ultima_gestion AS (
        SELECT 
            t.doc_cliente,
            t.doc_asesor,
            t.fecha_gestion,
            t.cod_tip,
            t.id_supervisor,
            t.rn
        FROM (
            SELECT 
                gd.doc_cliente,
                gd.doc_asesor,
                gd.fecha_gestion,
                gd.cod_tip,
                gd.id_supervisor,
                ROW_NUMBER() OVER (
                    PARTITION BY gd.doc_cliente 
                    ORDER BY gd.fecha_gestion DESC
                ) AS rn
            FROM GESTION_DETALLE gd
            WHERE
                YEAR(gd.fecha_gestion) = @anio
            AND MONTH(gd.fecha_gestion) = @mes
            AND gd.doc_asesor IN (SELECT dni FROM datos_asesores)
        ) t
        WHERE t.rn = 1   -- solo la última gestión por cliente
    ),
    datos_gestion AS (
        SELECT 
            ug.doc_asesor,
            COUNT(*) AS TOTAL_GESTIONES
        FROM ultima_gestion ug
        WHERE ug.doc_asesor IN (SELECT dni FROM datos_asesores)
        GROUP BY ug.doc_asesor
    ), 
    datos_desembolsos AS (
        SELECT 
            d.DNI_ASESOR_DERIVACION,
            COUNT(*) AS TOTAL_DESEMBOLSOS
        FROM desembolsos d
        WHERE d.DNI_ASESOR_DERIVACION IN (
            SELECT dni 
            FROM datos_asesores 
        )
          AND YEAR(d.FECHA_DESEMBOLSOS) = @anio
          AND MONTH(d.FECHA_DESEMBOLSOS) = @mes
        GROUP BY d.DNI_ASESOR_DERIVACION
    ), 
    datos_derivaciones AS (
        SELECT 
            da.dni_asesor,
            COUNT(*) AS TOTAL_DERIVACIONES
        FROM derivaciones_asesores da
        WHERE da.dni_asesor IN (
            SELECT dni 
            FROM datos_asesores 
        )
          AND YEAR(da.fecha_derivacion) = @anio
          AND MONTH(da.fecha_derivacion) = @mes
        GROUP BY da.dni_asesor
    ),
    datos_asignaciones AS (
        SELECT 
            ca.id_usuarioV,
            COUNT(*) AS TOTAL_ASIGNACIONES
        FROM clientes_asignados ca
        WHERE ca.id_usuarioV IN (
            SELECT id_usuario 
            FROM datos_asesores 
        )
          AND YEAR(ca.fecha_asignacion_sup) = @anio
          AND MONTH(ca.fecha_asignacion_sup) = @mes
        GROUP BY ca.id_usuarioV
    )
    SELECT 
        da.dni AS DniAsesor,
        da.nombres_completos AS NombreAsesor,
        ISNULL(dg.TOTAL_GESTIONES, 0) AS TotalGestiones,
        ISNULL(dd.TOTAL_DESEMBOLSOS, 0) AS TotalDesembolsos,
        ISNULL(ddr.TOTAL_DERIVACIONES, 0) AS TotalDerivaciones,
        ISNULL(da2.TOTAL_ASIGNACIONES, 0) AS TotalAsignaciones
    FROM datos_asesores da
    LEFT JOIN datos_gestion dg ON da.dni = dg.doc_asesor
    LEFT JOIN datos_desembolsos dd ON da.dni = dd.DNI_ASESOR_DERIVACION
    LEFT JOIN datos_derivaciones ddr ON da.dni = ddr.dni_asesor
    LEFT JOIN datos_asignaciones da2 ON da.id_usuario = da2.id_usuarioV
    ORDER BY da2.TOTAL_ASIGNACIONES DESC;
END;
GO


EXEC SP_REPORTES_SUPERVISOR_ASESORES_DEL_SUPERVISOR
    @idSupervisor = 4176,
    @mes = 10,
    @anio = 2023;
select * from usuarios WHERE ID_USUARIO_SUP = 4176