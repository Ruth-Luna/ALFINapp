alter PROCEDURE dbo.SP_REFERIDOS_GET_REFERIDOS_POR_DNI
    @DNI NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Declarar tabla temporal para almacenar referidos
    CREATE TABLE #Referidos
    (
        IdReferido INT,
        IdBaseClienteA365 INT,
        IdBaseClienteBanco INT,
        IdSupervisorReferido INT,
        NombreCompletoAsesor NVARCHAR(200),
        NombreCompletoCliente NVARCHAR(200),
        DniAsesor NVARCHAR(20),
        DniCliente NVARCHAR(20),
        FechaReferido DATETIME,
        TraidoDe NVARCHAR(200),
        Telefono NVARCHAR(50),
        Agencia NVARCHAR(100),
        FechaVisita DATETIME,
        OfertaEnviada DECIMAL(10, 2),
        FueProcesado BIT,
        CelularAsesor NVARCHAR(50),
        CorreoAsesor NVARCHAR(100),
        CciAsesor NVARCHAR(50),
        DepartamentoAsesor NVARCHAR(100),
        UbigeoAsesor NVARCHAR(20),
        BancoAsesor NVARCHAR(100),
        EstadoReferencia NVARCHAR(50),
        EstadoDesembolso NVARCHAR(50),
        FECHA_DESEMBOLSOS DATETIME NULL
    );

    -- Insertar los referidos filtrados por DNI y mes/año actual
    INSERT INTO #Referidos
        (
        IdReferido, IdBaseClienteA365, IdBaseClienteBanco, IdSupervisorReferido,
        NombreCompletoAsesor, NombreCompletoCliente, DniAsesor, DniCliente, FechaReferido,
        TraidoDe, Telefono, Agencia, FechaVisita, OfertaEnviada, FueProcesado,
        CelularAsesor, CorreoAsesor, CciAsesor, DepartamentoAsesor, UbigeoAsesor,
        BancoAsesor, EstadoReferencia, EstadoDesembolso, FECHA_DESEMBOLSOS
        )
    SELECT
        cr.id_referido,
        cr.id_base_cliente_a365,
        cr.id_base_cliente_banco,
        cr.id_supervisor_referido,
        cr.nombre_completo_asesor,
        cr.nombre_completo_cliente,
        cr.dni_asesor,
        cr.dni_cliente,
        cr.fecha_referido,
        cr.traido_de,
        cr.telefono_cliente,
        cr.agencia_referido,
        cr.fecha_visita_agencia,
        cr.oferta_enviada,
        cr.fue_procesado,
        cr.celular_asesor,
        cr.correo_asesor,
        cr.cci_asesor,
        cr.departamento_asesor,
        cr.ubigeo_asesor,
        cr.banco_asesor,
        cr.estado_referencia,
        NULL,
        -- FECHA_DESEMBOLSOS se inicializa como NULL, ya que no se conoce en este momento
        NULL
    -- EstadoDesembolso se actualizará luego
    FROM clientes_referidos cr
    WHERE cr.dni_asesor = @DNI
        AND cr.fecha_referido IS NOT NULL
        AND YEAR(cr.fecha_referido) = YEAR(GETDATE())
        AND MONTH(cr.fecha_referido) = MONTH(GETDATE());

    UPDATE r
        SET
            EstadoDesembolso = CASE
                                   WHEN d.dni_desembolso IS NULL THEN 'NO DESEMBOLSADO'
                                   ELSE 'DESEMBOLSADO'
                               END,
            FECHA_DESEMBOLSOS = d.FECHA_DESEMBOLSOS
        FROM #Referidos r
        OUTER APPLY (
            SELECT TOP 1
            d.dni_desembolso, d.FECHA_DESEMBOLSOS
        FROM desembolsos d
        WHERE d.dni_desembolso = r.DniCliente
            AND d.FECHA_DESEMBOLSOS IS NOT NULL
            AND r.FechaReferido IS NOT NULL
            AND YEAR(d.FECHA_DESEMBOLSOS) = YEAR(r.FechaReferido)
            AND MONTH(d.FECHA_DESEMBOLSOS) = MONTH(r.FechaReferido)
        ORDER BY d.FECHA_DESEMBOLSOS DESC
        ) d;


    -- Devolver el resultado
    SELECT
        r.*
    FROM #Referidos r;
END

EXEC dbo.SP_REFERIDOS_GET_REFERIDOS_POR_DNI @DNI = '73393133';

GO
CREATE PROCEDURE dbo.SP_REFERIDOS_GET_ALL_REFERIDOS
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.*
    FROM clientes_referidos r
    WHERE r.fecha_referido IS NOT NULL
        AND YEAR(r.fecha_referido) = YEAR(GETDATE())
        AND MONTH(r.fecha_referido) = MONTH(GETDATE());
END

GO
CREATE PROCEDURE dbo.SP_REFERIDOS_GET_REFERIDO_BY_ID
    @IdReferido INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.*
    FROM clientes_referidos r
    WHERE r.id_referido = @IdReferido;
END