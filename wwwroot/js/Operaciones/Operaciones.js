// Variable global para la API de la grilla.
let gridApi;

// Definición de las columnas para la tabla.
const derivacionesTableColumns = [
    { headerName: "Estado derivación", field: "estadoDerivacion" },
    { headerName: "DNI cliente", field: "dniCliente" },
    { headerName: "Nombre del cliente", field: "nombreCliente" },
    { headerName: "Teléfono", field: "telefono" },
    { headerName: "DNI asesor", field: "dniAsesor" },
    { headerName: "Oferta", field: "oferta" },
    { headerName: "Agencia", field: "agencia" },
    { headerName: "Fecha visita", field: "fechaVisita" },
    {
        headerName: "Estado evidencia",
        field: "estadoEvidencia",
        cellClass: "d-flex align-items-center justify-content-center",
        cellRenderer: (params) => {
            if (params.value === "Enviado") {
                return `<span class="badge-estado badge-enviado">Enviado</span>`;
            }
            return `<span class="badge-estado badge-no-enviado">No enviado</span>`;
        },
        comparator: (valueA, valueB) => (valueA === valueB ? 0 : valueA === 'No enviado' ? -1 : 1)
    },
    { headerName: "Fecha evidencia", field: "fechaEvidencia" },
    {
        headerName: "Acciones",
        field: "acciones",
        cellClass: "d-flex align-items-center justify-content-center",
        cellRenderer: () => `
            <div class="d-flex gap-2">
                <button class="btn btn-sm btn-primary" title="Enviar evidencia"><i class="ri-file-add-line"></i></button>
                <button class="btn btn-sm btn-info" title="Reagendamiento"><i class="ri-file-list-3-line"></i></button>
            </div>`,
        flex: 0,
        width: 130,
        sortable: false,
        resizable: false
    }
];

// Datos de ejemplo para la tabla.
const listaDerivaciones = [
    { estadoDerivacion: 'Pendiente', dniCliente: '12345678', nombreCliente: 'Juan Pérez', telefono: '987654321', dniAsesor: '87654321', oferta: 'Crédito Personal', agencia: 'Agencia Central', fechaVisita: '2024-08-15', estadoEvidencia: 'No enviado', fechaEvidencia: '2024-08-11', acciones: '' },
    { estadoDerivacion: 'Completado', dniCliente: '87654321', nombreCliente: 'Ana Gómez', telefono: '912345678', dniAsesor: '12345678', oferta: 'Tarjeta de Crédito', agencia: 'Sucursal Norte', fechaVisita: '2024-08-14', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-15', acciones: '' },
    { estadoDerivacion: 'En Proceso', dniCliente: '11223344', nombreCliente: 'Carlos Ruiz', telefono: '955555555', dniAsesor: '44332211', oferta: 'Seguro de Vida', agencia: 'Agencia Sur', fechaVisita: '2024-08-16', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-16', acciones: '' },
    { estadoDerivacion: 'Completado', dniCliente: '99887766', nombreCliente: 'María López', telefono: '977777777', dniAsesor: '66778899', oferta: 'Préstamo Hipotecario', agencia: 'Agencia Este', fechaVisita: '2024-08-12', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-13', acciones: '' },
    { estadoDerivacion: 'Rechazado', dniCliente: '55443322', nombreCliente: 'José Torres', telefono: '922222222', dniAsesor: '22334455', oferta: 'Línea de Crédito', agencia: 'Sucursal Oeste', fechaVisita: '2024-08-11', estadoEvidencia: 'No enviado', fechaEvidencia: '2024-08-12', acciones: '' },
    { estadoDerivacion: 'En Proceso', dniCliente: '66554433', nombreCliente: 'Luisa Castro', telefono: '944444444', dniAsesor: '33445566', oferta: 'Crédito Vehicular', agencia: 'Agencia Norte', fechaVisita: '2024-08-17', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-17', acciones: '' }
];

// Lógica para el filtro externo.
const externalFilterState = {
    dniCliente: '',
    agencia: 'Todos',
    asesor: 'Todos',
    fechaVisita: ''
};

function isExternalFilterPresent() {
    return Object.values(externalFilterState).some(value => value !== '' && value !== 'Todos');
}

function doesExternalFilterPass(node) {
    const { data } = node;
    const { dniCliente, agencia, asesor, fechaVisita } = externalFilterState;

    if (dniCliente && !String(data.dniCliente).includes(dniCliente)) return false;
    if (agencia !== 'Todos' && data.agencia !== agencia) return false;
    if (asesor !== 'Todos' && data.dniAsesor !== asesor) return false;
    if (fechaVisita && data.fechaVisita !== fechaVisita) return false;

    return true;
}

// Configuración principal de la grilla de Ag-grid.
const derivacionesGridOptions = {
    columnDefs: derivacionesTableColumns,
    rowData: listaDerivaciones,
    pagination: true,
    paginationPageSize: 10,
    enableBrowserTooltips: true,
    defaultColDef: {
        sortable: true,
        resizable: true,
        flex: 1,
        minWidth: 150
    },
    initialState: {
        sort: { sortModel: [{ colId: 'estadoEvidencia', sort: 'asc' }] }
    },
    isExternalFilterPresent: isExternalFilterPresent,
    doesExternalFilterPass: doesExternalFilterPass,

    onGridReady: (params) => {
        gridApi = params.api;
    },

    // AÑADIDO: Ajusta las columnas al 100% del contenedor cada vez que este cambie de tamaño.
    // Esto provoca el desbordamiento (scroll en X) solo cuando es necesario.
    onGridSizeChanged: (params) => {
        params.api.sizeColumnsToFit();
    }
};

function getAllDerivaciones(params) {
    // Lógica para obtener todas las derivaciones
    const url = window.location.href;
    const final_url = url + 'Operaciones/GetAllDerivaciones';

    fetch(final_url)
        .then(response => response.json())
        .then(data => {
            console.log(data);
        })
        .catch(error => {
            console.error('Error al obtener las derivaciones:', error);
        });
}

// Se ejecuta cuando el DOM está completamente cargado.
document.addEventListener('DOMContentLoaded', () => {
    const gridDiv = document.querySelector('#gridDerivaciones');
    agGrid.createGrid(gridDiv, derivacionesGridOptions);



    function onFilterChanged() {
        externalFilterState.dniCliente = document.getElementById('dniClienteDerivaciones').value;
        externalFilterState.agencia = document.getElementById('agenciaDerivaciones').value;
        externalFilterState.asesor = document.getElementById('asesorDerivaciones').value;
        externalFilterState.fechaVisita = document.getElementById('fechaVisitaDerivacion').value;

        if (gridApi) {
            gridApi.onFilterChanged();
        }
    }

    const agenciaSelect = document.getElementById('agenciaDerivaciones');
    const asesorSelect = document.getElementById('asesorDerivaciones');

    // Carga dinámica de opciones para los filtros <select>.
    const uniqueAgencies = [...new Set(listaDerivaciones.map(item => item.agencia))];
    const uniqueAdvisors = [...new Set(listaDerivaciones.map(item => item.dniAsesor))];

    uniqueAgencies.forEach(agencia => agenciaSelect.appendChild(new Option(agencia, agencia)));
    uniqueAdvisors.forEach(asesor => asesorSelect.appendChild(new Option(asesor, asesor)));

    // Asignación de eventos a los campos de filtro.
    document.getElementById('dniClienteDerivaciones').addEventListener('input', onFilterChanged);
    agenciaSelect.addEventListener('change', onFilterChanged);
    asesorSelect.addEventListener('change', onFilterChanged);
    document.getElementById('fechaVisitaDerivacion').addEventListener('change', onFilterChanged);
});