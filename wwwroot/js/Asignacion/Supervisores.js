/* FUNCIONES PARA LEER Y MOSTRAR DATOS */
parsedDataGral = [];
function import_assignments_file (event) {
    const fileInput = event.target;
    const file = fileInput.files[0];
    if (!file) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Por favor, selecciona un archivo para importar.',
        });
        return;
    }
    const allowedExtensions = ['csv', 'txt'];
    const fileName = file.name;
    const fileExtension = fileName.split('.').pop().toLowerCase();
    if (!allowedExtensions.includes(fileExtension)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Formato de archivo no permitido. Por favor, sube un archivo CSV o TXT.',
        });
        return;
    } 
    const reader = new FileReader();

    reader.onload = function(e) {
        const content = e.target.result;
        var parsedData = [];
        parsedData = parseCSV(content);
        const loading = Swal.fire({
            title: 'Cargando datos...',
            text: 'Por favor, espera mientras se procesan los datos.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        load_visualization_data(parsedData);
        parsedDataGral = parsedData; // Guardar los datos globalmente
        loading.close();
        Swal.fire({
            icon: 'success',
            title: 'Archivo importado correctamente',
            text: 'Se han importado ' + parsedData.length + ' registros. Puedes proceder a asignar supervisores.',
            showConfirmButton: true,
            timer: 2000
        })
    }
    reader.onerror = function() {
        Swal.fire({
            icon: 'error',
            title: 'Error al leer el archivo',
            text: 'Por favor, verifica el archivo y vuelve a intentarlo.',
        });
    }
    reader.readAsText(file);
    fileInput.value = ''; // Limpiar el input para permitir la carga del mismo archivo nuevamente
    document.getElementById("imported-file-name").textContent = fileName;
    document.getElementById("imported-file-name").style.display = "block";
}

function parseCSV(text) {
    const lines = text.trim().split('\n');
    if (lines.length <= 1) return [];
    return lines.slice(1).map(line => line.split(',').map(cell => cell.trim()));
}

function load_visualization_data(data) {
    const tableContainer = document.getElementById("load-visualization-table");
    tableContainer.innerHTML = ""; // Limpiar antes de volver a renderizar

    new gridjs.Grid({
        columns: [
            "DNI CLIENTE",
            "DNI SUPERVISOR",
            "CELULAR 1",
            "CELULAR 2",
            "CELULAR 3",
            "CELULAR 4",
            "CELULAR 5",
            "D. BASE"
        ],
        data: data,
        sort: true,
        pagination: {
            limit: 10
        },
        search: true
    }).render(tableContainer);

    document.getElementById("load-visualization-total-input").value = "";
    document.getElementById("load-visualization-total-input").value = data.length;
}

async function cross_assignments() {
    if (!parsedDataGral || parsedDataGral.length === 0) {
        Swal.fire({
            icon: 'error',
            title: 'Error al cruzar asignaciones',
            text: 'No hay datos para cruzar. Por favor, importa un archivo primero.',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    const dataToSend = parsedDataGral.map(row => ({
        dni_cliente: row[0] ?? "",
        dni_supervisor: row[1] ?? "",
        telefono_1: row[2] === "NULL" ? "" : row[2],
        telefono_2: row[3] === "NULL" ? "" : row[3],
        telefono_3: row[4] === "NULL" ? "" : row[4],
        telefono_4: row[5] === "NULL" ? "" : row[5],
        telefono_5: row[6] === "NULL" ? "" : row[6],
        d_base: row[7] ?? ""
    }));

    const dataJson = JSON.stringify(dataToSend);
    console.log("Datos a cruzar:", dataJson);

    const baseUrl = window.location.origin;
    const url = baseUrl + "/Asignaciones/CrossAssignments";

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: dataJson
        });

        const result = await response.json();
        console.log("Respuesta del servidor:", result);

        if (result.IsSuccess) {
            Swal.fire({
                icon: 'success',
                title: 'Asignaciones cruzadas con éxito',
                text: result.Message,
                confirmButtonText: 'Aceptar'
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error en el servidor',
                text: result.Message,
                confirmButtonText: 'Aceptar'
            });
        }

    } catch (error) {
        console.error("Error en fetch:", error);
        Swal.fire({
            icon: 'error',
            title: 'Error al cruzar asignaciones',
            text: 'Ocurrió un error al intentar cruzar las asignaciones. Por favor, inténtalo de nuevo más tarde.',
            confirmButtonText: 'Aceptar'
        });
    }
}

// Función que simula la obtención de datos para la primera tabla
/*function fetchLoadVisualizationData() {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve([
                ["12345678", "87654321", "987654321", "912345678", "999888777", "", "", "BASE_A"],
                ["23456789", "98765432", "912345679", "", "998877665", "911223344", "", "BASE_B"],
                ["34567890", "76543210", "999000111", "988776655", "", "", "910101010", "BASE_C"],
                ["45678901", "65432109", "911112223", "", "", "933445566", "922334455", "BASE_D"],
                ["56789012", "54321098", "988877776", "977665544", "966554433", "", "", "BASE_E"],
                ["67890123", "43210987", "", "955443322", "944332211", "933221100", "", "BASE_F"],
                ["78901234", "32109876", "922110011", "911009988", "", "900887766", "900776655", "BASE_G"],
                ["89012345", "21098765", "955000111", "", "944111222", "", "933000444", "BASE_H"],
                ["90123456", "10987654", "999123456", "988654321", "977543210", "966432109", "955321098", "BASE_I"],
                ["01234567", "09876543", "910101011", "920202022", "", "930303033", "", "BASE_J"],
                ["11223344", "44332211", "945454545", "956565656", "967676767", "", "978787878", "BASE_K"],
                ["22334455", "55443322", "", "912121212", "", "923232323", "934343434", "BASE_L"]
            ]);
        }, 400);
    });
}

// Wrapper para la primera tabla
const loadVisualizationTableData = () => fetchLoadVisualizationData();

// Función que simula la obtención de datos para la segunda tabla
function fetchClientListData() {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve([
                ["12345678", "Ana Ramírez", "Campaña Primavera", "S/ 150", "Agencia Norte", "Tipo A", "Luis Pérez", "Lista 1", "2025-05-01"],
                ["23456789", "José García", "Campaña Verano", "S/ 200", "Agencia Sur", "Tipo B", "María López", "Lista 2", "2025-05-03"],
                ["34567890", "Lucía Martínez", "Campaña Otoño", "S/ 120", "Agencia Este", "Tipo A", "Carlos Ruiz", "Lista 3", "2025-04-25"],
                ["45678901", "Pedro Fernández", "Campaña Invierno", "S/ 180", "Agencia Oeste", "Tipo C", "Sofía Torres", "Lista 4", "2025-04-28"],
                ["56789012", "Mónica Santos", "Campaña Primavera", "S/ 160", "Agencia Norte", "Tipo B", "Ricardo Díaz", "Lista 1", "2025-05-02"],
                ["67890123", "Diego Vargas", "Campaña Verano", "S/ 210", "Agencia Sur", "Tipo C", "Patricia Herrera", "Lista 2", "2025-05-04"],
                ["78901234", "Carolina Gil", "Campaña Otoño", "S/ 130", "Agencia Este", "Tipo A", "Fernando Paredes", "Lista 3", "2025-04-26"],
                ["89012345", "Raúl Mendoza", "Campaña Invierno", "S/ 190", "Agencia Oeste", "Tipo B", "Valeria Rojas", "Lista 4", "2025-04-29"],
                ["90123456", "Patricia Ruiz", "Campaña Primavera", "S/ 170", "Agencia Norte", "Tipo C", "Eduardo Vega", "Lista 1", "2025-05-05"],
                ["01234567", "Andrés Soto", "Campaña Verano", "S/ 220", "Agencia Sur", "Tipo A", "Marina Ramos", "Lista 2", "2025-05-06"],
                ["11223344", "Beatriz Fuentes", "Campaña Otoño", "S/ 140", "Agencia Este", "Tipo B", "Jorge Lima", "Lista 3", "2025-04-27"],
                ["22334455", "Héctor Salas", "Campaña Invierno", "S/ 200", "Agencia Oeste", "Tipo C", "Natalia Bravo", "Lista 4", "2025-04-30"]
            ]);
        }, 400);
    });
}
*/
// Wrapper para la segunda tabla
const clientListTableData = () => fetchClientListData();


// Función que simula la obtención de datos para la tercera tabla
/*function fetchAssignmentsListData() {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve([
                ["12345678", "Luis Pérez", "Lista 1", "10", "2025-05-01"],
                ["23456789", "María López", "Lista 2", "15", "2025-05-03"],
                ["34567890", "Carlos Ruiz", "Lista 3", "12", "2025-04-25"],
                ["45678901", "Sofía Torres", "Lista 4", "18", "2025-04-28"],
                ["56789012", "Ricardo Díaz", "Lista 1", "11", "2025-05-02"],
                ["67890123", "Patricia Herrera", "Lista 2", "16", "2025-05-04"],
                ["78901234", "Fernando Paredes", "Lista 3", "13", "2025-04-26"],
                ["89012345", "Valeria Rojas", "Lista 4", "19", "2025-04-29"],
                ["90123456", "Eduardo Vega", "Lista 1", "12", "2025-05-05"],
                ["01234567", "Marina Ramos", "Lista 2", "17", "2025-05-06"],
                ["11223344", "Jorge Lima", "Lista 3", "14", "2025-04-27"],
                ["22334455", "Natalia Bravo", "Lista 4", "20", "2025-04-30"]
            ]);
        }, 400);
    });
}

// Wrapper para la tercera tabla
const assignmentsListTableData = () => fetchAssignmentsListData();
*/
/*
// Inicialización de las tablas Grid.js
document.addEventListener("DOMContentLoaded", function () {
    //Primera tabla: “loadVisualizationTableData”
    new gridjs.Grid({
        columns: [
            "DNI CLIENTE",
            "DNI SUPERVISOR",
            "CELULAR 1",
            "CELULAR 2",
            "CELULAR 3",
            "CELULAR 4",
            "CELULAR 5",
            "D. BASE"
        ],
        data: loadVisualizationTableData,
        sort: true,
        pagination: {
            limit: 10
        }
    }).render(document.getElementById("load-visualization-table"));

    // Segunda tabla: “clientListTableData”
    new gridjs.Grid({
        columns: [
            "DNI CLIENTE",
            "NOM. CLIENTE",
            "CAMPAÑA",
            "OFERTA MAX.",
            "AGENCIA",
            "TIPO BASE",
            "NOMBRE SUP",
            "NOMBRE LISTA",
            "D. BASE"
        ],
        data: clientListTableData,
        sort: true,
        pagination: {
            limit: 10
        }
    }).render(document.getElementById("client-list-table"));

    // tabla del modal
    new gridjs.Grid({
        columns: [
            "DNI SUP",
            "NOM. SUP",
            "NOMBRE LISTA",
            "CANTIDAD",
            "FECHA ASIGNACIÓN"
        ],
        data: assignmentsListTableData,
        sort: true,
        pagination: {
            limit: 7
        }
    }).render(document.getElementById("assignments-list-table"));
});
*/
/* EVENTOS */
document.getElementById("assign-button").addEventListener("click", function () {
    Swal.fire({
        icon: 'success',
        title: 'Asignado correctamente',
        showConfirmButton: false,
        timer: 1500
    });
});

