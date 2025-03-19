function activarEdicion(campo) {
    const BotonEnvio = document.getElementById(campo + "BotonEnvio");
    const BotonEdicion = document.getElementById(campo + "BotonEdicion");
    const Usuario = document.getElementById(campo + "Usuario");
    const AlertUpdate = document.getElementById("AlertUpdate" + campo);

    BotonEnvio.style.display = "block";
    BotonEdicion.style.display = "none";
    Usuario.removeAttribute('readonly');
    Usuario.focus();
    AlertUpdate.style.display = "block";
}

function enviarEdicion(campo) {
    const BotonEnvio = document.getElementById(campo + "BotonEnvio");
    const BotonEdicion = document.getElementById(campo + "BotonEdicion");
    const AlertUpdate = document.getElementById("AlertUpdate" + campo);
    const Usuario = document.getElementById(campo + "Usuario");

    BotonEnvio.style.display = "none";
    BotonEdicion.style.display = "block";
    AlertUpdate.style.display = "none";
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
    if (campo!="Correo") {
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