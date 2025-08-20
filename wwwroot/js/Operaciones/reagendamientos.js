
var App = App || {};

App.reagendamientos = (() => {

    let gridApi;

    const listaReagendamientos = [
        {
            historico: '', estadoReagendamiento: 'Enviado', numReagendamiento: 'R-001', dniCliente: '12345678', nombreCliente: 'Juan Pérez', telefono: '987654321', dniAsesor: '87654321', oferta: 'Crédito Personal', agencia: 'Agencia Central', fechaVisita: '2025-08-15', fechaReagendamiento: '2025-08-20', supervisor: 'Supervisor A', history: [
                { estadoReagendamiento: 'Pendiente', numReagendamiento: 'R-001-1', dniCliente: '12345678', nombreCliente: 'Juan Pérez', telefono: '987654321', dniAsesor: '87654321', oferta: 'Crédito Personal', agencia: 'Agencia Central', fechaVisita: '2025-08-10', fechaReagendamiento: '2025-08-15', supervisor: 'Supervisor A' },
                { estadoReagendamiento: 'Enviado', numReagendamiento: 'R-001-2', dniCliente: '12345678', nombreCliente: 'Juan Pérez', telefono: '987654321', dniAsesor: '87654321', oferta: 'Crédito Personal', agencia: 'Agencia Central', fechaVisita: '2025-08-12', fechaReagendamiento: '2025-08-18', supervisor: 'Supervisor A' }
            ]
        },
        {
            historico: '', estadoReagendamiento: 'Pendiente', numReagendamiento: 'R-002', dniCliente: '87654321', nombreCliente: 'Ana Gómez', telefono: '912345678', dniAsesor: '12345678', oferta: 'Tarjeta de Crédito', agencia: 'Sucursal Norte', fechaVisita: '2025-08-14', fechaReagendamiento: '2025-08-22', supervisor: 'Supervisor B', history: [
                { estadoReagendamiento: 'Enviado', numReagendamiento: 'R-002-1', dniCliente: '87654321', nombreCliente: 'Ana Gómez', telefono: '912345678', dniAsesor: '12345678', oferta: 'Tarjeta de Crédito', agencia: 'Sucursal Norte', fechaVisita: '2025-08-09', fechaReagendamiento: '2025-08-14', supervisor: 'Supervisor B' }
            ]
        },
        {
            historico: '', estadoReagendamiento: 'Enviado', numReagendamiento: 'R-003', dniCliente: '11223344', nombreCliente: 'Carlos Ruiz', telefono: '955555555', dniAsesor: '44332211', oferta: 'Seguro de Vida', agencia: 'Agencia Sur', fechaVisita: '2025-08-16', fechaReagendamiento: '2025-08-21', supervisor: 'Supervisor A', history: [
                { estadoReagendamiento: 'Pendiente', numReagendamiento: 'R-003-1', dniCliente: '11223344', nombreCliente: 'Carlos Ruiz', telefono: '955555555', dniAsesor: '44332211', oferta: 'Seguro de Vida', agencia: 'Agencia Sur', fechaVisita: '2025-08-11', fechaReagendamiento: '2025-08-16', supervisor: 'Supervisor A' },
                { estadoReagendamiento: 'Enviado', numReagendamiento: 'R-003-2', dniCliente: '11223344', nombreCliente: 'Carlos Ruiz', telefono: '955555555', dniAsesor: '44332211', oferta: 'Seguro de Vida', agencia: 'Agencia Sur', fechaVisita: '2025-08-13', fechaReagendamiento: '2025-08-19', supervisor: 'Supervisor A' }
            ]
        },
        {
            historico: '', estadoReagendamiento: 'Pendiente', numReagendamiento: 'R-004', dniCliente: '55443322', nombreCliente: 'José Torres', telefono: '922222222', dniAsesor: '22334455', oferta: 'Línea de Crédito', agencia: 'Sucursal Oeste', fechaVisita: '2025-08-11', fechaReagendamiento: '2025-08-19', supervisor: 'Supervisor B', history: [
                { estadoReagendamiento: 'Enviado', numReagendamiento: 'R-004-1', dniCliente: '55443322', nombreCliente: 'José Torres', telefono: '922222222', dniAsesor: '22334455', oferta: 'Línea de Crédito', agencia: 'Sucursal Oeste', fechaVisita: '2025-08-06', fechaReagendamiento: '2025-08-11', supervisor: 'Supervisor B' }
            ]
        },
        {
            historico: '', estadoReagendamiento: 'Enviado', numReagendamiento: 'R-005', dniCliente: '66554433', nombreCliente: 'Luisa Castro', telefono: '944444444', dniAsesor: '33445566', oferta: 'Crédito Vehicular', agencia: 'Agencia Norte', fechaVisita: '2025-08-17', fechaReagendamiento: '2025-08-25', supervisor: 'Supervisor A', history: [
                { estadoReagendamiento: 'Pendiente', numReagendamiento: 'R-005-1', dniCliente: '66554433', nombreCliente: 'Luisa Castro', telefono: '944444444', dniAsesor: '33445566', oferta: 'Crédito Vehicular', agencia: 'Agencia Norte', fechaVisita: '2025-08-12', fechaReagendamiento: '2025-08-17', supervisor: 'Supervisor A' },
                { estadoReagendamiento: 'Enviado', numReagendamiento: 'R-005-2', dniCliente: '66554433', nombreCliente: 'Luisa Castro', telefono: '944444444', dniAsesor: '33445566', oferta: 'Crédito Vehicular', agencia: 'Agencia Norte', fechaVisita: '2025-08-14', fechaReagendamiento: '2025-08-20', supervisor: 'Supervisor A' }
            ]
        }
    ];

    // Definición de las columnas para la tabla de Reagendamientos.
    const reagendamientosTableColumns = [
        {
            headerName: "Histórico", field: "historico",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const btn = document.createElement('button');
                btn.className = 'btn btn-sm btn-primary';
                btn.title = 'Ver Histórico';
                btn.innerHTML = '<i class="ri-history-line"></i>';
                btn.addEventListener('click', () => {
                    const modalElement = document.getElementById('modalHistoricoReagendamiento');
                    if (modalElement) {
                        const modal = new bootstrap.Modal(modalElement);
                        modal.show();
                        // Populate historical grid
                        const histGridDiv = document.getElementById('gridHistóricoReagendamientos');
                        if (histGridDiv) {
                            // Clear previous content
                            while (histGridDiv.firstChild) {
                                histGridDiv.removeChild(histGridDiv.firstChild);
                            }
                            const histColumns = reagendamientosTableColumns.slice(1);
                            const histGridOptions = {
                                columnDefs: histColumns,
                                rowData: params.data.history || [],
                                pagination: true,
                                paginationPageSize: 10,
                                defaultColDef: { sortable: true, resizable: true, minWidth: 150 },
                                onGridReady: (p) => {
                                    p.api.sizeColumnsToFit({ defaultMinWidth: 150 });
                                },
                                onGridSizeChanged: (p) => {
                                    p.api.sizeColumnsToFit({ defaultMinWidth: 150 });
                                },
                            };
                            agGrid.createGrid(histGridDiv, histGridOptions);
                        }
                        console.log('Abrir modal histórico para:', params.data);
                    } else {
                        console.error('Modal con ID #modalHistoricoReagendamiento no encontrado.');
                    }
                });
                return btn;
            },
            sortable: false, resizable: false, width: 100
        },
        {
            headerName: "Estado reagendamiento", field: "estadoReagendamiento",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                if (params.value === "Enviado") {
                    return `<span class="badge-estado badge-reag-enviado">Enviado</span>`; // Requiere una clase CSS verde
                }
                return `<span class="badge-estado badge-reag-pendiente">Pendiente</span>`; // Requiere una clase CSS amarilla
            }
        },
        { headerName: "N° reagendamiento", field: "numReagendamiento" },
        { headerName: "DNI cliente", field: "dniCliente" },
        { headerName: "Nombre cliente", field: "nombreCliente" },
        { headerName: "Teléfono", field: "telefono" },
        { headerName: "DNI asesor", field: "dniAsesor" },
        { headerName: "Oferta", field: "oferta" },
        { headerName: "Agencia", field: "agencia" },
        { headerName: "Fecha visita", field: "fechaVisita" },
        { headerName: "Fecha reagendamiento", field: "fechaReagendamiento" }
    ];

    // Lógica para los filtros externos.
    const externalFilterState = {
        dniCliente: '', supervisor: 'Todos', asesor: 'Todos', agencia: 'Todos', fechaReagendamiento: '', fechaVisita: ''
    };

    function isExternalFilterPresent() {
        return Object.values(externalFilterState).some(value => value !== '' && value !== 'Todos');
    }

    function doesExternalFilterPass(node) {
        const { data } = node;
        const { dniCliente, supervisor, asesor, agencia, fechaReagendamiento, fechaVisita } = externalFilterState;

        if (dniCliente && !String(data.dniCliente).includes(dniCliente)) return false;
        if (supervisor !== 'Todos' && data.supervisor !== supervisor) return false;
        if (asesor !== 'Todos' && data.dniAsesor !== asesor) return false;
        if (agencia !== 'Todos' && data.agencia !== agencia) return false;
        if (fechaReagendamiento && data.fechaReagendamiento !== fechaReagendamiento) return false;
        if (fechaVisita && data.fechaVisita !== fechaVisita) return false;

        return true;
    }

    const reagendamientosGridOptions = {
        columnDefs: reagendamientosTableColumns,
        rowData: listaReagendamientos,
        pagination: true,
        paginationPageSize: 10,
        defaultColDef: { sortable: true, resizable: true, minWidth: 150 },
        onGridReady: (params) => {
            gridApi = params.api;
            params.api.sizeColumnsToFit({ defaultMinWidth: 150 });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 150 });
        },
        isExternalFilterPresent: isExternalFilterPresent,
        doesExternalFilterPass: doesExternalFilterPass,
    };

    function populateFilters() {
        const supervisorSelect = document.getElementById('supervisorReagendamientos');
        const asesorSelect = document.getElementById('asesorReagendamientos');
        const agenciaSelect = document.getElementById('agenciaReagendamientos');

        const uniqueSupervisors = [...new Set(listaReagendamientos.map(item => item.supervisor))];
        const uniqueAdvisors = [...new Set(listaReagendamientos.map(item => item.dniAsesor))];
        const uniqueAgencies = [...new Set(listaReagendamientos.map(item => item.agencia))];

        uniqueSupervisors.forEach(supervisor => supervisorSelect.appendChild(new Option(supervisor, supervisor)));
        uniqueAdvisors.forEach(asesor => asesorSelect.appendChild(new Option(asesor, asesor)));
        uniqueAgencies.forEach(agencia => agenciaSelect.appendChild(new Option(agencia, agencia)));
    }

    function setupEventListeners() {
        const onFilterChanged = () => {
            externalFilterState.dniCliente = document.getElementById('dniClienteReagendamientos').value;
            externalFilterState.supervisor = document.getElementById('supervisorReagendamientos').value;
            externalFilterState.asesor = document.getElementById('asesorReagendamientos').value;
            externalFilterState.agencia = document.getElementById('agenciaReagendamientos').value;
            externalFilterState.fechaReagendamiento = document.getElementById('fechaReagendamientos').value;
            externalFilterState.fechaVisita = document.getElementById('fechaVisitaReagendamientos').value;

            if (gridApi) {
                gridApi.onFilterChanged();
            }
        };

        const filterInputs = [
            'dniClienteReagendamientos', 'supervisorReagendamientos', 'asesorReagendamientos',
            'agenciaReagendamientos', 'fechaReagendamientos', 'fechaVisitaReagendamientos'
        ];

        filterInputs.forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                const eventType = element.type === 'text' ? 'input' : 'change';
                element.addEventListener(eventType, onFilterChanged);
            }
        });
    }

    // --- INTERFAZ PÚBLICA DEL MÓDULO ---
    return {
        init: () => {
            // 3. Renderizado de la tabla en el div especificado.
            const gridDiv = document.querySelector('#gridReagendamientos');
            if (gridDiv) {
                agGrid.createGrid(gridDiv, reagendamientosGridOptions);
                populateFilters();
                setupEventListeners();
                console.log("Módulo de Reagendamientos inicializado.");
            }
        }
    };
})();