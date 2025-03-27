document.addEventListener("DOMContentLoaded", function () {
    var targetNode = document.getElementById("div-derivaciones-asesor");

    var observer = new MutationObserver(function (mutationsList) {
        for (var mutation of mutationsList) {
            if (mutation.attributeName === "style") {
                var displayValue = window.getComputedStyle(targetNode).display;
                if (displayValue === "block") {
                    console.log("El div ahora es visible. Iniciando gráfico...");
                    cargarDerivacionesAsesorFecha();
                    cargarGestionInforme();
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

    var chart = new ApexCharts(document.querySelector("#div-reporte-derivacion-asesor-fecha"), options);
    chart.render();
}

function cargarGestionInforme() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var GestionInformes = reportesData["tipificacionesGestion"];

    var tipificacion = [];
    var descripcionMap = {}; // Para la leyenda y tooltip
    var contador = [];
    var contadorReal = [];

    GestionInformes.forEach(item => {
        let id = item["idTipificacion"];
        let count = item["contadorTipificaciones"];

        tipificacion.push(id);
        descripcionMap[id] = item["descripcionTipificaciones"]; // Guardar descripciones
        contadorReal.push(count);
        contador.push(count > 200 ? 200 : count); // Limitar a 200
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
            categories: tipificacion, // Mostrar solo los IDs
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
        colors: ['#008FFB'], // Azul para las barras
        title: {
            text: 'Gestión de Informes',
            align: 'center'
        },
        legend: {
            position: 'top'
        },
        tooltip: {
            intersect: false, // Permite que el tooltip aparezca en el eje X
            y: {
                formatter: function (value, { dataPointIndex }) {
                    let id = tipificacion[dataPointIndex]; // Obtener ID
                    let descripcion = descripcionMap[id] || "Desconocido"; // Obtener descripción
                    let countReal = contadorReal[dataPointIndex]; // Obtener número real
                    return `${id} ${descripcion}\nTotal: ${countReal}`; // Formato corregido
                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-gestion-asesor"), options);
    chart.render();

    // Crear botón de leyenda con ícono
    var legendContainer = document.getElementById('leyenda-tipificaciones');
    legendContainer.innerHTML = ""; // Limpiar antes de añadir

    let legendButton = document.createElement('button');
    legendButton.className = "btn btn-info";
    legendButton.style.marginBottom = "10px";

    // Agregar ícono de Bootstrap
    let icon = document.createElement("i");
    icon.className = "bi bi-list"; // Icono de lista
    legendButton.appendChild(icon);

    // Contenedor flotante de la leyenda
    let legendPopup = document.createElement('div');
    legendPopup.className = "legend-popup";
    legendPopup.style.display = "none"; // Ocultar inicialmente
    legendPopup.style.position = "absolute";
    legendPopup.style.background = "white";
    legendPopup.style.border = "1px solid #ccc";
    legendPopup.style.boxShadow = "2px 2px 10px rgba(0, 0, 0, 0.2)";
    legendPopup.style.padding = "10px";
    legendPopup.style.borderRadius = "8px";
    legendPopup.style.top = "50px"; // Ajusta la posición según necesidad
    legendPopup.style.right = "20px"; // Ajusta la posición según necesidad
    legendPopup.style.zIndex = "1000";
    legendPopup.style.maxHeight = "200px";
    legendPopup.style.overflowY = "auto";

    let legendList = document.createElement('ul');
    legendList.style.listStyle = "none";
    legendList.style.padding = "0";
    legendList.style.margin = "0";

    for (let id in descripcionMap) {
        let li = document.createElement('li');
        li.innerHTML = `<strong>${id}:</strong> ${descripcionMap[id]}`;
        legendList.appendChild(li);
    }

    legendPopup.appendChild(legendList);
    document.body.appendChild(legendPopup); // Añadir al `body` para que flote correctamente

    // Mostrar/ocultar la leyenda emergente
    legendButton.onclick = function (event) {
        event.stopPropagation(); // Evita que el clic cierre inmediatamente la ventana
        legendPopup.style.display = (legendPopup.style.display === "none") ? "block" : "none";
    };

    // Cerrar la ventana emergente si se hace clic fuera
    document.addEventListener('click', function (event) {
        if (!legendPopup.contains(event.target) && event.target !== legendButton) {
            legendPopup.style.display = "none";
        }
    });

    legendContainer.appendChild(legendButton);
}