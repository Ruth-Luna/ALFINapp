SELECT TOP (1000) [id_referido]
      ,[id_base_cliente_a365]
      ,[id_base_cliente_banco]
      ,[id_supervisor_referido]
      ,[nombre_completo_asesor]
      ,[nombre_completo_cliente]
      ,[dni_asesor]
      ,[dni_cliente]
      ,[fecha_referido]
      ,[traido_de]
      ,[telefono_cliente]
      ,[agencia_referido]
      ,[oferta_enviada]
      ,[fecha_visita_agencia]
      ,[fue_procesado]
      ,[celular_asesor]
      ,[correo_asesor]
      ,[cci_asesor]
      ,[departamento_asesor]
      ,[ubigeo_asesor]
      ,[banco_asesor]
      ,[estado_referencia]
  FROM [CORE_ALFIN].[dbo].[clientes_referidos]


GO
CREATE PROCEDURE sp_referir_cliente_guardar_referencia
(
    @dni NVARCHAR(20),
    @dni_asesor NVARCHAR(20),
    @traido_de NVARCHAR(30),
    @telefono_cliente NVARCHAR(20),
    @agencia_referido NVARCHAR(100),
    @fecha_visita_agencia DATETIME,
    @celular_asesor NVARCHAR(20),
    @correo_asesor NVARCHAR(100),
    @cci_asesor NVARCHAR(20),
    @ubigeo_asesor NVARCHAR(10),
    @departamento_asesor NVARCHAR(50),
    @banco_asesor NVARCHAR(50)
)
AS
BEGIN
    WITH DataClienteA365 AS (
        SELECT bc.id_base 
        FROM base_clientes bc
        WHERE bc.dni = @dni
    ), DataBancoCliente AS (
        SELECT bc.id_base 
        FROM base_clientes_banco bc
        WHERE bc.dni = @dni_asesor
    ), asesor AS (
        SELECT u.id_usuario, u.ID_USUARIO_SUP
        FROM usuarios u
        WHERE u.dni = @dni_asesor
    )
    INSERT INTO clientes_referidos (
        id_base_cliente_a365,
        id_base_cliente_banco,
        id_supervisor_referido,
        nombre_completo_asesor,
        nombre_completo_cliente,
        dni_asesor,
        dni_cliente,
        fecha_referido,
        traido_de,
        telefono_cliente,
        agencia_referido,
        oferta_enviada,
        fecha_visita_agencia,
        fue_procesado,
        celular_asesor,
        correo_asesor,
        cci_asesor,
        departamento_asesor,
        ubigeo_asesor,
        banco_asesor,
        estado_referencia
    ) VALUES (
        (SELECT id_base FROM DataCliente),
        (SELECT id_base FROM DataCliente),
        (SELECT ID_USUARIO_SUP FROM asesor),
        (SELECT CONCAT(u.nombres, ' ', u.apellidos) FROM usuarios u WHERE u.dni = @dni_asesor),
        (SELECT CONCAT(c.nombres, ' ', c.apellidos) FROM clientes c WHERE c.dni = @dni),
        @dni_asesor,
        @dni,
        GETDATE(),
        @traido_de,
        @telefono_cliente,
        @agencia_referido,
        0, -- oferta_enviada
        @fecha_visita_agencia,
        0, -- fue_procesado
        @celular_asesor,
        @correo_asesor,
        @cci_asesor,
        @departamento_asesor,
        @ubigeo_asesor,
        @banco_asesor,
        1 -- estado_referencia
    )
END;
GO
DROP PROCEDURE sp_referir_cliente_guardar_referencia
