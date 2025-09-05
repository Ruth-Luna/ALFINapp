function cargarEvidencia(data) {
    const reagendacionContent = document.getElementById('reagendacion-de-derivacion-content');
    reagendacionContent.classList.add('d-none');
}

﻿function formatDateTime(dateString, format = 'dd/mm/yyyy') {
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

function DescargarResumenExcelDerivaciones() {
    let dni = $('#dniClienteDerivaciones').val() || null;
    let supervisor = $('#supervisorDerivaciones').val() || null;
    let asesor = $('#asesorDerivaciones').val() || null;
    let agencia = $('#agenciaDerivaciones').val() || null;
    let fechaDerivacion = $('#fechaDerivacion').val() || null;
    let fechaVisita = $('#fechaVisitaDerivacion').val() || null;

    const Fecha = obtenerFechaActual();

    Swal.fire({
        title: 'Cargando...',
        timerProgressBar: true,
        allowEscapeKey: false,
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    
    $.ajax({
        url: '/Operaciones/ExportarDerivacionesExcel',
        type: 'GET',
        data: {
            dni : dni,
            idAsesor: asesor,
            idSupervisor: supervisor,
            agencia: agencia,
            fecha_derivacion: fechaDerivacion,
            fecha_visita: fechaVisita
        },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            var a = document.createElement('a');
            a.href = window.URL.createObjectURL(data);


            a.download = 'ReporteResumenDerivacion_' + Fecha + '.xlsx';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            Swal.close();
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error en la operación',
                text: 'Hubo un problema al generar el archivo Excel',
                confirmButtonText: 'Ok',
            });
            Swal.close();
        }
    });
}

var App = App || {};
App.derivaciones = (() => {
    let gridApi;
    let listaDerivaciones = [];

    let derivacionesTableColumns = [
        {
            headerName: "Estado Derivación",
            field: "estadoDerivacion",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const container = document.createElement('div');
                container.className = 'd-inline-flex gap-2';

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
            width: 150
        },
        { 
            headerName: "DNI cliente", 
            width: 120,
            cellRenderer: (params) => {
                const $container = $('<div>', { class: 'd-flex gap-2' });

                if (params.data.fueDesembolsado == true) {
                    $container.append($('<i>', {
                        class: 'bi-cash-stack text-success'
                    }));
                }
                $container.append($('<div>').text(params.data.dniCliente));
                
                return $container[0];
            }
        },
        { headerName: "Cliente", field: "nombreCliente", width: 150 },
        { headerName: "Teléfono", field: "telefonoCliente", width: 90 },
        //{ headerName: "DNI asesor", field: "dniAsesor", width: 90 },
        { headerName: "Nombre asesor", field: "nombreAsesor", width: 150 },
        {
            headerName: "Oferta",
            field: "ofertaMax",
            valueFormatter: params => {
                if (params.value === 0) return 'No aplica';
                return `S/. ${Number(params.value).toLocaleString('es-PE')}`;
            },
            width: 80
        },
        { headerName: "Agencia", field: "nombreAgencia", width: 100 },
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
                const $container = $('<div>', { class: 'd-inline-flex gap-1' });

                // Badge para evidencia (E)
                const $badgeE = $('<div>', {
                    class: `af-badge ${params.value === "Enviado" ? 'af-badge-bg-success' : 'af-badge-bg-danger'}`,
                    html: `
                        <i class="${params.value === "Enviado" ? 'ri-checkbox-circle-fill' : 'ri-close-circle-fill'}"></i>
                        <span>E</span>
                    `,
                    title: params.value === "Enviado" ? 'Evidencia enviada' : 'Evidencia no enviada'
                });

                $container.append($badgeE);
                return $container[0];
            },
            comparator: (valueA, valueB) => (valueA === valueB ? 0 : valueA === 'No enviado' ? -1 : 1),
            width: 60
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
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';

                let btnReagendamiento = document.createElement('button');
                var mostrarBtnReagendamiento = true;
                let btnEnviarEvidencia = document.createElement('button');
                var mostrarBtnEnviarEvidencia = true;
                if (!params.data.puedeSerReagendado) {
                    btnReagendamiento.className = 'btn btn-sm btn-secondary disabled';
                    btnReagendamiento.title = 'Reagendamiento';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';

                    btnEnviarEvidencia.className = 'btn btn-sm btn-info';
                    btnEnviarEvidencia.title = 'Enviar evidencia';
                    btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>';

                    btnEnviarEvidencia.addEventListener('click', async () => {
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
                            await modal_id_derivacion_to_be_uploaded(params.data.idDerivacion);

                            modal.show();
                        }
                    });
                } else {
                    btnReagendamiento.className = 'btn btn-sm btn-primary';
                    btnReagendamiento.title = 'Reagendamiento';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';

                    btnReagendamiento.addEventListener('click', async () => {
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
                            await modal_id_derivacion_to_be_uploaded(params.data.idDerivacion);

                            modal.show();
                        }
                    });

                    mostrarBtnReagendamiento = true;

                    btnEnviarEvidencia.className = 'btn btn-sm btn-secondary disabled';
                    btnEnviarEvidencia.title = 'Enviar evidencia';
                    btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>';

                    mostrarBtnEnviarEvidencia = false;
                }

                if (mostrarBtnReagendamiento) {
                    container.appendChild(btnReagendamiento);
                }
                if (mostrarBtnEnviarEvidencia) {
                    container.appendChild(btnEnviarEvidencia);
                }

                return container;
            },
            width: 130,
            sortable: false,
            resizable: false
        }
    ];

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
        getRowClass: (params) => {
            if (params.data.puedeSerReagendado === true) {
                return ['ag-row-reagendable'];
            }
            return [];
        },
        // AGREGAR ESTAS LÍNEAS:
        isExternalFilterPresent: isExternalFilterPresent,
        doesExternalFilterPass: doesExternalFilterPass,
        onGridReady: (params) => {
            gridApi = params.api;
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        }
    };

    const externalFilterState = {
        dniCliente: ''
    };

    function isExternalFilterPresent() {
        return externalFilterState.dniCliente !== '';
    }

    function doesExternalFilterPass(node) {
        const { data } = node;
        const { dniCliente } = externalFilterState;
        
        if (dniCliente && !String(data.dniCliente).includes(dniCliente)) {
            return false;
        }
        
        return true;
    }

    function setupDNIFilter() {
        const dniInput = document.getElementById('dniClienteDerivaciones');
        if (dniInput) {
            dniInput.addEventListener('input', function() {
                externalFilterState.dniCliente = this.value.trim();
                if (gridApi) {
                    gridApi.onFilterChanged();
                }
            });
        }
    }

    function createTable(rol) {
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
                if (derivacionesTableColumns[i].field === 'nombreAsesor' || derivacionesTableColumns[i].field === 'docSupervisor') {
                    derivacionesTableColumns.splice(i, 1);
                }
            }
        }
    }

    function init() {
        const gridDiv = document.querySelector('#gridDerivaciones');
        if (gridDiv) {
            // Obtener el rol del elemento #idRol
            const idRolElement = document.getElementById('idRol');
            const rol = idRolElement ? parseInt(idRolElement.value) || 0 : 0;
            
            // Modificar columnas según el rol
            createTable(rol);
            
            // Inicializar tabla vacía
            derivacionesGridOptions.rowData = [];
            gridApi = agGrid.createGrid(gridDiv, derivacionesGridOptions);

            // Configurar filtro por DNI
            setupDNIFilter();
        }
    }

    function updateTableData(data) {
        listaDerivaciones = data || [];
        if (gridApi) {
            gridApi.setGridOption('rowData', listaDerivaciones);
        }
    }

    function exportXLSX() {
        if (!gridApi) return;

        const rowData = [];
        gridApi.forEachNodeAfterFilterAndSort(node => rowData.push(node.data));

        if (rowData.length === 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Atención',
                text: 'No hay datos para exportar.'
            });
            return;
        }

        const worksheet = XLSX.utils.json_to_sheet(rowData);

        const colWidths = Object.keys(rowData[0]).map(key => ({
            wch: Math.max(
                key.length,
                ...rowData.map(r => (r[key] ? r[key].toString().length : 0))
            )
        }));
        worksheet['!cols'] = colWidths;

        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Derivaciones");

        XLSX.writeFile(workbook, "Derivaciones.xlsx");
    }

    return {
        init,
        updateTableData,
        exportXLSX,
        gridApi: () => gridApi
    };
})();