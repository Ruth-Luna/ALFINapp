let reportesData;
document.addEventListener('DOMContentLoaded', function () {
    var reportesElement = document.getElementById("reportes-data");
    reportesData = JSON.parse(reportesElement.getAttribute("data-json"));
    cargarDerivacionesGenerales();
    cargarProgresoAsignacion();
    cargarProgreso();
});

function cargarDerivacionesGenerales() {
    var derivaciones = document.getElementById('div-derivaciones');
    derivaciones.style.display = 'block';
    var lineaGestionVsDerivacion = reportesData["lineaGestionVsDerivacion"];

    var fechas = [];
    var contadorGestion = [];
    var contadorDerivacion = [];

    lineaGestionVsDerivacion.forEach(item => {
        fechas.push(item["fecha"]);
        contadorGestion.push(item["gestiones"]);
        contadorDerivacion.push(item["derivaciones"]);
    });

    var options = {
        chart: {
            height: 350,
            type: "line",
            stacked: false
        },
        dataLabels: {
            enabled: true,
            style: {
                fontSize: '12px'
            },
            background: {
                enabled: true,
                foreColor: "#fff",
                borderRadius: 4,
                padding: 4
            }
        },
        colors: ["#FF1654", "#247BA0"], // Derivaciones - rojo, Gestiones - azul
        series: [
            {
                name: "Derivaciones por Fecha",
                data: contadorDerivacion
            },
            {
                name: "Gestiones por Fecha",
                data: contadorGestion
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
            categories: fechas
        },
        yaxis: [
            {
                seriesName: "Derivaciones por Fecha",
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
                    text: "Num. Derivaciones",
                    style: {
                        color: "#FF1654"
                    }
                }
            },
            {
                opposite: true,
                seriesName: "Gestiones por Fecha",
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
                    text: "Num. Gestiones",
                    style: {
                        color: "#247BA0"
                    }
                }
            }
        ],
        tooltip: {
            shared: true,
            intersect: false,
            x: {
                show: true
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
    var progresoGral = reportesData["progresoGeneral"];
    var totalDerivaciones = progresoGral["totaL_DERIVADOS"];
    var totalDesembolsadas = progresoGral["totaL_DESEMBOLSADOS"];
    var porcentaje = Math.floor((totalDesembolsadas / totalDerivaciones) * 100);

    var options = {
        chart: {
            type: "donut",
            height: 350 // Aumentamos el tamaño
        },
        series: [totalDesembolsadas, totalDerivaciones - totalDesembolsadas], // Parte desembolsada vs resto
        labels: ["Desembolsados", "Derivados"],
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
                        return `${totalDerivaciones - totalDesembolsadas} derivaciones no desembolsadas`;
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

function cargarProgresoAsignacion() {
    var progresoGeneral = reportesData["progresoGeneral"];
    var totalAsignaciones = progresoGeneral["totaL_ASIGNADOS"];
    var totalGestionados = progresoGeneral["totaL_GESTIONADOS"];
    var porcentaje = Math.floor((totalGestionados / totalAsignaciones) * 100);

    var options = {
        chart: {
            type: "donut",
            height: 350
        },
        series: [totalGestionados, totalAsignaciones - totalGestionados],
        labels: ["Gestionados", "Asignados"],
        colors: ["#008FFB", "#9e9e9e"],
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
                                return "Gestionados\nvs\nAsignaciones"; // Texto con saltos de línea
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
                        return `${totalGestionados} de ${totalAsignaciones} gestionados`;
                    } else {
                        return `${totalAsignaciones - totalGestionados} pendientes por asignar`;
                    }
                }
            }
        },
        legend: {
            position: "bottom"
        }
    };
    var chart = new ApexCharts(document.querySelector("#chart-progreso-total-y-tipificados"), options);
    chart.render();
}

