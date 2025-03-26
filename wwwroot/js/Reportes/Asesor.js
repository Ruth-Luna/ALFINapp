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
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var derivacionesFecha = reportesData["derivacionesFecha"];

    var fechas = [];
    var contador = [];
    var contadorEsperado = [];

    derivacionesFecha.forEach(item => {
        fechas.push(item["fecha"]);
        contador.push(item["contador"]);
        contadorEsperado.push(10); // Línea constante de referencia
    });

    var options = {
        series: [
            {
                name: 'Derivaciones',
                type: 'bar',
                data: contador
            },
            {
                name: 'Esperado',
                type: 'line',
                data: contadorEsperado
            }
        ],
        chart: {
            height: 350,
            type: 'line'  // Se usa 'line' para que soporte tanto barras como líneas
        },
        plotOptions: {
            bar: { columnWidth: '60%' }
        },
        stroke: {
            width: [0, 4] // Grosor de la barra (0) y de la línea (4)
        },
        colors: ['#00E396', '#FF4560'], // Verde para las barras, rojo para la línea
        xaxis: { categories: fechas },
        dataLabels: { enabled: false },
        legend: { position: 'top' }
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-derivacion-asesor"), options);
    chart.render();
}
