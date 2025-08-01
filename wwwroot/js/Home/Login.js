
let idUsuario = 0
document.addEventListener('DOMContentLoaded', function () {

    const inputs = document.querySelectorAll('.otp-input');

    inputs.forEach((input, index) => {
        input.addEventListener('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '').slice(0, 1);

            if (this.value && index < inputs.length - 1) {
                inputs[index + 1].focus();
            }
        });

        input.addEventListener('keydown', function (e) {
            if (e.key === "Backspace" && this.value === "" && index > 0) {
                inputs[index - 1].focus();
            }
        });
    });
    $('#btnRegresarStep1').on('click', function () {
        console.log("click")
        $('#recuperarContraseñaModal').modal('show');
        document.getElementById("step1").classList.remove("d-none");
        document.getElementById("step2").classList.add("d-none");

    });
    $('#btnAbrirModalRecuperarPassword').on('click', function () {
        verificarEstadoCodigo();
    });
    $('#btnValidarUser, #btnSolicitarNuevoCodigo').on('click', function () {
        ValidarCorreoUsuario();
    });
    $('#btnVerificarCodigo').on('click', function () {
        VerificarCodigoIngresado();
    });
    
});

function obtenerCodigoOTP() {
    let codigo = '';
    $('#otpInputs .otp-input').each(function () {
        codigo += $(this).val().trim();
    });
    return codigo;
}

async function ValidarCorreoUsuario() {
    const usuario = $('#usuarioRecuperacion').val().trim();
    const correo = $('#correoRecuperacion').val().trim();
    const btn = document.getElementById('btnValidarUser');

    if (!usuario || !correo) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos incompletos',
            text: 'Por favor, completa todos los campos antes de continuar.'
        });
        return;
    }

    btn.disabled = true;
    btn.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>Enviando...`;

    try {
        const response = await $.ajax({
            url: '/Home/ValidarCorreoYUsuario',
            type: 'POST',
            data: { usuario: usuario, correo: correo }
        });

        if (response.success) {
            await EnviarCodigoRecuperacionPassword();

            document.getElementById("step1").classList.add("d-none");
            document.getElementById("step2").classList.remove("d-none");
            document.getElementById("alertaCodigo").classList.remove("d-none");

            const otpInputs = document.querySelectorAll(".otp-input");
            if (otpInputs.length > 0) otpInputs[0].focus();
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Información Incorrecta',
                text: response.mensaje
            });
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error del servidor',
            text: 'No se pudo validar los datos. Inténtalo nuevamente.'
        });
    } finally {
        btn.disabled = false;
        btn.innerHTML = `Enviar código`;
    }
}

function EnviarCodigoRecuperacionPassword() {

    const usuario = $('#usuarioRecuperacion').val().trim();
    const correo = $('#correoRecuperacion').val().trim();

    $.ajax({
        url: '/Home/InsertarSolicitudRecuperacionContrasenia',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            usuario: usuario,
            correo: correo
        }),
        success: function (response) {
            console.log(response)
            if (response.success) {
                idUsuario = response.idUsuario
                localStorage.setItem("idSolicitud", response.idSolicitud);
                localStorage.setItem("idUsuario", response.idUsuario);
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function (xhr) {
            Swal.fire('Error', 'No se pudo procesar la solicitud.', 'error');
        }
    });
}

function VerificarCodigoIngresado() {
    let idUsuarioLC = localStorage.getItem("idUsuario");
    const codigoIngresado = obtenerCodigoOTP();
    console.log(codigoIngresado)
    console.log(idUsuarioLC)
    if (codigoIngresado.length !== 6) {
        console.log("sientro")
        $('#lblMensajeAlerta')
            .removeClass('d-none') 
            .text('Código Inválido. Por favor ingrese el código completo')
            .fadeIn();

        setTimeout(() => {
            $('#lblMensajeAlerta').fadeOut();
        }, 4000);
        return;
    }

    $.ajax({
        url: '/Home/VerificarCodigoCorreo',
        type: 'POST',
        data:{
            idUsuario: idUsuarioLC, 
            codigo: codigoIngresado
        },
        success: function (response) {
            console.log(response)
            if (response.success) {
                $('#recuperarContraseñaModal').modal('show');

                document.getElementById("step3").classList.remove("d-none");
                document.getElementById("step2").classList.add("d-none");
                document.getElementById("alertaCambioContrasena").classList.remove("d-none");
            } else {
                $('#lblMensajeAlerta')
                    .removeClass('d-none') 
                    .text(response.mensaje)
                    .fadeIn();

                setTimeout(() => {
                    $('#lblMensajeAlerta').fadeOut();
                }, 4000);
            }
        },
        error: function (xhr) {
            Swal.fire('Error', 'No se pudo verificar el código.', 'error');
        }
    });
}

function verificarEstadoCodigo() {
    const idSolicitud = localStorage.getItem("idSolicitud");
    if (!idSolicitud) {
        $('#recuperarContraseñaModal').modal('show');
        return;
    }

    $.ajax({
        url: '/Home/ObtenerEstadoCodigoCorreo',
        type: 'GET',
        data: { idSolicitud: idSolicitud },
        success: function (data) {
            console.log(data);

            const fechaExpiracion = new Date(data.fechaExpiracion);
            const ahora = new Date();
            const codigoExpirado = fechaExpiracion > ahora && data.estado === null;
            console.log(codigoExpirado)

            if (codigoExpirado) {
                console.log("Código valido");
                $('#recuperarContraseñaModal').modal('show');
                document.getElementById("step1").classList.add("d-none");
                document.getElementById("step3").classList.add("d-none");
                document.getElementById("step2").classList.remove("d-none");
                document.getElementById("lblMensajeAlerta").classList.add("d-none");
                return;
            }

            console.log("Código Inválido");
            $('#recuperarContraseñaModal').modal('show');

            document.getElementById("step1").classList.remove("d-none");
            document.getElementById("step3").classList.add("d-none");
            document.getElementById("step2").classList.add("d-none");
            document.getElementById("lblMensajeAlerta").classList.remove("d-none");

            const otpInputs = document.querySelectorAll(".otp-input");
            if (otpInputs.length > 0) otpInputs[0].focus();
        },
        error: function () {
            Swal.fire('Error', 'No se pudo obtener el estado del código.', 'error');
        }
    });
}