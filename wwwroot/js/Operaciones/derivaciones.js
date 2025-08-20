
var App = App || {};

App.derivaciones = (() => {

    let gridApi;

    const listaDerivaciones = [
        { estadoDerivacion: 'Pendiente', dniCliente: '12345678', nombreCliente: 'Juan Pérez', telefono: '987654321', dniAsesor: '87654321', oferta: 'Crédito Personal', agencia: 'Agencia Central', fechaDerivacion: '2024-08-14T10:00:00', fechaVisita: '2024-08-15', estadoEvidencia: 'No enviado', fechaEvidencia: '2024-08-11T11:30:00', acciones: '' },
        { estadoDerivacion: 'Completado', dniCliente: '87654321', nombreCliente: 'Ana Gómez', telefono: '912345678', dniAsesor: '12345678', oferta: 'Tarjeta de Crédito', agencia: 'Sucursal Norte', fechaDerivacion: '2024-08-13T11:20:15', fechaVisita: '2024-08-14', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-15T09:05:10', acciones: '' },
        { estadoDerivacion: 'En Proceso', dniCliente: '11223344', nombreCliente: 'Carlos Ruiz', telefono: '955555555', dniAsesor: '44332211', oferta: 'Seguro de Vida', agencia: 'Agencia Sur', fechaDerivacion: '2024-08-15T12:00:00', fechaVisita: '2024-08-16', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-16T18:00:00', acciones: '' },
        { estadoDerivacion: 'Completado', dniCliente: '99887766', nombreCliente: 'María López', telefono: '977777777', dniAsesor: '66778899', oferta: 'Préstamo Hipotecario', agencia: 'Agencia Este', fechaDerivacion: '2024-08-11T09:45:30', fechaVisita: '2024-08-12', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-13T14:22:05', acciones: '' },
        { estadoDerivacion: 'Rechazado', dniCliente: '55443322', nombreCliente: 'José Torres', telefono: '922222222', dniAsesor: '22334455', oferta: 'Línea de Crédito', agencia: 'Sucursal Oeste', fechaDerivacion: '2024-08-10T16:10:00', fechaVisita: '2024-08-11', estadoEvidencia: 'No enviado', fechaEvidencia: '2024-08-12T10:00:00', acciones: '' },
        { estadoDerivacion: 'En Proceso', dniCliente: '66554433', nombreCliente: 'Luisa Castro', telefono: '944444444', dniAsesor: '33445566', oferta: 'Crédito Vehicular', agencia: 'Agencia Norte', fechaDerivacion: '2024-08-16T08:30:00', fechaVisita: '2024-08-17', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-17T11:45:12', acciones: '' }
    ];

    const derivacionesTableColumns = [
        { headerName: "Estado derivación", field: "estadoDerivacion" },
        { headerName: "DNI cliente", field: "dniCliente" },
        { headerName: "Nombre del cliente", field: "nombreCliente" },
        { headerName: "Teléfono", field: "telefono" },
        { headerName: "DNI asesor", field: "dniAsesor" },
        { headerName: "Oferta", field: "oferta" },
        { headerName: "Agencia", field: "agencia" },
        // --- INICIO DE CAMBIOS ---
        {
            headerName: "Fecha derivación",
            field: "fechaDerivacion",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm:ss')
        },
        {
            headerName: "Fecha visita",
            field: "fechaVisita",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy')
        },
        {
            headerName: "Estado evidencia",
            field: "estadoEvidencia",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => params.value === "Enviado" ? `<span class="badge-estado badge-enviado">Enviado</span>` : `<span class="badge-estado badge-no-enviado">No enviado</span>`,
            comparator: (valueA, valueB) => (valueA === valueB ? 0 : valueA === 'No enviado' ? -1 : 1)
        },
        {
            headerName: "Fecha evidencia",
            field: "fechaEvidencia",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm:ss')
        },
        // --- FIN DE CAMBIOS ---
        {
            headerName: "Acciones",
            field: "acciones",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                // ... El resto del cellRenderer de acciones se mantiene igual
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';
                // --- Botón 1: Reagendamiento ---
                const btnReagendamiento = document.createElement('button');
                btnReagendamiento.className = 'btn btn-sm btn-primary';
                btnReagendamiento.title = 'Reagendamiento';
                btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';
                btnReagendamiento.addEventListener('click', () => {
                    const modalElement = document.getElementById('modalReagendamiento');
                    if (modalElement) {
                        const modalTitle = modalElement.querySelector('#labelModalReagendamiento');
                        if (modalTitle) {
                            modalTitle.textContent = `Reagendamiento de cita para: ${params.data.nombreCliente}`;
                        }
                        bootstrap.Modal.getOrCreateInstance(modalElement).show();
                    }
                });
                // --- Botón 2: Enviar evidencia ---
                const btnEnviarEvidencia = document.createElement('button');
                btnEnviarEvidencia.className = 'btn btn-sm btn-info';
                btnEnviarEvidencia.title = 'Enviar evidencia';
                btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>';
                btnEnviarEvidencia.addEventListener('click', () => {
                    const modalElement = document.getElementById('modalEvidenciasDerivaciones');
                    if (modalElement) {
                        bootstrap.Modal.getOrCreateInstance(modalElement).show();
                    }
                });
                container.appendChild(btnReagendamiento);
                container.appendChild(btnEnviarEvidencia);
                return container;
            },
            width: 130,
            sortable: false,
            resizable: false
        }
    ];

    const externalFilterState = { dniCliente: '', agencia: 'Todos', asesor: 'Todos', fechaVisita: '' };

    // --- FUNCIONES PRIVADAS DEL MÓDULO ---

    function formatDateTime(dateString, format = 'dd/mm/yyyy') {
        if (!dateString) return ''; // Si no hay fecha, retorna vacío
        const date = new Date(dateString);
        // Verificamos si la fecha es válida para evitar errores
        if (isNaN(date.getTime())) return dateString;

        const pad = (num) => String(num).padStart(2, '0');
        const day = pad(date.getDate());
        const month = pad(date.getMonth() + 1); // Los meses en JS empiezan en 0
        const year = date.getFullYear();
        const hours = pad(date.getHours());
        const minutes = pad(date.getMinutes());
        const seconds = pad(date.getSeconds());

        if (format === 'dd/mm/yyyy hh:mm:ss') {
            return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;
        }
        // Por defecto, o si el formato es 'dd/mm/yyyy'
        return `${day}/${month}/${year}`;
    }

    function isExternalFilterPresent() { return Object.values(externalFilterState).some(value => value !== '' && value !== 'Todos'); }

    function doesExternalFilterPass(node) {
        const { data } = node;
        const { dniCliente, agencia, asesor, fechaVisita } = externalFilterState;
        if (dniCliente && !String(data.dniCliente).includes(dniCliente)) return false;
        if (agencia !== 'Todos' && data.agencia !== agencia) return false;
        if (asesor !== 'Todos' && data.dniAsesor !== asesor) return false;
        if (fechaVisita && data.fechaVisita !== fechaVisita) return false;
        return true;
    }

    function actualizarContadores() {
        if (!gridApi) return;
        const resultadoContenedor = document.getElementById('resultadoDerivacionesContenedor');
        const resultadoSpan = document.getElementById('resultadoDerivaciones');
        if (!resultadoContenedor || !resultadoSpan) return;
        if (isExternalFilterPresent()) {
            resultadoSpan.textContent = gridApi.getDisplayedRowCount();
            resultadoContenedor.classList.remove('d-none');
        } else {
            resultadoContenedor.classList.add('d-none');
        }
    }

    const derivacionesGridOptions = {
        columnDefs: derivacionesTableColumns,
        rowData: listaDerivaciones,
        pagination: true,
        paginationPageSize: 10,
        enableBrowserTooltips: true,
        defaultColDef: { sortable: true, resizable: true, minWidth: 150 },
        initialState: { sort: { sortModel: [{ colId: 'estadoEvidencia', sort: 'asc' }] } },
        isExternalFilterPresent: isExternalFilterPresent,
        doesExternalFilterPass: doesExternalFilterPass,
        onGridReady: (params) => {
            gridApi = params.api;
            document.getElementById('totalDelMesDerivaciones').textContent = listaDerivaciones.length;
            params.api.sizeColumnsToFit({ defaultMinWidth: 150 });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 150 });
        },
        onFilterChanged: actualizarContadores
    };

    // Función que configura los event listeners para los filtros.
    function setupEventListeners() {
        const onFilterChanged = () => {
            externalFilterState.dniCliente = document.getElementById('dniClienteDerivaciones').value;
            externalFilterState.agencia = document.getElementById('agenciaDerivaciones').value;
            externalFilterState.asesor = document.getElementById('asesorDerivaciones').value;
            externalFilterState.fechaVisita = document.getElementById('fechaVisitaDerivacion').value;
            if (gridApi) {
                gridApi.onFilterChanged();
            }
        };

        document.getElementById('dniClienteDerivaciones').addEventListener('input', onFilterChanged);
        document.getElementById('agenciaDerivaciones').addEventListener('change', onFilterChanged);
        document.getElementById('asesorDerivaciones').addEventListener('change', onFilterChanged);
        document.getElementById('fechaVisitaDerivacion').addEventListener('change', onFilterChanged);
    }

    // Función que pobla los selects de los filtros.
    function populateFilters() {
        const agenciaSelect = document.getElementById('agenciaDerivaciones');
        const asesorSelect = document.getElementById('asesorDerivaciones');

        const uniqueAgencies = [...new Set(listaDerivaciones.map(item => item.agencia))];
        const uniqueAdvisors = [...new Set(listaDerivaciones.map(item => item.dniAsesor))];

        uniqueAgencies.forEach(agencia => agenciaSelect.appendChild(new Option(agencia, agencia)));
        uniqueAdvisors.forEach(asesor => asesorSelect.appendChild(new Option(asesor, asesor)));
    }

    // --- INTERFAZ PÚBLICA DEL MÓDULO ---
    // Exponemos solo la función 'init' para que pueda ser llamada desde fuera.
    return {
        init: () => {
            const gridDiv = document.querySelector('#gridDerivaciones');
            if (gridDiv) {
                agGrid.createGrid(gridDiv, derivacionesGridOptions);
                populateFilters();
                setupEventListeners();
            }
        }
    };

})();