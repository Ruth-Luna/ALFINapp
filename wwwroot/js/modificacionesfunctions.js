function agregarNuevoAsesorView() {
    // Realiza una llamada AJAX para cargar la vista parcial
    $.ajax({
        url: "/Asesores/AgregarNuevoAsesorView", // URL del controlador que devuelve la vista parcial
        type: "GET",
        success: function (response) {
            $('#interfazAgregarAsesor').html(response);
            $('#interfazAgregarAsesor').css('display', 'block'); // Asegurar que el contenedor sea visible
            $('#seccionAgregarAsesores').css('display', 'none');
        },
        error: function () {
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la interfaz para agregar un nuevo asesor.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function cerraragregarNuevoAsesorView() {
    $('#interfazAgregarAsesor').css('display', 'none');
    $('#seccionAgregarAsesores').css('display', 'block');
}

function desactivarAsesor(dni, idUsuario) {
    $.ajax({
        type: "POST",
        url: "/Asesor/DesactivarAsesor",
        data: {
            dni: dni,
            idUsuario: idUsuario
        },
        success: function (response) {
            if (response) {
                Swal.fire({
                    title: 'Desactivacion de Asesor',
                    text: 'El Asesor fue desactivado correctamente.',
                    icon: 'warning',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Desactivacion de Asesor',
                    text: 'Ocurrio un error al tratar de Desactivar al Asesor comunicarse con el servicio tecnico.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (message) {
            Swal.fire({
                title: 'Desactivacion de Asesor',
                text: 'Ocurrio un error al tratar de Desactivar al Asesor comunicarse con el servicio tecnico.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function activarAsesor(dni, idUsuario) {
    $.ajax({
        type: "POST",
        url: "/Asesor/ActivarAsesor",
        data: {
            dni: dni,
            idUsuario: idUsuario
        },
        success: function (response) {
            if (response) {
                Swal.fire({
                    title: 'Activacion de Asesor',
                    text: 'El Asesor fue activado correctamente.',
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Activacion de Asesor',
                    text: 'Ocurrio un error al tratar de Activar al Asesor comunicarse con el servicio tecnico.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (response) {
            Swal.fire({
                title: 'Activacion de Asesor',
                text: 'Ocurrio un error al tratar de Activar al Asesor comunicarse con el servicio tecnico.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function modificarAsignacionesAsesor(IdUsuario, dni) {
    var labelForAsesor = document.getElementById('seccionModificarAsignaciones_' + IdUsuario);
    // Realiza una llamada AJAX para cargar la vista parcial
    $.ajax({
        url: "/Supervisor/ObtenerVistaModificarAsignaciones", // URL del controlador que devuelve la vista parcial
        type: "GET",
        data: {
            IdUsuario: IdUsuario,
            dni: dni
        },
        success: function (response) {
            $('#seccionModificarAsignaciones_' + IdUsuario).html(response);
            $('#seccionModificarAsignaciones_' + IdUsuario).css('display', 'block'); // Asegurar que el contenedor sea visible
        },
        error: function () {
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la interfaz para modificar las asignaciones del asesor.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function cerrarmodificarAsignacionesAsesor(IdUsuario) {
    $('#seccionModificarAsignaciones_' + idUsuario).css('display', 'none'); // Asegurar que el contenedor sea visible
}

function cargarInterfazAsesor(idAsesor) {
    // Realiza una llamada AJAX para cargar la vista parcial
    if (idAsesor) {
        // Realiza una llamada AJAX para obtener la interfaz del asesor seleccionado
        $.ajax({
            url: "/Supervisor/ObtenerInterfazAsesor", // URL del controlador que devuelve la vista parcial
            type: "GET",
            data: { idUsuario: idAsesor },
            success: function (response) {
                if (response.success === false) {
                    Swal.fire({
                        title: 'Error al cargar la vista',
                        text: response.message,
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                }
                else {
                    // Cargar la interfaz en el div correspondiente
                    $('#interfazAsesorSelect').html(response);
                    $('#interfazAsesorSelect').css('display', 'block'); // Asegurar que el contenedor sea visible
                }
            },
            error: function () {
                Swal.fire({
                    title: 'Error',
                    text: 'No se pudo cargar la interfaz del asesor seleccionado.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        });
    } else {
        // Si no se selecciona un asesor, limpiar el contenido
        $('#interfazAsesorSelect').html('');
    }
}
function cerrarInterfazAsesor() {
    $('#cargarActualizacionDeAsignacionXAsesor').css('display', 'none'); // Asegurar que el contenedor sea visible
}

function cargarAsesoresTipificacionesView() {
    $.ajax({
        url: "/Tipificacionesasignacion/ModificarAsignacionPorTipificacionView",
        type: "GET",
        data: {},
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar la vista',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
            else {
                // Cargar la interfaz en el div correspondiente
                $('#interfazTipificacionXAsesorSelect').html(response);
                $('#interfazTipificacionXAsesorSelect').css('display', 'block'); // Asegurar que el contenedor sea visible
                $('#modificarXTipificacionSeccion').css('display', 'none'); // Asegurar que el contenedor sea visible
            }
        },
        error: function () {
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la interfaz del asesor seleccionado.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function cerrarAsesoresTipificacionView() {
    $('#interfazTipificacionXAsesorSelect').css('display', 'none'); // Asegurar que el contenedor sea visible
    $('#modificarXTipificacionSeccion').css('display', 'block'); // Asegurar que el contenedor sea visible
}

function cargarActualizacionDeAsignacionXAsesor(idUsuarioBusqueda) {
    $.ajax({
        url: "/Asignaciones/CargarActualizarAsignacion",
        data: { idUsuario: idUsuarioBusqueda },
        type: "GET",
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar la vista',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
            else {
                // Cargar la interfaz en el div correspondiente
                $('#cargarActualizacionDeAsignacionXAsesor').html(response);
                $('#cargarActualizacionDeAsignacionXAsesor').css('display', 'block'); // Asegurar que el contenedor sea visible
            }
        },
        error: function () {
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la interfaz para actualizar un asesor.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}