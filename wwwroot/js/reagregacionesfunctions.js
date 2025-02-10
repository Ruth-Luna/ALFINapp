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

function validarDNI() {
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
        $.ajax({
            url: `/Consulta/VerificarDNIenBDoBanco`,
            type: 'GET',
            data: { dni: dniValue },
            success: function (data) {
                if (data.existe === false && data.error === false) {
                    Swal.fire({
                        title: 'Error',
                        text: data.message,
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                } else if (data.existe === false && data.error === true) {
                    Swal.fire({
                        title: 'Error',
                        text: 'Hubo un error al verificar el DNI.',
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                } else {
                    Swal.fire({
                        title: 'Cliente encontrado',
                        text: 'El cliente existe en la base de datos.',
                        icon: 'success',
                        confirmButtonText: 'Aceptar'
                    });
                    datosClienteExistente.style.display = "block";
                    datosClienteExistente.innerHTML = data; // Carga la vista parcial
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                Swal.fire({
                    title: 'Error',
                    text: 'Hubo un error al verificar el DNI.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }
        });
    }
}

// Función para cargar los datos correspondientes según la fuente base seleccionada
/*function cargarDatos(fuenteBase) {
    // Seleccionar todas las secciones con ID que comiencen con 'detalle-campana_'
    const todasLasSecciones = document.querySelectorAll("[id^='detalle-campana_']");

    // Ocultar todas las secciones
    todasLasSecciones.forEach(seccion => {
        seccion.style.display = "none";
    });

    // Mostrar solo la sección correspondiente a 'fuenteBase'
    const fuenteSeccion = document.getElementById('detalle-campana_' + fuenteBase);
    if (fuenteSeccion) {
        fuenteSeccion.style.display = "block";
    } else {
        console.warn(`No se encontró la sección con ID: detalle-campana_${fuenteBase}`);
    }
}*/

function VistaDeriveEsteCliente(DNIdatos, tipoBase) {
    
}