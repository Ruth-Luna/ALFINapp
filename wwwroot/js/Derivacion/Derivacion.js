let currentPage = 1;
let rowsPerPage = 20;
let rows = document.querySelectorAll("tbody tr");
let totalPages = Math.ceil(rows.length / rowsPerPage);

document.addEventListener("DOMContentLoaded", function () {
    var informationTabla = document.getElementById("total-actual");
    var tables = {
        "tablaDerivacionesGestion": document.getElementById("tablaDerivacionesGestion"),
        "tablaGeneralGestion": document.getElementById("tablaGeneralGestion"),
        "tablaGeneralSistema": document.getElementById("tablaGeneralSistema") // La tabla que aparece primero
    };
    function updateTableInfo() {
        let activeTable = Object.values(tables).find(table => isTableVisible(table));
        if (!activeTable) {
            informationTabla.textContent = `Mostrando 0 registros`;
            return;
        }
        let tbody = activeTable.querySelector("tbody");
        let rows = tbody ? tbody.getElementsByTagName("tr") : [];
        let visibleRows = Array.from(rows).filter(row => row.offsetParent !== null); // Detectar filas visibles
        let rowCount = visibleRows.length;
        if (activeTable.id === "tablaGeneralSistema") {
            rowCount = Math.max(rowCount - 1, 0); // Restar 1 solo en este caso
        }
        informationTabla.textContent = `Mostrando ${rowCount} registros`;
    }
    function isTableVisible(table) {
        if (!table) return false;
        let style = window.getComputedStyle(table);
        return style.display !== "none" && style.visibility !== "hidden" && style.opacity !== "0";
    }
    // Observador para detectar cambios en las tablas
    var observer = new MutationObserver(updateTableInfo);
    Object.values(tables).forEach(table => {
        if (table) {
            observer.observe(table, { childList: true, subtree: true, attributes: true });
        }
    });
    // Ejecutar la función una vez al inicio
    setTimeout(updateTableInfo, 10000);
});


document.addEventListener("DOMContentLoaded", function () {
    activatePagination(0);
});

function cargarDerivacionesXAsesorSistema(DniAsesor) {

    const tablaGeneralSistema = document.getElementById("tablaGeneralSistema");
    const tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion");
    const tablaGeneralGestion = document.getElementById("tablaGeneralGestion");

    if (!tablaGeneralSistema) return;

    const tabla = tablaGeneralSistema.querySelector("table");
    if (!tabla) return;

    const filas = Array.from(tabla.getElementsByTagName("tr")).slice(2); // Excluye encabezados

    if (DniAsesor === "") {
        filas.forEach(fila => (fila.style.display = "")); // Muestra todas las filas
        tablaGeneralSistema.style.display = "block";
        tablaDerivacionesGestion.style.display = "none";
        tablaDerivacionesGestion.loadedfield = "false";
        tablaGeneralGestion.style.display = "none";
        activatePagination(0);
        return;
    }

    let tieneCoincidencias = false;

    filas.forEach(fila => {
        const columnaDni = fila.cells[4]; // Columna 5 (DNI Asesor, índice 4 basado en 0)
        if (!columnaDni) return;

        const dniFila = columnaDni.textContent.trim();
        const coincide = dniFila === DniAsesor;

        fila.style.display = coincide ? "" : "none";
        tieneCoincidencias ||= coincide;
    });

    // Si no hay coincidencias, podrías ocultar la tabla o mostrar un mensaje
    if (!tieneCoincidencias) {
        console.warn("No se encontraron registros para el DNI:", DniAsesor);
    }
}

function cargarDerivacionesSistemaSupervisor(idAsesor) {

    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    asesorDni = document.getElementById(idAsesor).value.toString();
    if (asesorDni === "") {
        tablaGeneralSistema.style = "display: block;"

        tablaGeneralGestion.style = "display: none;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    } else {
        tablaGeneralSistema.style = "display: none;"
        tablaGeneralGestion.style = "display: none;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    }
}


function sortTableDerivaciones(idTabla, numCol, type) {
    const table = document.getElementById(idTabla);
    if (!table) return;

    const rows = Array.from(table.rows).slice(2); // Ya verificaste que es correcto

    const isAscending = table.dataset.sortOrder !== 'asc';
    table.dataset.sortOrder = isAscending ? 'asc' : 'desc';

    rows.sort((a, b) => {
        const getCellValue = (row) => {
            const cell = row.cells[numCol];
            return cell ? cell.textContent.trim() : '';
        };

        let aText = getCellValue(a);
        let bText = getCellValue(b);

        if (type === 'number') {
            return isAscending
                ? (parseFloat(aText) || 0) - (parseFloat(bText) || 0)
                : (parseFloat(bText) || 0) - (parseFloat(aText) || 0);
        }

        if (type === 'date') {
            const parseDate = (dateStr) => {
                if (!dateStr || dateStr.trim() === '') return new Date(0);

                const regex = /^(\d{1,2})\/(\d{1,2})\/(\d{4})\s(\d{1,2}):(\d{1,2}):(\d{1,2})$/;
                const match = dateStr.match(regex);

                if (!match) return new Date(0);

                const [, day, month, year, hours, minutes, seconds] = match.map(Number);
                return new Date(year, month - 1, day, hours, minutes, seconds);
            };

            const aDate = parseDate(aText);
            const bDate = parseDate(bText);
            return isAscending ? aDate - bDate : bDate - aDate;
        }

        return isAscending ? aText.localeCompare(bText) : bText.localeCompare(aText);
    });

    rows.forEach(row => table.tBodies[0].appendChild(row));
}

function cargarTipoFiltro(filtro) {
    const filtrosTabla = document.getElementById("filtrosTabla");
    const filtroDni = document.getElementById("filtroDNI");
    const filtroFecha = document.getElementById("filtroFecha");
    const tablaSistema = document.getElementById("tablaDerivacionesSistema");
    const tablaGestion = document.getElementById("tablaDerivacionesGestion");

    // Reset input values
    if (filtroDni.querySelector('input')) {
        filtroDni.querySelector('input').value = '';
    }
    if (filtroFecha.querySelector('input')) {
        filtroFecha.querySelector('input').value = '';
    }

    // Reset table filters
    if (tablaSistema) {
        filtrarTabla("tablaDerivacionesSistema", "", 0, "text");
    }
    if (tablaGestion) {
        filtrarTabla("tablaDerivacionesGestion", "", 0, "text");
    }

    if (filtro === "dni") {
        filtrosTabla.style = "display: block;";
        filtroDni.style = "display: block;";
        filtroFecha.style = "display: none;";
    } else if (filtro === "fecha") {
        filtrosTabla.style = "display: block;";
        filtroDni.style = "display: none;";
        filtroFecha.style = "display: block;";
    } else {
        filtrosTabla.style = "display: none;";
        filtroDni.style = "display: none;";
        filtroFecha.style = "display: none;";
    }
}

let activeDnis = [];

let filtrosActivos = {
    dni: "",
    fechaExacta: "",
    intervaloInicio: "",
    intervaloFinal: "",
};

function filtrarTablaGeneral(idTabla, colIndex) {
    const table = document.getElementById(idTabla);
    if (!table) return;
    const rows = Array.from(table.getElementsByTagName("tr")).slice(2);

    rows.forEach(row => {
        const cell = row.cells[colIndex];
        if (!cell) return;

        let cellValue = cell.textContent.trim();
        let showRow = true;

        // Filtro por DNI
        if (filtrosActivos.dni) {
            if (!cellValue.toLowerCase().includes(filtrosActivos.dni.toLowerCase())) {
                showRow = false;
            }
        }

        // Filtro por fecha exacta
        if (filtrosActivos.fechaExacta) {
            const match = cellValue.match(/(\d{1,2})\/(\d{1,2})\/(\d{4})/);
            if (match) {
                const formattedDate = `${match[3]}-${match[2].padStart(2, '0')}-${match[1].padStart(2, '0')}`;
                if (formattedDate !== filtrosActivos.fechaExacta) {
                    showRow = false;
                }
            } else {
                showRow = false;
            }
        }

        // Filtro por intervalo de fechas
        const fechaActual = new Date(parsearFechaISO(cellValue));
        if (filtrosActivos.intervaloInicio) {
            const inicio = new Date(filtrosActivos.intervaloInicio);
            if (fechaActual < inicio) {
                showRow = false;
            }
        }
        if (filtrosActivos.intervaloFinal) {
            const fin = new Date(filtrosActivos.intervaloFinal);
            if (fechaActual > fin) {
                showRow = false;
            }
        }

        // Si no hay ningún filtro, mostrar todo y resetear paginación
        const hayFiltros = filtrosActivos.dni || filtrosActivos.fechaExacta || filtrosActivos.intervaloInicio || filtrosActivos.intervaloFinal;
        if (!hayFiltros) {
            showRow = true;
            activatePagination(0);
        }

        row.style.display = showRow ? "" : "none";
    });
}

function parsearFecha(fechaStr) {
    const match = fechaStr.match(/(\d{1,2})\/(\d{1,2})\/(\d{4})/);
    if (!match) return "";
    return `${match[3]}-${match[2].padStart(2, '0')}-${match[1].padStart(2, '0')}`;
}

function parsearFechaISO(fechaStr) {
    const matchCompleta = fechaStr.match(/(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{1,2}):(\d{2}):(\d{2})/);
    const matchSoloFecha = fechaStr.match(/(\d{1,2})\/(\d{1,2})\/(\d{4})/);

    if (matchCompleta) {
        const [_, d, m, y, h, mi, s] = matchCompleta;
        return new Date(`${y}-${m.padStart(2, '0')}-${d.padStart(2, '0')}T${h.padStart(2, '0')}:${mi}:${s}`);
    } else if (matchSoloFecha) {
        const [_, d, m, y] = matchSoloFecha;
        return new Date(`${y}-${m.padStart(2, '0')}-${d.padStart(2, '0')}`);
    } else {
        return new Date(0); // fecha inválida
    }
}


function filtrarTabla(idTabla, value, colIndex, type) {
    switch (type) {
        case "dni":
            filtrosActivos.dni = value;
            break;
        case "date":
            filtrosActivos.fechaExacta = value;
            break;
        case "intervalo-inicio":
            filtrosActivos.intervaloInicio = value;
            break;
        case "intervalo-final":
            filtrosActivos.intervaloFinal = value;
            break;
    }

    filtrarTablaGeneral(idTabla, colIndex);
}


function activatePagination(direction) {
    if (direction === -1 && currentPage > 1) {
        currentPage--;
    } else if (direction === 1 && currentPage < totalPages) {
        currentPage++;
    }
    let start = (currentPage - 1) * rowsPerPage;
    let end = start + rowsPerPage;
    rows.forEach((row, index) => {
        // Ensure the first row (header) is always visible
        if (index === 0) {
            row.style.display = "";
        } else {
            row.style.display = (index > start && index <= end) ? "" : "none";
        }
    });
    document.getElementById("page-indicator").innerText = `Página ${currentPage}`;
}

function cargar_asesores_del_supervisor(idSupJson, idAseJson, asesorSelect, idSupervisor) {
    const supJson = document.getElementById(idSupJson);
    if (!supJson) {
        console.error("Element with ID", idSupJson, "not found.");
        return;
    }
    asesoresSelect = document.getElementById(asesorSelect);
    if (!asesoresSelect) {
        console.error("Element with ID", asesorSelect, "not found.");
        return;
    }
    asesoresSelect.innerHTML = `
            <input type="text" class="custom-search" id="busqueda-asesor-derivacion"
                onkeyup="filter_option_refac('busqueda-asesor-derivacion', 'selected-options-select-asesores-derivacion')"
                placeholder="Buscar...">
            <div class="custom-option" id="option-non-select-derivacion" data-value="non-select"
                onclick="select_option_refac(
                    'select-option-asesor-derivacion',
                    'selected-option-asesor-derivacion',
                    'selected-options-select-asesores-derivacion',
                    '',
                    'Seleccione un Asesor',
                    () => cargarDerivacionesXAsesorSistema('')
                )">
                SIN FILTRO
            </div>
        `;
    asesoresSelect.style = "display: block;"

    if (idSupervisor === "" || idSupervisor === null || idSupervisor === 0) {
        try {
            const asesoresJson = document.getElementById(idAseJson);
            var asesoresData = JSON.parse(asesoresJson.getAttribute("data-json"));
            asesoresData.forEach(asesor => {
                asesoresSelect.innerHTML += `
                    <div class="custom-option" id="option-${asesor["dni"]}-select-derivacion" data-value="${asesor.dni}"
                        onclick="select_option_refac(
                            'select-option-asesor-derivacion'
                            , 'selected-option-asesor-derivacion'
                            , 'selected-options-select-asesores-derivacion'
                            , '${asesor["dni"]}'
                            , '${asesor["dni"]} - ${asesor["nombresCompletos"]}'
                            , () => cargarDerivacionesXAsesorSistema('${asesor["dni"]}')
                        )">
                        ${asesor["dni"]} - ${asesor["nombresCompletos"]}
                    </div>
                `;
            });
            return;
        } catch (error) {
            console.error("Error parsing supervisor JSON:", error);
            Swal.fire({
                title: 'Error',
                text: 'Ocurrió un error al cargar los asesores',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    }
    var supervisorData = JSON.parse(supJson.getAttribute("data-json"));
    try {
        var supervisorInfo;
        supervisorData.forEach(supervisor => {
            if (supervisor["idUsuario"] === idSupervisor) {
                supervisorInfo = supervisor;
            }
        });
        if (!supervisorInfo) {
            Swal.fire({
                title: 'Error',
                text: 'No se encontró información del supervisor',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
        const asesoresData = supervisorInfo["vendedores"];
        var asesores = [];
        asesoresData.forEach(asesor => {
            asesores.push({
                idusuario: asesor["idUsuario"],
                dni: asesor["dni"],
                nombres: asesor["nombresCompletos"],
            });
        });

        asesores.forEach(asesor => {
            asesoresSelect.innerHTML += `
                <div class="custom-option" id="option-${asesor.dni}-select-derivacion" data-value="${asesor.dni}"
                    onclick="select_option_refac(
                        'select-option-asesor-derivacion'
                        , 'selected-option-asesor-derivacion'
                        , 'selected-options-select-asesores-derivacion'
                        , '${asesor.dni}'
                        , '${asesor.dni} - ${asesor.nombres}'
                        , () => cargarDerivacionesXAsesorSistema('${asesor.dni}')
                    )">
                    ${asesor.dni} - ${asesor.nombres}
                </div>
            `;
        });
    } catch (error) {
        console.error("Error parsing supervisor JSON:", error);
        Swal.fire({
            title: 'Error',
            text: 'Ocurrió un error al cargar los asesores',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
    }
}