function sortTable(tableId, colIndex, type) {
    const table = document.getElementById(tableId);
    const rows = Array.from(table.rows).slice(2); 
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
            
            return isAscending
                ? aText.toLowerCase().localeCompare(bText.toLowerCase())
                : bText.toLowerCase().localeCompare(aText.toLowerCase());
        }
        return isAscending
            ? aText.localeCompare(bText)
            : bText.localeCompare(aText);
    });
    rows.forEach(row => table.tBodies[0].appendChild(row)); 
}

//document.getElementById('startDate').addEventListener('input', filterByDate);
//document.getElementById('endDate').addEventListener('input', filterByDate);

//function filterByDate() {
//    const table = document.getElementById('clientesTable');
//    const rows = table.tBodies[0].rows;
    
//    const startDate = document.getElementById('startDate').value;
//    const endDate = document.getElementById('endDate').value;
    
//    function parseDate(dateStr) {
//        const [date, time] = dateStr.split(' '); 
//        const [day, month, year] = date.split('/'); 
        
//        return new Date(`${year}-${month}-${day}`);
//    }
    
//    const startDateObj = new Date(startDate); 
//    const endDateObj = new Date(endDate); 
//    Array.from(rows).forEach(row => {
//        const rowDateStr = row.cells[5].textContent.trim(); 
//        const rowDateObj = parseDate(rowDateStr); 

        
//        let match = false;
//        if (startDateObj instanceof Date && !isNaN(startDateObj) && endDateObj instanceof Date && !isNaN(endDateObj)) {
            
//            if (rowDateObj >= startDateObj && rowDateObj <= endDateObj) {
//                match = true; 
//            }
//        } else if (startDateObj instanceof Date && !isNaN(startDateObj)) {
            
//            if (rowDateObj >= startDateObj) {
//                match = true; 
//            }
//        } else if (endDateObj instanceof Date && !isNaN(endDateObj)) {
            
//            if (rowDateObj <= endDateObj) {
//                match = true; 
//            }
//        } else {
//            match = true; 
//        }
        
//        if (match) {
//            row.style.display = ''; 
//        } else {
//            row.style.display = 'none'; 
//        }
//    });
//}



function filtrarPorRol(rol) {
    const table = document.getElementById('clientesTable');
    const rows = table.tBodies[0].rows;

    Array.from(rows).slice(1).forEach(row => {
        let cell = row.cells[6].textContent.trim();
        if (!cell) return; 

        if (rol === "") {
            row.style.display = ''; 
            return;
        }
        let match = cell === rol;
        if (rol === "ASESOR") {
            match = cell.includes("VENDEDOR") || cell.includes("ASESOR");
        }
        row.style.display = match ? '' : 'none'; 
    });
}

//document.getElementById('searchInput').addEventListener('input', function () {
//    const searchField = document.getElementById('searchField').value;
//    const filter = this.value.toLowerCase();
//    const table = document.getElementById('clientesTable');
//    const rows = table.tBodies[0].rows;

    
//    const startDate = document.getElementById('startDate').value;
//    const endDate = document.getElementById('endDate').value;

    
//    function parseDate(dateStr) {
//        const [day, month, year, time] = dateStr.split(/[\/\s:]+/);
//        if (time) {
//            const [hour, minute, second] = time.split(':');
//            return new Date(`${year}-${month}-${day}T${hour}:${minute}:${second}`);
//        } else {
//            return new Date(`${year}-${month}-${day}`);
//        }
//    }

//    Array.from(rows).slice(1).forEach(row => {
//        let cellValue = '';
//        let rowDate = '';

//        switch (searchField) {
//            case 'apellidoPaterno':
//                cellValue = row.cells[2].textContent.toLowerCase(); 
//                break;
//            /*case 'apellidoMaterno':
//                cellValue = row.cells[3].textContent.toLowerCase(); 
//                break;
//            case 'nombre':
//                cellValue = row.cells[4].textContent.toLowerCase(); 
//                break;*/
//            case 'ofertaMax':
//                cellValue = row.cells[3].textContent.toLowerCase(); 
//                break;
//            case 'comentarioPrincipal':
//                cellValue = row.cells[5].textContent.toLowerCase(); 
//                break;
//            case 'dni':
//                cellValue = row.cells[1].textContent.toLowerCase(); 
//                break;
//            case 'campaña':
//                cellValue = row.cells[4].textContent.toLowerCase(); 
//                break;
//            case 'tipificacionRelevante':
//                cellValue = row.cells[7].textContent.toLowerCase(); 
//                break;
//            case 'fechaAsignacion':
//                rowDate = row.cells[5].textContent.trim(); 
//                break;
//            case 'nombres':
//                //cellValue = row.cells[1].textContent.toLowerCase(); 
//                cellValue = row.cells[3].textContent.toLowerCase(); 
//                break;
//            case 'dniadm':
//                cellValue = row.cells[2].textContent.toLowerCase(); 
//                break;
//            default:
//                cellValue = ''; 
//                break;
//        }

//        let match = false;

//        if (searchField === 'fechaAsignacion') {
            
//            const rowDateObj = parseDate(rowDate);
//            const startDateObj = startDate ? parseDate(startDate) : null;
//            const endDateObj = endDate ? parseDate(endDate) : null;
            
//            if (startDateObj && rowDateObj < startDateObj) {
//                match = false;
//            } else if (endDateObj && rowDateObj > endDateObj) {
//                match = false;
//            } else {
//                match = true;
//            }
//        } else {
//            match = cellValue.includes(filter);
//        }

//        if (match) {
//            row.style.display = ''; 
//        } else {
//            row.style.display = 'none'; 
//        }
//    });
//});

//document.getElementById('searchField').addEventListener('change', function () {
//    const searchField = this.value;
//    const startDateInput = document.getElementById('startDate');
//    const endDateInput = document.getElementById('endDate');
//    const searchInput = document.getElementById('searchInput');
//    const rolFiltro = document.getElementById('rolFiltro');

    
//    if (searchField === 'fechaAsignacion') {
//        startDateInput.style.display = 'block';
//        endDateInput.style.display = 'block';
//        searchInput.style.display = 'none';
//        rolFiltro.style.display = 'none';
//    } if (searchField === 'rol') {
//        startDateInput.style.display = 'none';
//        endDateInput.style.display = 'none';
//        searchInput.style.display = 'none';
//        rolFiltro.style.display = 'block';
//    }
//    else {
//        startDateInput.style.display = 'none';
//        endDateInput.style.display = 'none';
//        searchInput.style.display = 'block';
//        rolFiltro.style.display = 'none';
//    }
//});
