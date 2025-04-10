function gpieasignacionFecha () {
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

function gpiederivacionesFecha () {
    var reportes = document.getElementById('reportes-por-fecha');
    var reportesData = JSON.parse(reportes.getAttribute("data-json"));
    var progresoGeneral = reportesData["progresoGeneral"];

    var options = {
        series: [
            progresoGeneral["totaL_DERIVADOS"],
            progresoGeneral["totaL_DESEMBOLSADOS"]
        ],
        chart: {
            height: 350,
            type: 'pie'
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
    var chart = new ApexCharts(document.querySelector("#div-asignaciones-der-des-general-por-fechas"), options);
    chart.render();
}