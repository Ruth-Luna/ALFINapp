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

function activarEdicion(idUsuario) {
    const DNI = document.getElementById('DNI_' + idUsuario);
    const NombresCompletos = document.getElementById('NombresCompletos_' + idUsuario);
    const REGION = document.getElementById('REGION_' + idUsuario);
    const NOMBRECAMPAÑA = document.getElementById('NOMBRECAMPAÑA_' + idUsuario);
    const ModificarDatosBtn = document.getElementById('ModificarDatosBtn_' + idUsuario);
    const ActivarEdicionBtn = document.getElementById('ActivarEdicionBtn_' + idUsuario);
    const IDUSUARIOSUP = document.getElementById('IDUSUARIOSUP_' + idUsuario);
    const visualizacionIDUSUARIOSUP = document.getElementById('visualizacionIDUSUARIOSUP_' + idUsuario);
    const IdRol = document.getElementById('IdRol_' + idUsuario);
    const VISUALIZACIONROL = document.getElementById('VISUALIZACIONROL_' + idUsuario);
    Swal.fire({
        title: '¿Desea editar los datos del Usuario?',
        showDenyButton: true,
        confirmButtonText: `Editar`,
        denyButtonText: `Cancelar`,
    }).then((result) => {
        if (result.isConfirmed) {
            DNI.readOnly = false;
            DNI.style.backgroundColor = 'lightblue';
            NombresCompletos.readOnly = false;
            NombresCompletos.style.backgroundColor = 'lightblue';
            REGION.readOnly = false;
            REGION.style.backgroundColor = 'lightblue';
            NOMBRECAMPAÑA.readOnly = false;
            NOMBRECAMPAÑA.style.backgroundColor = 'lightblue';
            ModificarDatosBtn.style.display = 'block';
            ActivarEdicionBtn.style.display = 'none';
            IDUSUARIOSUP.style.display = 'block';
            visualizacionIDUSUARIOSUP.style.display = 'none';
            IdRol.style.display = 'block';
            VISUALIZACIONROL.style.display = 'none';
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