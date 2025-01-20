function loadAsignarClienteAVendedor() {
    $.ajax({
        url: '/Supervisor/AsignarVendedorView',
        type: 'GET',
        success: function (result) {
            if (result.success != false) {
                $('#modalContentAsignarVend').html(result);
                $('#asignarVendModal').modal('show');
            } else {
                Swal.fire({
                    title: 'Error al cargar los datos',
                    text: `${result.message}`,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }
        },
        error: function () {
            Swal.fire({
                title: 'Error al cargar los datos',
                text: 'Hubo un error al intentar cargar los datos. Por favor, inténtalo nuevamente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

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

    if (fechaInicio == "" || fechaFin == "") {

        Swal.fire({
            title: 'Error al descargar datos',
            text: 'Debe seleccionar un rango de fechas.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
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
    
    console.log("Fecha inicio:", fechaInicio, "Fecha fin:", fechaFin);
    window.location.href = '/Excel/DescargarClientesAsignados?fechaInicio=' + fechaInicio + '&fechaFin=' + fechaFin;
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

function loadFuncionalidadAsesores() {
    $.ajax({
        url: '/Supervisor/ModificarAsesoresView',
        type: 'GET',
        success: function (response) {
            $('#modalContentModificarAsesorModal').html(response); // Insertar la vista parcial en el modal
            $('#modificarAsesorModal').modal('show'); // Mostrar el modal
        },
        error: function () {
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la vista de agregar usuario.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function loadReasignarClientesAsignados(){
    $.ajax({
        url: '/AsesoresSecundarios/AsignarAsesoresSecundariosView',
        type: 'GET',
        success: function (response) {
            if (response.error === true) {
                Swal.fire({
                    title: 'Error',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                    });
                    
            }
            else {
                $('#modalContentReasignarAsesorSecundarioModal').html(response); // Insertar la vista parcial en el modal
                $('#ReasignarAsesorSecundarioModal').modal('show'); // Mostrar el modal
            }
        },
        error: function () {
            Swal.fire({
                title: 'Error',
                text: 'No se pudo cargar la vista de agregar usuario.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}