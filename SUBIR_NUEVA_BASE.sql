SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BASE_A365_20250623](
	[PROPENSION_IC] [varchar](50) NULL,
	[USER_V3] [varchar](50) NULL,
	[DNI] [varchar](50) NULL,
	[X_APPATERNO] [varchar](50) NULL,
	[X_APMATERNO] [varchar](50) NULL,
	[X_NOMBRE] [varchar](50) NULL,
	[OFERTA_MAX] [varchar](50) NULL,
	[tasa_minima] [varchar](50) NULL,
	[PLAZO] [varchar](50) NULL,
	[CUOTA] [varchar](50) NULL,
	[CAPACIDAD_MAX] [varchar](50) NULL,
	[TIPO_GEST] [varchar](50) NULL,
	[TIPO_CLIENTE_COMERCIAL] [varchar](50) NULL,
	[CampaÃ±a] [varchar](50) NULL,
	[SALDO_DIFERENCIAL_REENG] [varchar](50) NULL,
	[TIPO_CLIENTE] [varchar](50) NULL,
	[color_final] [varchar](50) NULL,
	[PERFIL_RO] [varchar](50) NULL,
	[TIPO_BASE] [varchar](50) NULL,
	[DEPARTAMENTO] [varchar](50) NULL,
	[PROVINCIA] [varchar](50) NULL,
	[DISTRITO] [varchar](50) NULL,
	[SUCURSAL_COMERCIAL] [varchar](50) NULL,
	[Agencia_comercial] [varchar](50) NULL,
	[REGION_COMERCIAL] [varchar](50) NULL,
	[VARIACION_OFERTA_CAMPAÃ‘A_ANTERIOR] [varchar](50) NULL,
	[VARIACION_TASA_CAMPAÃ‘A_ANTERIOR] [varchar](50) NULL,
	[VAR_TASA_CREDITO_ANTERIOR] [varchar](50) NULL,
	[FLG_CET_6M] [varchar](50) NULL,
	[FLAG_DEUDA_V_OFERTA] [varchar](50) NULL,
	[GRUPO_TASA] [varchar](50) NULL,
	[GRUPO_MONTO] [varchar](50) NULL,
	[MGNEG] [varchar](50) NULL,
	[Tasa_1] [varchar](50) NULL,
	[Tasa_2] [varchar](50) NULL,
	[Tasa_3] [varchar](50) NULL,
	[Tasa_4] [varchar](50) NULL,
	[Tasa_5] [varchar](50) NULL,
	[Tasa_6] [varchar](50) NULL,
	[Tasa_7] [varchar](50) NULL,
	[Edad] [varchar](50) NULL,
	[RANGO_EDAD] [varchar](50) NULL,
	[RANGO_OFERTA] [varchar](50) NULL,
	[RANGO_SUELDO] [varchar](50) NULL,
	[BLOQUE] [varchar](50) NULL,
	[FRESCURA] [varchar](50) NULL
) ON [PRIMARY]
GO

SELECT TOP 150 * FROM [dbo].[BASE_A365_20250623]


INSERT INTO base_clientes (
    DNI, X_NOMBRE, X_APPATERNO, X_APMATERNO, 
    departamento, provincia, distrito, edad
)
SELECT 
    DNI, 
    X_NOMBRE, 
    X_APPATERNO, 
    X_APMATERNO, 
    DEPARTAMENTO, 
    PROVINCIA, 
    DISTRITO, 
    CASE 
        WHEN ISNUMERIC(Edad) = 1 THEN CAST(Edad AS INT)
        ELSE NULL
    END
FROM [dbo].[BASE_A365_20250623] AS B
WHERE NOT EXISTS (
    SELECT 1 
    FROM base_clientes AS C
    WHERE C.DNI = B.DNI
)

INSERT INTO detalle_base 


select top 150 * from base_clientes ORDER BY id_base


INSERT INTO detalle_base (
	id_base, sucursal_comercial, agencia_comercial, region_comercial, 
	oferta_max, tasa_minima, tasa_1, tasa_2, tasa_3, tasa_4, tasa_5, tasa_6, tasa_7,
	plazo, cuota, rango_edad, rango_oferta, rango_sueldo,
	capacidad_max, tipo_gest, tipo_cliente, color_final, 
	propension_ic, user_v3, tipo_base, flag_deuda_v_oferta, perfil_ro, mgneg,
	VARIACION_OFERTA_CAMPAÑA_ANTERIOR, VARIACION_TASA_CAMPAÑA_ANTERIOR,
	VAR_TASA_CREDITO_ANTERIOR, flg_cet_6m, bloque, frescura
)
SELECT 
	C.id_base,
	B.SUCURSAL_COMERCIAL,
	B.Agencia_comercial,
	B.REGION_COMERCIAL,
	TRY_CAST(B.OFERTA_MAX AS decimal(10,2)),
	TRY_CAST(B.tasa_minima AS decimal(5,2)),
	TRY_CAST(B.Tasa_1 AS decimal(5,2)),
	TRY_CAST(B.Tasa_2 AS decimal(5,2)),
	TRY_CAST(B.Tasa_3 AS decimal(5,2)),
	TRY_CAST(B.Tasa_4 AS decimal(5,2)),
	TRY_CAST(B.Tasa_5 AS decimal(5,2)),
	TRY_CAST(B.Tasa_6 AS decimal(5,2)),
	TRY_CAST(B.Tasa_7 AS decimal(5,2)),
	TRY_CAST(B.PLAZO AS int),
	TRY_CAST(B.CUOTA AS decimal(10,2)),
	B.RANGO_EDAD,
	B.RANGO_OFERTA,
	B.RANGO_SUELDO,
	TRY_CAST(B.CAPACIDAD_MAX AS decimal(20,3)),
	B.TIPO_GEST,
	B.TIPO_CLIENTE,
	B.color_final,
	TRY_CAST(B.PROPENSION_IC AS int),
	B.USER_V3,
	B.TIPO_BASE,
	TRY_CAST(B.FLAG_DEUDA_V_OFERTA AS int),
	B.PERFIL_RO,
	TRY_CAST(B.MGNEG AS decimal(6,3)),
	TRY_CAST(B.[VARIACION_OFERTA_CAMPAÃ‘A_ANTERIOR] AS int),
	TRY_CAST(B.[VARIACION_TASA_CAMPAÃ‘A_ANTERIOR] AS decimal(8,2)),
	TRY_CAST(B.VAR_TASA_CREDITO_ANTERIOR AS decimal(8,2)),
	TRY_CAST(B.FLG_CET_6M AS int),
	B.BLOQUE,
	TRY_CAST(B.FRESCURA AS int)
FROM [dbo].[BASE_A365_20250623] AS B
JOIN base_clientes AS C ON C.DNI = B.DNI



SELECT COUNT(*) AS TotalRegistros
FROM BASE_A365_20250623
WHERE DNI IS NOT NULL;
