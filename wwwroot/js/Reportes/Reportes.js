let reportesData;
document.addEventListener('DOMContentLoaded', function () {
    var reportesElement = document.getElementById("reportes-data");
    var roldiv = document.getElementById("rol-general");
    var idrol = roldiv.getAttribute("data");
    idrol = idrol === "null" ? null : parseInt(idrol);
    reportesData = JSON.parse(reportesElement.getAttribute("data-json"));
    console.log(idrol);
    if (idrol === 1 || idrol === 4 || idrol === 2) {
        cargarDerivacionesGenerales();
        cargarProgresoAsignacion();
        cargarProgreso();
        cargarTop5DerivacionesGenerales();
        cargarPiesContactabilidad();
    } else if (idrol === 3) {
        cargarDerivacionesGenerales();
        cargarProgresoAsignacion();
        cargarProgreso();
        cargarPiesContactabilidad();
    } else {
        Swal.fire({
            icon: 'warning',
            title: 'Error en permisos',
            text: 'No tienes permisos para ver los reportes.',
            confirmButtonText: 'Aceptar'
        });
    }
});



function cargarDerivacionesGenerales() {
    var derivaciones = document.getElementById('div-derivaciones');
    derivaciones.style.display = 'block';
    var lineaGestionVsDerivacion = reportesData["lineaGestionVsDerivacion"];

    // Función para formatear fechas a 'dd/MM/yy'
    function formatDate(date) {
        var d = new Date(date);
        var day = ('0' + d.getUTCDate()).slice(-2);
        var month = ('0' + (d.getUTCMonth() + 1)).slice(-2);
        var year = d.getUTCFullYear() % 100;
        return `${day}/${month}/${year}`;
    }

    // Función para parsear 'dd/MM/yy' a Date
    function parseDate(dateStr) {
        var parts = dateStr.split('/');
        var day = parseInt(parts[0], 10);
        var month = parseInt(parts[1], 10) - 1;
        var year = 2000 + parseInt(parts[2], 10);
        return new Date(year, month, day);
    }

    // Obtener la fecha actual
    var today = new Date();
    var currentDateStr = formatDate(today);

    // Obtener el mes y año actuales
    var year = today.getFullYear();
    var month = today.getMonth();

    // Obtener el número de días en el mes actual
    var daysInMonth = new Date(year, month + 1, 0).getDate();

    // Generar la lista de todos los días del mes en formato 'dd/MM/yy'
    var allDates = [];
    for (var day = 1; day <= daysInMonth; day++) {
        var date = new Date(year, month, day);
        allDates.push(formatDate(date));
    }

    // Crear mapas para la data de derivaciones y gestiones
    var derivacionMap = {};
    var gestionMap = {};
    lineaGestionVsDerivacion.forEach(item => {
        var formattedFecha = formatDate(item["fecha"]);
        derivacionMap[formattedFecha] = item["derivaciones"] || 0;
        gestionMap[formattedFecha] = item["gestiones"] || 0;
    });

    // Generar la data para todos los días, usando 0 si no hay data y null para días futuros
    var contadorDerivacion = allDates.map(date => {
        var dateObj = parseDate(date);
        if (dateObj <= today) {
            return derivacionMap[date] || 0;
        } else {
            return null;
        }
    });
    var contadorGestion = allDates.map(date => {
        var dateObj = parseDate(date);
        if (dateObj <= today) {
            return gestionMap[date] || 0;
        } else {
            return null;
        }
    });

    // Identificar los domingos y crear anotaciones
    var annotations = {
        xaxis: []
    };
    allDates.forEach(date => {
        var dateObj = parseDate(date);
        if (dateObj.getDay() === 0) { // 0 es domingo
            annotations.xaxis.push({
                x: date,
                borderColor: '#00C22A',
                opacity: 0.2,
                label: {
                    borderColor: '#00C22A',
                    style: {
                        color: '#00C22A',
                        background: '#00FF80',
                    },
                    text: 'No laborable'
                }
            });
        }
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
        colors: ["#FF1654", "#247BA0"],
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
            categories: allDates
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
            position: 'bottom',
            horizontalAlign: 'center'
        },
        annotations: annotations
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
    console.log(progresoGeneral);
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
                        return `${totalAsignaciones - totalGestionados} pendientes por gestionar`;
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

function cargarTop5DerivacionesGenerales() {
    var top5Der = reportesData["top5asesores"];
    var contador = [];
    var nombres = [];
    var dnis = [];

    top5Der.forEach(item => {
        contador.push(item["contador"]);
        nombres.push(item["nombres_completos"]);
        dnis.push(item["dni"]);
    });

    var divTop5 = document.getElementById('div-derivaciones-top-5');
    divTop5.style.display = 'block';

    var options = {
        chart: {
            type: "bar",
            height: 350
        },
        series: [
            {
                name: "Total Derivaciones",
                data: contador
            }
        ],
        xaxis: {
            categories: nombres,
            title: {
                text: "Asesores"
            }
        },
        yaxis: {
            title: {
                text: "Total Derivaciones"
            }
        },
        colors: ["#008FFB"],
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: "50%"
            }
        },
        dataLabels: {
            enabled: true
        },
        tooltip: {
            y: {
                formatter: function (val, index) {
                    return `${val} derivaciones (DNI: ${dnis[index.dataPointIndex]})`;
                }
            }
        },
        legend: {
            position: "top"
        }
    };
    
    var chart = new ApexCharts(document.querySelector("#chart-top-5-derivaciones"), options);
    chart.render();
}

function cargarPiesContactabilidad() {
    var pieContactabilidad = reportesData["pieContactabilidad"];
    var divPie = document.getElementById('div-pie-contactabilidad-reporte');
    var estado = [];
    var total = [];
    var porcentaje = [];

    pieContactabilidad.forEach(item => {
        estado.push(item.estado);
        total.push(item.total);
        porcentaje.push(item.porcentaje);
    });

    var options = {
        series: total,
        chart: {
            type: 'donut',
            height: 350
        },
        labels: estado,
        dataLabels: {
            enabled: true,
            formatter: function (val, opts) {
                return porcentaje[opts.seriesIndex].toFixed(2) + '%';
            }
        },
        legend: {
            position: 'bottom'
        },
        tooltip: {
            y: {
                formatter: function (val, opts) {
                    return `${val} contactos (${porcentaje[opts.seriesIndex].toFixed(2)}%)`;
                }
            }
        }
    };
    divPie.style.display = 'block';
    var chart = new ApexCharts(document.querySelector("#chart-contactabilidad-reporte"), options);
    chart.render();
}