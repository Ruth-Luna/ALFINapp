/// <summary>
/// This function is responsible for validating and saving a new asesor de creditos.
/// It performs client-side validation for the input fields and sends an AJAX request to the server to save the new asesor.
/// </summary>
function guardarNuevoAsesor() {
    // Get the DNI value and perform validation
    var dni = document.getElementById('Nuevo_dni').value.trim();
    if (!/^[0-9]{8}$/.test(dni) && !/^[0-9]{9}$|^[A-Z]{1}[0-9]{8}$/i.test(dni)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El Documento debe tener 8 dígitos (DNI) o 9 dígitos numéricos / 1 letra + 8 dígitos (Carnet de Extranjería). Además, no puede estar vacío.'
        });
        return;
    }

    // Define an array of field IDs that should only contain letters
    var soloLetrasCampos = ['apellido_paterno', 'apellido_materno', 'nombres'];
    var valid = true;
    // Perform validation for each field in the array
    soloLetrasCampos.forEach(function (campo) {
        var valor = document.getElementById(campo).value.trim();

        if (!/^[a-zA-Z\s]+$/.test(valor)) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'El campo "' + campo + '" solo puede tener letras.'
            });
            valid = false;
            return;
        }
    });

    // If any field fails validation, log an error message and return
    if (!valid) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Por favor, corrija los campos con errores.'
        });
        return;
    }

    // Get the telefono value and perform validation
    var telefono = document.getElementById('telefono').value.trim();
    if (!/^[0-9]{9}$/.test(telefono)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El campo Telefono solo puede tener valores Numericos con un maximo de 9 digitos y No puede estar Vacio'
        });
        return;
    }

    var correo = document.getElementById('correo').value.trim();
    if (!/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(correo)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El campo Correo debe tener un formato de correo valido'
        });
        return;
    }


    // Prepare the data to be sent in the AJAX request
    var dataToSend = {
        Dni: dni.toUpperCase(),
        Departamento: document.getElementById('departamento').value.trim().toUpperCase(),
        Provincia: document.getElementById('provincia').value.trim().toUpperCase(),
        Distrito: document.getElementById('distrito').value.trim().toUpperCase(),
        REGION: document.getElementById('region').value.trim().toUpperCase(),
        NombresCompletos: (document.getElementById('nombres').value.trim() + ' ' 
            + document.getElementById('apellido_paterno').value.trim() + ' ' 
            + document.getElementById('apellido_materno').value.trim()).toUpperCase(),
        Telefono: telefono,
        IdRol: 3,
        TipoDocumento: document.getElementById('tipo_documento').value.trim().toUpperCase(),
        Correo: correo
    };

    $.ajax({
        url: "/Asesor/AgregarNuevoAsesor",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(dataToSend),
        success: function (response) {
            if (response.success === true) {
                Swal.fire({
                    icon: 'success',
                    title: 'Informacion del asesor',
                    text: response.message,
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        location.reload();
                    }
                });
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
        },
        error: function () {
            // Display an error message if the AJAX request fails
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Hubo un error al agregar el usuario.'
            });
        }
    });
}

/**
 * Valida el DNI del supervisor y aplica estilos de error si es inválido.
 *
 * @param {string} [campo='dni'] - El ID del campo de entrada del DNI.
 */
function validarDNISupervisor(campo = 'dni') {
    var dni = document.getElementById(campo).value;
    var dniInput = document.getElementById(campo);
    var errorElement = document.getElementById(campo + '-error');
    var regex = /^[0-9]{8}$/;

    if (!regex.test(dni)) {
        dniInput.classList.add('is-invalid'); // Aplica el estilo de error de Bootstrap
        errorElement.style.display = 'block'; // Muestra el mensaje de error
    } else {
        dniInput.classList.remove('is-invalid'); // Remueve el estilo de error de Bootstrap
        errorElement.style.display = 'none'; // Oculta el mensaje de error
    }
}

/**
 * Valida el DNI del usuario y aplica estilos de error si es inválido.
 *
 * @param {string} [campo='dni'] - El ID del campo de entrada del DNI.
 */
function validarCarnetSupervisor(campo = 'dni') {
    var dni = document.getElementById(campo).value;
    var dniInput = document.getElementById(campo);
    var errorElement = document.getElementById(campo + '-error');
    var regex = /^[0-9]{9}$|^[A-Z]{1}[0-9]{8}$/i;

    if (!regex.test(dni)) {
        dniInput.classList.add('is-invalid'); // Aplica el estilo de error de Bootstrap
        errorElement.style.display = 'block'; // Muestra el mensaje de error
    } else {
        dniInput.classList.remove('is-invalid'); // Remueve el estilo de error de Bootstrap
        errorElement.style.display = 'none'; // Oculta el mensaje de error
    }
}


/// <summary>
/// This function is responsible for validating the telefono input field in the "Agregar Nuevo Asesor" view.
/// It checks if the telefono value only contains numeric characters.
/// If the validation fails, it adds the 'is-invalid' class to the input field and displays an error message.
/// If the validation passes, it removes the 'is-invalid' class and hides the error message.
/// </summary>
/// <param name="telefono">The value entered in the telefono input field.</param>
/// <returns>No return value.</returns>
function validarTelefono(campo = 'telefono') {
    var telefono = document.getElementById(campo).value;
    var telefonoInput = document.getElementById(campo);
    var errorElement = document.getElementById(campo + '-error');
    var regex = /^[0-9]{9}$/;

    if (!regex.test(telefono)) {
        telefonoInput.classList.add('is-invalid');
        errorElement.style.display = 'block';
    } else {
        telefonoInput.classList.remove('is-invalid');
        errorElement.style.display = 'none';
    }
}

function validarEmail(campo) {
    var email = document.getElementById(campo).value;
    var emailInput = document.getElementById(campo);
    var errorElement = document.getElementById(campo + '-error');
    var regex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

    if (!regex.test(email)) {
        emailInput.classList.add('is-invalid');
        errorElement.style.display = 'block';
    } else {
        emailInput.classList.remove('is-invalid');
        errorElement.style.display = 'none';
    }
}

/// <summary>
/// This function is responsible for validating input fields that should only contain letters.
/// It checks if the input value matches the regular expression /^[a-zA-Z\s]+$/ and applies appropriate styles and error messages.
/// </summary>
/// <param name="campo">The ID of the input field to be validated.</param>
/// <returns>No return value.</returns>
function validarLetras(campo) {
    var campol = document.getElementById(campo).value;
    var campolInput = document.getElementById(campo);
    var errorElement = document.getElementById(campo + '-error');
    var regex = /^[a-zA-Z\s]+$/;

    if (!regex.test(campol)) {
        campolInput.classList.add('is-invalid'); // Apply Bootstrap's 'is-invalid' class to the input field
        errorElement.style.display = 'block'; // Display the error message
    } else {
        campolInput.classList.remove('is-invalid'); // Remove Bootstrap's 'is-invalid' class from the input field
        errorElement.style.display = 'none'; // Hide the error message
    }
}

function cambiarTipoDoc(doc) {
    let labelDocumento = document.getElementById('label-documento');
    let inputDocumento = document.getElementById('Nuevo_dni');
    let contenedorDocumento = document.getElementById('documento-container');
    let mensajeError = document.getElementById('Nuevo_dni-error');

    if (doc == "DNI") {
        labelDocumento.textContent = "DNI";
        mensajeError.textContent = "El Documento debe de tener el formato de DNI (8 Digitos numericos).";
        inputDocumento.setAttribute("oninput", "validarDNISupervisor('Nuevo_dni')");
        contenedorDocumento.style.display = "block"; // Mostrar input
    } else if (doc == "CE") {
        labelDocumento.textContent = "Carnet de Extranjería";
        mensajeError.textContent = "El Documento debe de tener el formato de Carnet de Extranjería (9 Digitos numericos o 1 Letra seguida de 8 Digitos Numericos).";
        inputDocumento.setAttribute("oninput", "validarCarnetSupervisor('Nuevo_dni')");
        contenedorDocumento.style.display = "block"; // Mostrar input

    } else {
        contenedorDocumento.style.display = "none"; // Ocultar input si no se selecciona nada
        inputDocumento.removeAttribute("oninput");
    }
}