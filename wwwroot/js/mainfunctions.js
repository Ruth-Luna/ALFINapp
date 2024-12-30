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
            if (response.success) {
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

function loadTipificarCliente(idBase) {
    $.ajax({
        url: '/User/TipificarClienteView', // Controlador y acción
        type: 'GET',
        data: { id_base: idBase },
        success: function (result) {
            $('#modalContentTipificar').html(result); // Inserta el contenido de la vista en el modal
            $('#tipificarClienteModal').modal('show'); // Muestra el modal
        },
        error: function () {
            Swal.fire({
                title: 'Error al cargar los datos',
                text: 'Hubo un error al intentar cargar los datos. Por favor, inténtalo nuevamente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function openAddClientModal() {
    $.ajax({
        url: '/User/AddingClient', // Acción para cargar la vista parcial
        type: 'GET',
        success: function (result) {
            $('#modalContentAgregar').html(result); // Carga la vista parcial dentro del modal
            $('#addClientModal').modal('show'); // Muestra el modal
        },
        error: function (xhr, status, error) {
            console.log("Error:", status, error); // Muestra el error en consola
            Swal.fire({
                title: 'Error al cargar el formulario',
                text: 'Hubo un error al cargar el formulario de agregar cliente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}