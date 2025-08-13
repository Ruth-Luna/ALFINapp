GO
ALTER PROCEDURE SP_SUPERVISORES_GET_DETALLES_ASIGNACIONES_POR_ASESORES
    @Asesores dbo.IdLists READONLY
AS
BEGIN
    SELECT 
        a.ids AS idUsuarioA,
        u.Nombres_Completos AS nombreUsuarioA,
        COUNT(ca.id_cliente) AS totalClientesAsignados,
        SUM(CASE WHEN ca.id_tipificacion_mayor_peso IS NOT NULL THEN 1 ELSE 0 END) AS totalClientesGestionados,
        SUM(CASE WHEN ca.id_tipificacion_mayor_peso IS NULL THEN 1 ELSE 0 END) AS totalClientesPendientes,
        u.estado AS estaActivo
    FROM @Asesores a
    JOIN usuarios u 
        ON u.id_usuario = a.ids
    LEFT JOIN clientes_asignados ca 
        ON u.id_usuario = ca.id_usuarioV
    WHERE YEAR(ca.fecha_asignacion_vendedor) = YEAR(GETDATE())
    AND MONTH(ca.fecha_asignacion_vendedor) = MONTH(GETDATE())
    GROUP BY a.ids, u.Nombres_Completos, u.estado
END

DECLARE @misAsesores dbo.IdLists;
INSERT INTO @misAsesores (ids)
VALUES (2108), (3109), (3110), (3111);

EXEC SP_GET_DETALLES_ASIGNACIONES_POR_ASESORES @misAsesores;

select * from usuarios where id_rol = 3;

SELECT * FROM derivaciones_asesores WHERE dni_cliente = '20093497'