SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_REPORTES_SUPERVISOR_DETALLADO]
    @id_usuario INT,
    @anio INT = NULL,
    @mes INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DniSupervisor NVARCHAR(20);

    -- Obtener el DNI del supervisor
    SELECT @DniSupervisor = dni
    FROM usuarios
    WHERE id_usuario = @id_usuario;

    -- Si no se pasan mes/año, usar fecha actual
    IF @mes IS NULL AND @anio IS NULL
    BEGIN
        SET @mes = MONTH(GETDATE());
        SET @anio = YEAR(GETDATE());
    END;

    WITH CTE_CLIENTES_ASIGNADOS AS (
        SELECT ca.id_cliente
        FROM clientes_asignados ca
        WHERE ca.id_usuarioS = @id_usuario
          AND YEAR(ca.fecha_asignacion_sup) = @anio
          AND MONTH(ca.fecha_asignacion_sup) = @mes
    ),
    CTE_GESTION AS (
        SELECT id_asignacion, cod_tip
        FROM (
            SELECT 
                gd.id_asignacion,
                gd.cod_tip,
                ROW_NUMBER() OVER (
                    PARTITION BY gd.doc_cliente, gd.doc_asesor
                    ORDER BY gd.fecha_gestion DESC
                ) AS rn
            FROM GESTION_DETALLE gd
            INNER JOIN base_clientes bc ON bc.dni = gd.doc_cliente
            INNER JOIN clientes_enriquecidos ce ON ce.id_base = bc.id_base
            INNER JOIN CTE_CLIENTES_ASIGNADOS ca ON ca.id_cliente = ce.id_cliente
            WHERE YEAR(gd.fecha_gestion) = @anio
              AND MONTH(gd.fecha_gestion) = @mes
        ) AS sub
        WHERE rn = 1
    ),
    CTE_DESEMBOLSOS AS (
        SELECT DISTINCT d.dni_desembolso
        FROM desembolsos d
        WHERE d.doc_supervisor = @DniSupervisor
          AND YEAR(d.FECHA_DESEMBOLSOS) = @anio
          AND MONTH(d.FECHA_DESEMBOLSOS) = @mes
    ),
    CTE_DERIVADOS AS (
        SELECT DISTINCT d.dni_cliente
        FROM derivaciones_asesores d
        WHERE d.doc_supervisor = @DniSupervisor
          AND YEAR(d.fecha_derivacion) = @anio
          AND MONTH(d.fecha_derivacion) = @mes
    )
    SELECT 
        (SELECT COUNT(*) FROM CTE_GESTION) AS totalGestionado,
        (SELECT COUNT(*) FROM CTE_CLIENTES_ASIGNADOS) AS totalAsignaciones,
        (SELECT COUNT(*) 
         FROM CTE_CLIENTES_ASIGNADOS ca
         WHERE NOT EXISTS (
             SELECT 1 FROM CTE_GESTION g WHERE g.id_asignacion IS NOT NULL
         )
        ) AS totalSinGestionar,
        (SELECT COUNT(*) FROM CTE_DERIVADOS) AS totalDerivado,
        (SELECT COUNT(*) FROM CTE_DESEMBOLSOS) AS totalDesembolsado;
END
GO



EXEC [dbo].[SP_REPORTES_SUPERVISOR_DETALLADO] @id_usuario = 4176

SELECT * FROM usuarios WHERE id_rol = 2

SELECT top 150 * FROM GESTION_DETALLE
SELECT top 150 * FROM clientes_asignados
SELECT top 150 * FROM desembolsos ORDER BY FECHA_GEST DESC
SELECT top 150 * FROM derivaciones_asesores