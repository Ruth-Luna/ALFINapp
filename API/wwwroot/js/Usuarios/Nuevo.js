function guardarNuevoUsuario() {
    var dni = document.getElementById('Nuevo_dni').value.trim();
    if (!/^[0-9]{8}$/.test(dni) && !/^[0-9]{9}$|^[A-Z]{1}[0-9]{8}$/i.test(dni)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El Documento debe tener 8 dígitos (DNI) o 9 dígitos numéricos / 1 letra + 8 dígitos (Carnet de Extranjería). Además, no puede estar vacío.'
        });
        return;
    }

    var soloLetrasCampos = ['Nuevo_apellido_paterno', 'Nuevo_apellido_materno', 'Nuevo_nombres'];
    var valid = true;
    soloLetrasCampos.forEach(function (campo) {
        var valor = document.getElementById(campo).value.trim();
        if (!/^[a-zA-Z\s]+$/.test(valor)) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'El campo "' + campo + '" solo puede tener letras. Y no puede estar vacio'
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
            text: 'Existen errores en los campos.'
        });
        console.log('Existen errores en los campos.');
        return;
    }

    // Get the telefono value and perform validation
    var telefono = document.getElementById('Nuevo_telefono').value.trim();
    if (!/^[0-9]{9}$/.test(telefono)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El campo Telefono solo puede tener valores Numericos de 9 digitos y No puede estar Vacio'
        });
        return;
    }

    // Prepare the data to be sent in the AJAX request
    var dataToSend = {
        Dni: dni.toUpperCase(),
        Departamento: document.getElementById('Nuevo_departamento').value.trim().toUpperCase(),
        Provincia: document.getElementById('Nuevo_provincia').value.trim().toUpperCase(),
        Distrito: document.getElementById('Nuevo_distrito').value.trim().toUpperCase(),
        REGION: document.getElementById('Nuevo_region').value.trim().toUpperCase(),
        NombresCompletos: (document.getElementById('Nuevo_nombres').value.trim() + ' ' 
            + document.getElementById('Nuevo_apellido_paterno').value.trim() + ' ' 
            + document.getElementById('Nuevo_apellido_materno').value.trim()).toUpperCase(),
        Telefono: telefono,
        IdRol: document.getElementById('Nuevo_rol').value.trim().toUpperCase(),
        IDUSUARIOSUP: parseInt(document.getElementById('Nuevo_Supervisor').value),
        TipoDocumento: document.getElementById('tipo_documento').value.trim().toUpperCase()
    };

    console.log(dataToSend);

    // Send an AJAX request to the server
    $.ajax({
        url: "/Usuarios/CrearUsuario",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(dataToSend),
        success: function (response) {
            // Check the response status and display a success or error message accordingly
            if (response.success === true) {
                Swal.fire({
                    icon: 'success',
                    title: 'Informacion del asesor',
                    text: response.message,
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

function agregarCampoSupervisores(nuevoRol) {
    console.log(nuevoRol);
    const campoSupervisores = document.getElementById('campoSupervisores');
    const Nuevo_Supervisor = document.getElementById('Nuevo_Supervisor');
    if (nuevoRol == 3) {
        console.log("Entrando");
        campoSupervisores.style.display = 'block';
    }
    else {
        campoSupervisores.style.display = 'none';
        Nuevo_Supervisor.value = '';
    }
}