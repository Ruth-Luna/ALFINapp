SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Consulta_Obtener_detalle_cliente_para_tipificar_A365]
    @IdBase INT,
    @IdUsuarioV INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @IdCliente INT;
    -- Obtener el id_cliente de la base de clientes
    SELECT TOP 1 @IdCliente = id_cliente
    FROM base_clientes bc
    JOIN clientes_enriquecidos ce ON bc.id_base = ce.id_base
    WHERE bc.id_base = @IdBase

    IF @IdCliente IS NULL
    BEGIN
        RAISERROR('No se encontró un cliente asociado a la base proporcionada.', 16, 1);
        RETURN;
    END


    ;WITH ultima_detalle_base AS (
        SELECT *,
            ROW_NUMBER() OVER (PARTITION BY id_base ORDER BY id_detalle DESC) AS rn
        FROM detalle_base
    ), telefono_cliente_manuales AS (
        SELECT 
            ta.id_cliente,
            ta.telefono,
            ta.comentario,
            ta.ultima_tipificacion,
            ta.fecha_ultima_tipificacion
        FROM telefonos_agregados ta
        WHERE ta.id_cliente = @IdCliente
    )


    SELECT 
        -- Propiedades de BaseCliente
        bc.Dni AS Dni,
        bc.x_appaterno AS XAppaterno,
        bc.x_apmaterno AS XApmaterno,
        bc.x_nombre AS XNombre,
        bc.Edad AS Edad,
        bc.Departamento AS Departamento,
        bc.Provincia AS Provincia,
        bc.Distrito AS Distrito,
        bc.id_base AS IdBase,

        -- Propiedades de DetalleBase
        db.Campaña AS Campaña,
        db.oferta_max AS OfertaMax,
        db.tasa_minima AS TasaMinima,
        db.Sucursal AS Sucursal,
        db.agencia_comercial AS AgenciaComercial,
        db.Plazo AS Plazo,
        db.Cuota AS Cuota,
        db.grupo_tasa AS GrupoTasa,
        db.grupo_monto AS GrupoMonto,
        db.Propension AS Propension,
        db.tipo_cliente AS TipoCliente,
        db.cliente_nuevo AS ClienteNuevo,
        db.Color AS Color,
        db.color_final AS ColorFinal,
        db.PERFIL_RO as PerfilRo,

        -- Propiedades de ClientesEnriquecido
        ce.id_cliente AS IdCliente,
        ce.telefono_1 AS Telefono1,
        ce.telefono_2 AS Telefono2,
        ce.telefono_3 AS Telefono3,
        ce.telefono_4 AS Telefono4,
        ce.telefono_5 AS Telefono5,
        ce.comentario_telefono_1 AS ComentarioTelefono1,
        ce.comentario_telefono_2 AS ComentarioTelefono2,
        ce.comentario_telefono_3 AS ComentarioTelefono3,
        ce.comentario_telefono_4 AS ComentarioTelefono4,
        ce.comentario_telefono_5 AS ComentarioTelefono5,
        ce.ultima_tipificacion_telefono_1 AS UltimaTipificacionTelefono1,
        ce.ultima_tipificacion_telefono_2 AS UltimaTipificacionTelefono2,
        ce.ultima_tipificacion_telefono_3 AS UltimaTipificacionTelefono3,
        ce.ultima_tipificacion_telefono_4 AS UltimaTipificacionTelefono4,
        ce.ultima_tipificacion_telefono_5 AS UltimaTipificacionTelefono5,
        ce.fecha_ultima_tipificacion_telefono_1 AS FechaUltimaTipificacionTelefono1,
        ce.fecha_ultima_tipificacion_telefono_2 AS FechaUltimaTipificacionTelefono2,
        ce.fecha_ultima_tipificacion_telefono_3 AS FechaUltimaTipificacionTelefono3,
        ce.fecha_ultima_tipificacion_telefono_4 AS FechaUltimaTipificacionTelefono4,
        ce.fecha_ultima_tipificacion_telefono_5 AS FechaUltimaTipificacionTelefono5,

        -- Propiedades Tasas y Detalles
        db.oferta_12M AS Oferta12m,
        db.tasa_12M AS Tasa12m,
        db.cuota_12M AS Cuota12m,
        db.oferta_18M AS Oferta18m,
        db.tasa_18M AS Tasa18m,
        db.cuota_18M AS Cuota18m,
        db.oferta_24M AS Oferta24m,
        db.tasa_24M AS Tasa24m,
        db.cuota_24M AS Cuota24m,
        db.oferta_36M AS Oferta36m,
        db.tasa_36M AS Tasa36m,
        db.cuota_36M AS Cuota36m,
        db.Usuario AS Usuario,
        db.segmento_user AS SegmentoUser,

        -- Propiedades de ClientesAsignados
        ca.id_asignacion AS IdAsignacion,
        ca.fuente_base AS FuenteBase,

        -- Propiedades de TelefonoClienteManuales
        ta.telefono AS TelefonoManual,
        ta.comentario AS ComentarioTelefonoManual,
        ta.ultima_tipificacion AS UltimaTipificacionTelefonoManual,
        ta.fecha_ultima_tipificacion AS FechaUltimaTipificacionTelefonoManual

    FROM base_clientes bc
    INNER JOIN ultima_detalle_base db ON bc.id_base = db.id_base
    INNER JOIN clientes_enriquecidos ce ON bc.id_base = ce.id_base
    INNER JOIN clientes_asignados ca ON ce.id_cliente = ca.id_cliente
    LEFT JOIN telefono_cliente_manuales ta ON ce.id_cliente = ta.id_cliente
    WHERE 
        bc.id_base = @IdBase 
        AND db.tipo_base = ca.fuente_base
        AND db.rn = 1
        AND ca.id_usuarioV = 2108
        
END;
GO

EXEC [dbo].[SP_Consulta_Obtener_detalle_cliente_para_tipificar_A365] @IdBase = 119, @IdUsuarioV = 2108;



SELECT TOP 1 bc.* FROM clientes_asignados ca
JOIN clientes_enriquecidos ce ON ca.id_cliente = ce.id_cliente
JOIN base_clientes bc ON ce.id_base = bc.id_base

ORDER BY id_asignacion DESC;