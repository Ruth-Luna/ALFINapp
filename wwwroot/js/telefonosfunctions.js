function guardarTelefonoVendedor(idcliente) {
    const numeroTelefono = document.getElementById('telefonoVendedorAgregado').value;
    const idClienteTelefono = idcliente;

    if (!numeroTelefono.trim()) {
        Swal.fire({
            title: 'Error al agregar número de teléfono',
            text: 'El número de teléfono no puede estar vacío.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (!/^\d+$/.test(numeroTelefono)) {
        Swal.fire({
            title: 'Error al agregar número de teléfono',
            text: 'Ingrese solo números',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (!/^\d{9}$/.test(numeroTelefono)) {
        Swal.fire({
            title: 'Error al agregar número de teléfono',
            text: 'El número de teléfono debe tener exactamente 9 dígitos.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        url: '/Telefonos/AgregarTelefonoPorAsesor',
        type: 'POST',
        data: {
            numeroTelefono: numeroTelefono,
            idClienteTelefono: idClienteTelefono
        },
        success: function (result) {
            if (result.success === true) {
                Swal.fire({
                    title: 'Mensaje de Confirmación',
                    text: `${result.message}. Este la pagina se recargara en 3 segundos.`,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                setTimeout(function () {
                    location.reload();
                }, 3000);
            } else {
                Swal.fire({
                    title: 'Error en la operación',
                    text: result.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error',
                text: 'Error al agregar un nuevo numero de Telefono (Comunicarse con Servicio Técnico)',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}


function cargarEdicionDeTelefonos(idcliente) {
    if (!idcliente) {
        Swal.fire({
            title: 'Error al cargar la vista',
            text: 'El ID del cliente no es válido.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    Swal.fire({
        title: '¿Está seguro?',
        text: 'Agregar un nuevo teléfono notificará al Supervisor. ¿Está seguro de continuar?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Continuar',
        cancelButtonText: 'Cancelar',
        dangerMode: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Vendedor/AgregarTelefonoView',
                type: 'GET',
                data: { "idCliente": idcliente },
                success: function (response) {
                    document.getElementById('agregarTelefonoBtnCliente').style.display = 'none';
                    document.getElementById('cancelaragregarTelefonoBtnCliente').style.display = 'block';
                    document.getElementById('agregarTelefonoContainer').style.display = 'block';
                    $('#agregarTelefonoContainer').html(response);
                },
                error: function () {
                    Swal.fire({
                        title: 'Error al cargar la vista',
                        text: 'Hubo un error al cargar la vista.',
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                }
            });
        }
    });
}

function descargarEdicionDeTelefonos() {
    document.getElementById('agregarTelefonoBtnCliente').style.display = 'block';
    document.getElementById('cancelaragregarTelefonoBtnCliente').style.display = 'none';
    document.getElementById('agregarTelefonoContainer').style.display = 'none';
}
