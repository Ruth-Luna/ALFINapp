SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_tipificacion_insertar_nueva_tipificacion]
(
    @id_asignacion INT,
    @id_tipificacion INT,
    @telefono NVARCHAR(20)
)
AS
BEGIN
    DECLARE @derivacion_fecha DATETIME = NULL;
    IF (@id_tipificacion = 2)
    BEGIN
        SET @derivacion_fecha = GETDATE();
    END
    INSERT INTO clientes_tipificados
        (id_asignacion, id_tipificacion, fecha_tipificacion, origen, telefono_tipificado, derivacion_fecha)
    VALUES
        (@id_asignacion, @id_tipificacion, GETDATE(), 'nuevo', @telefono, @derivacion_fecha);

    DECLARE @id_clientetip_insertada INT = SCOPE_IDENTITY(),
        @DescripcionUltimaTipificacion NVARCHAR(255),
        @id_cliente INT;

    SELECT @id_cliente = id_cliente
    FROM clientes_asignados
    WHERE id_asignacion = @id_asignacion;
    
    SELECT @DescripcionUltimaTipificacion = descripcion_tipificacion
    FROM tipificaciones
    WHERE id_tipificacion = @id_tipificacion;
        
    IF EXISTS (
        SELECT 1
        FROM clientes_enriquecidos ce
        WHERE ce.id_cliente = @id_cliente
        AND ce.telefono_1 = @telefono
    )
    BEGIN 
        UPDATE clientes_enriquecidos
        SET fecha_ultima_tipificacion_telefono_1 = GETDATE(),
            ultima_tipificacion_telefono_1 = @DescripcionUltimaTipificacion,
            id_clientetip_telefono_1 = @id_clientetip_insertada
        WHERE id_cliente = @id_cliente
    END
    ELSE IF EXISTS (
        SELECT 1
        FROM clientes_enriquecidos ce
        WHERE ce.id_cliente = @id_cliente
        AND ce.telefono_2 = @telefono
    )
    BEGIN 
        UPDATE clientes_enriquecidos
        SET fecha_ultima_tipificacion_telefono_2 = GETDATE(),
            ultima_tipificacion_telefono_2 = @DescripcionUltimaTipificacion,
            id_clientetip_telefono_2 = @id_clientetip_insertada
        WHERE id_cliente = @id_cliente
    END
    ELSE IF EXISTS (
        SELECT 1
        FROM clientes_enriquecidos ce
        WHERE ce.id_cliente = @id_cliente
        AND ce.telefono_3 = @telefono
    )
    BEGIN 
        UPDATE clientes_enriquecidos
        SET fecha_ultima_tipificacion_telefono_3 = GETDATE(),
            ultima_tipificacion_telefono_3 = @DescripcionUltimaTipificacion,
            id_clientetip_telefono_3 = @id_clientetip_insertada
        WHERE id_cliente = @id_cliente
    END
    ELSE IF EXISTS (
        SELECT 1
        FROM clientes_enriquecidos ce
        WHERE ce.id_cliente = @id_cliente
        AND ce.telefono_4 = @telefono
    )
    BEGIN 
        UPDATE clientes_enriquecidos
        SET fecha_ultima_tipificacion_telefono_4 = GETDATE(),
            ultima_tipificacion_telefono_4 = @DescripcionUltimaTipificacion,
            id_clientetip_telefono_4 = @id_clientetip_insertada
        WHERE id_cliente = @id_cliente
    END
    ELSE IF EXISTS (
        SELECT 1
        FROM clientes_enriquecidos ce
        WHERE ce.id_cliente = @id_cliente
        AND ce.telefono_5 = @telefono
    )
    BEGIN 
        UPDATE clientes_enriquecidos
        SET fecha_ultima_tipificacion_telefono_5 = GETDATE(),
            ultima_tipificacion_telefono_5 = @DescripcionUltimaTipificacion,
            id_clientetip_telefono_5 = @id_clientetip_insertada
        WHERE id_cliente = @id_cliente
    END

    IF EXISTS (
        SELECT 1
        FROM telefonos_agregados ta
        WHERE ta.telefono = @telefono
    )
    BEGIN 
        UPDATE telefonos_agregados
        SET ultima_tipificacion = @DescripcionUltimaTipificacion,
            fecha_ultima_tipificacion = GETDATE(),
            id_clientetip = @id_clientetip_insertada
        WHERE telefono = @telefono;
    END
    
    
    DECLARE @tipificacionInfo TABLE
    (
        id_tipificacion INT,
        peso INT,
        descripcion NVARCHAR(255)
    );

    INSERT INTO @tipificacionInfo
        (id_tipificacion, peso, descripcion)
    SELECT t.id_tipificacion, peso, descripcion_tipificacion
    FROM tipificaciones t
    JOIN clientes_tipificados ct ON t.id_tipificacion = ct.id_tipificacion
    WHERE ct.id_asignacion = @id_asignacion;

    DECLARE @pesoMayorTipificacion INT,
            @descripcionTipificacionMayorPeso NVARCHAR(255);

    SELECT TOP 1
        @pesoMayorTipificacion = peso,
        @descripcionTipificacionMayorPeso = descripcion
    FROM @tipificacionInfo
    ORDER BY peso DESC;

    -- Actualizar la asignación con la tipificación de mayor peso
    UPDATE clientes_asignados
    SET id_tipificacion_mayor_peso = @id_tipificacion,
        peso_tipificacion_mayor = @pesoMayorTipificacion,
        tipificacion_mayor_peso = @descripcionTipificacionMayorPeso,
        ultima_tipificacion_general = @DescripcionUltimaTipificacion,
        fecha_ultima_tipificacion = GETDATE()
    WHERE id_asignacion = @id_asignacion;

    -- La insercion de la tipificación se ha realizado correctamente
    RETURN;
END;
GO


SELECT TOP 150 * FROM telefonos_agregados

select TOP 150
    *
from clientes_tipificados
ORDER BY id_tipificacion DESC;

SELECT telefono, 
       LEN(telefono) AS longitud_visible, 
       DATALENGTH(telefono) AS bytes_almacenados
FROM telefonos_agregados
WHERE LEN(telefono) != DATALENGTH(telefono);




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_tipificacion_verificar_disponibilidad_para_tipificar]
    (
    @id_usuario INT,
    @tipificacion_id INT,
    @id_asignacion INT,
    @type_tipificacion INT,
    @telefono NVARCHAR(20) = NULL,
    @agencia_asignada NVARCHAR(50) = NULL,
    @fecha_visita DATETIME = NULL
)
AS
BEGIN
    CREATE TABLE #asignacion
    (
        id_asignacion INT,
        id_usuarioV INT,
        id_usuarioS INT,
        id_cliente INT,
        dni_cliente NVARCHAR(20)
    );
    CREATE TABLE #usuarioV
    (
        id_usuario INT,
        nombres NVARCHAR(100),
        dni NVARCHAR(20)
    );
    INSERT INTO #asignacion
        (id_asignacion, id_usuarioV, id_usuarioS, id_cliente, dni_cliente)
    SELECT ca.id_asignacion, ca.id_usuarioV, ca.id_usuarioS, ca.id_cliente, bc.dni
    FROM clientes_asignados ca JOIN clientes_enriquecidos ce ON ca.id_cliente = ce.id_cliente
        JOIN base_clientes bc ON ce.id_base = bc.id_base
    WHERE ca.id_asignacion = @id_asignacion

    INSERT INTO #usuarioV
        (id_usuario, nombres, dni)
    SELECT u.id_usuario, u.Nombres_Completos, u.dni
    FROM usuarios u
    WHERE u.id_usuario = @id_usuario;

    DECLARE @tipificacionMayorPeso INT,
            @descripcionTipificacionMayorPeso NVARCHAR(255),
            @pesoMayor INT,
            @agregadoPreviamente BIT;

    IF (@tipificacion_id = 2)
    BEGIN
        IF @fecha_visita IS NULL
        BEGIN
            SELECT 'Debe ingresar la fecha de visita para poder tipificar la asignación.' AS mensaje,
                0 AS resultado;
            RETURN;
        END
        ELSE IF @agencia_asignada IS NULL
        BEGIN
            SELECT 'Debe ingresar la agencia asignada para poder tipificar la asignación.' AS mensaje,
                0 AS resultado;
            RETURN;
        END
        ELSE
        BEGIN
            IF NOT EXISTS (
                SELECT TOP 1
                da.id_derivacion,
                da.dni_asesor,
                da.dni_cliente
            FROM derivaciones_asesores da
            WHERE da.id_cliente IN (SELECT TOP 1
                    id_cliente
                FROM #asignacion)
                AND da.dni_asesor IN (SELECT TOP 1
                    dni
                FROM #usuarioV)
                AND YEAR(da.fecha_derivacion)= YEAR(GETDATE())
                AND MONTH(da.fecha_derivacion) = MONTH(GETDATE())
            )
            BEGIN
                SELECT 'Debe registrar una derivación para poder tipificar la asignación.' AS mensaje,
                    0 AS resultado;
                RETURN;
            END
            ELSE IF EXISTS (
                SELECT TOP 1
                gd.id_derivacion
            FROM GESTION_DETALLE gd
            WHERE gd.doc_cliente IN (SELECT TOP 1
                    dni_cliente
                FROM #asignacion)
                AND gd.doc_asesor IN (SELECT TOP 1
                    dni
                FROM #usuarioV)
                AND YEAR(gd.fecha_gestion) = YEAR(GETDATE())
                AND MONTH(gd.fecha_gestion) = MONTH(GETDATE())
                AND DAY(gd.fecha_gestion) != DAY(GETDATE())
                AND gd.cod_tip = 2
            )
            BEGIN
                SELECT 'La asignación ya tiene una gestión registrada para el mes actual. Usted ha derivado previamente a este cliente' AS mensaje,
                    0 AS resultado;
                RETURN;
            END
            ELSE
            BEGIN
                SELECT 'Puede continuar con la derivacion' AS mensaje,
                    1 AS resultado;
            END
        END
    END
    ELSE 
    BEGIN
        SELECT 'Puede continuar con la tipificación de la asignación.' AS mensaje,
            1 AS resultado;
    END



END;
GO
