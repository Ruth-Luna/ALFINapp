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