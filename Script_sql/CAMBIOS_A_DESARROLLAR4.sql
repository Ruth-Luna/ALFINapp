SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_USUARIO_ACTUALIZAR_USUARIO]
    @id_usuario INT,
    @dni NVARCHAR(20) = NULL,
    @Apellido_Paterno NVARCHAR(50) = NULL,
    @Apellido_Materno NVARCHAR(50) = NULL,
    @Nombres NVARCHAR(50) = NULL,
    @Departamento NVARCHAR(50) = NULL,
    @Provincia NVARCHAR(50) = NULL,
    @Distrito NVARCHAR(50) = NULL,
    @Telefono NVARCHAR(11) = NULL,
    @estado NVARCHAR(50) = NULL,
    @IDUSUARIOSUP INT = NULL,
    @RESPONSABLESUP NVARCHAR(100) = NULL,
    @region NVARCHAR(50) = NULL,
    @NOMBRE_CAMPANIA NVARCHAR(100) = NULL,
    @idRol INT = NULL,
    @Usuario NVARCHAR(50) = NULL,
    @contrasenia NVARCHAR(100) = NULL,  
    @correo NVARCHAR(100) = NULL,
    @tipo_doc NVARCHAR(10) = NULL
AS
BEGIN

        -- Validar si el DNI ya existe para otro usuario
    IF @dni IS NOT NULL AND EXISTS (SELECT 1 FROM usuarios WHERE dni = UPPER(@dni) AND id_usuario != @id_usuario)
    BEGIN
        SELECT 'El DNI ingresado ya se encuentra insertado' AS mensaje, CAST(0 AS BIT) AS resultado;
        RETURN;
    END

    -- Validar que el usuario a actualizar existe
    IF NOT EXISTS (SELECT 1 FROM usuarios WHERE id_usuario = @id_usuario)
    BEGIN
        SELECT 'El usuario no existe' AS mensaje, CAST(0 AS BIT) AS resultado;
        RETURN;
    END

    DECLARE @NombreCompleto NVARCHAR(150) = 
        TRIM(
            ISNULL(UPPER(TRIM(@Nombres)), '') + ' ' + 
            ISNULL(UPPER(TRIM(@Apellido_Paterno)), '') + ' ' + 
            ISNULL(UPPER(TRIM(@Apellido_Materno)), '')
        );
    UPDATE usuarios
    SET 
        --dni = CASE WHEN @dni IS NULL OR LTRIM(RTRIM(@dni)) = '' THEN dni ELSE UPPER(@dni) END,
        dni = CASE WHEN @dni IS NULL THEN dni ELSE UPPER(@dni) END,
        --tipo_documento = CASE WHEN @tipo_doc IS NULL OR LTRIM(RTRIM(@tipo_doc)) = '' THEN NULL ELSE UPPER(@tipo_doc) END,
        tipo_documento = CASE WHEN @tipo_doc IS NULL THEN NULL ELSE UPPER(@tipo_doc) END,
        --Apellido_Paterno = CASE WHEN @Apellido_Paterno IS NULL OR LTRIM(RTRIM(@Apellido_Paterno)) = '' THEN NULL ELSE UPPER(@Apellido_Paterno) END,
        Apellido_Paterno = CASE WHEN @Apellido_Paterno IS NULL THEN NULL ELSE UPPER(@Apellido_Paterno) END,
        --Apellido_Materno = CASE WHEN @Apellido_Materno IS NULL OR LTRIM(RTRIM(@Apellido_Materno)) = '' THEN NULL ELSE UPPER(@Apellido_Materno) END,
        Apellido_Materno = CASE WHEN @Apellido_Materno IS NULL THEN NULL ELSE UPPER(@Apellido_Materno) END,
        --Nombres = CASE WHEN @Nombres IS NULL OR LTRIM(RTRIM(@Nombres)) = '' THEN NULL ELSE UPPER(@Nombres) END,
        Nombres = CASE WHEN @Nombres IS NULL THEN NULL ELSE UPPER(@Nombres) END,
        --Nombres_Completos = CASE 
        --    WHEN (@Nombres IS NULL OR @Apellido_Paterno IS NULL OR @Apellido_Materno IS NULL
        --          OR LTRIM(RTRIM(@Nombres)) = '' OR LTRIM(RTRIM(@Apellido_Paterno)) = '' OR LTRIM(RTRIM(@Apellido_Materno)) = '') 
        --    THEN NULL
        --    ELSE @NombreCompleto 
        --END,
        Nombres_Completos = CASE WHEN @NombreCompleto IS NULL OR TRIM(@NombreCompleto) = '' THEN NULL ELSE @NombreCompleto END,
        --Departamento = CASE WHEN @Departamento IS NULL OR LTRIM(RTRIM(@Departamento)) = '' THEN NULL ELSE UPPER(@Departamento) END,
        departamento = CASE WHEN @Departamento IS NULL THEN NULL ELSE UPPER(@Departamento) END,
        --Provincia = CASE WHEN @Provincia IS NULL OR LTRIM(RTRIM(@Provincia)) = '' THEN NULL ELSE UPPER(@Provincia) END,
        provincia = CASE WHEN @Provincia IS NULL THEN NULL ELSE UPPER(@Provincia) END,
        --Distrito = CASE WHEN @Distrito IS NULL OR LTRIM(RTRIM(@Distrito)) = '' THEN NULL ELSE UPPER(@Distrito) END,
        distrito = CASE WHEN @Distrito IS NULL THEN NULL ELSE UPPER(@Distrito) END,
        --Telefono = CASE WHEN @Telefono IS NULL OR LTRIM(RTRIM(@Telefono)) = '' THEN NULL ELSE @Telefono END,
        telefono = CASE WHEN @Telefono IS NULL THEN NULL ELSE @Telefono END,
        --Estado = CASE WHEN @estado IS NULL OR LTRIM(RTRIM(@estado)) = '' THEN NULL ELSE UPPER(@estado) END,
        Estado = CASE WHEN @estado IS NULL THEN NULL ELSE UPPER(@estado) END,
        ID_USUARIO_SUP = CASE WHEN @IDUSUARIOSUP IS NULL THEN NULL ELSE @IDUSUARIOSUP END,
        --RESPONSABLE_SUP = CASE WHEN @RESPONSABLESUP IS NULL OR LTRIM(RTRIM(@RESPONSABLESUP)) = '' THEN NULL ELSE UPPER(@RESPONSABLESUP) END,
        RESPONSABLE_SUP = CASE WHEN @RESPONSABLESUP IS NULL THEN NULL ELSE UPPER(@RESPONSABLESUP) END,
        --REGION = CASE WHEN @region IS NULL OR LTRIM(RTRIM(@region)) = '' THEN NULL ELSE UPPER(@region) END,
        REGION = CASE WHEN @region IS NULL THEN NULL ELSE UPPER(@region) END,
        --NOMBRE_CAMPAÑA = CASE WHEN @NOMBRE_CAMPANIA IS NULL OR LTRIM(RTRIM(@NOMBRE_CAMPANIA)) = '' THEN NULL ELSE UPPER(@NOMBRE_CAMPANIA) END,
        NOMBRE_CAMPAÑA = CASE WHEN @NOMBRE_CAMPANIA IS NULL THEN NULL ELSE UPPER(@NOMBRE_CAMPANIA) END,
        id_rol = CASE WHEN @idRol IS NULL THEN NULL ELSE @idRol END,
        --usuario = CASE WHEN @Usuario IS NULL OR LTRIM(RTRIM(@Usuario)) = '' THEN NULL ELSE UPPER(@Usuario) END,
        usuario = CASE WHEN @Usuario IS NULL THEN usuario ELSE UPPER(@Usuario) END,
        --contrasenia = CASE 
        --                 WHEN @contrasenia IS NULL OR LTRIM(RTRIM(@contrasenia)) = '' 
        --                      THEN contrasenia 
        --                  ELSE dbo.FN_EncriptarAES(@contrasenia) 
        --              END,
        contrasenia = CASE 
                WHEN @contrasenia IS NULL 
                THEN contrasenia 
                ELSE dbo.FN_EncriptarAES(@contrasenia) 
            END,
        --correo = CASE WHEN @correo IS NULL OR LTRIM(RTRIM(@correo)) = '' THEN NULL ELSE LOWER(@correo) END,
        correo = CASE WHEN @correo IS NULL THEN NULL ELSE LOWER(@correo) END,
        fecha_actualizacion = GETDATE()
    WHERE id_usuario = @id_usuario;

        -- Verificar si se actualizó correctamente
    IF @@ROWCOUNT > 0
    BEGIN
        SELECT 'Usuario actualizado correctamente' AS mensaje, CAST(1 AS BIT) AS resultado;
    END
    ELSE
    BEGIN
        SELECT 'No se pudo actualizar el usuario' AS mensaje, CAST(0 AS BIT) AS resultado;
    END

END

