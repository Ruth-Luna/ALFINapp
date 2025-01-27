function BuscarDNIAReferir(idDniLabel) {
    const dniBusqueda = document.getElementById(idDniLabel);
    const datosClienteExistente = document.getElementById('datosClienteExistente');

    if (dniBusqueda.value == "") {
        Swal.fire({
            title: 'Error al buscar',
            text: 'Debe ingresar un DNI.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    const dni = dniBusqueda.value;
    $.ajax({
        url: '/Referido/BuscarDNIReferido',
        type: 'GET',
        data: {
            dniBusqueda: dni
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al buscar',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Busqueda Exitosa',
                    text: "El DNI ha sido encontrado",
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                datosClienteExistente.innerHTML = response;
                datosClienteExistente.style.display = 'block';
            }
        },
        error: function () {
            Swal.fire({
                title: 'Error al buscar',
                text: 'Ocurrió un error al buscar el DNI.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function ReferirDNI(dni, fuenteBase) {
    const baseFuente = fuenteBase;
    
    const NombresCompletosUsuario = document.getElementById('NombresCompletosUsuario');
    const ApellidosCompletosUsuario = document.getElementById('ApellidosCompletosUsuario');
    const DNIUsuario = document.getElementById('DNIUsuario');
    if (dni == "" || fuenteBase == "" || NombresCompletosUsuario.value == "" || ApellidosCompletosUsuario.value == "" || DNIUsuario.value == "") {
        Swal.fire({
            title: 'Error al referir',
            text: 'Hay un error en el envio de Datos. O no se han completado los campos.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        url: '/Referido/ReferirCliente',
        type: 'POST',
        data: {
            dniReferir: dni,
            fuenteBase: baseFuente,
            nombres: NombresCompletosUsuario.value.toUpperCase(),
            apellidos: ApellidosCompletosUsuario.value.toUpperCase(),
            dniUsuario: DNIUsuario.value
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al referir',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });

            } else {
                Swal.fire({
                    title: 'Referido exitoso',
                    text: response.message + ". Puede cerrar esta pagina.",
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function () {
            Swal.fire({
                title: 'Error al referir',
                text: 'Ocurrió un error al referir el cliente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}