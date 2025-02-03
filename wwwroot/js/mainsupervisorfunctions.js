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

    if (fechaInicio > fechaFin) {
        Swal.fire({
            title: 'Error al descargar datos',
            text: 'La fecha de inicio no puede ser mayor que la fecha de fin.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
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

function sortTable(columnIndex, type) {
    const table = document.getElementById(tableId);
    const rows = Array.from(table.rows).slice(1); // Exclude header
    const isAscending = table.dataset.sortOrder !== 'asc';
    table.dataset.sortOrder = isAscending ? 'asc' : 'desc';

    rows.sort((a, b) => {
        const aText = a.cells[colIndex].textContent.trim();
        const bText = b.cells[colIndex].textContent.trim();

        if (type === 'number') {
            return isAscending
                ? parseFloat(aText) - parseFloat(bText)
                : parseFloat(bText) - parseFloat(aText);
        }
        if (type === 'date') {
            return isAscending
                ? new Date(aText) - new Date(bText)
                : new Date(bText) - new Date(aText);
        }
        if (type === 'bool') {
            return isAscending
                ? aText.toLowerCase() === 'true' ? -1 : 1
                : aText.toLowerCase() === 'true' ? 1 : -1;
        }

        return isAscending
            ? aText.localeCompare(bText)
            : bText.localeCompare(aText);
    });

    rows.forEach(row => table.tBodies[0].appendChild(row));
}

function showFileName() {
    const fileInput = document.getElementById('csvFileInput');
    const fileNameDisplay = document.getElementById('fileNameDisplay');

    // Si se selecciona un archivo, muestra su nombre
    if (fileInput.files && fileInput.files[0]) {
        fileNameDisplay.textContent = fileInput.files[0].name;
    } else {
        fileNameDisplay.textContent = ''; // Si no hay archivo, limpia el texto
    }
}