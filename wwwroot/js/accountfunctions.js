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
        url: '/Accounts/SubmitNewPassword',
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
                setTimeout(function() {
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