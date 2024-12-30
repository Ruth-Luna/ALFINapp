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

/*
function loadCulminacionCliente(id_asignacion) {
    if (!id_asignacion) {
        alert('El ID de asignación no es válido.');
        return;
    }

    const userConfirmed = confirm('Advertencia: Si culmina con el cliente, esta acción no se puede revertir. ¿Está seguro de continuar?');

    if (userConfirmed) {
        $.ajax({
            url: '/User/culminarCliente',
            type: 'POST',
            data: { "IdAsignacion": id_asignacion },
            success: function (result) {
                alert('Se ha culminado con el cliente correctamente.');
            },
            error: function (xhr, status, error) {
                console.error('Error al culminar con el cliente:', error);
                alert('Hubo un error al intentar culminar con el cliente. Por favor, inténtalo nuevamente.');
            }
        });
    } else {
        // Si el usuario cancela, muestra un mensaje opcional
        alert('La acción ha sido cancelada.');
    }
}*/