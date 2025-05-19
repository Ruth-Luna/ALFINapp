function cargarDerivacionesAsesorFecha() {
    var reportesAsesor = document.getElementById('reportes-asesor');
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

    var chart = new ApexCharts(document.querySelector("#div-reporte-derivacion-asesor-fecha"), options);
    chart.render();
}

function cargarGestionInforme() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var GestionInformes = reportesData["tipificacionesGestion"];

    var tipificacion = [];
    var descripcionMap = {};
    var contador = [];
    var contadorReal = [];

    GestionInformes.forEach(item => {
        let id = item["idTipificacion"];
        let count = item["contadorTipificaciones"];

        tipificacion.push(id);
        descripcionMap[id] = item["descripcionTipificaciones"];
        contadorReal.push(count);
        contador.push(count > 200 ? 200 : count);
    });

    var options = {
        series: [{
            name: 'Gestión',
            data: contador
        }],
        chart: {
            type: 'bar',
            height: 350
        },
        xaxis: {
            categories: tipificacion,
            title: {
                text: 'ID Tipificaciones'
            }
        },
        yaxis: {
            title: {
                text: 'Número de Tipificaciones'
            }
        },
        plotOptions: {
            bar: {
                columnWidth: '50%'
            }
        },
        dataLabels: {
            enabled: false
        },
        colors: ['#008FFB'],
        title: {
            text: 'Gestión de Informes',
            align: 'center'
        },
        legend: {
            position: 'top'
        },
        tooltip: {
            intersect: false,
            y: {
                formatter: function (value, { dataPointIndex }) {
                    let id = tipificacion[dataPointIndex];
                    let descripcion = descripcionMap[id] || "Desconocido";
                    let countReal = contadorReal[dataPointIndex];
                    return `${descripcion}\nTotal: ${countReal}`;
                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-gestion-asesor"), options);
    chart.render();
}

function cargarReporteDerivacionVsDesembolso() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var totalDerivaciones = reportesData["totalDerivaciones"];
    var totalDesembolsos = reportesData["totalDesembolsos"];
    var sinDesembolsar = totalDerivaciones - totalDesembolsos;

    var series = [
        totalDesembolsos,
        sinDesembolsar
    ];
    var labels = [
        "Desembolsos",
        "Sin Desembolsar"
    ];
    var totalGeneral = totalDerivaciones;
    var options = {
        series: series,
        chart: {
            type: 'donut',
            height: 350
        },
        labels: labels,
        colors: ["#FF4560", "#00E396"],
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
                            label: 'Total Derivaciones',
                            formatter: function (w) {
                                return totalGeneral;
                            }
                        }
                    }
                }
            }
        },
        tooltip: {
            intersect: false,
            y: {
                formatter: function (value, { dataPointIndex }) {
                    return `${labels[dataPointIndex]}: ${value} (Total Derivaciones: ${totalGeneral})`;
                }
            }
        }
    };
    var chart = new ApexCharts(document.querySelector("#div-reporte-derivaciones-vs-desembolsos"), options);
    chart.render();
}

function cargarReporteAsignacionVsGestion() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var totalAsignado = reportesData["totalAsignado"];
    var totalGestionado = reportesData["totalGestionado"];
    // Calculate clients that haven't been managed yet
    var sinGestionar = totalAsignado - totalGestionado;
    var series = [
        totalGestionado,
        sinGestionar
    ];
    var labels = [
        "Clientes Gestionados",
        "Clientes Sin Gestionar"
    ];
    var totalGeneral = totalAsignado;
    var options = {
        series: series,
        chart: {
            type: 'donut',
            height: 350
        },
        labels: labels,
        colors: ["#008FFB", "#FEB019"],
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
                            label: 'Total Asignado',
                            formatter: function (w) {
                                return totalGeneral;
                            }
                        }
                    }
                }
            }
        },
        tooltip: {
            intersect: false,
            y: {
                formatter: function (value, { dataPointIndex }) {
                    return `${labels[dataPointIndex]}: ${value} (Total: ${totalGeneral})`;
                }
            }
        }
    };
    var chart = new ApexCharts(document.querySelector("#div-reporte-asignaciones-vs-gestion"), options);
    chart.render();
}

function cargarReporteGeneralAsesor() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));

    var series = [
        reportesData["totalDerivaciones"],
        reportesData["totalDesembolsos"],
        reportesData["totalAsignado"],
        reportesData["totalGestionado"],
        reportesData["totalSinGestionar"]
    ];

    var labels = [
        "Derivaciones",
        "Desembolsos",
        "Clientes Asignados",
        "Clientes Tipificados",
        "Clientes No Tipificados"
    ];

    var totalGeneral = series.reduce((acc, val) => acc + val, 0);

    var options = {
        series: series,
        chart: {
            type: 'pie',
            height: 350
        },
        labels: labels,
        colors: ["#00E396", "#FF4560", "#008FFB", "#FEB019", "#775DD0"],
        legend: {
            position: 'bottom'
        },
        tooltip: {
            intersect: false,
            y: {
                formatter: function (value, { dataPointIndex }) {
                    return `${labels[dataPointIndex]}: ${value} (Total: ${totalGeneral})`;
                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-general-asesor"), options);
    chart.render();
}
