function ActualizarPermisosRoles(rol, idvista, idrol) {
    console.log(rol);
    console.log(idvista);
    console.log(idrol);
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
                    text: `El rol: ${rol} se actualizó correctamente la pagina se recargara en 3 segundos para ver los cambios correspondientes`,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });

                setTimeout(function() {
                    location.reload();
                }, 3000);
            }
        }
    });
}