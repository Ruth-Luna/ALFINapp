ALTER PROCEDURE [dbo].[SP_tipificacion_verificar_tipificacion_cliente]
(
    @idcliente INT,
    @tipificacion_id INT,
    @id_derivacion INT = NULL,
    @resultado INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @tipificacion_id = 2 AND @id_derivacion IS NULL
    BEGIN
        SET @resultado = 0;
        RETURN;
    END
    ELSE IF @tipificacion_id = 2 AND @id_derivacion IS NOT NULL
    BEGIN
        DECLARE @verificacion INT;
        SELECT @verificacion = COUNT(*)
        FROM derivaciones_asesores da
        WHERE da.id_derivacion = @id_derivacion;

        IF @verificacion = 0
        BEGIN
            SET @resultado = 0;
            RETURN;
        END
    END

    SET @resultado = 1;
END;
GO


GO
ALTER PROCEDURE [dbo].[SP_tipificaciones_subir_cliente_tipificado]
    (
    @telefono NVARCHAR(20),
    @id_tipificacion INT,
    @id_asignacion INT,
    @id_usuario INT,
    @id_cliente_tip INT OUTPUT   
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
    SET @id_cliente_tip = SCOPE_IDENTITY();
END;
GO

select top 1
    *
from clientes_tipificados
order by id_clientetip desc;


select top 1
    *
from derivaciones_asesores

SELECT top 150 * FROM base_clientes bc
JOIN detalle_base db ON db.id_base = bc.id_base
ORDER BY db.fecha_carga DESC;


GO
ALTER PROCEDURE [dbo].[SP_tipificaciones_actualizar_estado_tipificacion]
(
    @telefono NVARCHAR(20),
    @id_tipificacion INT,
    @id_asignacion INT,
    @id_cliente_tip INT
)
AS
BEGIN
    -- SET NOCOUNT ON;

    DECLARE @id_cliente INT;
    DECLARE @columna_encontrada INT;
    DECLARE @id_clientetip INT = @id_cliente_tip;
    DECLARE @descripcion_tipificacion NVARCHAR(255);
    DECLARE @peso INT;

    -- Obtener datos de la tipificación
    SELECT TOP 1
        @descripcion_tipificacion = t.descripcion_tipificacion,
        @peso = t.peso
    FROM tipificaciones t
    WHERE t.id_tipificacion = @id_tipificacion;

    -- Buscar en clientes enriquecidos
    SELECT TOP 1
        @id_cliente = ce.id_cliente,
        @columna_encontrada = 
            CASE 
                WHEN ce.telefono_1 = @telefono THEN 1
                WHEN ce.telefono_2 = @telefono THEN 2
                WHEN ce.telefono_3 = @telefono THEN 3
                WHEN ce.telefono_4 = @telefono THEN 4
                WHEN ce.telefono_5 = @telefono THEN 5
                ELSE 0
            END
    FROM clientes_enriquecidos ce
    WHERE @telefono IN (ce.telefono_1, ce.telefono_2, ce.telefono_3, ce.telefono_4, ce.telefono_5);

    IF @columna_encontrada IS NOT NULL AND @columna_encontrada BETWEEN 1 AND 5
    BEGIN
        -- Actualizar el campo correcto según el número detectado
        IF @columna_encontrada = 1
            UPDATE clientes_enriquecidos
            SET
                fecha_ultima_tipificacion_telefono_1 = GETDATE(),
                ultima_tipificacion_telefono_1 = @descripcion_tipificacion,
                id_clientetip_telefono_1 = @id_clientetip
            WHERE telefono_1 = @telefono;

        ELSE IF @columna_encontrada = 2
            UPDATE clientes_enriquecidos
            SET
                fecha_ultima_tipificacion_telefono_2 = GETDATE(),
                ultima_tipificacion_telefono_2 = @descripcion_tipificacion,
                id_clientetip_telefono_2 = @id_clientetip
            WHERE telefono_2 = @telefono;

        ELSE IF @columna_encontrada = 3
            UPDATE clientes_enriquecidos
            SET
                fecha_ultima_tipificacion_telefono_3 = GETDATE(),
                ultima_tipificacion_telefono_3 = @descripcion_tipificacion,
                id_clientetip_telefono_3 = @id_clientetip
            WHERE telefono_3 = @telefono;

        ELSE IF @columna_encontrada = 4
            UPDATE clientes_enriquecidos
            SET
                fecha_ultima_tipificacion_telefono_4 = GETDATE(),
                ultima_tipificacion_telefono_4 = @descripcion_tipificacion,
                id_clientetip_telefono_4 = @id_clientetip
            WHERE telefono_4 = @telefono;

        ELSE IF @columna_encontrada = 5
            UPDATE clientes_enriquecidos
            SET
                fecha_ultima_tipificacion_telefono_5 = GETDATE(),
                ultima_tipificacion_telefono_5 = @descripcion_tipificacion,
                id_clientetip_telefono_5 = @id_clientetip
            WHERE telefono_5 = @telefono;
    END
    ELSE
    BEGIN
        -- Buscar en tabla de teléfonos agregados
        SELECT TOP 1 @id_cliente = id_cliente
        FROM telefonos_agregados
        WHERE telefono LIKE '%' + @telefono + '%';

        IF @id_cliente IS NOT NULL
        BEGIN
            UPDATE telefonos_agregados
            SET
                fecha_ultima_tipificacion = GETDATE(),
                ultima_tipificacion = @descripcion_tipificacion,
                id_clientetip = @id_clientetip
            WHERE telefono = @telefono;
        END
    END

    -- Finalmente actualizar la asignación si corresponde
    DECLARE @peso_actual INT;

    SELECT @peso_actual = peso_tipificacion_mayor
    FROM clientes_asignados
    WHERE id_asignacion = @id_asignacion;

    IF @peso IS NOT NULL AND @peso > ISNULL(@peso_actual, 0)
    BEGIN
        UPDATE clientes_asignados
        SET
            tipificacion_mayor_peso = @descripcion_tipificacion,
            peso_tipificacion_mayor = @peso,
            fecha_tipificacion_mayor_peso = GETDATE()
        WHERE id_asignacion = @id_asignacion;
    END
END;
GO

EXEC SP_tipificaciones_actualizar_estado_tipificacion 
    @telefono = '985262345',
    @id_tipificacion = 2,
    @id_asignacion = 1445000,
    @id_cliente_tip = 20280;


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_derivacion_insertar_derivacion_nuevo_metodo]
    (
    @agencia_derivacion NVARCHAR(50),
    @fecha_visita DATETIME,
    @telefono NVARCHAR(50),
    @id_base INT,
    @id_usuario INT = NULL,
    @nombre_completos NVARCHAR(100) = NULL,
    @dni_asesor_auxiliar NVARCHAR(20) = NULL,
    @id_derivacion INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    IF @nombre_completos IS NULL OR @nombre_completos = ''
    BEGIN
        SELECT TOP 1
            @nombre_completos = CONCAT(x_nombre,' ', x_appaterno,' ', x_apmaterno)
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
    IF @id_usuario IS NOT NULL AND @id_usuario > 0
    BEGIN
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
    END
    ELSE IF @dni_asesor_auxiliar IS NOT NULL AND @dni_asesor_auxiliar <> '' AND EXISTS (
        SELECT 1
        FROM usuarios u
        WHERE u.dni = @dni_asesor_auxiliar
    )
    BEGIN
        SELECT TOP 1
            @dni_asesor = @dni_asesor_auxiliar,
            @id_supervisor = u.ID_USUARIO_SUP
        FROM usuarios u
        WHERE u.dni = @dni_asesor_auxiliar;

        SELECT TOP 1
            @nombre_supervisor = u.Nombres_Completos,
            @dni_supervisor = u.dni
        FROM usuarios u
        WHERE u.id_usuario = @id_supervisor;
    END
    ELSE
    BEGIN
        -- Si no se proporciona un asesor, usar un valor por defecto
        SET @dni_asesor = CASE
            WHEN @dni_asesor_auxiliar IS NOT NULL AND @dni_asesor_auxiliar <> '' THEN @dni_asesor_auxiliar
            ELSE 'DESCONOCIDO'
        END;
        SET @id_supervisor = NULL;
        SET @dni_supervisor = 'DESCONOCIDO';
        SET @nombre_supervisor = 'DESCONOCIDO';
    END

    -- Obtener DNI del cliente desde base
    SELECT TOP 1
        @dni_cliente = bc.dni
    FROM base_clientes bc
    WHERE bc.id_base = @id_base;

    -- Intentar obtener oferta desde detalle_base
    SELECT TOP 1
        @oferta = db.oferta_max
    FROM base_clientes bc
        JOIN detalle_base db ON db.id_base = bc.id_base
    WHERE bc.id_base = @id_base
        AND YEAR(db.fecha_carga) = YEAR(GETDATE())
        AND MONTH(db.fecha_carga) = MONTH(GETDATE())
    ORDER BY db.fecha_carga DESC;

    -- Si no hay oferta en detalle_base, intentar en base_clientes_banco
    IF @oferta IS NULL
    BEGIN
        SELECT TOP 1
            @oferta = bcb.oferta_max
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

    SET @id_derivacion = SCOPE_IDENTITY();
END;
GO


SELECT TOP 10 * FROM derivaciones_asesores
ORDER BY fecha_derivacion DESC;

delete from derivaciones_asesores
where id_derivacion IN (15577)

delete from agendamientos_reagendamientos
where id_derivacion IN (15577)

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trg_clientes_tipificados_insert_gestion]
    on [CORE_ALFIN].[dbo].[clientes_tipificados]
    after insert
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.debug_log (mensaje) VALUES ('Entró al trigger');
    DECLARE @clientes TABLE (id_asignacion INT,
    id_cliente INT,
    id_base INT,
    fuente_base NVARCHAR(100),
    documento_cliente NVARCHAR(50),
    id_base_banco INT,
    id_derivacion INT);

    insert into @clientes
    (id_asignacion, id_cliente, id_base, fuente_base
    , documento_cliente, id_base_banco, id_derivacion)
    SELECT
        ca.id_asignacion,
        ce.id_cliente,
        bc.id_base,
        ca.fuente_base,
        bc.dni as documento_cliente,
        bc.id_base_banco,
        da.id_derivacion
    FROM clientes_asignados ca
        JOIN clientes_enriquecidos ce ON ca.id_cliente = ce.id_cliente
        JOIN base_clientes bc ON ce.id_base = bc.id_base
        LEFT JOIN derivaciones_asesores da ON bc.dni = da.dni_cliente
    WHERE ca.id_asignacion IN (SELECT id_asignacion FROM inserted);

    INSERT INTO dbo.debug_log (mensaje, info) 
    VALUES ('Insert @clientes', (SELECT COUNT(*) FROM @clientes));

    DECLARE @detalle_base TABLE (id_base INT,
    id_detalle INT,
    oferta DECIMAL(18, 2),
    cod_campana NVARCHAR(50));

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
            bc.dni IN (SELECT dni FROM @clientes)
            AND ce.telefono_1 IN (SELECT telefono_tipificado FROM inserted)
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
            bc.dni IN (SELECT dni FROM @clientes)
            AND ce.telefono_2 IN (SELECT telefono_tipificado FROM inserted)
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
            bc.dni IN (SELECT dni FROM @clientes)
            AND ce.telefono_3 IN (SELECT telefono_tipificado FROM inserted)
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
            bc.dni IN (SELECT dni FROM @clientes)
            AND ce.telefono_4 IN (SELECT telefono_tipificado FROM inserted)
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
            bc.dni IN (SELECT dni FROM @clientes)
            AND ce.telefono_5 IN (SELECT telefono_tipificado FROM inserted)
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
            bc.dni IN (SELECT dni FROM @clientes)
            AND ta.telefono IN (SELECT telefono_tipificado FROM inserted)
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

    INSERT INTO dbo.debug_log (mensaje, info) 
VALUES ('Insert @TempTelefono', (SELECT COUNT(*) FROM @TempTelefono));

    DECLARE @usuarios_info TABLE (
        id_asignacion INT,
        id_usuarioV INT,
        doc_vendedor NVARCHAR(20),
        id_usuario_supervisor INT,
        doc_supervisor NVARCHAR(20),
        nombre_supervisor NVARCHAR(100));

    INSERT INTO @usuarios_info (
        id_asignacion, 
        id_usuarioV, 
        doc_vendedor,
        id_usuario_supervisor,
        doc_supervisor,
        nombre_supervisor)
    SELECT
        ca.id_asignacion,
        ca.id_usuarioV AS id_usuarioV,
        uv.dni as doc_vendedor,
        ca.id_usuarioS AS id_usuario_supervisor,
        us.dni AS doc_supervisor,
        us.Nombres_Completos AS nombre_supervisor
    FROM clientes_asignados ca
        JOIN usuarios uv ON ca.id_usuarioV = uv.id_usuario
        JOIN usuarios us ON ca.id_usuarioS = us.id_usuario
    WHERE ca.id_asignacion IN (SELECT id_asignacion FROM inserted);

    INSERT INTO dbo.debug_log (mensaje, info) 
VALUES ('Insert @usuarios_info', (SELECT COUNT(*) FROM @usuarios_info));

        
    INSERT INTO @detalle_base
        (id_base, id_detalle, oferta, cod_campana)
        SELECT
            db.id_base,
            db.id_detalle,
            ISNULL(db.oferta_max, 0) AS oferta,
            db.campaña as cod_campana
        FROM [CORE_ALFIN].[dbo].[detalle_base] db
        JOIN
            @clientes c
        ON db.id_base = c.id_base
        WHERE c.fuente_base <> 'ALFINBANCO'
            AND db.tipo_base = c.fuente_base
    UNION ALL
        SELECT
            c.id_base,
            bcb.id_base_banco AS id_detalle,
            ISNULL(bcb.oferta_max, 0) AS oferta,
            'BASE BANCO' as cod_campana
        FROM [CORE_ALFIN].[dbo].[base_clientes_banco] bcb
            JOIN @clientes c
            ON bcb.id_base_banco = c.id_base_banco
        WHERE c.fuente_base = 'ALFINBANCO'

    INSERT INTO dbo.debug_log (mensaje, info) 
VALUES ('Insert @detalle_base', (SELECT COUNT(*) FROM @detalle_base));

INSERT INTO dbo.debug_log (mensaje) VALUES ('Antes de insertar en GESTION_DETALLE');

    INSERT INTO [CORE_ALFIN].[dbo].[GESTION_DETALLE]
        (
        id_asignacion,
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
        id_derivacion,
        id_supervisor,
        supervisor
        )
    SELECT
        i.id_asignacion,
        'SYSTEMA365' AS cod_canal,
        'A365' AS canal,
        c.documento_cliente as doc_cliente,
        GETDATE() AS fecha_envio,
        GETDATE() AS fecha_gestion,
        CAST(GETDATE() as TIME) AS hora_gestion,
        i.telefono_tipificado AS telefono,
        t.origen_telefono AS origen_telefono,
        db.cod_campana AS cod_campaña, -- Assuming a default value, adjust as necessary
        i.id_tipificacion AS cod_tip,
        db.oferta AS oferta, -- Assuming no offer, adjust as necessary
        uf.doc_vendedor AS doc_asesor, -- Assuming no advisor document, adjust as necessary
        'nuevo' AS origen,
        'BD PROPIA' AS archivo_origen, -- Assuming no file origin, adjust as necessary
        GETDATE() AS fecha_carga,
        c.id_derivacion AS id_derivacion,
        uf.id_usuario_supervisor AS id_supervisor, -- Assuming no supervisor, adjust as necessary
        uf.nombre_supervisor AS supervisor
    -- Assuming no supervisor name, adjust as necessary
    FROM inserted i
        JOIN @clientes c ON i.id_asignacion = c.id_asignacion
        JOIN @detalle_base db ON c.id_base = db.id_base
        JOIN @TempTelefono t ON c.documento_cliente = t.dni
        JOIN @usuarios_info uf ON i.id_asignacion = uf.id_asignacion
END;
GO
ALTER TABLE [dbo].[clientes_tipificados] ENABLE TRIGGER [trg_clientes_tipificados_insert_gestion]
GO

DROP TRIGGER [dbo].[trg_clientes_tipificados_insert_gestion]





SELECT TOP 1000 * FROM GESTION_DETALLE ORDER BY id_feedback DESC;


SELECT TOP 100 * FROM clientes_asignados ORDER BY id_asignacion DESC;