function cargarDerivacionesXAsesor(DniAsesor) {
    $.ajax({
        type: 'GET',
        url: '/Derivacion/ObtenerDerivacionesXAsesor',
        data: {
            DniAsesor: DniAsesor
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#DerivacionXAsesor').html(response);
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las derivaciones',
                text: 'Hubo un error inesperado al cargar las derivaciones.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
    
}