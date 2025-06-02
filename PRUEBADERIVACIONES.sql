SELECT TOP (1000) [id_derivacion]
      ,[fecha_derivacion]
      ,[dni_asesor]
      ,[dni_cliente]
      ,[id_cliente]
      ,[nombre_cliente]
      ,[telefono_cliente]
      ,[nombre_agencia]
      ,[num_agencia]
      ,[fue_procesado]
      ,[fecha_visita]
      ,[estado_derivacion]
      ,[id_asignacion]
      ,[observacion_derivacion]
      ,[fue_enviado_email]
      ,[ID_DESEMBOLSO]
      ,[doc_supervisor]
      ,[oferta_max]
      ,[supervisor]
      ,[monto_desembolso]
      ,[real_error]
      ,[fue_reagendado]
      ,[fue_reprocesado]
      ,[evaluate]
      ,[fecha_reagendamiento]
  FROM [CORE_ALFIN].[dbo].[derivaciones_asesores]


  insert into [CORE_ALFIN].[dbo].[derivaciones_asesores] (
      [fecha_derivacion]
      ,[dni_asesor]
      ,[dni_cliente]
      ,[id_cliente]
      ,[nombre_cliente]
      ,[telefono_cliente]
      ,[nombre_agencia]
      ,[num_agencia]
      ,[fue_procesado]
      ,[fecha_visita]
      ,[estado_derivacion]
      ,[id_asignacion]
      ,[observacion_derivacion]
      ,[fue_enviado_email]
      ,[ID_DESEMBOLSO]
      ,[doc_supervisor]
      ,[oferta_max]
      ,[supervisor]
      ,[monto_desembolso]
      ,[real_error]
      ,[fue_reagendado]
      ,[fue_reprocesado]
      ,[evaluate]
      ,[fecha_reagendamiento])
    values (
        '2023-10-01'
        ,'12345678'
        ,'87654321'
        ,NULL
        ,'John Doe'
        ,'987654321'
        ,'Agencia Central'
        ,101
        ,1
        ,'2023-10-02'
        ,'deri_prueba'
        ,NULL
        ,'Observación de prueba'
        ,1
        ,NULL
        ,'11111111'
        ,5000.00
        ,'Supervisor A'
        ,NULL
        ,'prueba'
        ,0
        ,0
        ,NULL
        ,'2023-10-03');