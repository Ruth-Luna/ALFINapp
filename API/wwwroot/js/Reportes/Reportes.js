document.addEventListener('DOMContentLoaded', function () {
    initGraphicAsesor();
    initGraphicSupervisor();
    cargarParametrosGenerales();
    cargarReporteAsesor();
    cargarReporteSupervisor();
    cargarProgreso();
});

function initGraphicSupervisor(params) {
    let totalAsesoresSinClientes = 15;
    let totalAsesoresConClientes = 4;
    const options = {
        chart: {
            type: 'pie',
            height: 350,
            animations: {
                enabled: true,
                easing: 'easeinout',
                speed: 800,
                animateGradually: {
                    enabled: true,
                    delay: 150
                },
                dynamicAnimation: {
                    enabled: true,
                    speed: 350
                }
            },
            events: {
                dataPointSelection: function (event, chartContext, config) {
                    if (config.dataPointIndex === 0) {
                        cargarReporteSupervisor();
                    }
                }
            }
        },
        series: [totalAsesoresSinClientes, totalAsesoresConClientes],
        labels: ['Asesores Sin Tipificaciones', 'Asesores Con Tipificaciones'],
        colors: ['#394097', '#5699c2'],
        legend: {
            show: true,
            position: 'bottom'
        },
        dataLabels: {
            enabled: true
        },
        title: {
            text: 'Asesores por Supervisor'
        }
    };
    const chart = new ApexCharts(document.querySelector('#chart-supervisor'), options);
    chart.render();
}

function initGraphicAsesor(params) {
    let totalGestionados = 156;
    let totalNoGestionados = 44;
    const options = {
        chart: {
            type: 'pie',
            height: 350,
            animations: {
                enabled: true,
                easing: 'easeinout',
                speed: 800,
                animateGradually: {
                    enabled: true,
                    delay: 150
                },
                dynamicAnimation: {
                    enabled: true,
                    speed: 350
                }
            },
            events: {
                dataPointSelection: function (event, chartContext, config) {
                    if (config.dataPointIndex === 0) {
                        cargarReporteAsesor();
                    }
                }
            }
        },
        series: [totalGestionados, totalNoGestionados],
        labels: ['Gestionados', 'No Gestionados'],
        colors: ['#394097', '#5699c2'],
        legend: {
            show: true,
            position: 'bottom'
        },
        dataLabels: {
            enabled: true
        },
        title: {
            text: 'Clientes gestionados por asesor'
        }
    };
    const chart = new ApexCharts(document.querySelector('#chart-asesor'), options);
    chart.render();
}

function cargarReporteAsesor() {
    var reporte = document.getElementById('div-asesor-detalle');
    reporte.style.display = 'block';

    let numDerivados = 20;
    let numListaNegra = 20;
    let numPendientes = 200;
    let numRechazados = 10;
    let numDesembolsados = 5;

    const options = {
        chart: {
            type: 'bar',
            height: 350,
            animations: {
                enabled: true,
                easing: 'easeinout',
                speed: 800,
                animateGradually: {
                    enabled: true,
                    delay: 150
                },
                dynamicAnimation: {
                    enabled: true,
                    speed: 350
                }
            }
        },
        series: [{
            name: 'Clientes',
            data: [numDerivados, numListaNegra, numPendientes, numRechazados, numDesembolsados]
        }],
        xaxis: {
            categories: ['Derivados', 'Lista Negra', 'Pendientes', 'Rechazados', 'Desembolsados']
        },
        colors: ['#394097', '#5699c2', '#ff0000', '#ff0000', '#ff0000'],
        legend: {
            show: true,
            position: 'bottom'
        },
        title: {
            text: 'Clientes gestionados por asesor'
        }
    };
    const chart = new ApexCharts(document.querySelector('#chart-asesor-detalle'), options);
    chart.render();
}

function cargarReporteSupervisor() {
    var reporte = document.getElementById('div-supervisor-detalle');
    reporte.style.display = 'block';

    let asesor1 = 'MARIO VILCA';
    let asesor2 = 'ATENCIO JULIAN';
    let asesor3 = 'MARICIELO TATIANA';
    let totalUsuarios = [asesor1, asesor2, asesor3];
    let numDerivaciones = [10, 15, 20]; // Ejemplo de datos de derivaciones
    let numTipificaciones = [5, 10, 8]; // Ejemplo de datos de tipificaciones
    let totalClientes = [15, 25, 28]; // Total de clientes (suma de derivaciones y tipificaciones)

    const options = {
        chart: {
            type: 'bar',
            height: 350,
            animations: {
                enabled: true,
                easing: 'easeinout',
                speed: 800,
                animateGradually: {
                    enabled: true,
                    delay: 150
                },
                dynamicAnimation: {
                    enabled: true,
                    speed: 350
                }
            },
            stacked: false, // Disable stacking to show bars side by side
            horizontal: true
        },
        series: [
            {
                name: 'Derivaciones',
                data: numDerivaciones
            },
            {
                name: 'Tipificaciones',
                data: numTipificaciones
            },
            {
                name: 'Total de Clientes',
                data: totalClientes
            }
        ],
        xaxis: {
            categories: totalUsuarios
        },
        colors: ['#394097', '#5699c2', '#28a745'], // Added green color for total clients
        legend: {
            show: true,
            position: 'bottom'
        },
        title: {
            text: 'Gestion de Asesores'
        },
        plotOptions: {
            bar: {
                horizontal: true,
                barHeight: '70%', // Adjust bar height for better spacing
                dataLabels: {
                    position: 'top'
                }
            }
        }
    };
    const chart = new ApexCharts(document.querySelector('#chart-supervisor-detalle'), options);
    chart.render();
}

function cargarParametrosGenerales() {
    var options = {
        chart: {
            height: 350,
            type: "line",
            stacked: false
        },
        dataLabels: {
            enabled: false
        },
        colors: ["#FF1654", "#247BA0"],
        series: [
            {
                name: "Tipificaciones Por Fecha",
                data: [111, 220, 250, 150, 238, 281, 380, 460]
            },
            {
                name: "Derivaciones Por Fecha",
                data: [20, 29, 37, 36, 44, 41, 36, 44]
            }
        ],
        stroke: {
            width: [4, 4]
        },
        plotOptions: {
            bar: {
                columnWidth: "20%"
            }
        },
        xaxis: {
            categories: ['03/Mar', '04/Mar', '05/Mar', '06/Mar', '07/Mar', '08/Mar', '09/Mar', '10/Mar']
        },
        yaxis: [
            {
                axisTicks: {
                    show: true
                },
                axisBorder: {
                    show: true,
                    color: "#FF1654"
                },
                labels: {
                    style: {
                        colors: "#FF1654"
                    }
                },
                title: {
                    text: "Num Tipificaciones",
                    style: {
                        color: "#FF1654"
                    }
                }
            },
            {
                opposite: true,
                axisTicks: {
                    show: true
                },
                axisBorder: {
                    show: true,
                    color: "#247BA0"
                },
                labels: {
                    style: {
                        colors: "#247BA0"
                    }
                },
                title: {
                    text: "Num Derivaciones",
                    style: {
                        color: "#247BA0"
                    }
                }
            }
        ],
        tooltip: {
            shared: false,
            intersect: true,
            x: {
                show: false
            }
        },
        legend: {
            horizontalAlign: "left",
            offsetX: 40
        }
    };

    var chart = new ApexCharts(document.querySelector("#chart-derivaciones"), options);
    chart.render();
}

function cargarProgreso() {
    var options = {
        chart: {
            height: 280,
            type: "radialBar"
        },

        series: [67],

        plotOptions: {
            radialBar: {
                hollow: {
                    margin: 15,
                    size: "20%"
                },

                dataLabels: {
                    showOn: "always",
                    name: {
                        offsetY: -10,
                        show: true,
                        color: "#888",
                        fontSize: "13px"
                    },
                    value: {
                        color: "#111",
                        fontSize: "30px",
                        show: true
                    }
                }
            }
        },

        stroke: {
            lineCap: "round",
        },
        labels: ["Progress"]
    };

    var chart = new ApexCharts(document.querySelector("#chart-progreso-general"), options);

    chart.render();
}