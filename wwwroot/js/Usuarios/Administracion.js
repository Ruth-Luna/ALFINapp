function cambiarDatos(idUsuario) {
    var datosUsuario = {
        IdUsuario: parseInt(idUsuario),
        Dni: document.getElementById('DNI_' + idUsuario).value.trim(),
        NombresCompletos: document.getElementById('NombresCompletos_' + idUsuario).value.trim(),
        Rol: document.getElementById('Rol_' + idUsuario).value.trim(),
        Departamento: document.getElementById('Departamento_' + idUsuario).value.trim(),
        Provincia: document.getElementById('Provincia_' + idUsuario).value.trim(),
        Distrito: document.getElementById('Distrito_' + idUsuario).value.trim(),
        Telefono: document.getElementById('Telefono_' + idUsuario).value.trim(),
        Estado: document.getElementById('Estado_' + idUsuario).value.trim(),
        IDUSUARIOSUP: parseInt(document.getElementById('IDUSUARIOSUP_' + idUsuario).value),
        RESPONSABLESUP: document.getElementById('RESPONSABLESUP_' + idUsuario).value.trim(),
        REGION: document.getElementById('REGION_' + idUsuario).value.trim(),
        NOMBRECAMPAÑA: document.getElementById('NOMBRECAMPAÑA_' + idUsuario).value.trim(),
        IdRol: parseInt(document.getElementById('IdRol_' + idUsuario).value),
    };
    $.ajax({
        url: "/Usuarios/ModificarUsuario",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(datosUsuario),
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al actualizar los datos',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                setTimeout(function () {
                    location.reload();
                }, 2000);
            } else {
                Swal.fire({
                    title: 'Datos actualizados correctamente',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'OK'
                });
                setTimeout(function () {
                    location.reload();
                }, 2000);
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
            setTimeout(function () {
                location.reload();
            }, 2000);
        }
    });
}

function CambiarEstadoUsuario(accion, idUsuario) {
    $.ajax({
        url: "/Usuarios/CambiarEstadoUsuario",
        type: "POST",
        data: {
            IdUsuario: idUsuario,
            accion: accion
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cambiar el estado del Usuario',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                setTimeout(function () {
                    location.reload();
                }, 2000);
            } else {
                Swal.fire({
                    title: 'El Estado del asesor fue cambiado correctamente',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'OK'
                });
                setTimeout(function () {
                    location.reload();
                }, 2000);
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
            setTimeout(function () {
                location.reload();
            }, 2000);
        }
    });
}

function habilitarEdicion(campo) {
    
}