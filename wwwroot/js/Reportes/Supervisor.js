function cargarReportesSupervisor() {
    var reportesElement = document.getElementById('reportes-supervisor');
    var reportesData = JSON.parse(reportesElement.getAttribute("data-json"));

    var tipificacionesAsesor = reportesData["tipificacionesAsesores"];
    var asesoresNombres = [];
    var totalTipificados = [];
    var totalDerivaciones = [];
    var totalDerProcesadas = [];
    var totalDesembolsos = [];

    tipificacionesAsesor.forEach(item => {
        asesoresNombres.push(item["nombreAsesor"]);
        totalTipificados.push(item["totalTipificados"]);
        totalDerivaciones.push(item["totalDerivaciones"]);
        totalDerProcesadas.push(item["totalDerivacionesProcesadas"]);
        totalDesembolsos.push(item["totalDesembolsos"]);
    });

    var options = {
        series: [
            { name: 'Tipificaciones', data: totalTipificados },
            { name: 'Derivaciones', data: totalDerivaciones },
            { name: 'Derivaciones Procesadas', data: totalDerProcesadas },
            { name: 'Desembolsos', data: totalDesembolsos }
        ],
        chart: {
            type: 'bar',
            height: 400,
            stacked: true
        },
        plotOptions: {
            bar: { horizontal: false, columnWidth: '60%' }
        },
        colors: ['#00E396', '#775DD0', '#FF4560', '#008FFB'],
        xaxis: { categories: asesoresNombres },
        dataLabels: { enabled: false },
        legend: { position: 'top' },
        tooltip: {
            shared: true,
            intersect: false
        },
        title: {
            text: 'Reporte de Supervisores',
            align: 'center'
        },
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-general-supervisor"), options);
    chart.render();
}

function cargarReportesDerivacionesSupervisor() {
    var reportesElement = document.getElementById('reportes-supervisor');
    var reportesData = JSON.parse(reportesElement.getAttribute("data-json"));

    var options = {
        series: [
            reportesData["totalDerivacionesDesembolsadas"],
            reportesData["totalDerivacionesNoDesembolsadas"],
            reportesData["totalAsignacionesProcesadas"],
            reportesData["totalAsignaciones"] - reportesData["totalAsignacionesProcesadas"]
        ],
        chart: {
            type: 'pie',
            height: 350
        },
        labels: [
            "Derivaciones Desembolsadas",
            "Derivaciones No Desembolsadas",
            "Asignaciones Procesadas",
            "Asignaciones Pendientes"
        ],
        colors: ["#00E396", "#FF4560", "#008FFB", "#FEB019"],
        legend: {
            position: 'bottom'
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-asignaciones-general-supervisor"), options);
    chart.render();
}


function cargarGraficoDerivacionesVsDesembolsos() {
    var reportesElement = document.getElementById('reportes-supervisor');
    var reportesData = JSON.parse(reportesElement.getAttribute("data-json"));

    var options = {
        series: [
            reportesData["totalDerivaciones"],
            reportesData["totalDerivacionesDesembolsadas"]
        ],
        chart: {
            type: 'pie',
            height: 350
        },
        labels: [
            "Total Derivaciones",
            "Total Desembolsos"
        ],
        colors: ["#008FFB", "#00E396"],
        legend: {
            position: 'bottom'
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-asignaciones-derivadas-supervisor"), options);
    chart.render();
}

function cargarDerivacionesSupervisorFecha() {
    var reportesAsesor = document.getElementById('reportes-supervisor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var derivacionesFecha = reportesData["derivacionesFecha"];

    var fechas = [];
    var contador = [];

    derivacionesFecha.forEach(item => {
        fechas.push(item["fecha"]);
        contador.push(item["contador"]);
    });

    var options = {
        series: [
            {
                name: 'Derivaciones',
                data: contador
            }
        ],
        chart: {
            height: 350,
            type: 'area'  
        },
        stroke: {
            curve: 'smooth'
        },
        colors: ['#00E396'], 
        xaxis: { 
            categories: fechas,
            title: {
                text: 'Fechas'
            }
        },
        yaxis: {
            title: {
                text: 'Número de Derivaciones'
            }
        },
        dataLabels: { enabled: false },
        legend: { position: 'top' },
        title: {
            text: 'Derivaciones por Fecha',
            align: 'center'
        },
        tooltip: {
            x: {
                format: 'dd/MM/yyyy'
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-derivaciones-por-fecha-supervisor"), options);
    chart.render();
}

function cargarDesembolsosSupervisorFecha() {
    var reportesAsesor = document.getElementById('reportes-supervisor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var derivacionesFecha = reportesData["desembolsosFecha"];

    var fechas = [];
    var contador = [];

    derivacionesFecha.forEach(item => {
        fechas.push(item["fecha"]);
        contador.push(item["contador"]);
    });

    var options = {
        series: [
            {
                name: 'Desembolsos',
                data: contador
            }
        ],
        chart: {
            height: 350,
            type: 'area'  
        },
        stroke: {
            curve: 'smooth'
        },
        colors: ['#d64339'], 
        xaxis: { 
            categories: fechas,
            title: {
                text: 'Fechas'
            }
        },
        yaxis: {
            title: {
                text: 'Número de Derivaciones'
            }
        },
        dataLabels: { enabled: false },
        legend: { position: 'top' },
        title: {
            text: 'Desembolsos por Fecha',
            align: 'center'
        },
        tooltip: {
            x: {
                format: 'dd/MM/yyyy'
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-desembolsos-por-fecha-supervisor"), options);
    chart.render();
}
