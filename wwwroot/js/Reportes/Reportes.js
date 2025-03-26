let reportesData;
document.addEventListener('DOMContentLoaded', function () {
    var reportesElement = document.getElementById("reportes-data");
    reportesData = JSON.parse(reportesElement.getAttribute("data-json"));
    cargarDerivacionesGenerales();
    cargarProgreso();
});

function cargarDerivacionesGenerales() {
    var derivaciones = document.getElementById('div-derivaciones');
    derivaciones.style.display = 'block';
    var derivacionesFecha = reportesData["numDerivacionesXFecha"];
    
    var fechas = [];
    var contador = [];
    
    derivacionesFecha.forEach(item => {
        fechas.push(item["fecha"]);
        contador.push(item["contador"]);
    });

    var options = {
        chart: {
            height: 350,
            type: "line",
            stacked: false
        },
        dataLabels: {
            enabled: true, // Activamos los dataLabels
            style: {
                fontSize: '12px',
                colors: ["#FF1654"]
            },
            background: {
                enabled: true,
                foreColor: "#fff",
                borderRadius: 4,
                padding: 4
            }
        },
        colors: ["#FF1654"],
        series: [
            {
                name: "Derivaciones Por Fecha",
                data: contador
            }
        ],
        stroke: {
            width: [4]
        },
        plotOptions: {
            bar: {
                columnWidth: "20%"
            }
        },
        xaxis: {
            categories: fechas
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
    var totalDerivaciones = reportesData["totalDerivaciones"];
    var totalDesembolsadas = reportesData["totalDerivacionesDesembolsadas"];
    var porcentaje = Math.floor((totalDesembolsadas / totalDerivaciones) * 100);

    var options = {
        chart: {
            height: 280,
            type: "radialBar"
        },
        series: [porcentaje],
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
        labels: ["Progress"],
        tooltip: {
            enabled: true,
            y: {
                formatter: function (val) {
                    return `${val}% (${totalDesembolsadas} de ${totalDerivaciones})`;
                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#chart-progreso-general"), options);
    chart.render();
}
