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
    const reagendamientosTableColumns = [
        {
            headerName: "Histórico y Acciones",
            field: "historico",
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
            sortable: false, resizable: false, width: 120
        },
        {
            headerName: "Estado Reagendacion",
            field: "estadoReagendacion",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const container = document.createElement('div');
                container.className = 'd-inline-flex gap-2';
                // Badge para Derivacion_status (D)
                const badgeD = document.createElement('div');
                badgeD.className = `af-badge ${(params.data.fueProcesadoFormulario && params.data.fueEnviadoEmail) ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeD.title = (params.data.fueProcesadoFormulario && params.data.fueEnviadoEmail) ? 'Derivación' : 'Derivación';
                badgeD.innerHTML = `
                  <i class="${(params.data.fueProcesadoFormulario && params.data.fueEnviadoEmail) ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                  <span>D</span>
                `;

                // Badge para Correo_status (C)
                const badgeC = document.createElement('div');
                badgeC.className = `af-badge ${params.data.fueEnviadoEmail ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeC.title = params.data.fueEnviadoEmail ? 'Correo electrónico' : 'Correo electrónico';
                badgeC.innerHTML = `
                    <i class="${params.data.fueEnviadoEmail ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>C</span>
                `;

                // Badge para Correo_status (F)
                const badgeF = document.createElement('div');
                badgeF.className = `af-badge ${params.data.fueProcesadoFormulario ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeF.title = params.data.fueProcesadoFormulario ? 'Formulario' : 'Formulario';
                badgeF.innerHTML = `
                    <i class="${params.data.fueProcesadoFormulario ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>F</span>
                `;

                const badgeDesembolsado = document.createElement('div');
                badgeDesembolsado.className = `af-badge ${params.data.fueDesembolsado ? 'af-badge-bg-info' : 'af-badge-bg-secondary'}`;
                badgeDesembolsado.title = params.data.fueDesembolsado ? 'Desembolsado' : 'Sin desembolso';
                badgeDesembolsado.innerHTML = `
                    <i class="${params.data.fueDesembolsado ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>${params.data.fueDesembolsado ? '⭐' : 'SD'}</span>
                `;

                container.appendChild(badgeD);
                container.appendChild(badgeC);
                container.appendChild(badgeF);
                container.appendChild(badgeDesembolsado);

                return container;
            },
            //tooltipValueGetter: (params) => {
            //    const derivacionStatus = params.data.Derivacion_status !== undefined ? params.data.Derivacion_status : 0;
            //    const correoStatus = params.data.Correo_status !== undefined ? params.data.Correo_status : 0;
            //    const derivacionText = derivacionStatus === 1 ? 'Derivación completada' : 'Derivación pendiente';
            //    const correoText = correoStatus === 1 ? 'Correo enviado' : 'Correo no enviado';
            //    return `${derivacionText}. ${correoText}.`;
            //},
            width: 180
        },
        {
            headerName: "N° Reagendamiento",
            field: "numeroReagendamiento",
            width: 180
        },
        {
            headerName: "DNI cliente",
            field: "dniCliente",
            width: 120
        },
        { headerName: "Cliente", field: "nombreCliente", width: 250 },
        { headerName: "Teléfono", field: "telefono", width: 120 },
        { headerName: "DNI asesor", field: "dniAsesor", width: 120 },
        {
            headerName: "Oferta",
            field: "oferta",
            valueFormatter: params => {
                if (params.value === 0) return 'No aplica';
                return `S/. ${Number(params.value).toLocaleString('es-PE')}`;
            },
            width: 120
        },
        { headerName: "Agencia", field: "agencia", width: 120 },
        // --- INICIO DE CAMBIOS ---
        {
            headerName: "Fecha visita",
            field: "fechaVisita",
            width: 130,
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

        enableBrowserTooltips: true,
        copyHeadersToClipboard: true,
        suppressClipboardPaste: true,
        enableCellTextSelection: true,

        initialState: { sort: { sortModel: [{ colId: 'estadoReagendamiento', sort: 'asc' }] } },

        defaultColDef: { sortable: true, resizable: true, minWidth: 50, flex: 1 },
        onGridReady: (params) => {
            gridApi = params.api;
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
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
                const agencias = a.split(',').map(x => x.trim());
                return agencias;
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
                const agencias = a.split(',').map(x => x.trim());
                return agencias;
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
                const agencias = a.split(',').map(x => x.trim());
                return agencias;
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
                    reagendamientosTableColumns.splice(i, 1);
                }
            }
        }
    }

    return {
        init: async (reagendamientos, rol, asesores, supervisores) => {
            const gridDiv = document.querySelector('#gridReagendamientos');
            if (gridDiv) {
                usuariorol = rol || 0;

                listaReagendamientos = reagendamientos || [];

                await updateTableData(usuariorol);

                reagendamientosGridOptions.rowData = listaReagendamientos;
                agGrid.createGrid(gridDiv, reagendamientosGridOptions);
                populateFilters(usuariorol, asesores, supervisores);
                setupEventListeners(asesores, supervisores);
            }
        },
        limpiarFiltros: () => {
            document.getElementById('dniClienteReagendamientos').value = '';
            document.getElementById('supervisorReagendamientos').value = 'Todos';
            document.getElementById('asesorReagendamientos').value = 'Todos';
            document.getElementById('agenciaReagendamientos').value = 'Todos';
            document.getElementById('fechaReagendamientos').value = '';
            document.getElementById('fechaVisitaReagendamientos').value = '';
            externalFilterState.dniCliente = '';
            externalFilterState.supervisor = 'Todos';
            externalFilterState.asesor = 'Todos';
            externalFilterState.agencia = 'Todos';
            externalFilterState.fechaReagendamiento = '';
            externalFilterState.fechaVisita = '';
            if (gridApi) {
                gridApi.onFilterChanged();
            }
        },
        exportarExcelReagendamientos: () => {
            if (!gridApi) return;

            const rowData = [];
            gridApi.forEachNodeAfterFilterAndSort(node => rowData.push(node.data));

            // Diccionario para renombrar columnas
            const columnMap = {
                dniCliente: "DNI Cliente",
                nombreCliente: "Nombre Cliente",
                fechaDerivacionOriginal: "Fecha Derivación Original",
                fechaAgendamiento: "Fecha Reagendamiento",
                fechaVisita: "Fecha Visita",
                telefono: "Teléfono",
                agencia: "Agencia",
                fechaDerivacion: "Fecha Derivación",
                dniAsesor: "DNI Asesor",
                oferta: "Oferta",
                puedeSerReagendado: "Puede Ser Reagendado",
                nombreAsesor: "Nombre Asesor",
                estadoReagendamiento: "Estado Reagendamiento",
                docSupervisor: "Documento Supervisor",
                numeroReagendamiento: "Número Reagendamiento",
                totalReagendamientos: "Total Reagendamientos",
                fueEnviadoEmail: "Fue Enviado el Email",
                fueProcesadoFormulario: "Fue Procesado el Formulario",
                fueDesembolsado: "Fue Desembolsado",
                fechaDesembolso: "Fecha Desembolso",
                montoFinanciado: "Monto Financiado"
            };

            const idsToDelete = ["idDerivacion", "idAgendamientosRe", "idDesembolsos"];

            const formattedData = rowData
                .filter(row => !row.debe_ser_eliminado)
                .map(row => {
                    const newRow = {};

                    Object.keys(row).forEach(key => {
                        if (idsToDelete.includes(key)) {
                            return;
                        }

                        let value = row[key];
                        if (value === true) value = "SI";
                        if (value === false) value = "NO";

                        const newKey = columnMap[key] || key;

                        if (key.toLowerCase().includes("fecha") && value) {
                            value = formatDateTime(value, "dd/mm/yyyy hh:mm");
                        }

                        newRow[newKey] = value;
                    });

                    return newRow;
                });

            const worksheet = XLSX.utils.json_to_sheet(formattedData);

            const colWidths = Object.keys(formattedData[0]).map(key => ({
                wch: Math.max(
                    key.length,
                    ...formattedData.map(r => (r[key] ? r[key].toString().length : 0))
                )
            }));
            worksheet["!cols"] = colWidths;

            const workbook = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(workbook, worksheet, "Reagendamientos");
            XLSX.writeFile(workbook, "Reagendamientos.xlsx");
        }
    };
})();