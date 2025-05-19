function gpieasignacionFecha() {
    var reportes = document.getElementById('reportes-por-fecha');
    var reportesData = JSON.parse(reportes.getAttribute("data-json"));
    var progresoGeneral = reportesData["progresoGeneral"];

    var options = {
        series: [
            progresoGeneral["totaL_ASIGNADOS"],
            progresoGeneral["totaL_GESTIONADOS"]
        ],
        chart: {
            type: 'pie',
            height: 350
        },
        labels: [
            "Total Asignados",
            "Total Gestionados"
        ],
        colors: ["#008FFB", "#00E396"],
        legend: {
            position: 'bottom'
        }
    };
    var chart = new ApexCharts(document.querySelector("#div-asignaciones-total-tip-general-por-fechas"), options);
    chart.render();
}

function gtablamesinforme() {
    var reportes = document.getElementById('reportes-por-meses');
    var reportesData = JSON.parse(reportes.getAttribute("data-json"));
    var rowData = reportesData["reporteTablaPorMeses"];

    const gridOptions = {
        rowData: rowData,
        columnDefs: [
            { field: "periodo", headerName: "PERIODO" },
            {
                field: "porcentaje_derivados",
                headerName: "% DERIVACION",
                valueFormatter: params => {
                    return params.value + '%';
                }
            },
            {
                field: "porcentaje_desembolsados"
                , headerName: "% DESEMBOLSOS",
                valueFormatter: params => {
                    return params.value + '%';
                }
            },
            {
                field: "porcentaje_no_derivado", headerName: "% NO DERIVADO",
                valueFormatter: params => {
                    return params.value + '%';
                }
            },
            { field: "total_desembolsados", headerName: "TOTAL DESEMBOLSOS" },
            { field: "total_gestionados", headerName: "TOTAL GESTIONADOS" },
        ],
        defaultColDef: {
            sortable: true,
            filter: true
        },
        pagination: true, // Habilita la paginaci�n
        paginationPageSize: 10, // N�mero de filas por p�gina
        paginationPageSizeSelector: [10, 20, 50, 100], // Opciones de tama�o de p�gina
    };

    const myGridElement = document.querySelector('#div-asignaciones-der-des-general-por-fechas-detalle');
    agGrid.createGrid(myGridElement, gridOptions);
}