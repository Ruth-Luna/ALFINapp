
var App = App || {};

App.derivaciones = (() => {

    let gridApi;

    let listaDerivaciones = [
        { estadoDerivacion: 'Pendiente', dniCliente: '12345678', nombreCliente: 'Juan Pérez', telefono: '987654321', dniAsesor: '87654321', oferta: 'Crédito Personal', agencia: 'Agencia Central', fechaVisita: '2024-08-15', estadoEvidencia: 'No enviado', fechaEvidencia: '2024-08-11', acciones: '' },
        { estadoDerivacion: 'Completado', dniCliente: '87654321', nombreCliente: 'Ana Gómez', telefono: '912345678', dniAsesor: '12345678', oferta: 'Tarjeta de Crédito', agencia: 'Sucursal Norte', fechaVisita: '2024-08-14', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-15', acciones: '' },
        { estadoDerivacion: 'En Proceso', dniCliente: '11223344', nombreCliente: 'Carlos Ruiz', telefono: '955555555', dniAsesor: '44332211', oferta: 'Seguro de Vida', agencia: 'Agencia Sur', fechaVisita: '2024-08-16', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-16', acciones: '' },
        { estadoDerivacion: 'Completado', dniCliente: '99887766', nombreCliente: 'María López', telefono: '977777777', dniAsesor: '66778899', oferta: 'Préstamo Hipotecario', agencia: 'Agencia Este', fechaVisita: '2024-08-12', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-13', acciones: '' },
        { estadoDerivacion: 'Rechazado', dniCliente: '55443322', nombreCliente: 'José Torres', telefono: '922222222', dniAsesor: '22334455', oferta: 'Línea de Crédito', agencia: 'Sucursal Oeste', fechaVisita: '2024-08-11', estadoEvidencia: 'No enviado', fechaEvidencia: '2024-08-12', acciones: '' },
        { estadoDerivacion: 'En Proceso', dniCliente: '66554433', nombreCliente: 'Luisa Castro', telefono: '944444444', dniAsesor: '33445566', oferta: 'Crédito Vehicular', agencia: 'Agencia Norte', fechaVisita: '2024-08-17', estadoEvidencia: 'Enviado', fechaEvidencia: '2024-08-17', acciones: '' }
    ];

    let derivacionesTableColumns = [
        { headerName: "Estado derivación", field: "estadoDerivacion" },
        { headerName: "DNI cliente", field: "dniCliente" },
        { headerName: "Nombre del cliente", field: "nombreCliente" },
        { headerName: "Teléfono", field: "telefonoCliente" },
        { headerName: "DNI asesor", field: "dniAsesor" },
        { headerName: "Oferta", field: "ofertaMax" },
        { headerName: "Agencia", field: "nombreAgencia" },
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
            cellRenderer: (params) => {
                if (params.value === "Enviado") {
                    return `<span class="badge-estado badge-enviado">Enviado</span>`;
                }
                return `<span class="badge-estado badge-no-enviado">No enviado</span>`;
            },
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
                // Contenedor principal para los botones.
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';

                // --- Botón 1: Reagendamiento ---
                const btnReagendamiento = document.createElement('button');
                btnReagendamiento.className = 'btn btn-sm btn-primary';
                btnReagendamiento.title = 'Reagendamiento'; // Corregido
                btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>'; // Icono ajustado a la acción

                // Event listener para abrir el modal de reagendamiento.
                btnReagendamiento.addEventListener('click', () => {
                    const modalElement = document.getElementById('modalEvidenciasDerivaciones');
                    if (modalElement) {
                        const modalTitle = modalElement.querySelector('#labelModalEvidenciasDerivaciones');
                        if (modalTitle) {
                            modalTitle.textContent = `Reagendamiento de cita para: ${params.data.nombreCliente}`;
                        }

                        const modalButton = modalElement.querySelector('#enviar-evidencia-o-reagendacion');
                        if (modalButton) {
                            modalButton.onclick = () => enviarReagendacion('fecha-reagendamiento-nueva', params.data.idDerivacion);
                        }
                        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

                        cargarReagendacion(params.data);
                        modal_id_derivacion_to_be_uploaded(params.data.idDerivacion);

                        modal.show();
                        console.log("Abriendo modal de reagendamiento para:", params.data);
                    } else {
                        console.error('El elemento del modal con ID "modalEvidenciasDerivaciones" no fue encontrado en el DOM.');
                    }
                });

                const btnEnviarEvidencia = document.createElement('button');
                btnEnviarEvidencia.className = 'btn btn-sm btn-info';
                btnEnviarEvidencia.title = 'Enviar evidencia'; // Corregido
                btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>'; // Icono ajustado a la acción

                btnEnviarEvidencia.addEventListener('click', () => {
                    const modalElement = document.getElementById('modalEvidenciasDerivaciones');
                    if (modalElement) {
                        const modalTitle = modalElement.querySelector('#labelModalEvidenciasDerivaciones');
                        if (modalTitle) {
                            modalTitle.textContent = `Evidencia de derivacion para: ${params.data.nombreCliente}`;
                        }
                        const modalButton = modalElement.querySelector('#enviar-evidencia-o-reagendacion');
                        if (modalButton) {
                            modalButton.onclick = () => submit_evidencia_derivacion(params.data.idDerivacion);
                        }
                        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

                        cargarEvidencia(params.data);
                        modal_id_derivacion_to_be_uploaded(params.data.idDerivacion);

                        modal.show();
                        console.log("Abriendo modal de evidencias para la fila:", params.data);
                    } else {
                        console.error('El elemento del modal con ID "modalEvidenciasDerivaciones" no fue encontrado en el DOM.');
                    }
                });

                container.appendChild(btnReagendamiento);
                container.appendChild(btnEnviarEvidencia);

                return container;
            },
            width: 130,
            sortable: false,
            resizable: false
        },
        { headerName: "DNI Supervisor", field: "docSupervisor", hide: true },
    ];

    const externalFilterState = { dniCliente: '', agencia: 'Todos', supervisor: 'Todos', asesor: 'Todos', fechaVisita: '' };

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
        const { dniCliente, agencia, asesor, fechaVisita, supervisor } = externalFilterState;
        if (dniCliente && !String(data.dniCliente).includes(dniCliente)) return false;
        if (agencia !== 'Todos' && data.nombreAgencia !== agencia) return false;
        if (asesor !== 'Todos' && data.dniAsesor !== asesor) return false;
        if (supervisor !== 'Todos' && data.docSupervisor !== supervisor) return false;
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

    async function getAllDerivaciones() {
        const url = window.location.origin;
        const final_url = url + '/Operaciones/GetAllDerivaciones';

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
            // console.log('Derivaciones obtenidas:', data);
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

    // Función que configura los event listeners para los filtros.
    function setupEventListeners(data) {
        const onFilterChanged = () => {
            externalFilterState.dniCliente = document.getElementById('dniClienteDerivaciones').value;
            externalFilterState.agencia = document.getElementById('agenciaDerivaciones').value;
            externalFilterState.asesor = document.getElementById('asesorDerivaciones').value;
            externalFilterState.supervisor = document.getElementById('supervisorDerivaciones').value;
            externalFilterState.fechaVisita = document.getElementById('fechaVisitaDerivacion').value;
            if (gridApi) {
                gridApi.onFilterChanged();
            }
        };

        document.getElementById('dniClienteDerivaciones').addEventListener('input', onFilterChanged);
        document.getElementById('agenciaDerivaciones').addEventListener('change', onFilterChanged);
        document.getElementById('asesorDerivaciones').addEventListener('change', onFilterChanged);
        document.getElementById('supervisorDerivaciones').addEventListener('change', (e) => {
            const selectedSupervisorId = parseInt(e.target.value, 10);

            const uniqueAdvisors = asesores.filter(a => a.idusuariosup === selectedSupervisorId);

            const selectAsesor = document.getElementById('asesorDerivaciones');
            uniqueAdvisors.forEach(asesor => selectAsesor.appendChild(new Option(asesor.nombresCompletos, asesor.idUsuario)));

            onFilterChanged();
        });
        document.getElementById('fechaVisitaDerivacion').addEventListener('change', onFilterChanged);
    }

    async function createTable(rol) {
        if (rol === 1 || rol === 4) {
            // Rol 1 o 4 -> no se elimina nada
            return;
        } else if (rol === 2) {
            // Rol 2 -> eliminar solo la columna de docSupervisor
            for (let i = derivacionesTableColumns.length - 1; i >= 0; i--) {
                if (derivacionesTableColumns[i].field === 'docSupervisor') {
                    derivacionesTableColumns.splice(i, 1); // borra esa entrada
                }
            }
        } else if (rol === 3) {
            // Rol 3 -> eliminar dniAsesor y docSupervisor
            for (let i = derivacionesTableColumns.length - 1; i >= 0; i--) {
                if (derivacionesTableColumns[i].field === 'dniAsesor' || derivacionesTableColumns[i].field === 'docSupervisor') {
                    derivacionesTableColumns.splice(i, 1);
                }
            }
        }
    }


    // Función que pobla los selects de los filtros.
    function populateFilters(rol) {
        if (rol === 1 || rol === 4) {
            const agenciaSelect = document.getElementById('agenciaDerivaciones');
            const asesorSelect = document.getElementById('asesorDerivaciones');
            const supervisorSelect = document.getElementById('supervisorDerivaciones');

            const uniqueAgencies = [...new Set(listaDerivaciones.map(item => item.nombreAgencia))];
            const uniqueAdvisors = [...new Set(listaDerivaciones.map(item => item.dniAsesor))];
            const uniqueSupervisors = [...new Set(listaDerivaciones.map(item => item.docSupervisor))];

            uniqueAgencies.forEach(agencia => agenciaSelect.appendChild(new Option(agencia, agencia)));
            uniqueAdvisors.forEach(asesor => asesorSelect.appendChild(new Option(asesor, asesor)));
            uniqueSupervisors.forEach(supervisor => supervisorSelect.appendChild(new Option(supervisor, supervisor)));
        } if (rol === 2) {
            const agenciaSelect = document.getElementById('agenciaDerivaciones');
            const asesorSelect = document.getElementById('asesorDerivaciones');

            const uniqueAgencies = [...new Set(listaDerivaciones.map(item => item.nombreAgencia))];
            const uniqueAdvisors = [...new Set(listaDerivaciones.map(item => item.dniAsesor))];

            uniqueAgencies.forEach(agencia => agenciaSelect.appendChild(new Option(agencia, agencia)));
            uniqueAdvisors.forEach(asesor => asesorSelect.appendChild(new Option(asesor, asesor)));
            // Luego ocultamos el select de supervisores.
            document.getElementById('supervisorDerivacionesCol').classList.add('d-none');
        } else if (rol === 3) {
            const agenciaSelect = document.getElementById('agenciaDerivaciones');
            const uniqueAgencies = [...new Set(listaDerivaciones.map(item => item.nombreAgencia))];
            uniqueAgencies.forEach(agencia => agenciaSelect.appendChild(new Option(agencia, agencia)));
            // Luego ocultamos los selects de asesores y supervisores.
            document.getElementById('asesorDerivacionesCol').classList.add('d-none');
            document.getElementById('supervisorDerivacionesCol').classList.add('d-none');
        } else {
            console.warn('Rol no reconocido. No se poblarán los filtros.');
        }
    }

    // --- INTERFAZ PÚBLICA DEL MÓDULO ---
    // Exponemos solo la función 'init' para que pueda ser llamada desde fuera.
    return {
        init: async () => {
            const gridDiv = document.querySelector('#gridDerivaciones');
            if (gridDiv) {
                data = await getAllDerivaciones();
                console.log('Datos obtenidos:', data);
                usuariorol = data.rolUsuario || 0;
                listaDerivaciones = data.derivaciones || [];
                await createTable(usuariorol);
                derivacionesGridOptions.rowData = listaDerivaciones;
                agGrid.createGrid(gridDiv, derivacionesGridOptions);
                populateFilters(usuariorol);
                setupEventListeners(data);
            }
        }
    };

})();