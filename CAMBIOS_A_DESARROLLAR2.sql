GO
CREATE PROCEDURE [dbo].[SP_tipificacion_verificar_tipificacion_cliente]
    (
    @idcliente INT,
    @tipificacion_id INT,
    @id_derivacion INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @tipificacion_id = 2 AND @id_derivacion IS NULL
    BEGIN
        RETURN 0;
    -- No se permite tipificar como "CLIENTE ACEPTO OFERTA DERIVACION" sin una derivación
    END
    ELSE IF @tipificacion_id = 2 AND @id_derivacion IS NOT NULL
    BEGIN
        DECLARE @verificacion INT;
        SELECT @verificacion = COUNT(*)
        FROM derivaciones_asesores da
        WHERE da.id_derivacion = @id_derivacion
        IF @verificacion = 0
        BEGIN
            RETURN 0;
        END
    END
    RETURN 1;
END;
GO


GO
CREATE PROCEDURE [dbo].[SP_tipificaciones_subir_cliente_tipificado]
    (
    @telefono NVARCHAR(20),
    @id_tipificacion INT,
    @id_asignacion INT,
    @id_usuario INT
)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @dni_cliente NVARCHAR(20),
            @fecha_derivacion DATETIME;

    SELECT TOP 1 @dni_cliente = bc.dni
    FROM base_clientes bc
    JOIN clientes_enriquecidos ce ON ce.id_base = bc.id_base
    JOIN clientes_asignados ca ON ca.id_cliente = ce.id_cliente
    WHERE ca.id_asignacion = @id_asignacion;


    SELECT TOP 1 @fecha_derivacion = da.fecha_derivacion
    FROM derivaciones_asesores da
    WHERE da.dni_cliente = @dni_cliente
    ORDER BY da.id_derivacion DESC;

    INSERT INTO clientes_tipificados
        (
        id_asignacion, id_tipificacion, fecha_tipificacion, origen, telefono_tipificado, derivacion_fecha)
    VALUES
        (@id_asignacion, @id_tipificacion, GETDATE(), 'nuevo', @telefono, CASE WHEN @id_tipificacion = 2 THEN @fecha_derivacion ELSE NULL END);
END;
GO

select top 1
    *
from clientes_tipificados
order by id_clientetip desc;


select top 1
    *
from derivaciones_asesores


GO
CREATE PROCEDURE [dbo].[SP_tipificaciones_actualizar_estado_tipificacion]
    (
    @telefono NVARCHAR(20),
    @id_tipificacion INT,
    @id_asignacion INT
)
AS
BEGIN
    DECLARE telefono_de_donde_es NVARCHAR(20);
    SELECT TOP 1 telefono_de_donde_es = 'bd1' FROM clientes_enriquecidos WHERE 
        telefono_1 = @telefono

END;
GO


SELECT TOP 1
    *
FROM base_clientes
WHERE dni = '987654321';


UPDATE correos
SET correo = 'santiagovl0308@outlook.es'
WHERE id = 12
