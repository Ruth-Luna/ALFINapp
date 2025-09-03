$(document).ready(function () {
    // Logica segun el rol
    idRol = parseInt($('#idRol').val());

    if (idRol === 1 || idRol === 4) {
        ListarSupervisores();
        ListarAsesores();
    } else if (idRol === 2) {
        // SUPERVISOR
        ListarAsesores();

        // Ocultar filtros supervisor
        $('.filtro-supervisor-col').hide();
    } else {
        // ASESOR
        // Ocultar filtros de asesor y supervisor
        $('.filtro-asesor-col, .filtro-supervisor-col').hide();
    }

    // Logica comun a todos los roles
    ListarAgencias();

    // Inicializar el app derivaciones
    if (App.derivaciones && App.derivaciones.init) {
        App.derivaciones.init();
    }
    // Inicializar data
    getAllDerivaciones();

    // Inicializar el app reagendamientos
    if (App.reagendamientos && App.reagendamientos.init) {
        App.reagendamientos.init();
    }
    // Inicializar data
    getAllReagendamientos();

    // Controlar eventos de selects de filtro
    $('#supervisorDerivaciones, #asesorDerivaciones, #agenciaDerivaciones, #fechaDerivacion, #fechaVisitaDerivacion').on('change', function () {
        if ($(this).attr('id') === 'supervisorDerivaciones') {
            // Pasar el objeto
            actualizarSelects($(this));
        }
        getAllDerivaciones();
    });

    $('#supervisorReagendamientos, #asesorReagendamientos, #agenciaReagendamientos, #fechaReagendamientos, #fechaVisitaReagendamientos').on('change', function () {
        if ($(this).attr('id') === 'supervisorReagendamientos') {
            // Pasar el objeto
            actualizarSelects($(this));
        }
        getAllReagendamientos();
    });

    // Evento para el campo DNI
    $('#dniClienteDerivaciones').on('input', function () {
        getAllDerivaciones();
    });

    $('#dniClienteReagendamientos').on('input', function () {
        getAllReagendamientos();
    });

    function actualizarSelects($this) {
        // Mostrar solo los asesores del supervisor seleccionado
        var idSupervisor = $this.val();

        const selectorMap = {
            'supervisorDerivaciones': '#asesorDerivaciones',
            'supervisorReagendamientos': '#asesorReagendamientos'
        };
        const $asesorSelect = selectorMap[$this.attr('id')];

        if (!idSupervisor) {
            $(`${$asesorSelect} option`).show();
        } else {
            // Ocultar asesores que no pertenecen al supervisor seleccionado
            $(`${$asesorSelect} option`).not('[value=""]').each(function () {
                var $option = $(this);
                var dataIdAsesor = $option.data('id-supervisor-asignado');
                if (dataIdAsesor != idSupervisor) {
                    $option.hide();
                } else {
                    $option.show();
                }
            });
            // Poner la opciones de todos los asesores si el asesor seleccionado actual no es del supervisor
            if ($('#asesorDerivaciones option:selected').data('id-supervisor-asignado') != idSupervisor) {
                $('#asesorDerivaciones').val('');
            }
        }
    };

    $('#btnDescargarDerivaciones').on('click', function () {
        Swal.fire({
            title: '¿Estás seguro?',
            text: "Se descargará el archivo Excel con el resumen de derivaciones.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, descargar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                DescargarResumenExcelDerivaciones();
            }
        });
    });

    $('#btnDescargarReagendamientos').on('click', function () {
        Swal.fire({
            title: '¿Estás seguro?',
            text: "Se descargará el archivo Excel con el resumen de reagendamientos.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, descargar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                DescargarResumenExcelReagendamientos();
            }
        });
    });

    $('#btnDescargarHistoricos').on('click', function () {
        Swal.fire({
            title: '¿Estás seguro?',
            text: "Se descargará el archivo Excel con el resumen de históricos.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, descargar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                DescargarResumenExcelHistoricos();
            }
        });
    });
});

function ListarAsesores() {
    $.ajax({
        url: '/Operaciones/ObtenerAsesoresRelacionados',
        method: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data && data.success) {
                // Llenar el combo de asesores
                let $asesorSelect = $('.filtro-asesor');
                $.each(data.data, function (index, asesor) {
                    $asesorSelect.append(`<option data-id-supervisor-asignado="${asesor.idusuariosup}" value="${asesor.idUsuario}">${asesor.nombresCompletos}</option>`);
                });
            } else {
                console.error('Error al cargar los asesores relacionados.');
            }
        },
        error: function () {
            console.error('Error al realizar la solicitud AJAX.');
        }
    });
}

// Funcion para cargar los supervisores realcionados
function ListarSupervisores() {
    $.ajax({
        url: '/Operaciones/ObtenerSupervisoresRelacionados',
        method: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data && data.success) {
                // Llenar el combo de supervisores
                let $supervisorSelect = $('.filtro-supervisor');
                $.each(data.data, function (index, supervisor) {
                    $supervisorSelect.append(`<option value="${supervisor.idUsuario}">${supervisor.nombresCompletos}</option>`);
                });
            } else {
                console.error('Error al cargar los supervisores relacionados.');
            }
        },
        error: function () {
            console.error('Error al realizar la solicitud AJAX.');
        }
    });
}

// Función para listar las agencias relacionadas
function ListarAgencias() {
    $.ajax({
        url: '/Operaciones/ListarAgencias',
        method: 'GET',
        contentType: 'application/json',
        success: function (data) {
            if (data && data.success) {
                // Llenar el combo de agencias
                let $agenciaSelect = $('.filtro-agencia');
                $.each(data.data, function (index, agencia) {
                    $agenciaSelect.append(`<option value="${agencia.ceco}">${agencia.agencia}</option>`);
                });
            } else {
                console.error('Error al cargar las agencias relacionadas.');
            }
        },
        error: function () {
            console.error('Error al realizar la solicitud AJAX.');
        }
    });
}

// Función para actualizar la data de la tabla usando jQuery AJAX
function getAllDerivaciones() {
    $.ajax({
        url: '/Operaciones/GetAllDerivaciones',
        method: 'GET',
        data:{
            idAsesor : $('#asesorDerivaciones').val() || null,
            idSupervisor : $('#supervisorDerivaciones').val() || null,
            agencia : $('#agenciaDerivaciones').val() || null,
            fecha_derivacion : $('#fechaDerivacion').val() || null,
            fecha_visita : $('#fechaVisitaDerivacion').val() || null,
            dni : $('#dniClienteDerivaciones').val() || null
        },
        contentType: 'application/json',
        success: function (data) {
            if (data && data.success) {
                if (App.derivaciones && App.derivaciones.updateTableData) {
                    App.derivaciones.updateTableData(data.data || []);
                    // Actualizar contador
                    $('#totalDelMesDerivaciones').html(data.data.length);
                }
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Atención',
                    text: (data && data.message) || 'No se encontraron derivaciones.'
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudieron cargar las derivaciones. Por favor, inténtelo de nuevo más tarde.'
            });
        }
    });
}

// Función para actualizar la data de la tabla usando jQuery AJAX
function getAllReagendamientos() {
    $.ajax({
        url: '/Operaciones/GetAllReagendamientos',
        method: 'GET',
        contentType: 'application/json',
        data:{
            idAsesor : $('#asesorReagendamientos').val() || null,
            idSupervisor : $('#supervisorReagendamientos').val() || null,
            fecha_reagendamiento : $('#fechaReagendamientos').val() || null,
            fecha_visita : $('#fechaVisitaReagendamientos').val() || null,
            agencia : $('#agenciaReagendamientos').val() || null,
            dni : $('#dniClienteReagendamientos').val() || null
        },
        success: function (data) {
            if (data && data.success) {
                if (App.reagendamientos && App.reagendamientos.updateTableData) {
                    App.reagendamientos.updateTableData(data.data || []);
                }
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Atención',
                    text: (data && data.message) || 'No se encontraron reagendamientos.'
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudieron cargar los reagendamientos. Por favor, inténtelo de nuevo más tarde.'
            });
        }
    });
}

// Función para obtener el histórico de reagendamientos
async function getHistorico(idDerivacion) {
    
    var response = [];

    await $.ajax({
        url: '/Operaciones/GetHistoricoReagendamientos',
        type: 'POST',
        data: JSON.stringify([idDerivacion]),
        contentType: 'application/json',
        dataType: 'json',
        success: function(data) {
            if (data.success === true) {
                response = data.data;
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: data.message || 'Ocurrió un error al obtener el histórico.'
                });
            }
        },
        error: function(xhr, status, error) {
            console.error('Error fetching historico:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Ocurrió un error al obtener el histórico.'
            });
        }
    });

    return response;
}

//----------------CODIGO DE  REAGENDACION  -------------///

// Función para cargar la información de la reagendación
function cargarReagendacion(data) {
    $('#reagendacion-de-derivacion-content').removeClass('d-none');
    $('#dni-reagendamiento').val(data.dniCliente);
    $('#oferta-reagendamiento').val(data.ofertaMax != undefined ? data.ofertaMax : data.oferta);
    $('#nombre-reagendamiento').val(data.nombreCliente);
    $('#agencia-reagendamiento').val(data.nombreAgencia != undefined ? data.nombreAgencia : data.agencia);
    $('#telefono-reagendamiento').val(data.telefonoCliente != undefined ? data.telefonoCliente : data.telefono);
    $('#fecha-reagendamiento-previo').val(formatDateTime(data.fechaVisita, 'dd/mm/yyyy'));
    // $('#fecha-reagendamiento-nueva').val(data.fechaVisita);
}

// Función para enviar la reagendación
async function enviarReagendacion(nuevaFechaVisita, idDerivacion) {
    Swal.fire({
        title: 'Reagendar cita',
        text: 'Reagendar la cita del cliente. Esto volverá a enviar el formulario y los correos al banco.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, reagendar',
        cancelButtonText: 'No, cancelar'
    }).then(async (result) => {
        if (!result.isConfirmed) return;

        let fechaVisitaInput;
        let fechaVisita;
        try {
            fechaVisitaInput = document.getElementById(nuevaFechaVisita).value;
            fechaVisita = new Date(fechaVisitaInput).toISOString();
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error al obtener la fecha',
                text: 'Por favor, asegúrate de que la fecha esté en el formato correcto.',
                confirmButtonText: 'Aceptar'
            });
            return;
        }

        if (!fechaVisita || isNaN(new Date(fechaVisita))) {
            Swal.fire({
                icon: 'error',
                title: 'Fecha inválida',
                text: 'Por favor, ingresa una fecha válida.',
                confirmButtonText: 'Aceptar'
            });
            return;
        }

        if (files.length === 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Sin evidencias',
                text: 'No se han adjuntado archivos. No puede continuar',
                confirmButtonText: 'Aceptar'
            })
            return;
        }

        const loadingSwal = Swal.fire({
            title: 'Enviando...',
            text: 'Por favor, espere mientras se procesa el reagendamiento.',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });
        const baseUrl = window.location.origin;
        

        // Crear FormData para enviar archivos
        // var formdata = new FormData();
        // formdata.append("files", fileInput.files[0], "GerenteZonalController.cs");
        // formdata.append("dnicliente", "73393133");

        // var requestOptions = {
        //     method: 'POST',
        //     body: formdata,
        //     redirect: 'follow'
        // };

        // fetch("http://localhost:5051/Download/UploadImage", requestOptions)
        //     .then(response => response.text())
        //     .then(result => console.log(result))
        //     .catch(error => console.log('error', error));
        // Ejemplo de respuesta esperada del servidor
        // {"url":"http://localhost:5051/temp-images/3213f8ee-d7f5-4491-806c-57f6cb0691db.png"}
        const downloadURL = `${baseUrl}/Download/UploadImage`;

        var urls_string = [];
        for (const file of files) {
            try {
                const formdata = new FormData();
                formdata.append("files", file, file.name);

                const requestOptions = {
                    method: 'POST',
                    body: formdata,
                    redirect: 'follow'
                };

                const response1 = await fetch(downloadURL, requestOptions);
                if (response1.ok) {
                    const result1 = await response1.json();
                    urls_string.push(result1.url);
                } else {
                    console.error('Error al subir el archivo:', response1.statusText);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error al subir archivos',
                        text: `No se pudo subir el archivo ${file.name}. Por favor, intenta de nuevo.`,
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                }
            } catch (error) {
                console.error('Error al subir archivo:', file.name, error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error al subir archivos',
                    text: `No se pudo subir el archivo ${file.name}. Por favor, intenta de nuevo.`,
                    confirmButtonText: 'Aceptar'
                });
            }
        }

        const dto = {
            FechaReagendamiento: fechaVisita,
            IdDerivacion: idDerivacion,
            urlEvidencias: urls_string
        };

        const url = `${baseUrl}/Reagendamiento/Reagendar`;
        
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(dto)
            });

            const result = await response.json();
            Swal.close();

            if (!response.ok || result.success === false) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Error al reagendar la cita',
                    text: `${result.message}` || 'Ocurrió un error desconocido',
                    confirmButtonText: 'Aceptar'
                }).then(() => {
                    if (result.success == true) {
                        // Cerrar el modal
                        $('#modalEvidenciasDerivaciones').modal('hide');
                        // Actualizar las tablas
                        getAllDerivaciones();
                        getAllReagendamientos();
                    }

                });
                return;
            }

            Swal.fire({
                icon: 'success',
                title: 'Cita reagendada',
                text: `${result.message}.` || 'La cita ha sido reagendada con éxito.',
                confirmButtonText: 'Aceptar'
            }).then(() => {
                if (result.success == true) {
                    // Only close the modal if the user confirms
                    $('#modalEvidenciasDerivaciones').modal('hide');
                    // Recargar tablas
                    getAllDerivaciones();
                    getAllReagendamientos();
                }

            });
            return;
        } catch (error) {
            Swal.close();
            console.error('Error:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error al reagendar la cita',
                text: error.message || 'Ocurrió un error desconocido',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

//-----------------FIN REAGENDACION ---------------///

//------------------FIN EVIDENCIAS ---------------///

function enviarEvidencia(data) {
    activeId = data.idDerivacion; // Asegúrate de que 'idDerivacion' es el campo correcto
    console.log(data);
    console.log("ID de derivación activo:", activeId);
}

function modal_id_derivacion_to_be_uploaded(id) {
    activeId = id;
    fileInput.value = ''; // Limpiar el input file
    files = []; // Limpiar la lista de archivos
    updateFileList(); // Actualizar la lista visualmente
}

$('#drop-area').on('click', function() {
    fileInput.click();
});

$('#evidencia-derivacion-id-input').on('click', function(event) {
    event.stopPropagation();
});

$('#modalEvidenciasDerivaciones').on('paste', function(e) {
    const items = e.originalEvent.clipboardData.items;
    for (let item of items) {
        if (item.kind === 'file') {
            const file = item.getAsFile();
            files.push(file);
            updateFileInput();
            updateFileList();
            break;
        }
    }
});

// Drag and Drop
$('#drop-area').on('dragover', function(e) {
    e.preventDefault();
    $(this).addClass('bg-primary text-white');
});

$('#drop-area').on('dragleave', function() {
    $(this).removeClass('bg-primary text-white');
});

$('#drop-area').on('drop', function(e) {
    e.preventDefault();
    $(this).removeClass('bg-primary text-white');
    for (let file of e.originalEvent.dataTransfer.files) {
        files.push(file);
    }
    updateFileInput();
    updateFileList();
});

// Cambio en input file
$('#evidencia-derivacion-id-input').on('change', function() {
    for (let file of this.files) {
        files.push(file);
    }
    updateFileInput();
    updateFileList();
});

// Actualizar visualmente la lista de archivos
function updateFileList(type = 'evidencia') {
    let fileListActive;
    if (type === 'reagendamiento') {
        fileListActive = $('#file-list-reagendamiento');
    } else {
        fileListActive = $('#file-list');
    }
    fileListActive.empty();
    const filePreview = $('#file-preview');
    filePreview.toggleClass('d-none', files.length === 0);

    files.forEach((file, index) => {
        const listItem = $('<li>', {
            class: 'list-group-item d-flex justify-content-between align-items-center'
        });

        const fileInfo = $('<div>', {
            class: 'd-flex align-items-center'
        });

        const fileName = $('<span>').text(file.name);
        fileInfo.append(fileName);

        if (file.type.startsWith('image/')) {
            const img = $('<img>', {
                class: 'img-thumbnail ms-3',
                css: {
                    maxHeight: '50px',
                    maxWidth: '50px'
                }
            });

            const reader = new FileReader();
            reader.onload = function(e) {
                img.attr('src', e.target.result);
            };
            reader.readAsDataURL(file);

            fileInfo.append(img);
        }

        const deleteBtn = $('<button>', {
            class: 'btn btn-sm btn-danger',
            html: '&times;',
            title: 'Eliminar archivo',
            click: function() {
                files.splice(index, 1);
                updateFileInput();
                updateFileList();
            }
        });

        listItem.append(fileInfo, deleteBtn);
        fileListActive.append(listItem);
    });
}

// Actualizar el input[type=file] para que contenga los archivos en `files[]`
async function enviarEvidencia(data) {
    activeId = data.idDerivacion; // Asegúrate de que 'idDerivacion' es el campo correcto
    console.log(data);
    console.log("ID de derivación activo:", activeId);
}

async function modal_id_derivacion_to_be_uploaded(id) {
    activeId = id;
    fileInput.value = ''; // Limpiar el input file
    files = []; // Limpiar la lista de archivos
    updateFileList(); // Actualizar la lista visualmente
}

dropArea.addEventListener('click', () => {
    fileInput.click()
});

fileInput.addEventListener('click', (event) => {
    event.stopPropagation();
});

modalGeneral.addEventListener('paste', (e) => {
    const items = e.clipboardData.items;
    for (let item of items) {
        if (item.kind === 'file') {
            const file = item.getAsFile();
            files.push(file);
            updateFileInput();
            updateFileList();
            break;
        }
    }
});

// Drag and Drop
dropArea.addEventListener('dragover', (e) => {
    e.preventDefault();
    dropArea.classList.add('bg-primary', 'text-white');
});

dropArea.addEventListener('dragleave', () => {
    dropArea.classList.remove('bg-primary', 'text-white');
});

dropArea.addEventListener('drop', (e) => {
    e.preventDefault();
    dropArea.classList.remove('bg-primary', 'text-white');
    for (let file of e.dataTransfer.files) {
        files.push(file);
    }
    updateFileInput();
    updateFileList();
});

// Cambio en input file
fileInput.addEventListener('change', () => {
    for (let file of fileInput.files) {
        files.push(file);
    }
    updateFileInput();
    updateFileList();
});

// Actualizar visualmente la lista de archivos
function updateFileList(type = 'evidencia') {
    let fileListActive;
    if (type === 'reagendamiento') {
        fileListActive = document.getElementById('file-list-reagendamiento');
    } else {
        fileListActive = document.getElementById('file-list');
    }
    fileListActive.innerHTML = '';
    const filePreview = document.getElementById('file-preview');
    filePreview.classList.toggle('d-none', files.length === 0);

    files.forEach((file, index) => {
        const listItem = document.createElement('li');
        listItem.className = 'list-group-item d-flex justify-content-between align-items-center';

        const fileInfo = document.createElement('div');
        fileInfo.className = 'd-flex align-items-center';

        const fileName = document.createElement('span');
        fileName.textContent = file.name;

        fileInfo.appendChild(fileName);

        if (file.type.startsWith('image/')) {
            const img = document.createElement('img');
            img.className = 'img-thumbnail ms-3';
            img.style.maxHeight = '50px';
            img.style.maxWidth = '50px';

            const reader = new FileReader();
            reader.onload = ((imgElement) => {
                return (e) => {
                    imgElement.src = e.target.result;
                };
            })(img); // <- cerramos la función con `img` como argumento
            reader.readAsDataURL(file);

            fileInfo.appendChild(img);
        }

        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'btn btn-sm btn-danger';
        deleteBtn.innerHTML = '&times;';
        deleteBtn.title = 'Eliminar archivo';
        deleteBtn.onclick = () => {
            files.splice(index, 1);
            updateFileInput();
            updateFileList();
        };

        listItem.appendChild(fileInfo);
        listItem.appendChild(deleteBtn);
        fileListActive.appendChild(listItem);
    });
}

// Actualizar el input[type=file] para que contenga los archivos en `files[]`
function updateFileInput() {
    const dt = new DataTransfer();
    files.forEach(f => dt.items.add(f));
    fileInput.files = dt.files;
}

async function submit_evidencia_derivacion(id_derivacion_send) {
    if (files.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'No hay archivos',
            text: 'Por favor, arrastra o selecciona archivos para subir.',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    baseUrl = window.location.origin;
    const downloadURL = `${baseUrl}/Download/UploadImage`;

    url_strings = [];

    for (const file of files) {
        try {
            const formData = new FormData();
            formData.append('files', file, file.name);

            const requestOptions = {
                method: 'POST',
                body: formData,
                redirect: 'follow'
            };

            const response1 = await fetch(downloadURL, requestOptions);
            if (response1.ok) {
                const result1 = await response1.json();
                url_strings.push(result1.url);
            } else {
                console.error('Error al subir el archivo:', response1.statusText);
                Swal.fire({
                    icon: 'error',
                    title: 'Error al subir archivos',
                    text: `No se pudo subir el archivo ${file.name}. Por favor, intenta de nuevo.`,
                    confirmButtonText: 'Aceptar'
                });
                return;
            }
        } catch (error) {
            console.error('Error al subir el archivo:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error al subir archivos',
                text: `No se pudo subir el archivo ${file.name}. Por favor, intenta de nuevo.`,
                confirmButtonText: 'Aceptar'
            });
        }
    }
    
    const loading = Swal.fire({
        title: 'Subiendo archivos...',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    const url = `${baseUrl}/Derivacion/UploadEvidencia`;
    const dto = {
        idDerivacion: id_derivacion_send,
        urlEvidencias: url_strings
    };

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(dto),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        Swal.close();

        if (!response.ok) {
            Swal.fire({
                icon: 'error',
                title: 'Error al subir archivos',
                text: 'Por favor, intenta de nuevo más tarde.',
                confirmButtonText: 'Aceptar'
            });
            return;
        }

        const result = await response.json();
        if (result.success) {
            Swal.fire({
                icon: 'success',
                title: 'Archivos subidos',
                text: 'Los archivos se han subido correctamente.',
                confirmButtonText: 'Aceptar'
            }).then(() => {
                // Only close the modal if the user confirms
                if (result.success == true) {
                    $('#modalEvidenciasDerivaciones').modal('hide');
                    // Actualizar la tabla de derivaciones
                    getAllDerivaciones();
                    getAllReagendamientos();
                }
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: result.message || 'Ocurrió un error al subir los archivos.',
                confirmButtonText: 'Aceptar'
            });
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error de conexión',
            text: 'No se pudo conectar al servidor. Por favor, verifica tu conexión a Internet.',
            confirmButtonText: 'Aceptar'
        });
    }
}

async function archivoAHex(file) {
    const buffer = await file.arrayBuffer();
    return '0x' + [...new Uint8Array(buffer)]
        .map(b => b.toString(16).padStart(2, '0'))
        .join('');
}

function cargarEvidencia(data) {
    const reagendacionContent = document.getElementById('reagendacion-de-derivacion-content');
    reagendacionContent.classList.add('d-none');
}
//----------------FIN CARGA DE EVIDENCIA -----------///

////------------------APPS DE LAS TABLAS -----------///
var App = App || {};

App.derivaciones = (() => {
    let gridApi;
    let listaDerivaciones = [];

    let derivacionesTableColumns = [
        {
            headerName: "Estado Derivación",
            field: "estadoDerivacion",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const container = document.createElement('div');
                container.className = 'd-inline-flex gap-2';

                // Badge para Derivacion_status (D)
                const badgeD = document.createElement('div');
                badgeD.className = `af-badge ${params.data.fueProcesado ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeD.innerHTML = `
                    <i class="${params.data.fueProcesado ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>D</span>
                `;

                // Badge para Correo_status (C)
                const badgeC = document.createElement('div');
                badgeC.className = `af-badge ${params.data.fueEnviadoEmail ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeC.innerHTML = `
                    <i class="${params.data.fueEnviadoEmail ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>C</span>
                `;

                // Badge para Correo_status (F)
                const badgeF = document.createElement('div');
                badgeF.className = `af-badge ${params.data.fueProcesado ? 'af-badge-bg-success' : 'af-badge-bg-warning'}`;
                badgeF.innerHTML = `
                    <i class="${params.data.fueProcesado ? 'ri-checkbox-circle-fill' : 'ri-indeterminate-circle-fill'}"></i>
                    <span>F</span>
                `;

                container.appendChild(badgeD);
                container.appendChild(badgeC);
                container.appendChild(badgeF);

                return container;
            },
            width: 150
        },
        { 
            headerName: "DNI cliente", 
            width: 120,
            cellRenderer: (params) => {
                const $container = $('<div>', { class: 'd-flex gap-2' });

                if (params.data.fueDesembolsado == true) {
                    $container.append($('<i>', {
                        class: 'bi-cash-stack text-success'
                    }));
                }
                $container.append($('<div>').text(params.data.dniCliente));
                
                return $container[0];
            }
        },
        { headerName: "Cliente", field: "nombreCliente", width: 150 },
        { headerName: "Teléfono", field: "telefonoCliente", width: 90 },
        //{ headerName: "DNI asesor", field: "dniAsesor", width: 90 },
        { headerName: "Nombre asesor", field: "nombreAsesor", width: 150 },
        {
            headerName: "Oferta",
            field: "ofertaMax",
            valueFormatter: params => {
                if (params.value === 0) return 'No aplica';
                return `S/. ${Number(params.value).toLocaleString('es-PE')}`;
            },
            width: 80
        },
        { headerName: "Agencia", field: "nombreAgencia", width: 100 },
        {
            headerName: "Fecha derivación",
            field: "fechaDerivacion",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm'),
            width: 120
        },
        {
            headerName: "Fecha visita",
            field: "fechaVisita",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy'),
            width: 100
        },
        {
            headerName: "Estado evidencia",
            field: "estadoEvidencia",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const $container = $('<div>', { class: 'd-inline-flex gap-1' });

                // Badge para evidencia (E)
                const $badgeE = $('<div>', {
                    class: `af-badge ${params.value === "Enviado" ? 'af-badge-bg-success' : 'af-badge-bg-danger'}`,
                    html: `
                        <i class="${params.value === "Enviado" ? 'ri-checkbox-circle-fill' : 'ri-close-circle-fill'}"></i>
                        <span>E</span>
                    `,
                    title: params.value === "Enviado" ? 'Evidencia enviada' : 'Evidencia no enviada'
                });

                $container.append($badgeE);
                return $container[0];
            },
            comparator: (valueA, valueB) => (valueA === valueB ? 0 : valueA === 'No enviado' ? -1 : 1),
            width: 60
        },
        {
            headerName: "Fecha evidencia",
            field: "fechaEvidencia",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm'),
            width: 120
        },
        {
            headerName: "Acciones",
            field: "acciones",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';

                let btnReagendamiento = document.createElement('button');
                var mostrarBtnReagendamiento = true;
                let btnEnviarEvidencia = document.createElement('button');
                var mostrarBtnEnviarEvidencia = true;
                if (!params.data.puedeSerReagendado) {
                    btnReagendamiento.className = 'btn btn-sm btn-secondary disabled';
                    btnReagendamiento.title = 'Reagendamiento';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';

                    btnEnviarEvidencia.className = 'btn btn-sm btn-info';
                    btnEnviarEvidencia.title = 'Enviar evidencia';
                    btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>';

                    btnEnviarEvidencia.addEventListener('click', () => {
                        const modalElement = document.getElementById('modalEvidenciasDerivaciones');
                        if (modalElement) {
                            const modalTitle = modalElement.querySelector('#labelModalEvidenciasDerivaciones');
                            if (modalTitle) {
                                modalTitle.textContent = `Evidencia de derivacion para: ${params.data.nombreCliente}`;
                            }
                            const modalButton = modalElement.querySelector('#enviar-evidencia-o-reagendacion');
                            if (modalButton) {
                                modalButton.onclick = () => submit_evidencia_derivacion(params.data.idDerivacion);
                            }
                            const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

                            cargarEvidencia(params.data);
                            modal_id_derivacion_to_be_uploaded(params.data.idDerivacion);

                            modal.show();
                        }
                    });
                } else {
                    btnReagendamiento.className = 'btn btn-sm btn-primary';
                    btnReagendamiento.title = 'Reagendamiento';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';

                    btnReagendamiento.addEventListener('click', () => {
                        const modalElement = document.getElementById('modalEvidenciasDerivaciones');
                        if (modalElement) {
                            const modalTitle = modalElement.querySelector('#labelModalEvidenciasDerivaciones');
                            if (modalTitle) {
                                modalTitle.textContent = `Reagendamiento de cita para: ${params.data.nombreCliente}`;
                            }

                            const modalButton = modalElement.querySelector('#enviar-evidencia-o-reagendacion');
                            if (modalButton) {
                                modalButton.onclick = () => enviarReagendacion('fecha-reagendamiento-nueva', params.data.idDerivacion);
                            }
                            const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

                            cargarReagendacion(params.data);
                            modal_id_derivacion_to_be_uploaded(params.data.idDerivacion);

                            modal.show();
                        }
                    });

                    mostrarBtnReagendamiento = true;

                    btnEnviarEvidencia.className = 'btn btn-sm btn-secondary disabled';
                    btnEnviarEvidencia.title = 'Enviar evidencia';
                    btnEnviarEvidencia.innerHTML = '<i class="ri-file-add-line"></i>';

                    mostrarBtnEnviarEvidencia = false;
                }

                if (mostrarBtnReagendamiento) {
                    container.appendChild(btnReagendamiento);
                }
                if (mostrarBtnEnviarEvidencia) {
                    container.appendChild(btnEnviarEvidencia);
                }

                return container;
            },
            width: 130,
            sortable: false,
            resizable: false
        }
    ];

    const derivacionesGridOptions = {
        columnDefs: derivacionesTableColumns,
        rowData: listaDerivaciones,
        pagination: true,
        paginationPageSize: 20,
        enableBrowserTooltips: true,
        defaultColDef: { sortable: true, resizable: true, minWidth: 50, flex: 1 },
        copyHeadersToClipboard: true,
        suppressClipboardPaste: true,
        enableCellTextSelection: true,
        initialState: { sort: { sortModel: [{ colId: 'estadoEvidencia', sort: 'asc' }] } },
        getRowClass: (params) => {
            if (params.data.puedeSerReagendado === true) {
                return ['ag-row-reagendable'];
            }
            return [];
        },
        onGridReady: (params) => {
            gridApi = params.api;
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        }
    };

    function createTable(rol) {
        if (rol === 1 || rol === 4) {
            return;
        } else if (rol === 2) {
            for (let i = derivacionesTableColumns.length - 1; i >= 0; i--) {
                if (derivacionesTableColumns[i].field === 'docSupervisor') {
                    derivacionesTableColumns.splice(i, 1);
                }
            }
        } else if (rol === 3) {
            for (let i = derivacionesTableColumns.length - 1; i >= 0; i--) {
                if (derivacionesTableColumns[i].field === 'nombreAsesor' || derivacionesTableColumns[i].field === 'docSupervisor') {
                    derivacionesTableColumns.splice(i, 1);
                }
            }
        }
    }

    function init() {
        const gridDiv = document.querySelector('#gridDerivaciones');
        if (gridDiv) {
            // Obtener el rol del elemento #idRol
            const idRolElement = document.getElementById('idRol');
            const rol = idRolElement ? parseInt(idRolElement.value) || 0 : 0;
            
            // Modificar columnas según el rol
            createTable(rol);
            
            // Inicializar tabla vacía
            derivacionesGridOptions.rowData = [];
            gridApi = agGrid.createGrid(gridDiv, derivacionesGridOptions);

        }
    }

    function updateTableData(data) {
        listaDerivaciones = data || [];
        if (gridApi) {
            gridApi.setGridOption('rowData', listaDerivaciones);
        }
    }

    function exportXLSX() {
        if (!gridApi) return;

        const rowData = [];
        gridApi.forEachNodeAfterFilterAndSort(node => rowData.push(node.data));

        if (rowData.length === 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Atención',
                text: 'No hay datos para exportar.'
            });
            return;
        }

        const worksheet = XLSX.utils.json_to_sheet(rowData);

        const colWidths = Object.keys(rowData[0]).map(key => ({
            wch: Math.max(
                key.length,
                ...rowData.map(r => (r[key] ? r[key].toString().length : 0))
            )
        }));
        worksheet['!cols'] = colWidths;

        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Derivaciones");

        XLSX.writeFile(workbook, "Derivaciones.xlsx");
    }

    return {
        init,
        updateTableData,
        exportXLSX,
        gridApi: () => gridApi
    };
})();

App.reagendamientos = (() => {
    let gridApi;
    let listaReagendamientos = [];

    // Definición de las columnas para la tabla de Reagendamientos.
    const reagendamientosTableColumns = [
        {
            headerName: "Histórico", 
            field: "historico",
            width: 200,
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                const container = document.createElement('div');
                container.className = 'd-flex gap-2';
                
                let btnHistorico = document.createElement('button');
                btnHistorico.className = 'btn btn-sm btn-primary';
                btnHistorico.title = 'Ver Histórico';
                btnHistorico.innerHTML = '<i class="ri-history-line"></i>';
                btnHistorico.addEventListener('click', async () => {
                    const modalElement = document.getElementById('modalHistoricoReagendamiento');
                    if (modalElement) {
                        const modal = new bootstrap.Modal(modalElement);
                        modal.show();
                        var historicoData = await getHistorico(params.data.idDerivacion);
                        App.historico.init(
                            historicoData
                        );
                    } else {
                        console.error('Modal con ID #modalHistoricoReagendamiento no encontrado.');
                    }
                });
                
                let btnReagendamiento = document.createElement('button');
                if (params.data.puedeSerReagendado === true) {
                    btnReagendamiento.className = 'btn btn-sm btn-primary';
                    btnReagendamiento.title = 'Reagendamiento';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';

                    btnReagendamiento.addEventListener('click', () => {
                        const modalElement = document.getElementById('modalEvidenciasDerivaciones');
                        if (modalElement) {
                            const modalTitle = modalElement.querySelector('#labelModalEvidenciasDerivaciones');
                            if (modalTitle) {
                                modalTitle.textContent = `Reagendamiento de cita para: ${params.data.nombreCliente}`;
                            }

                            const modalButton = modalElement.querySelector('#enviar-evidencia-o-reagendacion');
                            if (modalButton) {
                                modalButton.onclick = () => enviarReagendacion('fecha-reagendamiento-nueva', params.data.idDerivacion);
                            }
                            const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

                            cargarReagendacion(params.data);
                            modal_id_derivacion_to_be_uploaded(params.data.idDerivacion);

                            modal.show();
                        }
                    });
                } else {
                    btnReagendamiento.className = 'btn btn-sm btn-secondary disabled';
                    btnReagendamiento.title = 'No puede ser reagendado';
                    btnReagendamiento.innerHTML = '<i class="ri-file-list-3-line"></i>';
                }

                container.appendChild(btnHistorico);
                container.appendChild(btnReagendamiento);
                return container;
            },
            sortable: false, 
            resizable: false, 
            width: 100
        },
        {
            headerName: "Estado", 
            field: "estadoReagendamiento",
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                // Crear el contenedor principal
                const $container = $('<div>', { class: 'd-flex align-items-center gap-2' });

                // Badge de estado
                let badgeClass, badgeText;
                if (params.value === "REAGENDACION EXITOSA") {
                    badgeClass = 'badge-estado badge-reag-enviado';
                    badgeText = 'Enviado';
                } else {
                    badgeClass = 'badge-estado badge-reag-pendiente';
                    badgeText = 'Pendiente';
                }
                const $badge = $('<span>', {
                    class: badgeClass,
                    text: badgeText,
                    css: {
                        lineHeight: '1.8',
                    }
                });

                // Icono de correo
                const fueEnviado = params.data.fueEnviadoEmail === true;
                const mailIconClass = fueEnviado
                    ? 'ri-mail-send-line text-success'
                    : 'ri-mail-send-line text-danger';
                const mailTitle = fueEnviado
                    ? 'Correo enviado'
                    : 'Correo no enviado';
                const $mailIcon = $('<i>', {
                    class: mailIconClass,
                    title: mailTitle,
                });

                $container.append($badge, $mailIcon);

                return $container[0];
            },
            width: 120
        },
        { 
            headerName: "N° Reagendamiento", 
            //field: "numeroReagendamiento",
            field: "numeroReagendamientoFormateado",
            width: 180
        },
        { 
            headerName: "DNI cliente", 
            width: 120,
            cellRenderer: (params) => {
                const $container = $('<div>', { class: 'd-flex gap-2' });

                if (params.data.fueDesembolsadoGeneral === true) {
                    $container.append($('<i>', {
                        class: 'bi-cash-stack text-success'
                    }));
                }
                $container.append($('<div>').text(params.data.dniCliente));
                
                return $container[0];
            }
        },
        { headerName: "Cliente", field: "nombreCliente", width: 250 },
        { headerName: "Teléfono", field: "telefono", width: 120 },
    //{ headerName: "DNI asesor", field: "dniAsesor", width: 120 },
        { headerName: "Asesor", field: "nombreAsesor", width: 120 },
        { 
            headerName: "Oferta", 
            field: "oferta",
            valueFormatter: params => {
                if (params.value === 0) return 'No aplica';
                return `S/. ${Number(params.value).toLocaleString('es-PE')}`;
            },
            width: 120 
        },
        { headerName: "Agencia", field: "agencia", width: 120 },
        {
            headerName: "Fecha Derivación",
            field: "fechaDerivacion",
            width: 150,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm')
        },
        {
            headerName: "Fecha visita",
            field: "fechaVisita",
            width: 130,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy')
        },
        {
            headerName: "Fecha reagendamiento",
            field: "fechaAgendamiento",
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm')
        }
    ];

    const reagendamientosGridOptions = {
        columnDefs: reagendamientosTableColumns,
        rowData: listaReagendamientos,
        pagination: true,
        paginationPageSize: 20,
        copyHeadersToClipboard: true,
        suppressClipboardPaste: true,
        enableCellTextSelection: true,
        enableBrowserTooltips: true,
        initialState: { sort: { sortModel: [{ colId: 'estadoReagendamiento', sort: 'asc' }] } },
        defaultColDef: { sortable: true, resizable: true, minWidth: 50, flex: 1 },
        getRowClass: (params) => {
            if (params.data.puedeSerReagendado === true) {
                return ['ag-row-reagendable'];
            }
            return [];
        },
        onGridReady: (params) => {
            gridApi = params.api;
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        },
        onGridSizeChanged: (params) => {
            params.api.sizeColumnsToFit({ defaultMinWidth: 50 });
        }
    };
    

    function createTable(rol) {
        if (rol === 1 || rol === 4 || rol === 2) {
            return;
        } else if (rol === 3) {
            for (let i = reagendamientosTableColumns.length - 1; i >= 0; i--) {
                if (reagendamientosTableColumns[i].field === 'dniAsesor') {
                    reagendamientosTableColumns.splice(i, 1);
                }
            }
        }
    }

    function init() {
        const gridDiv = document.querySelector('#gridReagendamientos');
        if (gridDiv) {
            // Obtener el rol del elemento #idRol
            const idRolElement = document.getElementById('idRol');
            const rol = idRolElement ? parseInt(idRolElement.value) || 0 : 0;
            
            // Modificar columnas según el rol
            createTable(rol);
            
            // Inicializar tabla vacía
            reagendamientosGridOptions.rowData = [];
            gridApi = agGrid.createGrid(gridDiv, reagendamientosGridOptions);
        }
    }

    function updateTableData(data) {
        listaReagendamientos = data || [];
        if (gridApi) {
            gridApi.setGridOption('rowData', listaReagendamientos);
        }
    }

    /**
     * Devuelve una lista de los IDs de derivación (números) de los reagendamientos.
     * @returns {int[]}
     */
    function obtenerListaIdsDerivacion() {
        return listaReagendamientos.map(item => item.idDerivacion);
    }

    return {
        init,
        updateTableData,
        obtenerListaIdsDerivacion,
        gridApi: () => gridApi
    };
})();

App.historico = (function () {
    let gridApi;
    let gridInitialized = false;

    let historico = [];


    let derivacionesTableColumns = [
        /*
        {
            headerName: 'Estado Reagendamiento',
            field: 'estadoReagendamiento',
            width: 220,
            sortable: true,
            cellClass: "d-flex align-items-center justify-content-center",
            cellRenderer: (params) => {
                if (params.value === 'REAGENDACION EXITOSA') {
                    return `<span class="badge-estado badge-enviado">Procesado</span>`;
                } else {
                    return `<span class="badge-estado badge-no-enviado">Pendiente</span>`;
                }
            },
        },
        */
        { headerName: 'N°', field: 'numeroReagendamientoFormateado', width: 70, sortable: true },
        {
            headerName: 'Cliente',
            field: 'dniCliente',
            width: 180,
            sortable: true,
            cellRenderer: (params) => {
                return `<span title="${params.data.nombreCliente || ''}">${params.value} - ${params.data.nombreCliente || ''}</span>`;
            },
        },
        {
            headerName: 'Telefono',
            field: 'telefono',
            width: 100,
            sortable: true
        },
        {
            headerName: 'Asesor',
            field: 'dniAsesor',
            width: 180,
            sortable: true,
            cellRenderer: (params) => {
                return `<span title="${params.data.nombreAsesor || ''}">${params.value} - ${params.data.nombreAsesor || ''}</span>`;
            },
        },
        {
            headerName: 'Oferta',
            field: 'oferta',
            width: 150,
            sortable: true,
            valueFormatter: params => {
                if (params.value === 0) return 'No aplica';
                return params.value.toLocaleString('es-ES', { style: 'currency', currency: 'PEN' });
            },
        },
        {
            headerName: 'Agencia',
            field: 'agencia',
            width: 220,
            sortable: true
        },
        {
            headerName: 'Fecha Derivación',
            field: 'fechaDerivacion',
            width: 150,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy')
        },
        {
            headerName: 'Fecha Visita',
            field: 'fechaVisita',
            width: 150,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy')
        },
        {
            headerName: 'Fecha ReAgendamiento',
            field: 'fechaAgendamiento',
            width: 220,
            sortable: true,
            valueFormatter: params => formatDateTime(params.value, 'dd/mm/yyyy hh:mm')
        },
    ];

    const derivacionesGridOptions = {
        columnDefs: derivacionesTableColumns,
        rowData: null,
        pagination: true,
        paginationPageSize: 20,
        defaultColDef: {
            resizable: true,
            sortable: true,
        },
        onGridReady: function (params) {
            gridApi = params.api;
            params.api.sizeColumnsToFit();
        }
    };

    function init(historicoData) {
        historico = historicoData || [];

        const gridDiv = document.getElementById('gridHistóricoReagendamientos');
        if (!gridDiv) {
            return;
        }
        gridDiv.innerHTML = '';
        derivacionesGridOptions.rowData = historicoData || [];

        agGrid.createGrid(gridDiv, derivacionesGridOptions);
        gridInitialized = true;
    }

    return {
        init: init
    };
})();

function DescargarResumenExcelDerivaciones() {
    let dni = $('#dniClienteDerivaciones').val() || null;
    let supervisor = $('#supervisorDerivaciones').val() || null;
    let asesor = $('#asesorDerivaciones').val() || null;
    let agencia = $('#agenciaDerivaciones').val() || null;
    let fechaDerivacion = $('#fechaDerivacion').val() || null;
    let fechaVisita = $('#fechaVisitaDerivacion').val() || null;

    const Fecha = obtenerFechaActual();

    Swal.fire({
        title: 'Cargando...',
        timerProgressBar: true,
        allowEscapeKey: false,
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    
    $.ajax({
        url: '/Operaciones/ExportarDerivacionesExcel',
        type: 'GET',
        data: {
            dni : dni,
            idAsesor: asesor,
            idSupervisor: supervisor,
            agencia: agencia,
            fecha_derivacion: fechaDerivacion,
            fecha_visita: fechaVisita
        },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            console.log(data)
            var a = document.createElement('a');
            a.href = window.URL.createObjectURL(data);


            a.download = 'ReporteResumenDerivacion_' + Fecha + '.xlsx';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            Swal.close();
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error en la operación',
                text: 'Hubo un problema al generar el archivo Excel',
                confirmButtonText: 'Ok',
            });
            Swal.close();
        }
    });
}

function DescargarResumenExcelReagendamientos(){
    let dni = $('#dniClienteReagendamientos').val() || null;
    let supervisor = $('#supervisorReagendamientos').val() || null;
    let asesor = $('#asesorReagendamientos').val() || null;
    let agencia = $('#agenciaReagendamientos').val() || null;
    let fechaReagendamiento = $('#fechaReagendamiento').val() || null;
    let fechaVisita = $('#fechaVisitaReagendamientos').val() || null;

    const Fecha = obtenerFechaActual();

    Swal.fire({
        title: 'Cargando...',
        timerProgressBar: true,
        allowEscapeKey: false,
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: '/Operaciones/ExportarReagendamientosExcel',
        type: 'GET',
        data: {
            dni : dni,
            idAsesor: asesor,
            idSupervisor: supervisor,
            agencia: agencia,
            fecha_reagendamiento: fechaReagendamiento,
            fecha_visita: fechaVisita
        },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            console.log(data)
            var a = document.createElement('a');
            a.href = window.URL.createObjectURL(data);


            a.download = 'ReporteResumenReagendamiento_' + Fecha + '.xlsx';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);

        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error en la operación',
                text: 'Hubo un problema al generar el archivo Excel',
                confirmButtonText: 'Ok',
            });
        },
        complete: function () {
            Swal.close();
        }
    });
}

function DescargarResumenExcelHistoricos(){

    Swal.fire({
        title: 'Cargando...',
        timerProgressBar: true,
        allowEscapeKey: false,
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    const Fecha = obtenerFechaActual();

    ListaIDsDerivacion = App.reagendamientos.obtenerListaIdsDerivacion();

    $.ajax({
        url: '/Operaciones/ExportarHistoricosExcel',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(
            ListaIDsDerivacion
        ),
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            console.log(data)
            var a = document.createElement('a');
            a.href = window.URL.createObjectURL(data);


            a.download = 'ReporteResumenHistorico_' + Fecha + '.xlsx';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error en la operación',
                text: 'Hubo un problema al generar el archivo Excel',
                confirmButtonText: 'Ok',
            });
        },
        complete: function () {
            Swal.close();
        }
    });
}

///---------------FUNCIONES AUXILIARES -------------///

// Función auxiliar para formatear fechas
function formatDateTime(dateString, format = 'dd/mm/yyyy') {
    if (!dateString) return '';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return dateString;

    const pad = (num) => String(num).padStart(2, '0');
    const day = pad(date.getDate());
    const month = pad(date.getMonth() + 1);
    const year = date.getFullYear();
    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());

    if (format === 'dd/mm/yyyy hh:mm:ss') {
        return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;
    } else if (format === 'yyyy-mm-dd') {
        return `${year}-${month}-${day}`;
    } else if (format === 'yyyy-mm-dd hh:mm') {
        return `${year}-${month}-${day} ${hours}:${minutes}`;
    } else if (format === 'dd/mm/yyyy hh:mm') {
        return `${day}/${month}/${year} ${hours}:${minutes}`;
    }
    return `${day}/${month}/${year}`;
}

function obtenerFechaActual() {
    const now = new Date();
    return now.getFullYear() + "-" +
        String(now.getMonth() + 1).padStart(2, '0') + "-" +
        String(now.getDate()).padStart(2, '0') + " " +
        String(now.getHours()).padStart(2, '0') + ":" +
        String(now.getMinutes()).padStart(2, '0') + ":" +
        String(now.getSeconds()).padStart(2, '0');
}