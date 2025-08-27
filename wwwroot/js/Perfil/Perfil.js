// Document ready
$(document).ready(function () {
    // carga inicial de datos
    cargarDatosUsuario();
});

function cargarDatosUsuario() {
    // Lógica para cargar los datos del usuario
    $.ajax({
        url: '/Perfil/ObtenerDatosUsuario',
        type: 'GET',
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return;
            }

            const usuario = response.data;

            // Campos de solo lectura
            $('#NombresCompletosUsuario').val(usuario.nombresCompletos || '');
            $('#TipoDocumentoUsuario').val(usuario.tipoDocumento || '');
            $('#DniUsuario').val(usuario.dni || '');
            $('#RolUsuario').val(usuario.rol || '');
            $('#NombreCampaniaUsuario').val(usuario.nombrecampania || '');
            $('#ContrasenaUsuario').val(usuario.contraseña || '');

            // Campos editables con data-original
            $('#CorreoUsuario').val(usuario.correo || '').attr('data-original', usuario.correo || '');
            $('#TelefonoUsuario').val(usuario.telefono || '').attr('data-original', usuario.telefono || '');
            $('#DepartamentoUsuario').val(usuario.departamento || '').attr('data-original', usuario.departamento || '');
            $('#ProvinciaUsuario').val(usuario.provincia || '').attr('data-original', usuario.provincia || '');
            $('#DistritoUsuario').val(usuario.distrito || '').attr('data-original', usuario.distrito || '');
            $('#RegionUsuario').val(usuario.region || '').attr('data-original', usuario.region || '');

            // Campos ocultos
            $('#IdUsuarioUsuario').val(usuario.idUsuario || '');
            $('#ApellidoPaternoUsuario').val(usuario.apellido_Paterno || '');
            $('#ApellidoMaternoUsuario').val(usuario.apellido_Materno || '');
            $('#NombresUsuario').val(usuario.nombres || '');
            $('#UsuarioUsuario').val(usuario.usuario || '');
            $('#IdRolUsuario').val(usuario.idRol || '');
            $('#IdSupervisorUsuario').val(usuario.idusuariosup || '');
            $('#ResponsableSupervisorUsuario').val(usuario.responsablesup || '');

            // Actualizar el saludo dinámicamente
            $('h3').text('Bienvenido ' + (usuario.nombresCompletos || 'Usuario'));
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error al cargar los datos',
                text: 'Ocurrió un error al obtener la información del usuario: ' + error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}

function activarEdicion(campo) {
    const BotonEnvio = document.getElementById(campo + "BotonEnvio");
    const BotonEdicion = document.getElementById(campo + "BotonEdicion");
    const Usuario = document.getElementById(campo + "Usuario");

    BotonEnvio.style.display = "block";
    BotonEdicion.style.display = "none";
    Usuario.removeAttribute('readonly');
    Usuario.focus();
}

function enviarEdicion(campo) {
    const BotonEnvio = document.getElementById(campo + "BotonEnvio");
    const BotonEdicion = document.getElementById(campo + "BotonEdicion");
    const Usuario = document.getElementById(campo + "Usuario");

    BotonEnvio.style.display = "none";
    BotonEdicion.style.display = "block";
    Usuario.setAttribute('readonly', 'true');

    if (Usuario.value == "") {
        Swal.fire({
            title: 'Error al actualizar los datos',
            text: 'El campo no puede estar vacío',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
    let UsuarioMayusculas = Usuario.value.trim();
    if (campo != "Correo") {
        UsuarioMayusculas = Usuario.value.trim().toUpperCase();
    }

    $.ajax({
        url: "/Perfil/EnviarEdicion",
        type: "POST",
        data: {
            campo: campo,
            nuevoValor: UsuarioMayusculas
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al actualizar los datos',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            } else {
                Swal.fire({
                    title: 'Datos actualizados correctamente',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'OK'
                });
                setTimeout(function () {
                    location.reload();
                }, 2000);
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}

function updatePasswordView() {
    var AlertUpdatePassword = document.getElementById('AlertUpdatePassword');
    AlertUpdatePassword.style.display = 'block';
    var UpdatePasswordButtonView = document.getElementById('UpdatePasswordButtonView');
    UpdatePasswordButtonView.style.display = 'none';
    var UpdatePasswordButtonSubmit = document.getElementById('UpdatePasswordButtonSubmit');
    UpdatePasswordButtonSubmit.style.display = 'block';
}

function updatePasswordSubmit() {
    var passwordInput = document.getElementById('ContrasenaNueva');

    if (passwordInput.value === null) {
        Swal.fire({
            title: 'Error al mostrar la vista',
            text: `La contraseña enviada: ${passwordInput} no existe, o no es valida`,
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        url: '/Perfil/SubmitNewPassword',
        type: 'POST',
        data: {
            newPassword: passwordInput.value
        },
        success: function (response) {
            if (response.success === true) {
                var AlertUpdatePassword = document.getElementById('AlertUpdatePassword');
                AlertUpdatePassword.style.display = 'none';
                var UpdatePasswordButtonView = document.getElementById('UpdatePasswordButtonView');
                UpdatePasswordButtonView.style.display = 'block';
                var UpdatePasswordButtonSubmit = document.getElementById('UpdatePasswordButtonSubmit');
                UpdatePasswordButtonSubmit.style.display = 'none';
                Swal.fire({
                    title: 'Contraseña actualizada',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar',
                    timer: 5000
                }).then(() => {
                    cargarDatosUsuario();
                });
            } else {
                Swal.fire({
                    title: 'Error al actualizar la contraseña',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error al actualizar la contraseña',
                text: `Error: ${error}`,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function actualizarCampos() {
    // Obtener valores de los campos y sus valores originales
    const fields = [
        { id: "DepartamentoUsuario", name: "Departamento" },
        { id: "ProvinciaUsuario", name: "Provincia" },
        { id: "DistritoUsuario", name: "Distrito" },
        { id: "TelefonoUsuario", name: "Telefono" },
        { id: "CorreoUsuario", name: "Correo" },
        { id: "RegionUsuario", name: "Region" }
    ];

    const data = {};
    let hasChanges = false;

    // Validar y recolectar datos
    for (const field of fields) {
        const input = document.getElementById(field.id);
        const value = input.value.trim();
        const original = input.getAttribute("data-original") || "";

        // Validar correo si no está vacío
        if (field.name === "Correo" && value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: 'El correo ingresado no es válido',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return;
        }

        // Incluir solo si el valor ha cambiado
        if (value !== original) {
            //data[field.name] = value || null;
            hasChanges = true;
        }
    }

    // Si no hay cambios, mostrar alerta temporal y salir
    if (!hasChanges) {
        Swal.fire({
            title: 'Sin cambios',
            text: 'Ningún dato se ha modificado',
            icon: 'info',
            timer: 2000, // Cierra la alerta después de 2 segundos
            timerProgressBar: true,
            showConfirmButton: false // No muestra el botón "OK"
        });
        return;
    }

    // Agregar todos los datos para la actualización, garantizando no perder información
    data.IdUsuario = $("#IdUsuarioUsuario").val();
    data.IdRol = parseInt($('#IdRolUsuario').val());

    data.Departamento = $('#DepartamentoUsuario').val()?.trim() || '';
    data.Provincia = $('#ProvinciaUsuario').val()?.trim() || '';
    data.Distrito = $('#DistritoUsuario').val()?.trim() || '';
    data.REGION = $('#RegionUsuario').val()?.trim() || '';

    data.Telefono = $('#TelefonoUsuario').val()?.trim() || '';
    data.Correo = $('#CorreoUsuario').val()?.trim() || '';

    data.TipoDocumento = $('#TipoDocumentoUsuario').val() || null;
    data.Apellido_Paterno = $('#ApellidoPaternoUsuario').val()?.trim() || '';
    data.Apellido_Materno = $('#ApellidoMaternoUsuario').val()?.trim() || '';
    data.Nombres = $('#NombresUsuario').val()?.trim() || '';

    data.Estado = $('#EstadoUsuario').val() || 'ACTIVO';
    data.NOMBRECAMPANIA = $('#NombreCampaniaUsuario').val()?.trim() || '';

    data.IDUSUARIOSUP = parseInt($('#IdSupervisorUsuario').val()) || 0;
    data.RESPONSABLESUP = $('#ResponsableSupervisorUsuario').val()?.trim() || '';

    // Enviar solicitud AJAX
    $.ajax({
        url: "/Usuarios/ActualizarUsuario",
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: response.message === "No se realizaron cambios en los campos" ? 'Sin cambios' : 'Datos actualizados correctamente',
                    text: response.message,
                    icon: response.message === "No se realizaron cambios en los campos" ? 'info' : 'success',
                    confirmButtonText: 'OK'
                }).then(() => {
                    if (response.message !== "No se realizaron cambios en los campos" && response.message !== "El usuario no existe") {
                        cargarDatosUsuario();
                    }
                });
            } else {
                Swal.fire({
                    title: 'Error al actualizar los datos',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: 'Ocurrió un error en el servidor: ' + error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}