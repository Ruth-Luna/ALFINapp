
var App = App || {};

App.reagendamientos = (() => {

    let gridApi;

    let listaReagendamientos = [];

    // Definición de las columnas para la tabla de Reagendamientos.
    // Definición de las columnas para la tabla de Reagendamientos.
    const reagendamientosTableColumns = [
        {
            headerName: "Histórico", field: "historico",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                // ... (el contenido de esta función no cambia)
                const btn = document.createElement('button');
                btn.className = 'btn btn-sm btn-primary';
                btn.title = 'Ver Histórico';
                btn.innerHTML = '<i class="ri-history-line"></i>';
                btn.addEventListener('click', () => {
                    const modalElement = document.getElementById('modalHistoricoReagendamiento');
                    if (modalElement) {
                        const modal = new bootstrap.Modal(modalElement);
                        modal.show();
                        const histGridDiv = document.getElementById('gridHistóricoReagendamientos');
                        if (histGridDiv) {
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
                    return `<span class="badge-estado badge-reag-enviado">Enviado</span>`;
                }
                return `<span class="badge-estado badge-reag-pendiente">Pendiente</span>`;
            }
        },
        { headerName: "N° reagendamiento", field: "numeroReagendamiento" },
        { headerName: "DNI cliente", field: "dniCliente" },
        { headerName: "Nombre cliente", field: "nombreCliente" },
        { headerName: "Teléfono", field: "telefono" },
        { headerName: "DNI asesor", field: "dniAsesor" },
        { headerName: "Oferta", field: "oferta" },
        { headerName: "Agencia", field: "agencia" },
        // --- INICIO DE CAMBIOS ---
        {
            headerName: "Fecha visita",
            field: "fechaVisita",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy')
        },
        {
            headerName: "Fecha reagendamiento",
            field: "fechaReagendamiento",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm:ss')
        }
        // --- FIN DE CAMBIOS ---
    ];
    async function fetchReagendamientos() {
        const url = window.location.origin;
        const final_url = url + '/Operaciones/GetAllReagendamientos';

        try {
            const response = await fetch(final_url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            if (!response.ok) {
                throw new Error(data.message || 'Error al obtener las derivaciones');
            }
            const data = await response.json();
            if (data.success === true) {
                return data.data || [];
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Atención',
                    text: data.message || 'No se encontraron derivaciones.'
                });
                return [];
            }
        } catch (error) {
            console.error('Error al obtener las derivaciones:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudieron cargar las derivaciones. Por favor, inténtelo de nuevo más tarde.'
            });
            return;
        }

    }
    // Lógica para los filtros externos.
    const externalFilterState = {
        dniCliente: '', supervisor: 'Todos', asesor: 'Todos', agencia: 'Todos', fechaReagendamiento: '', fechaVisita: ''
    };

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
        init: async () => {
            // 3. Renderizado de la tabla en el div especificado.
            const gridDiv = document.querySelector('#gridReagendamientos');
            if (gridDiv) {
                const data = await fetchReagendamientos();
                listaReagendamientos = data.reagendamientos || [];
                reagendamientosGridOptions.rowData = listaReagendamientos;
                agGrid.createGrid(gridDiv, reagendamientosGridOptions);
                populateFilters();
                setupEventListeners();
            }
        }
    };
})();