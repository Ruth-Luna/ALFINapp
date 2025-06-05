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
    
    var options = {
        series: series,
        chart: {
            type: 'donut',
            height: 350
        },
        labels: labels,
        colors: ["#008FFB", "#00E396"],
        legend: {
            position: 'bottom'
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '65%',
                    labels: {
                        show: true,
                        total: {
                            show: true,
                            showAlways: true,
                            label: 'Total Gestionado',
                            formatter: function() { return totalGestionado; }
                        }
                    }
                }
            }
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

    var totalDerivado = reportesData["totalDerivado"];
    var totalDesembolsado = reportesData["totalDesembolsado"];

    var options = {
        series: [totalDerivado, totalDesembolsado],
        chart: {
            type: 'donut',
            height: 350
        },
        labels: ["Total Derivaciones", "Total Desembolsos"],
        colors: ["#008FFB", "#00E396"],
        legend: {
            position: 'bottom'
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '65%',
                    labels: {
                        show: true,
                        total: {
                            show: true,
                            showAlways: true,
                            label: 'Total Derivaciones',
                            formatter: function() { return totalDerivado; }
                        }
                    }
                }
            }
        },
        title: {
            text: 'Derivaciones vs Desembolsos',
            align: 'center'
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-asignaciones-derivadas-supervisor"), options);
    chart.render();
}

function cargarMovimientosPorFechaSupervisor() {
    var reportesAsesor = document.getElementById('reportes-supervisor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var derivacionesFecha = reportesData["derivacionesFecha"];
    var desembolsosFecha = reportesData["desembolsosFecha"];

    // Crear un mapa combinado de todas las fechas
    var fechasMap = {};
    
    // Procesamos derivaciones
    derivacionesFecha.forEach(item => {
        if (!fechasMap[item["fecha"]]) {
            fechasMap[item["fecha"]] = { derivaciones: 0, desembolsos: 0 };
        }
        fechasMap[item["fecha"]].derivaciones = item["contador"];
    });
    
    // Procesamos desembolsos
    desembolsosFecha.forEach(item => {
        if (!fechasMap[item["fecha"]]) {
            fechasMap[item["fecha"]] = { derivaciones: 0, desembolsos: 0 };
        }
        fechasMap[item["fecha"]].desembolsos = item["contador"];
    });
    
    // Convertir el mapa a arrays ordenados por fecha
    
    var fechas = Object.keys(fechasMap);

    var derivacionesData = fechas.map(fecha => fechasMap[fecha].derivaciones);
    var desembolsosData = fechas.map(fecha => fechasMap[fecha].desembolsos);

    var options = {
        series: [
            {
                name: 'Derivaciones',
                data: derivacionesData
            },
            {
                name: 'Desembolsos',
                data: desembolsosData
            }
        ],
        chart: {
            height: 350,
            type: 'area',
            toolbar: {
                show: true
            }
        },
        stroke: {
            curve: 'smooth',
            width: 2
        },
        colors: ['#00E396', '#d64339'], 
        xaxis: { 
            categories: fechas,
            title: {
                text: 'Fechas'
            }
        },
        yaxis: {
            title: {
                text: 'Cantidad'
            }
        },
        dataLabels: { enabled: false },
        legend: { position: 'top' },
        title: {
            text: 'Derivaciones y Desembolsos por Fecha',
            align: 'center'
        },
        tooltip: {
            shared: true,
            intersect: false,
            x: {
                format: 'dd/MM/yyyy'
            }
        },
        fill: {
            type: 'gradient',
            gradient: {
                shadeIntensity: 1,
                opacityFrom: 0.7,
                opacityTo: 0.3
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-movimientos-por-fecha-supervisor"), options);
    chart.render();
}
