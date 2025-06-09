function validarTelefono() {
    const campobusqueda = document.getElementById('dnicliente').value;

    if (!campobusqueda) {
        Swal.fire({
            title: 'Error',
            text: 'El campo del cliente es obligatorio.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/Consulta/VerificarTelefono',
        data: {
            telefono: campobusqueda
        },
        success: function (response) {
            if (response.existe === false) {
                Swal.fire({
                    title: 'Cliente no encontrado',
                    text: response.message,
                    icon: 'info',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else if (response.error === true) {
                Swal.fire({
                    title: 'Error en la búsqueda',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }
            else {
                Swal.fire({
                    title: 'Cliente encontrado',
                    text: 'El cliente ha sido encontrado en la base de datos.',
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                const datosClienteExistente = document.getElementById("datos-cliente-existente");
                datosClienteExistente.style.display = "block";
                datosClienteExistente.innerHTML = response; // Carga la vista parcial
            }
        },
        error: function (error) {
            console.error(error);
            Swal.fire({
                title: 'Error',
                text: error,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
    });
}