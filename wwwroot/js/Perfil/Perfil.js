document.addEventListener("DOMContentLoaded", function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
});

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
                });
                setTimeout(function () {
                    location.reload();
                }, 5000);
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
        { id: "CorreoUsuario", name: "Correo" }
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
            data[field.name] = value || null;
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

    // Enviar solicitud AJAX
    $.ajax({
        url: "/Perfil/ActualizarCampos",
        type: "POST",
        data: data,
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: response.message === "No se realizaron cambios en los campos" ? 'Sin cambios' : 'Datos actualizados correctamente',
                    text: response.message,
                    icon: response.message === "No se realizaron cambios en los campos" ? 'info' : 'success',
                    confirmButtonText: 'OK'
                }).then(() => {
                    if (response.message !== "No se realizaron cambios en los campos" && response.message !== "El usuario no existe") {
                        location.reload();
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