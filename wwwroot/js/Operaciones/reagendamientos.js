async function getAllReagendamientos() {
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
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';
                // ... (el contenido de esta función no cambia)
                let btnHistorico = document.createElement('button');
                btnHistorico.className = 'btn btn-sm btn-primary';
                btnHistorico.title = 'Ver Histórico';
                btnHistorico.innerHTML = '<i class="ri-history-line"></i>';
                btnHistorico.addEventListener('click', async () => {
                    const modalElement = document.getElementById('modalHistoricoReagendamiento');
                    if (modalElement) {
                        const modal = new bootstrap.Modal(modalElement);
                        modal.show();
                        const histGridDiv = document.getElementById('gridHistóricoReagendamientos');
                        var historicoData = await getHistorico(params.data.idDerivacion);
                        console.log('Datos de histórico obtenidos:', historicoData); // --- IGNORE ---
                        App.historico.init(
                            historicoData,
                            usuariorol,
                            usuarioAsesores,
                            usuarioSupervisores
                        );
                    } else {
                        console.error('Modal con ID #modalHistoricoReagendamiento no encontrado.');
                    }
                });
                btnReagendamiento = document.createElement('button');

                if (params.data.puedeSerReagendado === true) {
                    btnReagendamiento.className = 'btn btn-sm btn-primary';
                    btnReagendamiento.title = 'Reagendamiento'; // Corregido
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>'; // Icono ajustado a la acción

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
                        } else {
                            console.error('El elemento del modal con ID "modalEvidenciasDerivaciones" no fue encontrado en el DOM.');
                        }
                    });
                } else {
                    btnReagendamiento.className = 'btn btn-sm btn-secondary disabled';
                    btnReagendamiento.title = 'No puede ser reagendado'; // Corregido
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>'; // Icono ajustado a la acción
                }

                container.appendChild(btnHistorico);
                container.appendChild(btnReagendamiento);
                return container;
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
            field: "fechaAgendamiento",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm')
        }
        // --- FIN DE CAMBIOS ---
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
        if (fechaReagendamiento) {
            const [year, month, day] = fechaReagendamiento.split('-');
            const fechaFormattedObj = new Date(Number(year), Number(month) - 1, Number(day));
            const fechaReagendamientoData = new Date(data.fechaAgendamiento);
            if (
                fechaFormattedObj.getFullYear() !== fechaReagendamientoData.getFullYear() ||
                fechaFormattedObj.getMonth() !== fechaReagendamientoData.getMonth() ||
                fechaFormattedObj.getDate() !== fechaReagendamientoData.getDate()
            ) {
                return false;
            }
        }
        if (fechaVisita) {
            const [year, month, day] = fechaVisita.split('-');
            const fechaFormattedObj = new Date(Number(year), Number(month) - 1, Number(day));
            const fechaVisitaData = new Date(data.fechaVisita);
            if (
                fechaFormattedObj.getFullYear() !== fechaVisitaData.getFullYear() ||
                fechaFormattedObj.getMonth() !== fechaVisitaData.getMonth() ||
                fechaFormattedObj.getDate() !== fechaVisitaData.getDate()
            ) {
                return false;
            }
        }
        return true;
    }

    const reagendamientosGridOptions = {
        columnDefs: reagendamientosTableColumns,
        rowData: listaReagendamientos,
        pagination: true,
        paginationPageSize: 20,
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

    function populateFilters(rol, asesores = [], supervisores = []) {
        if (rol === 1 || rol === 4) {
            const supervisorSelect = document.getElementById('supervisorReagendamientos');
            const asesorSelect = document.getElementById('asesorReagendamientos');
            const agenciaSelect = document.getElementById('agenciaReagendamientos');

            const uniqueSupervisors = [
                ...new Map(listaReagendamientos.map(item => [item.docSupervisor, {
                    dni: item.docSupervisor
                }])).values()
            ];

            let enrichmentSupervisors = uniqueSupervisors.map(supervisor => {
                const usuario = supervisores.find(u => u.dni === supervisor.dni);
                return {
                    dni: supervisor.dni,
                    nombre: usuario ? usuario.nombresCompletos : 'Desconocido'
                };
            });

            const uniqueAdvisors = asesores.map(a => ({
                dni: a.dni,
                nombre: a.nombresCompletos
            }));

            const uniqueAgencies = [...new Set(listaReagendamientos.map(item => item.agencia))];
            const enrichmentAgencies = uniqueAgencies.map(a => {
                const nameAgencia = a.split(' - ')[1];
                return [nameAgencia, a];
            });

            enrichmentSupervisors.forEach(supervisor => {
                supervisorSelect.appendChild(new Option(supervisor.nombre, supervisor.dni));
            });
            uniqueAdvisors.forEach(asesor => {
                asesorSelect.appendChild(new Option(asesor.nombre, asesor.dni));
            });
            enrichmentAgencies.forEach(([name, full]) => {
                agenciaSelect.appendChild(new Option(name, full));
            });
        } else if (rol === 2) {
            const supervisorSelect = document.getElementById('supervisorReagendamientos');
            const asesorSelect = document.getElementById('asesorReagendamientos');
            const agenciaSelect = document.getElementById('agenciaReagendamientos');

            const uniqueAdvisors = asesores.map(a => ({
                dni: a.dni,
                nombre: a.nombresCompletos
            }));

            const uniqueAgencies = [...new Set(listaReagendamientos.map(item => item.agencia))];
            const enrichmentAgencies = uniqueAgencies.map(a => {
                const nameAgencia = a.split(' - ')[1];
                return [nameAgencia, a];
            });

            uniqueAdvisors.forEach(asesor => {
                asesorSelect.appendChild(new Option(asesor.nombre, asesor.dni));
            });
            enrichmentAgencies.forEach(([name, full]) => {
                agenciaSelect.appendChild(new Option(name, full));
            });

            // Luego ocultamos el select de supervisores.
            document.getElementById('supervisorReagendamientosCol').classList.add('d-none');
        } else if (rol === 3) {
            const agenciaSelect = document.getElementById('agenciaReagendamientos');
            const uniqueAgencies = [...new Set(listaReagendamientos.map(item => item.agencia))];
            const enrichmentAgencies = uniqueAgencies.map(a => {
                const nameAgencia = a.split(' - ')[1];
                return [nameAgencia, a];
            });
            enrichmentAgencies.forEach(([name, full]) => {
                agenciaSelect.appendChild(new Option(name, full));
            });
            // Luego ocultamos los selects de asesores y supervisores.
            document.getElementById('asesorReagendamientosCol').classList.add('d-none');
            document.getElementById('supervisorReagendamientosCol').classList.add('d-none');
        }
    }

    function setupEventListeners(asesores, supervisores) {
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
            if (id === 'supervisorReagendamientos') {
                let element = document.getElementById(id);
                if (element) {
                    const eventType = element.type === 'text' ? 'input' : 'change';
                    element.addEventListener(
                        eventType, (e) => {
                            const supervisorDni = e.target.value;
                            if (supervisorDni === 'Todos') {
                                // Mostrar todos los asesores
                                const asesorSelect = document.getElementById('asesorReagendamientos');
                                asesorSelect.innerHTML = '';
                                asesorSelect.appendChild(new Option('Todos', 'Todos'));
                                const uniqueAdvisors = asesores.map(a => ({
                                    dni: a.dni,
                                    nombre: a.nombresCompletos
                                }));
                                uniqueAdvisors.forEach(asesor => {
                                    asesorSelect.appendChild(new Option(asesor.nombre, asesor.dni));
                                });
                                onFilterChanged();
                            } else {
                                const idSupervisor = supervisores.find(s => s.dni === supervisorDni).idUsuario;
                                const allAdvisors = asesores.filter(a => a.idusuariosup === idSupervisor);

                                const uniqueAdvisors = allAdvisors.map(u => ({
                                    dni: u.dni,
                                    nombre: u.nombresCompletos
                                }));
                                const selectAsesor = document.getElementById('asesorReagendamientos');
                                selectAsesor.innerHTML = '';
                                selectAsesor.appendChild(new Option('Todos', 'Todos'));
                                uniqueAdvisors.forEach(asesor => {
                                    selectAsesor.appendChild(new Option(asesor.nombre, asesor.dni));
                                });
                                onFilterChanged();
                            }
                        }
                    );
                }
            } else {
                element = document.getElementById(id);
                if (element) {
                    const eventType = element.type === 'text' ? 'input' : 'change';
                    element.addEventListener(eventType, onFilterChanged);
                }
            }
        });
    }

    async function updateTableData(rol) {
        if (rol === 1 || rol === 4 || rol === 2) {
            return;
        } else if (rol === 3) {
            for (let i = reagendamientosTableColumns.length - 1; i >= 0; i--) {
                if (reagendamientosTableColumns[i].field === 'dniAsesor') {
                    listaReagendamientos.splice(i, 1);
                }
            }
        }
    }

    // --- INTERFAZ PÚBLICA DEL MÓDULO ---
    return {
        init: async (reagendamientos, rol, asesores, supervisores) => {
            // 3. Renderizado de la tabla en el div especificado.
            const gridDiv = document.querySelector('#gridReagendamientos');
            if (gridDiv) {
                usuariorol = rol || 0;

                listaReagendamientos = reagendamientos || [];

                await updateTableData(usuariorol);

                reagendamientosGridOptions.rowData = listaReagendamientos;
                console.log('Datos de reagendamientos cargados:', listaReagendamientos); // --- IGNORE ---
                agGrid.createGrid(gridDiv, reagendamientosGridOptions);
                populateFilters(usuariorol, asesores, supervisores);
                setupEventListeners(asesores, supervisores);
            }
        }
    };
})();