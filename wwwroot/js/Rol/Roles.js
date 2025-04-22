function ActualizarPermisosRoles(rol, idvista, idrol) {
    console.log(rol);
    console.log(idvista);
    console.log(idrol);

    if (idvista === "1" || idvista === "6") {
        Swal.fire({
            title: 'Error al actualizar el rol',
            text: `El rol: ${rol} no se pudo actualizar, seleccione una vista`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (idvista ) {
        
    }
    $.ajax({
        url: "/Rol/ActualizarPermisosRoles",
        method: "POST",
        data: {
            rol: rol,
            idvista: idvista,
            idrol: idrol
        },

        success: function (response) {
            if (response.IsSuccess === false) {
                Swal.fire({
                    title: 'Error al actualizar el rol',
                    text: `El rol: ${rol} no se pudo actualizar`,
                    icon: 'warning',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Rol actualizado',
                    text: `El rol: ${rol} se actualizó correctamente la pagina se recargara para ver los cambios correspondientes`,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                }).then(() => location.reload());
            }
        }
    });
}