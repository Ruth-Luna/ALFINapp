function verificarTipificacion(index) {
    const tipificacionInput = document.getElementById(`tipificacionSelect_${index}`);
    const fechaVisitaContainer = document.getElementById(`fechaVisitaContainer_${index}`);

    if (tipificacionInput.value === "CLIENTE ACEPTO OFERTA DERIVACION") {
        fechaVisitaContainer.style.display = "block";
    } else {
        fechaVisitaContainer.style.display = "none";
    }
}

// Función para actualizar el ID de la tipificación cuando se selecciona una opción del datalist
function actualizarIdTipificacion(contadorNumeros) {
    var inputText = document.getElementById('tipificacionSelect_' + contadorNumeros);
    var datalist = document.getElementById('tipificacionesList_' + contadorNumeros);
    var hiddenInput = document.getElementById('tipificacionId_' + contadorNumeros);

    // Buscar la opción correspondiente en el datalist
    for (var option of datalist.options) {
        if (option.value === inputText.value) {
            hiddenInput.value = option.getAttribute('data-id'); // Actualizar el ID oculto
            return;
        }
    }

    hiddenInput.value = ''; // Si no hay coincidencia, limpiar el ID
}

// Función para enviar los cambios de datos personales de Clientes traidos por el Asesor
function guardarCambiosPorAsesor() {
    const formData = {};
    console.log('Guardando cambios...');
    
    // Recolectar datos del formulario
    document.querySelectorAll(".form-control-editable").forEach(input => {
        if (!input.hasAttribute("readonly")) {
            formData[input.id] = input.value;
        }
    });

    // Recolectar datos de los campos ocultos

    const idBase = document.getElementById("IDBASEUSUARIO").value;

    console.log('Datos enviados:', { formData, idBase });

    // Enviar datos como JSON
    $.ajax({
        url: '/Datos/ActualizarDatosProspecto',
        type: 'POST',
        data: {
            formData: formData,
            idBase: idBase,
        },
        success: function (result) {
            Swal.fire({
                title: 'Cambios guardados',
                text: 'Se han guardado los cambios correctamente.',
                icon: 'success',
                confirmButtonText: 'Aceptar'
            });
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error al guardar los cambios',
                text: 'Hubo un error al intentar guardar los cambios. Por favor, inténtalo nuevamente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            console.error('Error al guardar los cambios:', error);
        }
    });
}