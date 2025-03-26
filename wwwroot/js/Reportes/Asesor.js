document.addEventListener("DOMContentLoaded", function () {
    var targetNode = document.getElementById("div-derivaciones-asesor");

    var observer = new MutationObserver(function (mutationsList) {
        for (var mutation of mutationsList) {
            if (mutation.attributeName === "style") {
                var displayValue = window.getComputedStyle(targetNode).display;
                if (displayValue === "block") {
                    console.log("El div ahora es visible. Iniciando gráfico...");
                    cargarDerivacionesAsesorFecha();
                    observer.disconnect();
                }
            }
        }
    });

    observer.observe(targetNode, { attributes: true, attributeFilter: ["style"] });
});


function cargarDerivacionesAsesorFecha() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    reportesData = JSON.parse(reportesElement.getAttribute("data-json"));
    var options = {
        series: [
            {
                name: 'Actual',
                data: [
                    {
                        x: '2011',
                        y: 1292,
                    },
                    {
                        x: '2012',
                        y: 4432,
                    },
                    {
                        x: '2013',
                        y: 5423,
                    },
                    {
                        x: '2014',
                        y: 6653,
                    },
                    {
                        x: '2015',
                        y: 8133,
                    },
                    {
                        x: '2016',
                        y: 7132,
                    },
                    {
                        x: '2017',
                        y: 7332,
                    },
                    {
                        x: '2018',
                        y: 6553
                    }
                ]
            }
        ],
        chart: {
            height: 350,
            type: 'bar'
        },
        plotOptions: {
            bar: {
                columnWidth: '60%'
            }
        },
        colors: ['#00E396'],
        dataLabels: {
            enabled: false
        },
        legend: {
            show: true,
            showForSingleSeries: true,
            customLegendItems: ['Actual', 'Expected'],
            markers: {
                fillColors: ['#00E396', '#775DD0']
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-derivacion-asesor"), options);
    chart.render();
}