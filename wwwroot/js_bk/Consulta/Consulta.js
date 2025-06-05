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
        success: function (data) {
            if (data.success === false) {
                Swal.fire({
                    title: 'Error',
                    text: data.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });

                return;
            } else {
                Swal.fire({
                    title: 'Cliente encontrado',
                    text: 'El cliente ha sido encontrado en la base de datos.',
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                const datosClienteExistente = document.getElementById("datos-cliente-existente");
                datosClienteExistente.style.display = "block";
                datosClienteExistente.innerHTML = data; // Carga la vista parcial
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