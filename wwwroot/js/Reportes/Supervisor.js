function cargarReportesSupervisor() {
    var reportesElement = document.getElementById('reportes-supervisor');
    var reportesData = JSON.parse(reportesElement.getAttribute("data-json"));

    var tipificacionesAsesor = reportesData["tipificacionesAsesores"];
    var asesoresNombres = [];
    var totalSinGestionar = [];
    var totalDerivaciones = [];
    var totalGestionado = [];
    var totalGestionadoTrunc = [];
    var totalDesembolsos = [];

    tipificacionesAsesor.forEach(item => {
        asesoresNombres.push(item["nombreAsesor"]);
        totalSinGestionar.push(item["totalSinGestionar"]);
        totalGestionado.push(item["totalGestionado"]);
        
        // 🔹 Truncar valores mayores a 120
        totalGestionadoTrunc.push(item["totalGestionado"] > 120 ? 120 : item["totalGestionado"]);

        totalDerivaciones.push(item["totalDerivaciones"]);
        totalDesembolsos.push(item["totalDesembolsos"]);
    });

    var options = {
        series: [
            { name: 'Sin Gestionar', data: totalSinGestionar },
            { name: 'Gestionado', data: totalGestionadoTrunc },
            { name: 'Derivaciones', data: totalDerivaciones },
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
            intersect: false,
            custom: function({ series, seriesIndex, dataPointIndex, w }) {
                var totalRealGestionado = totalGestionado[dataPointIndex]; // 🔹 Obtener el total real

                var tooltipHTML = `<div style="padding: 10px; background: #fff; border-radius: 5px; box-shadow: 0px 2px 6px rgba(0, 0, 0, 0.2);">
                        <strong style="color: #333;">${w.globals.labels[dataPointIndex]}</strong><br>`;

                w.globals.series.forEach((serie, i) => {
                    tooltipHTML += `<span style="color: ${w.globals.colors[i]}; font-weight: bold;">●</span> 
                                    ${w.globals.seriesNames[i]}: <strong>${serie[dataPointIndex]}</strong><br>`;
                });

                // 🔹 Agregar el total real de "Gestionado" aunque esté truncado en el gráfico
                tooltipHTML += `<hr style="margin: 5px 0;">
                                <strong style="color: #FF9800;">Total Gestionado Real:</strong> ${totalRealGestionado}
                            </div>`;
                
                return tooltipHTML;
            }
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
    var totalDerivado = reportesData["totalDerivado"];
    var totalGestionado = reportesData["totalGestionado"];
    
    var sinDerivar = totalGestionado - totalDerivado;
    var series = [
        totalDerivado,
        sinDerivar
    ];
    var labels = [
        "Derivaciones",
        "Sin Derivar"
    ];
    var totalGeneral = totalGestionado;
    var options = {
        series: series,
        chart: {
            type: 'pie',
            height: 350
        },
        labels: labels,
        colors: ["#008FFB", "#00E396"],
        legend: {
            position: 'bottom'
        },
        title: {
            text: 'Derivaciones vs Gestionado no Derivado',
            align: 'center'
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
            reportesData["totalDerivado"],
            reportesData["totalDesembolsado"]
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
    var desembolsosFecha = reportesData["desembolsosFecha"];

    var fechas = [];
    var contador = [];

    desembolsosFecha.forEach(item => {
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
