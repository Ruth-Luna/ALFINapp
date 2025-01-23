function TakeThisClient(DNIdatos) {
    console.log("Función llamada", DNIdatos); // Verifica si se llama la función
    DNIdatos = String(DNIdatos).padStart(8, '0');
    // Identificar la TipoBase activa
    const activeTab = document.querySelector('.nav-link.active');
    const tipoBase = activeTab ? activeTab.id.replace('fuenteBaseTab-', '') : null;

    // Validar si se encontraron datos necesarios
    if (!DNIdatos || !tipoBase) {
        alert('No se pudo identificar el cliente o la base activa.');
        return;
    }

    console.log(DNIdatos, tipoBase);

    $.ajax({
        url: '/Reagregaciones/ReAsignarClienteAUsuario',
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
                alert(`Error con el servidor: ${xhr.status}`);
            }
        }
    });
}

function validarDNI() {
    const dniInput = document.getElementById("dnicliente");
    const Message = document.getElementById("error-message");
    const datosClienteExistente = document.getElementById("datos-cliente-existente");
    const dniForm = document.getElementById("DNIForm");

    const dniValue = dniInput.value;

    Message.style.display = "none";
    datosClienteExistente.style.display = "none";

    if (dniValue === "") {
        Message.style.display = "block";
        Message.innerHTML = "El DNI no puede estar vacío.";
    } else if (!/^\d{8}$/.test(dniValue)) {
        Message.style.display = "block";
        Message.innerHTML = "El DNI debe contener exactamente 8 dígitos y solo números.";
    } else {
        $.ajax({
            url: `/Reagregaciones/VerificarDNI`,
            type: 'GET',
            data: { dni: dniValue },
            success: function (data) {
                if (data.existe === false && data.error === false) {
                    Message.style.display = "block";
                    Message.innerHTML = data.message;
                } else if (data.existe === false && data.error === true) {
                    Message.style.display = "block";
                    Message.innerHTML = data.message;
                } else {
                    datosClienteExistente.style.display = "block";
                    datosClienteExistente.innerHTML = data; // Carga la vista parcial
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                Message.style.display = "block";
                Message.innerHTML = "Hubo un error al verificar el DNI.";
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