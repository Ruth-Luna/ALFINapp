function formatDateTime(dateString, format = 'dd/mm/yyyy') {
    if (!dateString) return '';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return dateString;

    const pad = (num) => String(num).padStart(2, '0');
    const day = pad(date.getDate());
    const month = pad(date.getMonth() + 1);
    const year = date.getFullYear();
    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());

    if (format === 'dd/mm/yyyy hh:mm:ss') {
        return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;
    } else if (format === 'yyyy-mm-dd') {
        return `${year}-${month}-${day}`;
    } else if (format === 'yyyy-mm-dd hh:mm') {
        return `${year}-${month}-${day} ${hours}:${minutes}`;
    } else if (format === 'dd/mm/yyyy hh:mm') {
        return `${day}/${month}/${year} ${hours}:${minutes}`;
    }
    return `${day}/${month}/${year}`;
}

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

App.derivaciones = (() => {
    console.log(App)
    let gridApi;

    let listaDerivaciones = [];

    let derivacionesTableColumns = [
        {
            headerName: "Estado de Derivacion",
            field: "estadoDerivacion",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const container = document.createElement('div');
                container.className = 'd-inline-flex gap-2';
                console.log(params.data)
                // Badge para Derivacion_status (D)
                const badgeD = document.createElement('div');
                badgeD.className = `af-badge ${params.data.fueProcesado ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeD.innerHTML = `
                    <i class="${params.data.fueProcesado ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>D</span>
                `;

                // Badge para Correo_status (C)
                const badgeC = document.createElement('div');
                badgeC.className = `af-badge ${params.data.fueEnviadoEmail ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeC.innerHTML = `
                    <i class="${params.data.fueEnviadoEmail ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>C</span>
                `;

                // Badge para Correo_status (F)
                const badgeF = document.createElement('div');
                badgeF.className = `af-badge ${params.data.fueProcesado ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeF.innerHTML = `
                    <i class="${params.data.fueProcesado ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>F</span>
                `;


                container.appendChild(badgeD);
                container.appendChild(badgeC);
                container.appendChild(badgeF);

                return container;
            },
            tooltipValueGetter: (params) => {
                const derivacionStatus = params.data.Derivacion_status !== undefined ? params.data.Derivacion_status : 0;
                const correoStatus = params.data.Correo_status !== undefined ? params.data.Correo_status : 0;
                const derivacionText = derivacionStatus === 1 ? 'Derivación completada' : 'Derivación pendiente';
                const correoText = correoStatus === 1 ? 'Correo enviado' : 'Correo no enviado';
                return `${derivacionText}. ${correoText}.`;
            },
            width: 150
        },
        { headerName: "DNI Cliente", field: "dniCliente", width: 110 },
        { headerName: "Cliente", field: "nombreCliente", width: 150 },
        { headerName: "Teléfono", field: "telefonoCliente", width: 100 },
        { headerName: "DNI asesor", field: "dniAsesor", width: 100 },
        {
            headerName: "Oferta",
            field: "ofertaMax",
            valueFormatter: params => {
                if (params.value === 0) return 'No aplica';
                return `S/. ${Number(params.value).toLocaleString('es-PE')}`;
            },
            width: 100
        },
        { headerName: "Agencia", field: "nombreAgencia" },
        {
            headerName: "Fecha derivación",
            field: "fechaDerivacion",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm'),
            width: 120
        },
        {
            headerName: "Fecha visita",
            field: "fechaVisita",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy'),
            width: 100
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
            comparator: (valueA, valueB) => (valueA === valueB ? 0 : valueA === 'No enviado' ? -1 : 1),
            width: 100
        },

        {
            headerName: "Fecha evidencia",
            field: "fechaEvidencia",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm'),
            width: 120
        },
        {
            headerName: "Acciones",
            field: "acciones",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                // ... El resto del cellRenderer de acciones se mantiene igual
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';

                let btnReagendamiento = document.createElement('button');
                let btnEnviarEvidencia = document.createElement('button');
                if (!params.data.puedeSerReagendado) {
                    btnReagendamiento.className = 'btn btn-sm btn-secondary disabled';
                    btnReagendamiento.title = 'Reagendamiento';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';

                    btnEnviarEvidencia.className = 'btn btn-sm btn-info';
                    btnEnviarEvidencia.title = 'Enviar evidencia';
                    btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>';

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
                        } else {
                            console.error('El elemento del modal con ID "modalEvidenciasDerivaciones" no fue encontrado en el DOM.');
                        }
                    });
                } else {
                    btnReagendamiento.className = 'btn btn-sm btn-primary';
                    btnReagendamiento.title = 'Reagendamiento';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';

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

                    btnEnviarEvidencia.className = 'btn btn-sm btn-secondary disabled';
                    btnEnviarEvidencia.title = 'Enviar evidencia';
                    btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>';
                }

                container.appendChild(btnReagendamiento);
                container.appendChild(btnEnviarEvidencia);

                return container;
            },
            width: 130,
            sortable: false,
            resizable: false
        }
    ];

    const externalFilterState = {
        dniCliente: '',
        agencia: 'Todos',
        asesor: 'Todos',
        supervisor: 'Todos',
        fechaVisita: '',
        fechaDerivacion: '',
    };

    function isExternalFilterPresent() {
        return Object.values(externalFilterState).some(value => value !== '' && value !== 'Todos');
    }
    function isExternalFilterPresent() { return Object.values(externalFilterState).some(value => value !== '' && value !== 'Todos'); }
    function isExternalFilterPresent() { return Object.values(externalFilterState).some(value => value !== '' && value !== 'Todos'); }

    function doesExternalFilterPass(node) {
        const { data } = node;

        const { dniCliente, agencia, asesor, supervisor, fechaVisita, fechaDerivacion } = externalFilterState;
        if (dniCliente && !String(data.dniCliente).includes(dniCliente)) return false;
        if (agencia !== 'Todos' && data.nombreAgencia !== agencia) return false;
        if (asesor !== 'Todos' && data.dniAsesor !== asesor) return false;
        if (supervisor !== 'Todos' && data.docSupervisor !== supervisor) return false;
        if (fechaVisita) {
            const [year, month, day] = fechaVisita.split('-');
            const fechaFormattedObj = new Date(Number(year), Number(month) - 1, Number(day));
            const fechaVisitaData = new Date(data.fechaVisita);
            if (
                fechaFormattedObj.getFullYear() !== fechaVisitaData.getFullYear() ||
                fechaFormattedObj.getMonth() !== fechaVisitaData.getMonth() ||
                fechaFormattedObj.getDate() !== fechaVisitaData.getDate()
            ) {
                console.log(`No coincide la fecha de visita: ${fechaFormattedObj} !== ${fechaVisitaData}`);
                return false;
            }
        }
        if (fechaDerivacion) {
            const [year, month, day] = fechaDerivacion.split('-');
            const fechaDerivacionFormatted = new Date(Number(year), Number(month) - 1, Number(day));
            const fechaDerivacionData = new Date(data.fechaDerivacion);
            if (
                fechaDerivacionFormatted.getFullYear() !== fechaDerivacionData.getFullYear() ||
                fechaDerivacionFormatted.getMonth() !== fechaDerivacionData.getMonth() ||
                fechaDerivacionFormatted.getDate() !== fechaDerivacionData.getDate()
            ) {
                console.log(`No coincide la fecha de derivación: ${fechaDerivacionFormatted} !== ${fechaDerivacionData}`);
                return false;
            }
        }
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
        paginationPageSize: 20,
        enableBrowserTooltips: true,
        defaultColDef: { sortable: true, resizable: true, minWidth: 50, flex: 1 },
        copyHeadersToClipboard: true,
        suppressClipboardPaste: true,
        enableCellTextSelection: true,
        initialState: { sort: { sortModel: [{ colId: 'estadoEvidencia', sort: 'asc' }] } },
        isExternalFilterPresent: isExternalFilterPresent,
        doesExternalFilterPass: doesExternalFilterPass,
        onGridReady: (params) => {
            gridApi = params.api;
            document.getElementById('totalDelMesDerivaciones').textContent = listaDerivaciones.length;
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
            gridApi.getColumnDefs().forEach(col => {
                console.log(col.field, "hide:", col.hide);
            });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        },
        onFilterChanged: actualizarContadores,
    };

    function setupEventListeners(asesores, supervisores) {
        const onFilterChanged = () => {
            externalFilterState.dniCliente = document.getElementById('dniClienteDerivaciones').value;
            externalFilterState.agencia = document.getElementById('agenciaDerivaciones').value;
            externalFilterState.asesor = document.getElementById('asesorDerivaciones').value;
            externalFilterState.supervisor = document.getElementById('supervisorDerivaciones').value;
            externalFilterState.fechaVisita = document.getElementById('fechaVisitaDerivacion').value;
            externalFilterState.fechaDerivacion = document.getElementById('fechaDerivacion').value;
            if (gridApi) {
                gridApi.onFilterChanged();
            }
        };

        document.getElementById('dniClienteDerivaciones').addEventListener('input', onFilterChanged);
        document.getElementById('agenciaDerivaciones').addEventListener('change', onFilterChanged);
        document.getElementById('asesorDerivaciones').addEventListener('change', onFilterChanged);
        document.getElementById('supervisorDerivaciones').addEventListener('change', (e) => {
            const supervisorDni = e.target.value;

            if (supervisorDni === 'Todos') {
                const selectAsesor = document.getElementById('asesorDerivaciones');
                selectAsesor.innerHTML = '';
                selectAsesor.appendChild(new Option('Todos', 'Todos'));
                const uniqueAdvisors = asesores.map(u => ({
                    dni: u.dni,
                    nombre: u.nombresCompletos
                }));
                uniqueAdvisors.forEach(asesor => selectAsesor.appendChild(new Option(asesor.nombre, asesor.dni)));
                onFilterChanged();
            } else {
                const idSupervisor = supervisores.find(s => s.dni === supervisorDni).idUsuario;
                const allAdvisors = asesores.filter(a => a.idusuariosup === idSupervisor);

                const uniqueAdvisors = allAdvisors.map(u => ({
                    dni: u.dni,
                    nombre: u.nombresCompletos
                }));
                const selectAsesor = document.getElementById('asesorDerivaciones');
                selectAsesor.innerHTML = '';
                selectAsesor.appendChild(new Option('Todos', 'Todos'));
                uniqueAdvisors.forEach(asesor => selectAsesor.appendChild(new Option(asesor.nombre, asesor.dni)));
                onFilterChanged();
            }
        });
        document.getElementById('fechaVisitaDerivacion').addEventListener('change', onFilterChanged);
        document.getElementById('fechaDerivacion').addEventListener('change', onFilterChanged);
    }

    async function createTable(rol) {
        if (rol === 1 || rol === 4) {
            return;
        } else if (rol === 2) {
            for (let i = derivacionesTableColumns.length - 1; i >= 0; i--) {
                if (derivacionesTableColumns[i].field === 'docSupervisor') {
                    derivacionesTableColumns.splice(i, 1);
                }
            }
        } else if (rol === 3) {
            for (let i = derivacionesTableColumns.length - 1; i >= 0; i--) {
                if (derivacionesTableColumns[i].field === 'dniAsesor' || derivacionesTableColumns[i].field === 'docSupervisor') {
                    derivacionesTableColumns.splice(i, 1);
                }
            }
        }
    }

    function populateFilters(rol, dataasesores = [], datasupervisores = []) {
        if (rol === 1 || rol === 4) {
            const agenciaSelect = document.getElementById('agenciaDerivaciones');
            const asesorSelect = document.getElementById('asesorDerivaciones');
            const supervisorSelect = document.getElementById('supervisorDerivaciones');

            const uniqueAgencies = [...new Set(listaDerivaciones.map(item => item.nombreAgencia))];
            const enrichmentAgencies = uniqueAgencies.map(a => {
                const nameAgencia = a.split(' - ')[1];
                return [nameAgencia, a];
            });

            const uniqueAdvisors = dataasesores.map(u => ({
                dni: u.dni,
                nombre: u.nombresCompletos
            }));

            const uniqueSupervisors = [
                ...new Map(listaDerivaciones.map(item => [item.docSupervisor, {
                    dni: item.docSupervisor
                }])).values()
            ];

            let enrichmentSupervisors = uniqueSupervisors.map(supervisor => {
                const usuario = datasupervisores.find(u => u.dni === supervisor.dni);
                return {
                    dni: supervisor.dni,
                    nombre: usuario ? usuario.nombresCompletos : 'Desconocido'
                };
            });

            enrichmentAgencies.forEach(([name, full]) => {
                agenciaSelect.appendChild(new Option(name, full));
            });
            uniqueAdvisors.forEach(asesor => {
                asesorSelect.appendChild(new Option(asesor.nombre, asesor.dni));
            });

            enrichmentSupervisors.forEach(supervisor => {
                supervisorSelect.appendChild(new Option(supervisor.nombre, supervisor.dni));
            });
        } else if (rol === 2) {
            const agenciaSelect = document.getElementById('agenciaDerivaciones');
            const asesorSelect = document.getElementById('asesorDerivaciones');

            const uniqueAgencies = [...new Set(listaDerivaciones.map(item => item.nombreAgencia))];
            const enrichmentAgencies = uniqueAgencies.map(a => {
                const nameAgencia = a.split(' - ')[1];
                return [nameAgencia, a];
            });
            const uniqueAdvisors = dataasesores.map(u => ({
                dni: u.dni,
                nombre: u.nombresCompletos
            }));

            enrichmentAgencies.forEach(([name, full]) => agenciaSelect.appendChild(new Option(name, full)));
            uniqueAdvisors.forEach(asesor => asesorSelect.appendChild(new Option(asesor.nombre, asesor.dni)));
            document.getElementById('supervisorDerivacionesCol').classList.add('d-none');
        } else if (rol === 3) {
            uniqueAgencies.forEach(agencia => agenciaSelect.appendChild(new Option(agencia, agencia)));
            // Luego ocultamos los selects de asesores y supervisores.
            // Luego ocultamos los selects de asesores y supervisores.
            document.getElementById('asesorDerivacionesCol').classList.add('d-none');
            document.getElementById('supervisorDerivacionesCol').classList.add('d-none');
        } else {
            console.warn('Rol no reconocido. No se poblarán los filtros.');
        }
    }

    return {
        init: async (derivaciones, rol, asesores, supervisores) => {
            const gridDiv = document.querySelector('#gridDerivaciones');
            if (gridDiv) {
                usuariorol = rol || 0;
                listaDerivaciones = derivaciones || [];
                await createTable(usuariorol);
                derivacionesGridOptions.rowData = listaDerivaciones;

                agGrid.createGrid(gridDiv, derivacionesGridOptions);
                populateFilters(usuariorol, asesores || [], supervisores || []);
                setupEventListeners(asesores || [], supervisores || []);
            }
        },
        exportXLSX: () => {
            if (!gridApi) return;
            const csv = gridApi.getDataAsCsv();
            const workbook = XLSX.read(csv, { type: "string" });
            XLSX.writeFile(workbook, "derivaciones.xlsx");
        }
    };

})();