async function getHistorico(idDerivacion) {
    const url = window.location.origin + '/Operaciones/GetHistoricoReagendamientos';
    try {
        const response = await fetch(url + '?idDerivacion=' + idDerivacion, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        if (data.success === true) {
            return data.historico;
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: data.message || 'Ocurrió un error al obtener el histórico.'
            });
            return [];
        }
    } catch (error) {
        console.error('Error fetching historico:', error);
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Ocurrió un error al obtener el histórico.'
        });
        return [];
    }
}

var App = App || {};

App.historico = (function () {
    let gridApi;
    let gridInitialized = false;

    let historico = [];
    let rol = 0;
    let usuarioAsesores = [];
    let usuarioSupervisores = [];

    let derivacionesTableColumns = [
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
        { headerName: 'N°', field: 'numeroReagendamiento', width: 60, sortable: true },
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

    function init(historicoData, usuariorol, asesores, supervisores) {
        historico = historicoData || [];
        rol = usuariorol || 0;
        usuarioAsesores = asesores || [];
        usuarioSupervisores = supervisores || [];

        const gridDiv = document.getElementById('gridHistóricoReagendamientos');
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