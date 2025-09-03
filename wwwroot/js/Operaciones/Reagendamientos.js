var App = App || {};
App.reagendamientos = (() => {
    let gridApi;
    let listaReagendamientos = [];

    // Definición de las columnas para la tabla de Reagendamientos.
    const reagendamientosTableColumns = [
        {
            headerName: "Histórico",
            field: "historico",
            width: 200,
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';

                let btnHistorico = document.createElement('button');
                btnHistorico.className = 'btn btn-sm btn-primary';
                btnHistorico.title = 'Ver Histórico';
                btnHistorico.innerHTML = '<i class="ri-history-line"></i>';
                btnHistorico.addEventListener('click', async () => {
                    const modalElement = document.getElementById('modalHistoricoReagendamiento');
                    if (modalElement) {
                        const modal = new bootstrap.Modal(modalElement);
                        modal.show();
                        var historicoData = await getHistorico(params.data.idDerivacion);
                        App.historico.init(
                            historicoData
                        );
                    } else {
                        console.error('Modal con ID #modalHistoricoReagendamiento no encontrado.');
                    }
                });

                let btnReagendamiento = document.createElement('button');
                if (params.data.puedeSerReagendado === true) {
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
                } else {
                    btnReagendamiento.className = 'btn btn-sm btn-secondary disabled';
                    btnReagendamiento.title = 'No puede ser reagendado';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';
                }

                container.appendChild(btnHistorico);
                container.appendChild(btnReagendamiento);
                return container;
            },
            sortable: false,
            resizable: false,
            width: 100
        },
        {
            headerName: "Estado",
            field: "estadoReagendamiento",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                // Crear el contenedor principal
                const $container = $('<div>', { class: 'd-flex align-items-center gap-2' });

                // Badge de estado
                let badgeClass, badgeText;
                if (params.value === "REAGENDACION EXITOSA") {
                    badgeClass = 'badge-estado badge-reag-enviado';
                    badgeText = 'Enviado';
                } else {
                    badgeClass = 'badge-estado badge-reag-pendiente';
                    badgeText = 'Pendiente';
                }
                const $badge = $('<span>', {
                    class: badgeClass,
                    text: badgeText,
                    css: {
                        lineHeight: '1.8',
                    }
                });

                // Icono de correo
                const fueEnviado = params.data.fueEnviadoEmail === true;
                const mailIconClass = fueEnviado
                    ? 'ri-mail-send-line text-success'
                    : 'ri-mail-send-line text-danger';
                const mailTitle = fueEnviado
                    ? 'Correo enviado'
                    : 'Correo no enviado';
                const $mailIcon = $('<i>', {
                    class: mailIconClass,
                    title: mailTitle,
                });

                $container.append($badge, $mailIcon);

                return $container[0];
            },
            width: 120
        },
        {
            headerName: "N° Reagendamiento",
            //field: "numeroReagendamiento",
            field: "numeroReagendamientoFormateado",
            width: 180
        },
        {
            headerName: "DNI cliente",
            width: 120,
            cellRenderer: (params) => {
                const $container = $('<div>', { class: 'd-flex gap-2' });

                if (params.data.fueDesembolsadoGeneral === true) {
                    $container.append($('<i>', {
                        class: 'bi-cash-stack text-success'
                    }));
                }
                $container.append($('<div>').text(params.data.dniCliente));

                return $container[0];
            }
        },
        { headerName: "Cliente", field: "nombreCliente", width: 250 },
        { headerName: "Teléfono", field: "telefono", width: 120 },
        //{ headerName: "DNI asesor", field: "dniAsesor", width: 120 },
        { headerName: "Asesor", field: "nombreAsesor", width: 120 },
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
        {
            headerName: "Fecha Derivación",
            field: "fechaDerivacion",
            width: 150,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm')
        },
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
    ];

    const reagendamientosGridOptions = {
        columnDefs: reagendamientosTableColumns,
        rowData: listaReagendamientos,
        pagination: true,
        paginationPageSize: 20,
        copyHeadersToClipboard: true,
        suppressClipboardPaste: true,
        enableCellTextSelection: true,
        enableBrowserTooltips: true,
        initialState: { sort: { sortModel: [{ colId: 'estadoReagendamiento', sort: 'asc' }] } },
        defaultColDef: { sortable: true, resizable: true, minWidth: 50, flex: 1 },
        isExternalFilterPresent: isExternalFilterPresent,
        doesExternalFilterPass: doesExternalFilterPass,
        getRowClass: (params) => {
            if (params.data.puedeSerReagendado === true) {
                return ['ag-row-reagendable'];
            }
            return [];
        },
        onGridReady: (params) => {
            gridApi = params.api;
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        }
    };

    // Agregar después de let listaDerivaciones = [];
    const externalFilterState = {
        dniCliente: ''
    };

    // Agregar después de externalFilterState
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

    // Agregar después de updateTableData
    function setupDNIFilter() {
        const dniInput = document.getElementById('dniClienteReagendamientos');
        if (dniInput) {
            dniInput.addEventListener('input', function () {
                externalFilterState.dniCliente = this.value.trim();
                if (gridApi) {
                    gridApi.onFilterChanged();
                }
            });
        }
    }

    function createTable(rol) {
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

    function init() {
        const gridDiv = document.querySelector('#gridReagendamientos');
        if (gridDiv) {
            // Obtener el rol del elemento #idRol
            const idRolElement = document.getElementById('idRol');
            const rol = idRolElement ? parseInt(idRolElement.value) || 0 : 0;

            // Modificar columnas según el rol
            createTable(rol);

            // Inicializar tabla vacía
            reagendamientosGridOptions.rowData = [];
            gridApi = agGrid.createGrid(gridDiv, reagendamientosGridOptions);

            // Configurar filtro por DNI
            setupDNIFilter();
        }
    }

    function updateTableData(data) {
        listaReagendamientos = data || [];
        if (gridApi) {
            gridApi.setGridOption('rowData', listaReagendamientos);
        }
    }

    function exportarReagendamientos() {
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

    return {
        init,
        updateTableData,
        gridApi: () => gridApi,
        exportarReagendamientos
    };
})();
