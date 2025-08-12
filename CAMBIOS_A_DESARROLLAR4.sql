GO
CREATE PROCEDURE SP_SUPERVISORES_CONSULTAR_VENTAS_POR_DESTINO
    @IdSupervisorActual INT,
    @Destino NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Destino IS NULL
    BEGIN
        SELECT *
        FROM clientes_asignados
        WHERE id_usuarioS = @IdSupervisorActual
          AND fecha_asignacion_sup IS NOT NULL
          AND YEAR(fecha_asignacion_sup) = YEAR(GETDATE())
          AND MONTH(fecha_asignacion_sup) = MONTH(GETDATE());
    END
    ELSE
    BEGIN
        SELECT *
        FROM clientes_asignados
        WHERE id_usuarioS = @IdSupervisorActual
          AND fecha_asignacion_sup IS NOT NULL
          AND YEAR(fecha_asignacion_sup) = YEAR(GETDATE())
          AND MONTH(fecha_asignacion_sup) = MONTH(GETDATE())
          AND destino = @Destino;
    END
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_USUARIO_ACTUALIZAR_USUARIO]
    @id_usuario int,
    @dni NVARCHAR(20) = NULL,
    @tipo_doc int = NULL,
    @Apellido_Paterno NVARCHAR(50) = NULL,
    @Apellido_Materno NVARCHAR(50) = NULL,
    @Usuario NVARCHAR(50) = NULL,
    @contraseniaH VARBINARY(MAX) = NULL,
    @Nombres NVARCHAR(50) = NULL,
    @NOMBRE_CAMPANIA NVARCHAR(100) = NULL,
    @rol NVARCHAR(50) = NULL,
    @region NVARCHAR(50) = NULL,
    @correo NVARCHAR(100) = NULL,
    @estado NVARCHAR(50) = NULL
AS
BEGIN
    UPDATE usuarios
    SET 
        dni = CASE WHEN @dni IS NOT NULL OR @dni <> '' THEN @dni ELSE dni END,
        tipo_doc = CASE WHEN @tipo_doc IS NOT NULL OR @tipo_doc <= 0 THEN @tipo_doc ELSE tipo_doc END,
        Apellido_Paterno = CASE WHEN @Apellido_Paterno IS NOT NULL OR @Apellido_Paterno <> '' THEN @Apellido_Paterno ELSE Apellido_Paterno END,
        Apellido_Materno = CASE WHEN @Apellido_Materno IS NOT NULL OR @Apellido_Materno <> '' THEN @Apellido_Materno ELSE Apellido_Materno END,
        Usuario = CASE WHEN @Usuario IS NOT NULL OR @Usuario <> '' THEN @Usuario ELSE Usuario END,
        contraseñaH = CASE WHEN @contraseniaH IS NOT NULL THEN @contraseniaH ELSE contraseñaH END,
        Nombres = CASE WHEN @Nombres IS NOT NULL OR @Nombres <> '' THEN @Nombres ELSE Nombres END,
        NOMBRE_CAMPAÑA = CASE WHEN @NOMBRE_CAMPANIA IS NOT NULL OR @NOMBRE_CAMPANIA <> '' THEN @NOMBRE_CAMPANIA ELSE NOMBRE_CAMPAÑA END,
        rol = CASE WHEN @rol IS NOT NULL OR @rol <> '' THEN @rol ELSE rol END,
        region = CASE WHEN @region IS NOT NULL OR @region <> '' THEN @region ELSE region END,
        correo = CASE WHEN @correo IS NOT NULL OR @correo <> '' THEN @correo ELSE correo END,
        estado = CASE WHEN @estado IS NOT NULL OR @estado <> '' THEN @estado ELSE estado END
    WHERE id_usuario = @id_usuario;
END;
GO


GO
CREATE PROCEDURE SP_USUARIO_GET_USUARIO_DNI
    @dni NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM usuarios
    WHERE dni = @dni;
END;


GO
CREATE PROCEDURE SP_USUARIO_GET_USUARIO_OCULTO_DNI
    @dni NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Asesores_Ocultos
    WHERE DNI_VICIDIAL = @dni;
END;

sp_help Asesores_Ocultos;