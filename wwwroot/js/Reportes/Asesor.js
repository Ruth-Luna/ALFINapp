function cargarDerivacionesAsesorFecha() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var derivacionesFecha = reportesData["derivacionesFecha"];

    var fechas = [];
    var contador = [];

    derivacionesFecha.forEach(item => {
        fechas.push(item["fecha"]);
        contador.push(item["contador"]);
    });

    var options = {
        series: [
            {
                name: 'Derivaciones',
                data: contador
            }
        ],
        chart: {
            height: 350,
            type: 'area'  
        },
        stroke: {
            curve: 'smooth'
        },
        colors: ['#00E396'], 
        xaxis: { 
            categories: fechas,
            title: {
                text: 'Fechas'
            }
        },
        yaxis: {
            title: {
                text: 'Número de Derivaciones'
            }
        },
        dataLabels: { enabled: false },
        legend: { position: 'top' },
        title: {
            text: 'Derivaciones por Fecha',
            align: 'center'
        },
        tooltip: {
            x: {
                format: 'dd/MM/yyyy'
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-derivacion-asesor-fecha"), options);
    chart.render();
}

function cargarGestionInforme() {
    var reportesAsesor = document.getElementById('reportes-asesor');
    var reportesData = JSON.parse(reportesAsesor.getAttribute("data-json"));
    var GestionInformes = reportesData["tipificacionesGestion"];

    var tipificacion = [];
    var descripcionMap = {}; 
    var contador = [];
    var contadorReal = [];

    GestionInformes.forEach(item => {
        let id = item["idTipificacion"];
        let count = item["contadorTipificaciones"];

        tipificacion.push(id);
        descripcionMap[id] = item["descripcionTipificaciones"]; 
        contadorReal.push(count);
        contador.push(count > 200 ? 200 : count); 
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
            categories: tipificacion, 
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
        colors: ['#008FFB'], 
        title: {
            text: 'Gestión de Informes',
            align: 'center'
        },
        legend: {
            position: 'top'
        },
        tooltip: {
            intersect: false,
            y: {
                formatter: function (value, { dataPointIndex }) {
                    let id = tipificacion[dataPointIndex];
                    let descripcion = descripcionMap[id] || "Desconocido";
                    let countReal = contadorReal[dataPointIndex];
                    return `${descripcion}\nTotal: ${countReal}`;
                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#div-reporte-gestion-asesor"), options);
    chart.render();

    var legendContainer = document.getElementById('leyenda-tipificaciones');
    legendContainer.innerHTML = ""; 
    
    let legendButton = document.createElement('button');
    legendButton.className = "btn btn-info";
    legendButton.style.marginBottom = "10px";
    
    let icon = document.createElement("i");
    icon.className = "bi bi-list-ul";
    legendButton.appendChild(icon);
    
    let legendPopup = document.createElement('div');
    legendPopup.className = "legend-popup";
    legendPopup.style.display = "none";
    legendPopup.style.position = "absolute"; 
    legendPopup.style.background = "white";
    legendPopup.style.border = "1px solid #ccc";
    legendPopup.style.boxShadow = "2px 2px 10px rgba(0, 0, 0, 0.2)";
    legendPopup.style.padding = "10px";
    legendPopup.style.borderRadius = "8px";
    legendPopup.style.zIndex = "1000";
    legendPopup.style.maxHeight = "200px";
    legendPopup.style.overflowY = "auto";
    legendPopup.style.minWidth = "150px"; 

    let legendList = document.createElement('ul');
    legendList.style.listStyle = "none";
    legendList.style.padding = "2px";
    legendList.style.margin = "0";

    for (let id in descripcionMap) {
        let li = document.createElement('li');
        li.innerHTML = `<strong>${id}:</strong> ${descripcionMap[id]}`;
        legendList.appendChild(li);
    }

    legendPopup.appendChild(legendList);
    document.body.appendChild(legendPopup); 

    legendButton.onclick = function (event) {
        event.stopPropagation(); 

        let rect = legendButton.getBoundingClientRect();

        legendPopup.style.top = `${rect.bottom + window.scrollY + 5}px`; 
        legendPopup.style.left = `${rect.left + window.scrollX}px`;

        legendPopup.style.display = (legendPopup.style.display === "none") ? "block" : "none";
    };

    document.addEventListener('click', function (event) {
        if (!legendPopup.contains(event.target) && event.target !== legendButton) {
            legendPopup.style.display = "none";
        }
    });

    legendContainer.appendChild(legendButton);
}