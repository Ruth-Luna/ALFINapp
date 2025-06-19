function guardarComentarioGeneral(idAsignacion) {
    let id_to_find = `comentarioGeneral[${idAsignacion}]`;
    const inputElement = document.getElementById(id_to_find);

    if (!inputElement) {
        Swal.fire({
            title: 'Error al enviar comentario',
            text: `No se encontró el elemento con id: ${id_to_find}`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    const comentarioGeneral = document.getElementById(id_to_find).value;
    if (!comentarioGeneral) { // Validación de campo vacío
        Swal.fire({
            title: 'Error al enviar comentario',
            text: 'El comentario no puede estar vacío.',
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }
    $.ajax({
        url: '/Datos/EnviarComentarioGeneral', // Controlador y acción
        type: 'POST',
        data: {
            idAsignacion: idAsignacion,
            comentarioGeneral: comentarioGeneral
        },
        success: function (response) {
            if (response.success === true) {
                Swal.fire({
                    title: 'Comentario enviado',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Error al enviar comentario',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                }); // Mensaje de error
            }
        },
        error: function (xhr, status, error) {
            console.error('Error al enviar comentario:', error);
            Swal.fire({
                title: 'Error al enviar comentario',
                text: 'Hubo un error inesperado al enviar el comentario.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function enviarComentario(idCliente, telefono, textareaId) {
    const comentario = document.getElementById(textareaId).value;

    if (!comentario) {
        Swal.fire({
            title: 'Error al enviar comentario',
            text: 'Por favor, ingrese un comentario antes de enviarlo.',
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    // Enviar la solicitud POST con los datos al controlador
    $.ajax({
        url: '/Datos/EnviarComentario', // Asegúrate de que coincida con la ruta del controlador
        type: 'POST', // Especificar método POST
        data: {
            Telefono: telefono,
            IdCliente: idCliente,
            Comentario: comentario
        },
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Anti-CSRF
        },
        success: function (response) {
            if (response.success === true) {
                Swal.fire({
                    title: 'Comentario enviado',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Error al enviar comentario',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error al enviar comentario:', error);
            Swal.fire({
                title: 'Error al enviar comentario',
                text: 'Hubo un error inesperado al enviar el comentario.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}


function enviarComentarioTelefonoDB(idCliente, telefono, textareaId, numTelefono) {
    const comentario = document.getElementById(textareaId).value;

    if (!comentario) {
        Swal.fire({
            title: 'Error al enviar comentario',
            text: 'Por favor, ingrese un comentario antes de enviarlo.',
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    // Enviar la solicitud POST con los datos al controlador
    $.ajax({
        url: '/Datos/EnviarComentarioTelefonoDB', // Asegúrate de que coincida con la ruta del controlador
        type: 'POST', // Especificar método POST
        data: {
            Telefono: telefono,
            IdCliente: idCliente,
            Comentario: comentario,
            numeroTelefono: numTelefono
        },
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Anti-CSRF
        },
        success: function (response) {
            if (response.success === true) {
                Swal.fire({
                    title: 'Comentario enviado',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Error al enviar comentario',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error al enviar comentario:', error);
            Swal.fire({
                title: 'Error al enviar comentario',
                text: 'Hubo un error inesperado al enviar el comentario.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}