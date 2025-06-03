function loadModificarVendedorAsignado(idAsignacion) {
    $.ajax({
        url: '/Supervisor/ModificarAsignacionVendedorView',
        type: 'GET',
        data: { id_asignacion: idAsignacion },
        success: function (result) {
            $('#modalContentModificarVendAsignar').html(result);
            $('#modificarVendModal').modal('show');
        },
        error: function () {
            Swal.fire({
                title: 'Error al cargar los datos',
                text: 'Error al cargar los datos del cliente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function descargarDatos() {
    const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin').value;
    const destinoBase = document.getElementById('BaseDestino').value;
    let messages = [];

    if (fechaInicio == "" || fechaFin == "") {
        messages.push("No se ha seleccionado un rango de fechas.");
    }
    
    if (destinoBase == "") {
        messages.push("No ha seleccionado una base destino.");
    }

    if (messages.length > 0) {
        Swal.fire({
            title: 'Precaucion',
            html: messages.join("<br>"),
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
    }
    
    window.location.href = '/Excel/DescargarClientesAsignados?fechaInicio=' + fechaInicio + '&fechaFin=' + fechaFin + '&destinoBase=' + destinoBase;
}