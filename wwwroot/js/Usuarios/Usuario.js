
document.addEventListener('DOMContentLoaded', function () {
   /* ListarUsuarioAdministrador()*/

});

$('#btnAgregarUsuario').on('click', function () {
    guardarNuevoUsuario();
});


function FechaFormat(fechaString) {
    const fecha = new Date(fechaString);
    const dia = fecha.getDate().toString().padStart(2, '0');
    const mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    const anio = fecha.getFullYear();
    return `${dia}/${mes}/${anio}`;
}

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

    if (!valid) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Existen errores en los campos.'
        });
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

    var email = document.getElementById('Nuevo_correo').value.trim();

    if (!/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'El campo Email no tiene un formato valido'
        });
        return;
    }

    var idRolValue = $('#Nuevo_rol').val().trim();
    var rol = '';
    if (idRolValue === '2') rol = 'SUPERVISOR';
    else if (idRolValue === '3') rol = 'ASESOR';
    else rol = 'DESCONOCIDO';

    var nombres = $('#Nuevo_nombres').val().trim().toUpperCase();
    var apellido_paterno = $('#Nuevo_apellido_paterno').val().trim().toUpperCase();

    var primerNombre = nombres.split(' ')[0];
    var primerApellido = apellido_paterno.split(' ')[0];

    // Prepare the data to be sent in the AJAX request
    var dataToSend = {
        Dni: $('#Nuevo_dni').val().trim().toUpperCase(),
        Departamento: $('#Nuevo_departamento').val().trim().toUpperCase(),
        Provincia: $('#Nuevo_provincia').val().trim().toUpperCase(),
        Distrito: $('#Nuevo_distrito').val().trim().toUpperCase(),
        REGION: $('#Nuevo_region').val().trim().toUpperCase(),
        Apellido_Paterno: apellido_paterno,
        Apellido_Materno: $('#Nuevo_apellido_materno').val().trim().toUpperCase(),
        Nombres: nombres,
        Telefono: telefono,
        Rol: rol,
        IdRol: $('#Nuevo_rol').val().trim().toUpperCase(),
        usuario: primerNombre + '.' + primerApellido,
        IDUSUARIOSUP: parseInt($('#Nuevo_Supervisor').val()),
        TipoDocumento: $('#tipo_documento').val().trim().toUpperCase(),
        Correo: email
    };

    $.ajax({
        url: "/Usuarios/CrearUsuario",
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
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Hubo un error al agregar el usuario.'
            });
        }
    });
}

function agregarCampoSupervisores(nuevoRol) {
    const campoSupervisores = document.getElementById('campoSupervisores');
    const Nuevo_Supervisor = document.getElementById('Nuevo_Supervisor');
    if (nuevoRol == 3) {
        campoSupervisores.style.display = 'block';
    }
    else {
        campoSupervisores.style.display = 'none';
        Nuevo_Supervisor.value = '';
    }
}

