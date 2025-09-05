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
        $('#asesorReagendamientos').val('').trigger('change');
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

function obtenerFechaActual() {
    const now = new Date();
    return now.getFullYear() + "-" +
        String(now.getMonth() + 1).padStart(2, '0') + "-" +
        String(now.getDate()).padStart(2, '0') + " " +
        String(now.getHours()).padStart(2, '0') + ":" +
        String(now.getMinutes()).padStart(2, '0') + ":" +
        String(now.getSeconds()).padStart(2, '0');
}