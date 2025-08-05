SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_derivacion_insertar_derivacion_test_N]
(
    @agencia_derivacion NVARCHAR(50),
    @fecha_visita DATETIME,
    @telefono NVARCHAR(50),
    @id_base INT,
    @id_usuario INT,
    @nombre_completos NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    IF @nombre_completos IS NULL OR @nombre_completos = ''
    BEGIN
        SELECT TOP 1 @nombre_completos = CONCAT(x_nombre,' ', x_appaterno,' ', x_apmaterno)
        FROM base_clientes
        WHERE id_base = @id_base;
    END

    DECLARE 
        @id_cliente INT,
        @dni_asesor NVARCHAR(20),
        @dni_cliente NVARCHAR(20),
        @oferta DECIMAL(18,2),
        @dni_supervisor NVARCHAR(15),
        @nombre_supervisor NVARCHAR(100),
        @id_supervisor INT;

    -- Obtener ID del cliente
    SELECT TOP 1
        @id_cliente = ce.id_cliente
    FROM clientes_enriquecidos ce
    WHERE ce.id_base = @id_base;

    -- Obtener DNI del asesor y su supervisor
    SELECT TOP 1
        @dni_asesor = u.dni,
        @id_supervisor = u.ID_USUARIO_SUP
    FROM usuarios u
    WHERE u.id_usuario = @id_usuario;

    SELECT TOP 1
        @nombre_supervisor = u.Nombres_Completos,
        @dni_supervisor = u.dni
    FROM usuarios u
    WHERE u.id_usuario = @id_supervisor;

    -- Obtener DNI del cliente desde base
    SELECT TOP 1
        @dni_cliente = bc.dni
    FROM base_clientes bc
    WHERE bc.id_base = @id_base;

    -- Intentar obtener oferta desde detalle_base
    SELECT TOP 1 @oferta = db.oferta_max
    FROM base_clientes bc
    JOIN detalle_base db ON db.id_base = bc.id_base
    WHERE bc.id_base = @id_base
        AND YEAR(db.fecha_carga) = YEAR(GETDATE())
        AND MONTH(db.fecha_carga) = MONTH(GETDATE())
    ORDER BY db.fecha_carga DESC;

    -- Si no hay oferta en detalle_base, intentar en base_clientes_banco
    IF @oferta IS NULL
    BEGIN
        SELECT TOP 1 @oferta = bcb.oferta_max
        FROM base_clientes_banco bcb
        WHERE bcb.dni = @dni_cliente
            AND YEAR(bcb.fecha_subida) = YEAR(GETDATE())
            AND MONTH(bcb.fecha_subida) = MONTH(GETDATE())
        ORDER BY bcb.fecha_subida DESC;
    END

    -- Validar oferta final
    IF @oferta IS NULL
    BEGIN
        SET @oferta = CAST(0.00 AS DECIMAL(18,2));
    END
    ELSE IF @oferta < 1000
    BEGIN
        SET @oferta = CAST(@oferta * 100 AS DECIMAL(18,2));
    END
    ELSE
    BEGIN
        SET @oferta = CAST(@oferta AS DECIMAL(18,2));
    END

    -- Insertar derivación
    INSERT INTO derivaciones_asesores
    (
        fecha_derivacion, dni_asesor, dni_cliente, id_cliente,
        nombre_cliente, telefono_cliente, nombre_agencia, num_agencia,
        fue_procesado, fecha_visita, estado_derivacion, oferta_max,
        doc_supervisor, supervisor, fue_enviado_email
    )
    VALUES
    (
        GETDATE(), @dni_asesor, @dni_cliente, @id_cliente,
        @nombre_completos, @telefono, CONCAT('73', @agencia_derivacion), NULL,
        0, @fecha_visita, 'DERIVACION PENDIENTE', @oferta,
        @dni_supervisor, @nombre_supervisor, 0
    );

    RETURN CAST(SCOPE_IDENTITY() AS INT);
END;
GO


SELECT top 12 * FROM clientes_tipificados ORDER BY id_clientetip DESC;