function TakeThisClient(DNIdatos, tipoBase) {
    DNIdatos = String(DNIdatos).padStart(8, '0');

    if (!DNIdatos || !tipoBase) {
        Swal.fire({
            title: 'Error al realizar la asignación',
            text: 'No se pudo identificar el cliente o la base activa.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Consulta/ReAsignarClienteAUsuario`;

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            DniAReasignar: DNIdatos,
            BaseTipo: tipoBase
        },
        success: function (result) {
            Swal.close();
            if (!result.success) {
                Swal.fire({
                    title: 'Error al realizar la asignación',
                    text: result.message || 'Ocurrió un error desconocido',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Asignación completa',
                    text: result.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            console.error('Error al enviar la solicitud:', error);
            Swal.fire({
                title: 'Error al realizar la asignación',
                text: 'Hubo un error al procesar la solicitud.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

