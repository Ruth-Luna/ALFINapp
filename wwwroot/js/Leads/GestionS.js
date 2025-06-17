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

$(document).ready(function () {
    $('#BaseDestino').select2({
        placeholder: "Seleccione una base o lista",
        allowClear: true,
        width: '100%'
    });
});
