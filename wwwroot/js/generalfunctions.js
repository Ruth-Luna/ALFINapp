


/**
 * Displays a modal with content fetched from a specified API endpoint.
 *
 * @param {string} apiName - The name of the API to fetch data from.
 * @param {string} controllerName - The name of the controller to fetch data from.
 * @param {string} type - The type of the HTTP request (e.g., 'GET', 'POST').
 * @param {string} templateTitle - The title to be displayed on the modal.
 *
 * @throws Will display a Swal alert if any of the parameters are null or undefined.
 */
function showMyModal(apiName, controllerName, type, templateTitle) {
    TemplateModalRenderer = document.getElementById('GeneralTemplateModal');
    ContentModalRenderer = document.getElementById('modalContentGeneralTemplate');
    TitleModalRenderer = document.getElementById('GeneralTemplateTitleModalLabel');

    if (controllerName === null) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${controllerName} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (apiName === null) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${apiName} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (type === null || type === undefined) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${type} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (templateTitle === undefined) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `El elemento enviado: ${templateTitle} no existe, o no es valido`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        url: `/${apiName}/${controllerName}`,
        type: `${type}`,
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al mostrar la vista',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#modalContentGeneralTemplate').html(response); // Insertar la vista parcial en el modal
                TitleModalRenderer.innerText = templateTitle; // Asigna el título al modal
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error al realizar la solicitud AJAX:', textStatus, errorThrown);
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la vista.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function mostrarOpciones(id) {
    let opcionesDiv = document.getElementById("opciones_" + id);
    opcionesDiv.style.display = opcionesDiv.style.display === "block" ? "none" : "block";
    console.log(opcionesDiv.style.display);
}

function seleccionarOpcion(id, value, text, index = null) {
    console.log(id, value, text);
    document.getElementById("selectedOption_" + id).innerText = text;
    document.getElementById(id).value = value;
    let opciones = document.getElementById("opciones_" + id);
    opciones.style.display = "none";
    if (index != null) {
        verificarTipificacion(index);
    }
    console.log('MODIFICADO');
}

function filtrarOpciones(id) {
    let input = document.getElementById("busqueda_" + id);
    let filter = input.value.toLowerCase();
    let opcionesDiv = document.getElementById("opciones_" + id);
    let opciones = opcionesDiv.getElementsByClassName("custom-option");

    for (let i = 0; i < opciones.length; i++) {
        let texto = opciones[i].innerText.toLowerCase();
        opciones[i].style.display = texto.includes(filter) ? "block" : "none";
    }
    console.log('MODIFICADO');
}

document.addEventListener("click", function (event) {
    let selects = document.querySelectorAll(".custom-select");

    selects.forEach(select => {
        let opcionesDiv = select.querySelector(".custom-options");

        // Si el menú está visible y el clic NO es dentro del select, ocultarlo
        if (opcionesDiv.style.display === "block" && !select.contains(event.target)) {
            opcionesDiv.style.display = "none";
        }
    });
    console.log('MODIFICADO');
});
