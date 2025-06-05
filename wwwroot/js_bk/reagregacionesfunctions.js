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

async function validarDNI() {
    const dniInput = document.getElementById("dnicliente");
    const datosClienteExistente = document.getElementById("datos-cliente-existente");

    const dniValue = dniInput.value;

    datosClienteExistente.style.display = "none";

    if (dniValue === "") {
        Swal.fire({
            title: 'Error',
            text: 'El campo del Cliente es obligatorio.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    } else if (!/^\d{8}$/.test(dniValue)) {
        Swal.fire({
            title: 'Error',
            text: 'El DNI debe contener exactamente 8 dígitos y solo números.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    } else {
        let loadingSwal = Swal.fire({
            title: 'Enviando...',
            text: 'Por favor, espera mientras se busque el Dni.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        const baseUrl = window.location.origin;
        const url = `${baseUrl}/Consulta/VerificarDNIenBDoBanco?dni=${encodeURIComponent(dniValue)}`;
        try {
            const response = await fetch(url, {
                method: 'GET'
            });
            const contentType = response.headers.get("content-type");
            Swal.close();
            if (contentType && contentType.includes("application/json")) {
                const result = await response.json();
                if (!result.success) {
                    Swal.fire({
                        title: 'Error',
                        text: result.message || 'Ocurrió un error desconocido',
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                }
            } else {
                Swal.fire({
                    title: 'Cliente encontrado',
                    text: 'Se encontro una entrada del cliente en una de nuestras bases de datos conocidas.',
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                datosClienteExistente.style.display = "block";
                const html = await response.text();
                datosClienteExistente.innerHTML = html; // Carga la vista parcial
            }
        } catch (error) {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Hubo un error al verificar el DNI.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
    }
}