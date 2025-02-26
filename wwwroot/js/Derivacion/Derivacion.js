function cargarDerivacionesXAsesor(DniAsesor) {
    if (DniAsesor == "") {
        document.getElementById("DerivacionXAsesor").style = "display: none;"
        document.getElementById("DerivacionesSupervisorGeneral").style = "display: block;"
    } else {
        $.ajax({
            type: 'GET',
            url: '/Derivacion/ObtenerDerivacionesXAsesor',
            data: {
                DniAsesor: DniAsesor
            },
            success: function (response) {
                if (response.success === false) {
                    Swal.fire({
                        title: 'Error al cargar las derivaciones',
                        text: response.message,
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                } else {
                    $('#DerivacionXAsesor').html(response);
                    document.getElementById("DerivacionXAsesor").style = "display: block;"
                    document.getElementById("DerivacionesSupervisorGeneral").style = "display: none;"
                }
            },
            error: function (error) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: 'Hubo un error inesperado al cargar las derivaciones.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        });

    }

}

function cargarDerivacionesSistema(DniAsesor) {
    $.ajax({
        type: 'GET',
        url: '/Derivacion/cargarDerivacionesGestion',
        data: {
            DniSupervisorGeneral: DniSupervisorGeneral
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#DerivacionesSupervisorGeneral').html(response);
                document.getElementById("DerivacionXAsesor").style = "display: none;"
                document.getElementById("DerivacionesSupervisorGeneral").style = "display: block;"
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las derivaciones',
                text: 'Hubo un error inesperado al cargar las derivaciones.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function cargarDerivacionesGestion(DniAsesor) {
    $.ajax({
        type: 'GET',
        url: '/Derivacion/ObtenerDerivacionesGestion',
        data: {
            DniAsesor: DniAsesor
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#tablaDerivacionesGestion').html(response);
                document.getElementById("tablaDerivacionesSistema").style = "display: none;"
                document.getElementById("tablaDerivacionesGestion").style = "display: block;"
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las derivaciones',
                text: 'Hubo un error inesperado al cargar las derivaciones.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}