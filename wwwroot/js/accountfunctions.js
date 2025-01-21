function updatePasswordView() {
    passwordInput = document.getElementById('ContrasenaUsuario');
    passwordUpdateButtonView = document.getElementById('UpdatePasswordButtonView');
    UpdatePasswordButtonSubmit = document.getElementById('UpdatePasswordButtonSubmit');
    MessageUpdatePassword = document.getElementById('AlertUpdatePassword');

    // Verifica si el campo está en readonly
    if (passwordInput.hasAttribute('readonly')) {
        // Si está en readonly, lo quita para permitir edición
        passwordInput.removeAttribute('readonly');
    } else {
        // Si no está en readonly, lo pone como readonly nuevamente
        passwordInput.setAttribute('readonly', true);
    }
    // Oculta o muestra el botón para actualizar la contraseña
    passwordUpdateButtonView.style.display = 'none';
    UpdatePasswordButtonSubmit.style.display = 'block';
    MessageUpdatePassword.style.display = 'block';
}

function updatePasswordSubmit() {
    var passwordInput = document.getElementById('ContrasenaUsuario');
    var passwordUpdateButtonView = document.getElementById('UpdatePasswordButtonView');
    var UpdatePasswordButtonSubmit = document.getElementById('UpdatePasswordButtonSubmit');
    var MessageUpdatePassword = document.getElementById('AlertUpdatePassword');
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
        url: '/Accounts/SubmitNewPassword',
        type: 'POST',
        data: {
            newPassword: passwordInput.value
        },
        success: function (response) {
            if (response.success === true) {
                if (passwordInput.hasAttribute('readonly')) {
                    passwordInput.removeAttribute('readonly');
                } else {
                    passwordInput.setAttribute('readonly', true);
                }
                passwordUpdateButtonView.style.display = 'block';
                UpdatePasswordButtonSubmit.style.display = 'none';
                MessageUpdatePassword.style.display = 'none';
                Swal.fire({
                    title: 'Contraseña actualizada',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
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