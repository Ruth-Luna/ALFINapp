document.addEventListener("DOMContentLoaded", function () {
    var targetNode = document.getElementById("div-derivaciones-supervisor");

    var observer = new MutationObserver(function (mutationsList) {
        for (var mutation of mutationsList) {
            if (mutation.attributeName === "style") {
                var displayValue = window.getComputedStyle(targetNode).display;
                if (displayValue === "block") {
                    console.log("El div ahora es visible. Iniciando gráfico...");
                    cargarReportesSupervisor();
                    cargarReportesDerivacionesSupervisor();
                    cargarGraficoDerivacionesVsDesembolsos();
                    observer.disconnect();
                }
            }
        }
    });

    observer.observe(targetNode, { attributes: true, attributeFilter: ["style"] });
});


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
        legend: { position: 'top' }
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
