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
            type: "donut",
            height: 350 // Aumentamos el tamaño
        },
        series: [totalDesembolsadas, totalDerivaciones - totalDesembolsadas], // Parte desembolsada vs resto
        labels: ["Desembolsadas", "Pendientes"],
        colors: ["#008FFB", "#9e9e9e"], // Azul para desembolsadas, gris para fondo
        plotOptions: {
            pie: {
                donut: {
                    size: "65%", // Hace el círculo más grande
                    labels: {
                        show: true,
                        name: {
                            show: true,
                            fontSize: "16px",
                            color: "#888",
                            offsetY: -10,
                            formatter: function () {
                                return "Desembolsos\nvs\nDerivaciones"; // Texto con saltos de línea
                            }
                        },
                        value: {
                            show: true,
                            fontSize: "30px",
                            fontWeight: "bold",
                            color: "#111",
                            formatter: function (val) {
                                return `${porcentaje}%`;
                            }
                        }
                    }
                }
            }
        },
        stroke: {
            show: true,
            width: 2,
            colors: ["#fff"]
        },
        tooltip: {
            enabled: true,
            y: {
                formatter: function (val, { seriesIndex }) {
                    if (seriesIndex === 0) {
                        return `${totalDesembolsadas} de ${totalDerivaciones} desembolsadas`;
                    } else {
                        return `${totalDerivaciones - totalDesembolsadas} pendientes`;
                    }
                }
            }
        },
        legend: {
            position: "bottom"
        }
    };
    var chart = new ApexCharts(document.querySelector("#chart-progreso-general"), options);
    chart.render();
}
