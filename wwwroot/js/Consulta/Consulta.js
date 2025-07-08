function validarTelefono() {
    const campobusqueda = document.getElementById('dnicliente').value;

    if (!campobusqueda) {
        Swal.fire({
            title: 'Error',
            text: 'El campo del cliente es obligatorio.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/Consulta/VerificarTelefono',
        data: {
            telefono: campobusqueda
        },
        success: function (response) {
            if (response.existe === false) {
                Swal.fire({
                    title: 'Cliente no encontrado',
                    text: response.message,
                    icon: 'info',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else if (response.error === true) {
                Swal.fire({
                    title: 'Error en la búsqueda',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }
            else {
                Swal.fire({
                    title: 'Cliente encontrado',
                    text: 'El cliente ha sido encontrado en la base de datos.',
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                const datosClienteExistente = document.getElementById("datos-cliente-existente");
                datosClienteExistente.style.display = "block";
                datosClienteExistente.innerHTML = response; // Carga la vista parcial
            }
        },
        error: function (error) {
            console.error(error);
            Swal.fire({
                title: 'Error',
                text: error,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
    });
}

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

