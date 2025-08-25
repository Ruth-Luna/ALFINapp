
let idUsuario = 0
let usuario = null;
let correo = null;
let ip = null;
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
    $('#btnValidarUser').on('click', function () {
        ValidarCorreoUsuario('btnValidarUser');
    });

    $('#btnSolicitarNuevoCodigo').on('click', function () {
        EnviarCodigoRecuperacionPassword()
    });

    $('#btnVerificarCodigo').on('click', function () {
        VerificarCodigoIngresado();
    });

    $('#btnCambiarContrasenia').on('click', function () {
        cambiarContraseniaCorreo();
    });
    
});

function obtenerCodigoOTP() {
    let codigo = '';
    $('#otpInputs .otp-input').each(function () {
        codigo += $(this).val().trim();
    });
    return codigo;
}
async function ValidarCorreoUsuario(buttonId) {
    usuario = $('#usuarioRecuperacion').val().trim();
    correo = $('#correoRecuperacion').val().trim();

    localStorage.setItem("usuarioRecuperacion", usuario);
    localStorage.setItem("correoRecuperacion", correo);

    const btn = document.getElementById(buttonId);

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

    // Mostrar alerta específica para btnSolicitarNuevoCodigo
    let alertInstance = null;
    if (buttonId === 'btnSolicitarNuevoCodigo') {
        alertInstance = Swal.fire({
            title: 'Enviando código',
            text: 'Por favor, espera mientras se envía el código.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    }

    try {
        const response = await $.ajax({
            url: '/Home/ValidarCorreoYUsuario',
            type: 'POST',
            data: { usuario: usuario, correo: correo }
        });

        if (response.success) {
            await EnviarCodigoRecuperacionPassword();

            // Actualizar la interfaz para mostrar el paso 2
            document.getElementById("step1").classList.add("d-none");
            document.getElementById("step2").classList.remove("d-none");
            document.getElementById("alertaCodigo").classList.remove("d-none");

            const otpInputs = document.querySelectorAll(".otp-input");
            if (otpInputs.length > 0) otpInputs[0].focus();

            // Cerrar la alerta de "Enviando código" si existe
            if (alertInstance) {
                Swal.close();
            }
        } else {
            // Cerrar la alerta de "Enviando código" si existe antes de mostrar el error
            if (alertInstance) {
                Swal.close();
            }
            Swal.fire({
                icon: 'error',
                title: 'Información Incorrecta',
                text: response.mensaje
            });
        }
    } catch (error) {
        // Cerrar la alerta de "Enviando código" si existe antes de mostrar el error
        if (alertInstance) {
            Swal.close();
        }
        Swal.fire({
            icon: 'error',
            title: 'Error del servidor',
            text: 'No se pudo validar los datos. Inténtalo nuevamente.'
        });
    } finally {
        btn.disabled = false;
        btn.innerHTML = buttonId === 'btnValidarUser' ? 'Enviar código' : 'Solicitar nuevo código';
    }
}
async function getPublicIp() {
    try {
        const response = await fetch('https://api.ipify.org?format=json');
        const data = await response.json();
        return data.ip;
    } catch (error) {
        console.error('Error al obtener la IP pública:', error);
        return null;
    }
}

async function EnviarCodigoRecuperacionPassword() {
     usuario = $('#usuarioRecuperacion').val().trim();
     correo = $('#correoRecuperacion').val().trim();

    if (!usuario) {
        usuario = localStorage.getItem("usuarioRecuperacion");
    }
    if (!correo) {
        correo = localStorage.getItem("correoRecuperacion");
    }

    // Obtener la IP pública
    ip = await getPublicIp();
    if (!ip) {
        Swal.fire('Error', 'No se pudo obtener la dirección IP.', 'error');
        return;
    }
    console.log(usuario)
    console.log(correo)
    console.log(ip)
    return $.ajax({
        url: '/Home/InsertarSolicitudRecuperacionContrasenia',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            usuario: usuario,
            correo: correo,
            ip: ip
        }),
        success: function (response) {
            console.log(response);
            if (response.success) {


                idUsuario = response.idUsuario;
                localStorage.setItem("idSolicitud", response.idSolicitud);
                localStorage.setItem("idUsuario", response.idUsuario);
                $('#lblMensajeAlerta')
                    .removeClass('d-none text-danger')
                    .addClass('text-success fw-semibold')
                    .text('Código enviado correctamente.')
                    .fadeIn();
                setTimeout(() => {
                    $('#lblMensajeAlerta').fadeOut();
                }, 4000);

                verificarEstadoCodigo();
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

    if (codigoIngresado.length !== 6) {
        $('#lblMensajeAlerta')
            .removeClass('d-none text-danger')
            .addClass('text-danger fw-semibold')
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

                setTimeout(() => {
                    $('#lblMensajeAlerta').fadeOut();
                }, 4000);

            } else {
                $('#lblMensajeAlerta')
                    .removeClass('d-none text-danger')
                    .addClass('text-danger fw-semibold')
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

let timerInterval = null;

function startTimer(fechaExpiracion) {
    // Limpiar
    if (timerInterval) {
        clearInterval(timerInterval);
    }

    // Actualizar el temporizador
    const updateTimer = () => {
        const now = new Date();
        const diffInSeconds = Math.floor((fechaExpiracion - now) / 1000);
        if (diffInSeconds <= 0) {
            document.getElementById("tiempoExpiracion").textContent = "El código ha expirado";
            clearInterval(timerInterval);
            timerInterval = null;
            return;
        }
        const minutes = Math.floor(diffInSeconds / 60);
        const seconds = diffInSeconds % 60;
        const formattedTime = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        document.getElementById("tiempoExpiracion").innerHTML = `<strong>El código expira en: ${formattedTime}</strong>`;
    };

    // Actualizar cada segundo
    updateTimer();
    timerInterval = setInterval(updateTimer, 1000);
}

function verificarEstadoCodigo() {
    const idSolicitud = localStorage.getItem("idSolicitud");
    if (!idSolicitud) {
        document.getElementById("step1").classList.remove("d-none");
        document.getElementById("step2").classList.add("d-none");
        document.getElementById("step3").classList.add("d-none");
        $('#recuperarContraseñaModal').modal('show');

        return;
    }

    $.ajax({
        url: '/Home/ObtenerEstadoCodigoCorreo',
        type: 'GET',
        data: { idSolicitud: idSolicitud },
        success: function (data) {

            const fechaExpiracion = new Date(data.fechaExpiracion);
            const fechaActual = new Date();
            const codigoExpirado = fechaExpiracion > fechaActual && data.estado === null;

            if (codigoExpirado) {
                $('#recuperarContraseñaModal').modal('show');
                document.getElementById("step1").classList.add("d-none");
                document.getElementById("step3").classList.add("d-none");
                document.getElementById("step2").classList.remove("d-none");
   
                // Iniciar el temporizador
                startTimer(fechaExpiracion);


                return;
            }

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
function cambiarContraseniaCorreo() {
    const idUsuario = localStorage.getItem("idUsuario");
    const nueva = $('#txtNuevaContrasenia').val().trim();
    const confirmar = $('#txtConfirmarContrasenia').val().trim();

    if (!nueva || !confirmar) {
        Swal.fire("Advertencia", "Completa ambos campos de contraseña.", "warning");
        return;
    }

    if (nueva !== confirmar) {
        Swal.fire("Error", "Las contraseñas no coinciden.", "warning");
        return;
    }

    $.ajax({
        url: '/Home/ActualizarContraseniaCorreo',
        type: 'POST',
        data: {
            idUsuario: idUsuario,
            contrasenia: nueva
        },
        success: function (response) {
            console.log(response)
            if (response.success) {
                Swal.fire("Éxito", response.mensaje, "success").then(() => {
                    $('#recuperarContraseñaModal').modal('hide');
                    window.location.href = '/';
                    localStorage.removeItem("correoRecuperacion");
                    localStorage.removeItem("idSolicitud");
                    localStorage.removeItem("idUsuario");
                    localStorage.removeItem("usuarioRecuperacion");
s
                });
            } else {
                Swal.fire("Advertencia", response.mensaje, "error");
            }
        },
        error: function (xhr) {
            console.error(xhr);
            Swal.fire("Error", "Ocurrió un error al actualizar la contraseña.", "error");
        }
    });
}