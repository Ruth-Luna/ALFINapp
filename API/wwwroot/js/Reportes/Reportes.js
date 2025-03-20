document.addEventListener('DOMContentLoaded', function () {
    createGraphicDerivacion();
    createGraphicTipificaciones();
    createGraphicDesembolsos();
    initGraphic();
});

function initGraphic(params) {
    const asesores = JSON.parse(document.getElementById('asesores-data').textContent);

    let totalClientes = 200;
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
        colors: ['#00ff00', '#ff0000'],
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

function createGraphicDerivacion(params) {
    
}

function createGraphicTipificaciones(params) {
    
}

function createGraphicDesembolsos(params) {
    
}

function cargarReporteAsesor() {
    
}