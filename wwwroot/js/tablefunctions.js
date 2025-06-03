function sortTable(tableId, colIndex, type) {
    const table = document.getElementById(tableId);
    const rows = Array.from(table.rows).slice(2); // Exclude header
    const isAscending = table.dataset.sortOrder !== 'asc';
    table.dataset.sortOrder = isAscending ? 'asc' : 'desc';
    rows.sort((a, b) => {
        if (a.cells[colIndex].querySelector('button')) return 0;

        const aText = a.cells[colIndex].textContent.trim();
        const bText = b.cells[colIndex].textContent.trim();

        if (type === 'number') {
            return isAscending
                ? parseFloat(aText) - parseFloat(bText)
                : parseFloat(bText) - parseFloat(aText);
        }
        if (type === 'date') {
            const parseDate = (dateStr) => {
                const [date, time] = dateStr.split(' ');
                const [day, month, year] = date.split('/');
                return new Date(`${year}-${month}-${day}T${time}`);
            };

            let aDate = parseDate(aText);
            let bDate = parseDate(bText);

            return isAscending
                ? aDate - bDate
                : bDate - aDate;
        }
        if (type === 'bool') {
            return isAscending
                ? aText.toLowerCase() === 'true' ? -1 : 1
                : aText.toLowerCase() === 'true' ? 1 : -1;
        }
        if (type === 'string') {
            // Normalize text for case-insensitive comparison
            return isAscending
                ? aText.toLowerCase().localeCompare(bText.toLowerCase())
                : bText.toLowerCase().localeCompare(aText.toLowerCase());
        }
        return isAscending
            ? aText.localeCompare(bText)
            : bText.localeCompare(aText);
    });
    rows.forEach(row => table.tBodies[0].appendChild(row)); // Re-attach sorted rows
}

document.getElementById('startDate').addEventListener('input', filterByDate);
document.getElementById('endDate').addEventListener('input', filterByDate);

function filterByDate() {
    const table = document.getElementById('clientesTable');
    const rows = table.tBodies[0].rows;
    // Obtener las fechas de los campos de fecha
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;
    // Función para convertir la fecha a un formato Date que JavaScript pueda comparar
    function parseDate(dateStr) {
        const [date, time] = dateStr.split(' '); // Divide la fecha y la hora
        const [day, month, year] = date.split('/'); // Divide el día, mes y año
        // Devolvemos la fecha en formato yyyy-mm-dd (sin la hora)
        return new Date(`${year}-${month}-${day}`);
    }
    // Convertir las fechas seleccionadas a objetos Date
    const startDateObj = new Date(startDate); // Asumimos 00:00:00 como hora por defecto
    const endDateObj = new Date(endDate); // Asumimos 23:59:59 como hora por defecto
    Array.from(rows).forEach(row => {
        const rowDateStr = row.cells[5].textContent.trim(); // Fecha de Asignación (columna 7)
        const rowDateObj = parseDate(rowDateStr); // Convertir la fecha de la fila a objeto Date

        // Verificar si la fecha está dentro del rango
        let match = false;
        if (startDateObj instanceof Date && !isNaN(startDateObj) && endDateObj instanceof Date && !isNaN(endDateObj)) {
            // Si ambos startDateObj y endDateObj son válidos, comparamos el rango
            if (rowDateObj >= startDateObj && rowDateObj <= endDateObj) {
                match = true; // La fecha de la fila está dentro del rango
            }
        } else if (startDateObj instanceof Date && !isNaN(startDateObj)) {
            // Si solo startDateObj es válido, comparamos solo con la fecha inicial
            if (rowDateObj >= startDateObj) {
                match = true; // La fecha de la fila es posterior o igual a la fecha inicial
            }
        } else if (endDateObj instanceof Date && !isNaN(endDateObj)) {
            // Si solo endDateObj es válido, comparamos solo con la fecha final
            if (rowDateObj <= endDateObj) {
                match = true; // La fecha de la fila es anterior o igual a la fecha final
            }
        } else {
            match = true; // Si no se seleccionó ninguna fecha, se muestra la fila
        }
        // Mostrar u ocultar la fila según el resultado de la comparación
        if (match) {
            row.style.display = ''; // Mostrar la fila
        } else {
            row.style.display = 'none'; // Ocultar la fila
        }
    });
}

function filtrarPorRol(rol) {
    const table = document.getElementById('clientesTable');
    const rows = table.tBodies[0].rows;

    Array.from(rows).slice(1).forEach(row => {
        let cell = row.cells[6].textContent.trim();
        if (!cell) return; // Si no existe la celda, saltar

        if (rol === "") {
            row.style.display = ''; // Muestra todas las filas
            return;
        }
        let match = cell === rol;
        if (rol === "ASESOR") {
            match = cell.includes("VENDEDOR") || cell.includes("ASESOR");
        }
        row.style.display = match ? '' : 'none'; // Muestra u oculta la fila
    });
}

document.getElementById('searchInput').addEventListener('input', function () {
    const searchField = document.getElementById('searchField').value;
    const filter = this.value.toLowerCase();
    const table = document.getElementById('clientesTable');
    const rows = table.tBodies[0].rows;

    // Obtener fechas del filtro si se seleccionaron
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    // Función para convertir la fecha a un formato Date que JavaScript pueda comparar
    function parseDate(dateStr) {
        const [day, month, year, time] = dateStr.split(/[\/\s:]+/);
        if (time) {
            const [hour, minute, second] = time.split(':');
            return new Date(`${year}-${month}-${day}T${hour}:${minute}:${second}`);
        } else {
            return new Date(`${year}-${month}-${day}`);
        }
    }

    Array.from(rows).slice(1).forEach(row => {
        let cellValue = '';
        let rowDate = '';

        switch (searchField) {
            case 'apellidoPaterno':
                cellValue = row.cells[2].textContent.toLowerCase(); // Apellido Paterno
                break;
            /*case 'apellidoMaterno':
                cellValue = row.cells[3].textContent.toLowerCase(); // Columna de Apellido Materno
                break;
            case 'nombre':
                cellValue = row.cells[4].textContent.toLowerCase(); // Columna de Nombre
                break;*/
            case 'ofertaMax':
                cellValue = row.cells[3].textContent.toLowerCase(); // Columna de Oferta Max
                break;
            case 'comentarioPrincipal':
                cellValue = row.cells[5].textContent.toLowerCase(); // Columna de Comentario Principal
                break;
            case 'dni':
                cellValue = row.cells[1].textContent.toLowerCase(); // DNI
                break;
            case 'campaña':
                cellValue = row.cells[4].textContent.toLowerCase(); // Campaña
                break;
            case 'tipificacionRelevante':
                cellValue = row.cells[7].textContent.toLowerCase(); // Tipificación más Relevante
                break;
            case 'fechaAsignacion':
                rowDate = row.cells[5].textContent.trim(); // Fecha de Asignación
                break;
            case 'nombres':
                //cellValue = row.cells[1].textContent.toLowerCase(); // Nombres
                cellValue = row.cells[3].textContent.toLowerCase(); // Nombres
                break;
            case 'dniadm':
                cellValue = row.cells[2].textContent.toLowerCase(); // Nombres
                break;
            default:
                cellValue = ''; // En caso de no coincidir con ninguna opción
                break;
        }

        let match = false;

        if (searchField === 'fechaAsignacion') {
            // Convertir la fecha en el formato de la tabla a un objeto Date
            const rowDateObj = parseDate(rowDate);
            const startDateObj = startDate ? parseDate(startDate) : null;
            const endDateObj = endDate ? parseDate(endDate) : null;
            // Verificar si la fecha de la fila está dentro del rango seleccionado
            if (startDateObj && rowDateObj < startDateObj) {
                match = false;
            } else if (endDateObj && rowDateObj > endDateObj) {
                match = false;
            } else {
                match = true;
            }
        } else {
            match = cellValue.includes(filter);
        }

        if (match) {
            row.style.display = ''; // Muestra la fila
        } else {
            row.style.display = 'none'; // Oculta la fila
        }
    });
});

document.getElementById('searchField').addEventListener('change', function () {
    const searchField = this.value;
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    const searchInput = document.getElementById('searchInput');
    const rolFiltro = document.getElementById('rolFiltro');

    // Mostrar u ocultar los campos de fecha según la opción seleccionada
    if (searchField === 'fechaAsignacion') {
        startDateInput.style.display = 'block';
        endDateInput.style.display = 'block';
        searchInput.style.display = 'none';
        rolFiltro.style.display = 'none';
    } if (searchField === 'rol') {
        startDateInput.style.display = 'none';
        endDateInput.style.display = 'none';
        searchInput.style.display = 'none';
        rolFiltro.style.display = 'block';
    }
    else {
        startDateInput.style.display = 'none';
        endDateInput.style.display = 'none';
        searchInput.style.display = 'block';
        rolFiltro.style.display = 'none';
    }
});
