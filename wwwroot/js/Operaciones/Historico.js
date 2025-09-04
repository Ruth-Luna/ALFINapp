var App = App || {};
App.historico = (function () {
    let gridApi;
    let gridInitialized = false;

    let historico = [];

    let derivacionesTableColumns = [
        /*
        {
            headerName: 'Estado Reagendamiento',
            field: 'estadoReagendamiento',
            width: 220,
            sortable: true,
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                if (params.value === 'REAGENDACION EXITOSA') {
                    return `<span class="badge-estado badge-enviado">Procesado</span>`;
                } else {
                    return `<span class="badge-estado badge-no-enviado">Pendiente</span>`;
                }
            },
        },
        */
        { headerName: 'N°', field: 'numeroReagendamientoFormateado', width: 70, sortable: true },
        {
            headerName: 'Cliente',
            field: 'dniCliente',
            width: 180,
            sortable: true,
            cellRenderer: (params) => {
                return `<span title="${params.data.nombreCliente || ''}">${params.value} - ${params.data.nombreCliente || ''}</span>`;
            },
        },
        {
            headerName: 'Telefono',
            field: 'telefono',
            width: 100,
            sortable: true
        },
        {
            headerName: 'Asesor',
            field: 'dniAsesor',
            width: 180,
            sortable: true,
            cellRenderer: (params) => {
                return `<span title="${params.data.nombreAsesor || ''}">${params.value} - ${params.data.nombreAsesor || ''}</span>`;
            },
        },
        {
            headerName: 'Oferta',
            field: 'oferta',
            width: 150,
            sortable: true,
            valueFormatter: params => {
                if (params.value === 0) return 'No aplica';
                return params.value.toLocaleString('es-ES', { style: 'currency', currency: 'PEN' });
            },
        },
        {
            headerName: 'Agencia',
            field: 'agencia',
            width: 220,
            sortable: true
        },
        {
            headerName: 'Fecha Derivación',
            field: 'fechaDerivacion',
            width: 150,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm')
        },
        {
            headerName: 'Fecha Visita',
            field: 'fechaVisita',
            width: 150,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy')
        },
        {
            headerName: 'Fecha ReAgendamiento',
            field: 'fechaAgendamiento',
            width: 220,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm')
        },
    ];

    const derivacionesGridOptions = {
        columnDefs: derivacionesTableColumns,
        rowData: null,
        pagination: true,
        paginationPageSize: 20,
        defaultColDef: {
            resizable: true,
            sortable: true,
        },
        onGridReady: function (params) {
            gridApi = params.api;
            params.api.sizeColumnsToFit();
        }
    };

    function init(historicoData) {
        historico = historicoData || [];

        const gridDiv = document.getElementById('gridHistoricoReagendamientos');
        if (!gridDiv) {
            return;
        }
        gridDiv.innerHTML = '';
        derivacionesGridOptions.rowData = historicoData || [];

        agGrid.createGrid(gridDiv, derivacionesGridOptions);
        gridInitialized = true;
    }

    return {
        init: init
    };
})();