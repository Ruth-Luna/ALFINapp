document.addEventListener("DOMContentLoaded", function () {
    createTableMetas("view-reportes-metas-data", "grid-metas-report");
    createGpieByImportes("view-reportes-g-pie-dates-data", "g-pie-report-general-by-importes");
    createGpieByDates("view-reportes-g-pie-dates-data", "g-pie-report-general-by-date");
});

function createTableMetas(idData, idTable) {
    var dataId = document.getElementById(idData);
    var data = JSON.parse(dataId.getAttribute("data-json"));
    const gridOptions = {
        rowData: data,
        columnDefs: [
            { field: "nombresCompletos", headerName: "ASESOR" },
            { field: "totalDerivaciones", headerName: "DERIVACIONES" },
            {
                field: "totalImporte",
                headerName: "IMPORTE DESEMBOLSADO",
                valueFormatter: params => {
                    return params.value ? params.value.toLocaleString('es-PE', { style: 'currency', currency: 'PEN' }) : '0';
                }
            },
            { field: "totalGestion", headerName: "GESTION" },
            { field: "metasGestiones", headerName: "META GESTION" },
            { 
                field: "metasImporte", 
                headerName: "META IMPORTE",
                valueFormatter: params => {
                    return params.value ? params.value.toLocaleString('es-PE', { style: 'currency', currency: 'PEN' }) : '0';
                }
            },
            { field: "metasDerivaciones", headerName: "META DERIVACIONES" },
            {
                field: "porcentajeGestiones",
                headerName: "PORCENTAJE GESTION",
                valueFormatter: params => {
                    return params.value != null ? parseFloat(params.value).toFixed(2) + '%' : '0%';
                }
            },
            { 
                field: "porcentajeImporte", 
                headerName: "PORCENTAJE IMPORTE",
                valueFormatter: params => {
                    return params.value != null ? parseFloat(params.value).toFixed(2) + '%' : '0%';
                }
            },
            { 
                field: "porcentajeDerivaciones", 
                headerName: "PORCENTAJE DERIVACIONES",
                valueFormatter: params => {
                    return params.value != null ? parseFloat(params.value).toFixed(2) + '%' : '0%';
                }
            }
        ],
        defaultColDef: {
            sortable: true,
            filter: true
        },
        pagination: true,
        paginationPageSize: 10,
        paginationPageSizeSelector: [10, 20, 50, 100],
    };

    const myGridElement = document.getElementById(idTable);
    agGrid.createGrid(myGridElement, gridOptions);
}

function createGpieByDates(idData, idDiv) {
    var dataId = document.getElementById(idData);
    var data = JSON.parse(dataId.getAttribute("data-json"));
    var options = {
        chart: {
            type: 'pie',
            height: 350,
            title: {
                text: 'REPORTE GENERAL POR ASESOR - POR FECHA ' + data["pERIODO"],
                align: 'center',
                verticalAlign: 'middle',
                style: {
                    fontSize: '16px',
                    fontWeight: 'bold'
                }
            }
        },
        series: [
            data["totaL_GESTIONADOS"],
            data["totaL_DERIVADOS"],
        ],
        labels: [
            "GESTIONADOS",
            "DERIVADOS",
        ],
        colors: ["#008FFB", "#00E396"],
        legend: {
            position: 'bottom'
        },
    }
    var chart = new ApexCharts(document.querySelector("#" + idDiv), options);
    chart.render();
}

function createGpieByImportes(idData, idDiv) {
    var dataId = document.getElementById(idData);
    var data = JSON.parse(dataId.getAttribute("data-json"));
    var options = {
        chart: {
            type: 'pie',
            height: 350,
            title: {
                text: 'REPORTE IMPORTES POR ASESOR',
                align: 'center',
                verticalAlign: 'middle',
                style: {
                    fontSize: '16px',
                    fontWeight: 'bold'
                }
            }
        },
        series: [
            data["total_importes"],
            data["totaL_DESEMBOLSADOS"],
            data["totaL_DERIVADOS"],
        ],
        labels: [
            "IMPORTES",
            "DESEMBOLSOS",
            "DERIVADOS",
        ],
        colors: ["#008FFB", "#00E396", "#FF4560"],
        legend: {
            position: 'bottom'
        },
    }
    var chart = new ApexCharts(document.querySelector("#" + idDiv), options);
    chart.render();
}