document.addEventListener('DOMContentLoaded', function () {
    initGraphic();
});

function initGraphic(params) {

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

function createGraphicDerivacion(params) {
    
}

function createGraphicTipificaciones(params) {
    
}

function createGraphicDesembolsos(params) {
    
}

function cargarReporteAsesor() {
    let numDerivados = 0;
    let numListaNegra = 0;
    let numPendientes = 0;
    let numRechazados = 0;
    let numDesembolsados = 0;

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
                        cargarReporteDerivador();
                    }
                }
            }
        },
        series: [{
            name: 'Derivados',
            
        }],
    };
}