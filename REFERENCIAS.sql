GO
ALTER PROCEDURE sp_referir_cliente_guardar_referencia
(
    @dni NVARCHAR(20),
    @dni_asesor NVARCHAR(20),
    @nombre_completo_asesor NVARCHAR(200),
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
        SELECT TOP 1 bc.id_base, bc.x_nombre, bc.x_appaterno, bc.x_apmaterno, db.oferta_max
        FROM base_clientes bc 
        JOIN detalle_base db ON bc.id_base = db.id_base
        WHERE bc.dni = @dni
            AND YEAR(db.fecha_carga) = YEAR(GETDATE())
            AND MONTH(db.fecha_carga) = MONTH(GETDATE())
        ORDER BY db.fecha_carga DESC
    ), 
    DataBancoCliente AS (
        SELECT TOP 1 bcb.id_base_banco, bcb.NOMBRES, bcb.PATERNO, bcb.MATERNO, bcb.oferta_max
        FROM base_clientes_banco bcb
        WHERE bcb.dni = @dni
            AND YEAR(bcb.fecha_subida) = YEAR(GETDATE())
            AND MONTH(bcb.fecha_subida) = MONTH(GETDATE())
        ORDER BY bcb.fecha_subida DESC
    ), 
    asesor AS (
        SELECT u.id_usuario, u.ID_USUARIO_SUP, u.Nombres_Completos
        FROM usuarios u
        WHERE u.dni = @dni_asesor
    ), 
    supervisor AS (
        SELECT u.id_usuario
        FROM usuarios u
        WHERE u.id_usuario IN (SELECT TOP 1 ID_USUARIO_SUP FROM asesor)
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
    ) 
    VALUES (
        (SELECT TOP 1 id_base FROM DataClienteA365),
        (SELECT TOP 1 id_base_banco FROM DataBancoCliente),
        ISNULL((SELECT TOP 1 id_usuario FROM supervisor), NULL),
        CASE 
            WHEN EXISTS (SELECT 1 FROM asesor a)
                THEN (SELECT TOP 1 a.Nombres_Completos FROM asesor a)
            ELSE @nombre_completo_asesor
        END,
        CASE 
            WHEN EXISTS (SELECT 1 FROM DataClienteA365) 
                THEN (SELECT TOP 1 CONCAT(x_nombre, ' ', x_appaterno, ' ', x_apmaterno) FROM DataClienteA365)
            WHEN EXISTS (SELECT 1 FROM DataBancoCliente) 
                THEN (SELECT TOP 1 CONCAT(NOMBRES, ' ', PATERNO, ' ', MATERNO) FROM DataBancoCliente)
            ELSE NULL
        END,
        @dni_asesor,
        @dni,
        GETDATE(),
        @traido_de,
        @telefono_cliente,
        @agencia_referido,
        CASE 
            WHEN EXISTS (SELECT 1 FROM DataClienteA365) 
                THEN (SELECT TOP 1 oferta_max FROM DataClienteA365)
            WHEN EXISTS (SELECT 1 FROM DataBancoCliente) 
                THEN (SELECT TOP 1 oferta_max*100 FROM DataBancoCliente)
            ELSE NULL
        END,
        @fecha_visita_agencia,
        0, -- fue_procesado (valor inicial, asumo que es booleano 0/1)
        @celular_asesor,
        @correo_asesor,
        @cci_asesor,
        @departamento_asesor,
        @ubigeo_asesor,
        @banco_asesor,
        'DERIVACION PENDIENTE'
    );
END;
GO
ALTER PROCEDURE sp_referir_enviar_emails_de_referencia
(
    @dni NVARCHAR(20)
)
AS
BEGIN
    DECLARE 
        @dni_cliente NVARCHAR(20),
        @nombre_cliente NVARCHAR(200),
        @telefono NVARCHAR(20),
        @agencia NVARCHAR(100),
        @fecha_visita DATETIME,
        @dni_vendedor NVARCHAR(20),
        @nombre_vendedor NVARCHAR(200),
        @oferta_max DECIMAL(18,2),
        @mensaje NVARCHAR(MAX),
        @subject NVARCHAR(200);

    -- Obtener los datos del cliente referido
    SELECT TOP 1
        @dni_cliente = c.dni_cliente,
        @nombre_cliente = c.nombre_completo_cliente,
        @telefono = c.telefono_cliente,
        @agencia = c.agencia_referido,
        @fecha_visita = c.fecha_visita_agencia,
        @dni_vendedor = c.dni_asesor,
        @nombre_vendedor = c.nombre_completo_asesor,
        @oferta_max = ISNULL(c.oferta_enviada, 0)
    FROM clientes_referidos c
    WHERE c.dni_cliente = @dni
        AND CAST(c.fecha_referido AS DATE) = CAST(GETDATE() AS DATE)
    ORDER BY c.fecha_referido DESC;

    -- Validación: Si no hay datos, salir
    IF @dni_cliente IS NULL
        RETURN;

    -- Construir mensaje HTML
    SET @mensaje = 
        '<h2>REFERIDOS</h2>
        <table>
            <tr><td>CANAL TELECAMPO</td><td>A365</td></tr>
            <tr><td>CODIGO EJECUTIVO</td><td>' + @dni_vendedor + '</td></tr>
            <tr><td>CDV ALFINBANCO</td><td>' + @nombre_vendedor + '</td></tr>
            <tr><td>DNI CLIENTE</td><td>' + @dni_cliente + '</td></tr>
            <tr><td>NOMBRE CLIENTE</td><td>' + @nombre_cliente + '</td></tr>
            <tr><td>MONTO SOLICITADO</td><td>' + CAST(@oferta_max AS NVARCHAR) + '</td></tr>
            <tr><td>CELULAR</td><td>' + @telefono + '</td></tr>
            <tr><td>AGENCIA</td><td>' + @agencia + '</td></tr>
            <tr><td>FECHA DE VISITA A AGENCIA</td><td>' + CONVERT(NVARCHAR, @fecha_visita, 103) + '</td></tr>
            <tr><td>HORA DE VISITA A AGENCIA</td><td>NO ESPECIFICADO</td></tr>
        </table>';

    -- Construir asunto del correo
    SET @subject = 'REFERIDOS - SISTEMA ALFIN ' + CONVERT(NVARCHAR, GETDATE(), 103);

    -- Enviar correo
    EXEC msdb.dbo.sp_send_dbmail 
        @profile_name = 'WyA', 
        @recipients = 'santiagovl0308@gmail.com', 
        @body = @mensaje, 
        @body_format = 'HTML', 
        @subject = @subject;
    
    RETURN;
END

EXEC sp_referir_enviar_emails_de_referencia '20568075';


SELECT TOP 500 dni FROM base_clientes 
join detalle_base on base_clientes.id_base = detalle_base.id_base
ORDER BY base_clientes.id_base DESC;

20568075
43573431

celularRegex
select * from clientes_referidos order by fecha_referido desc;


select * from usuarios where id_rol = 4