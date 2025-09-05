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

    $('#btnLimpiarFiltrosOperaciones').on('click', function () {
        $('#dniClienteDerivaciones').val('').trigger('input');
        $('#supervisorDerivaciones').val('').trigger('change');
        $('#asesorDerivaciones').val('').trigger('change');
        $('#agenciaDerivaciones').val('').trigger('change');
        $('#fechaDerivacion').val('').trigger('change');
        $('#fechaVisitaDerivacion').val('').trigger('change');
    });

    $('#btnLimpiarFiltrosReagendamiento').on('click', function () {
        $('#dniClienteReagendamientos').val('').trigger('input');
        $('#supervisorReagendamientos').val('').trigger('change');
        $('#asesorReagendamientosCol').val('').trigger('change');
        $('#agenciaReagendamientos').val('').trigger('change');
        $('#fechaReagendamientos').val('').trigger('change');
        $('#fechaVisitaReagendamientos').val('').trigger('change');
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
                console.log(response)
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

function obtenerFechaActual() {
    const now = new Date();
    return now.getFullYear() + "-" +
        String(now.getMonth() + 1).padStart(2, '0') + "-" +
        String(now.getDate()).padStart(2, '0') + " " +
        String(now.getHours()).padStart(2, '0') + ":" +
        String(now.getMinutes()).padStart(2, '0') + ":" +
        String(now.getSeconds()).padStart(2, '0');
}