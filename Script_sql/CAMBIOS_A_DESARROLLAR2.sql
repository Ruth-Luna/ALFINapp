-- ESPERAR A ENVIAR ESTOS CAMBIOS EL SISTEMA NO ESTA PREPARADO PARA TRIGERS
-- TRIGGER PARA INSERTAR EN GESTION_DETALLE CUANDO SE INSERTA EN CLIENTES_TIPIFICADOS
-- SE ASUME QUE SI SE INSERTA EN CLIENTES_TIPIFICADOS ES PORQUE LA GESTION FUE EXITOSA
-- Y SE TIENE QUE INSERTAR EN GESTION_DETALLE
-- TAMBIEN SE ASUME QUE SI SE INSERTA EN CLIENTES_TIPIFICADOS
-- EL REGISTRO YA TIENE QUERER TODA LA INFORMACION NECESARIA
-- PARA INSERTAR EN GESTION_DETALLE
-- SI NO TIENE LA INFORMACION NECESARIA, EL TRIGGER FALLARA
-- Y NO SE INSERTARA EN GESTION_DETALLE
-- POR LO TANTO, SE DEBE ASEGURAR QUE LA INFORMACION EN CLIENTES_TIPIFICADOS
-- ES CORRECTA Y COMPLETA ANTES DE INSERTAR

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
ALTER TABLE [dbo].[clientes_tipificados] DISABLE TRIGGER [trg_clientes_tipificados_insert_gestion]
GO
