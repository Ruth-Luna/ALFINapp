function cambiarDatos(idUsuario) {
    var datosUsuario = {
        IdUsuario: parseInt(idUsuario),
        Dni: document.getElementById('txtDNI_EU').value.trim() || null,
        NombresCompletos: document.getElementById('txtNombreCompleto_EU').value.trim() || null,
        Estado: document.getElementById('selectEstado_EU').value.trim() === 'CAMBIO' ? null : document.getElementById('selectEstado_EU').value.trim() || null,
        IDUSUARIOSUP: parseInt(document.getElementById('txtSupervisor_EU').value),
        REGION: document.getElementById('txtRegion_EU').value.trim() || null,
        NOMBRECAMPAÑA: document.getElementById('txtNombreCampania_EU').value.trim() || null,
        IdRol: parseInt(document.getElementById('txtRol_EU').value) || null,
        FechaInicio: document.getElementById('dateFechaInicio_EU').value ? new Date(document.getElementById('dateFechaInicio_EU').value).toISOString() : null,
        FechaCese: document.getElementById('dateFechaCese_EU').value ? new Date(document.getElementById('dateFechaCese_EU').value).toISOString() : null,
        Correo: document.getElementById('txtCorreo_EU').value.trim() || null
    };
    if (datosUsuario.Dni === "" ||
        datosUsuario.NombresCompletos === "" ||
        datosUsuario.Estado === "" ||
        datosUsuario.IdRol === "" || 
        datosUsuario.Correo === "" ) {
        Swal.fire({
            title: 'Error al actualizar los datos',
            text: 'Debe completar los campos DNI, Nombres Completos o Rol',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        return;
    }

    console.log(datosUsuario);
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
                }, 3000);
            } else {
                Swal.fire({
                    title: 'Datos actualizados correctamente',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        location.reload();
                    }
                });
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    location.reload();
                }
            });
        }
    });
}

function mostrarSupervisor(IdRol) {
    divSupervisor_EU = document.getElementById("divSupervisor_EU");
    if (IdRol == 3) {
        divSupervisor_EU.style.display = "block";
    } else {
        divSupervisor_EU.style.display = "none";
    }
}

function mostrarFechas(estado) {
    let divFechaInicio_EU = document.getElementById("divFechaInicio_EU");
    let divFechaCese_EU = document.getElementById("divFechaCese_EU");
    let divFechas_EU = document.getElementById("divFechas_EU");
    let alertaFecha = document.getElementById("alertaFecha");
    if (estado == 'CAMBIO') {
        divFechaInicio_EU.style.display = "block";
        divFechaCese_EU.style.display = "block";
        divFechas_EU.style.display = "block";
        alertaFecha.style.display = "none";
    }
    else {
        divFechaInicio_EU.style.display = "none";
        divFechaCese_EU.style.display = "none";
        divFechas_EU.style.display = "none";
        alertaFecha.style.display = "none";
    }
}