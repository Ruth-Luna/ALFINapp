SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trg_agendamientos_re_after_insertion_and_update]
ON [dbo].[derivaciones_asesores]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Insertamos en agendamientos_reagendamientos con un JOIN a la tabla usuarios
    INSERT INTO agendamientos_reagendamientos
    (
        id_derivacion,
        oferta,
        fecha_visita,
        telefono,
        agencia,
        estado_derivacion,
        fue_enviado_email,
        fue_enviado_form,
        dni_supervisor,
        fecha_agendamiento,
        fecha_derivacion,
        dni_asesor,
        dni_cliente
    )
    SELECT
        i.id_derivacion,
        i.oferta_max,
        i.fecha_visita,
        i.telefono_cliente AS telefono,
        i.nombre_agencia AS agencia,
        i.estado_derivacion,
        i.fue_enviado_email,
        i.fue_procesado AS fue_enviado_form,
        u.dni AS dni_supervisor,
        GETDATE() AS fecha_agendamiento,
        GETDATE() AS fecha_derivacion,
        i.dni_asesor,
        i.dni_cliente
    FROM inserted i
    LEFT JOIN usuarios ua ON ua.dni = i.dni_asesor
    LEFT JOIN usuarios u ON u.ID_USUARIO = ua.ID_USUARIO_SUP
END;
GO

-- Habilitar el trigger, si es necesario (asegúrate que el nombre esté bien)
-- Si el nombre correcto del trigger es "trg_agendamientos_re_after_insertion", este ALTER no es necesario
-- y podrías querer eliminar esta línea o corregir el nombre si es un trigger previo
-- ALTER TABLE [dbo].[derivaciones_asesores] ENABLE TRIGGER [trg_derivaciones_asesores_after_insertion]


SELECT TOP 1200 * FROM base_clientes ORDER BY id_base DESC 


SELECT * FROM derivaciones_asesores ORDER BY id_derivacion DESC