function TakeThisClient(DNIdatos, tipoBase) {
    console.log("Función llamada", DNIdatos); // Verifica si se llama la función
    DNIdatos = String(DNIdatos).padStart(8, '0');
    // Identificar la TipoBase activa

    // Validar si se encontraron datos necesarios
    if (!DNIdatos || !tipoBase) {
        Swal.fire({
            title: 'Error al realizar la asignación',
            text: 'No se pudo identificar el cliente o la base activa.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    console.log(DNIdatos, tipoBase);

    $.ajax({
        url: '/Consulta/ReAsignarClienteAUsuario',
        type: 'POST',
        data: {
            DniAReasignar: DNIdatos,
            BaseTipo: tipoBase
        },
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Anti-CSRF
        },
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: 'Asignación completa',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                setTimeout(function () {
                    location.reload();
                }, 5000);
            } else {
                Swal.fire({
                    title: 'Error al realizar la asignación',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error al enviar comentario:', error);
            console.log('XHR:', xhr);
            console.log('Status:', status);
            console.log('Response Text:', xhr.responseText);
            if (xhr.status !== 200) {
                Swal.fire({
                    title: 'Hay un error en la solicitud',
                    text: error,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
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