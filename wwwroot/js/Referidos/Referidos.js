function cargarDerivacionManual(IdReferido) {
    $.ajax({
        type: 'GET',
        url: '/Referidos/DatosEnviarDerivacion',
        data: {
            IdReferido: IdReferido
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las referencias a derivar',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#modalContentGeneralTemplate').html(response); // Inserta el contenido de la vista en el modal
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#GeneralTemplateTitleModalLabel').text("Precaucion se mandara esta Derivacion"); // Inserta el contenido de la vista en el modal
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las Referencias',
                text: 'Hubo un error inesperado al cargar las Referencias a enviar.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function enviarDerivacionPorReferencia(IdReferido) {

    $.ajax({
        type: 'POST',
        url: '/Referidos/EnviarDerivacionPorReferencia',
        data: {
            IdReferido: IdReferido
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al enviar la derivacion',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#GeneralTemplateModal').modal('hide');
                Swal.fire({
                    title: 'Derivacion enviada',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al enviar la derivacion',
                text: 'Hubo un error inesperado al enviar la derivacion.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}