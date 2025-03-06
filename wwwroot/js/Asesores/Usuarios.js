/// <summary>
/// This function is responsible for validating and saving a new asesor de creditos.
/// It performs client-side validation for the input fields and sends an AJAX request to the server to save the new asesor.
/// </summary>
function guardarNuevoAsesor() {
    // Define an array of input field IDs
    var inputs = ['dni', 'departamento', 'provincia', 'distrito', 'region', 'rol', 'apellido_paterno', 'apellido_materno', 'nombres', 'telefono'];

    // Get the DNI value and perform validation
    var dni = document.getElementById('dni').value.trim();
    if (!/^[0-9]{8}$/.test(dni) && !/^[0-9]{9}$/.test(dni)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El Documento debe tener el formato de DNI (8 Digitos Numericos) o de Carnet de Extranjeria (9 Digitos Numericos).'
        });
        return;
    }

    // Define an array of field IDs that should only contain letters
    var soloLetrasCampos = ['region', 'apellido_paterno', 'apellido_materno', 'nombres'];
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
        console.log('Existen errores en los campos.');
        return;
    }

    // Get the telefono value and perform validation
    var telefono = document.getElementById('telefono').value.trim();
    if (!/^[0-9]{9}$/.test(telefono)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El campo Telefono solo puede tener valores Numericos y No puede estar Vacio'
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
        IdRol: 3
    };

    console.log(dataToSend);

    // Send an AJAX request to the server
    $.ajax({
        url: "/Asesor/AgregarNuevoAsesor",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(dataToSend),
        success: function (response) {
            // Check the response status and display a success or error message accordingly
            if (response.success === true) {
                Swal.fire({
                    icon: 'success',
                    title: 'Informacion del asesor',
                    text: response.message
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
    var regex2 = /^[0-9]{9}$/;

    if (!regex.test(dni) && !regex2.test(dni)) {
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
