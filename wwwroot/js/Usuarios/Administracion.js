
let gridOptions;
$(document).ready(function () {
    // Ocultar elementos según el rol del usuario
    var rolUser = $('#rolUser').val();
    if (rolUser == 2) {
        $('#btnExportExcel').hide();
        $('#txtFiltrarRol').parent().hide();
    }

    $('#btnExportExcel').on('click', function () {
        Swal.fire({
            title: '¿Estás seguro?',
            text: "Se descargará el archivo Excel con el resumen de usuarios.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, descargar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                DescargarResumenExcel();
            }
        });
    });
});
document.addEventListener('DOMContentLoaded', function () {

    const columnDefs = [
        {
            headerName: "#",
            valueGetter: params => params.node.rowIndex + 1,
            width: 60,
        },
        { headerName: "DNI", field: "dni", width: 100 },
        { headerName: "Usuario", field: "usuarioNombre", width: 200 },
        { headerName: "Rol", field: "rol", width: 150 },
        { headerName: "Supervisor Asignado", field: "supervisor", width: 250 },
        { headerName: "Campaña", field: "campania", width: 150 },
        { headerName: "Correo", field: "correo", width: 250 },
        { headerName: "Fecha Inicio", field: "fechaInicio", width: 130 },
        {
            headerName: "Estado",
            field: "estado",
            width: 110,
            filter: 'agTextColumnFilter',
            getQuickFilterText: params => (params.value ?? '').toString().trim().toUpperCase(),
            cellRenderer: (params) => {
                const esActivo = params.value?.toUpperCase() === 'ACTIVO';
                const clase = esActivo ? 'bg-success' : 'bg-danger';
                const nuevoEstado = esActivo ? 0 : 1;

                return `
                    <span 
                        class="badge rounded-pill w-100 text-white py-2 ${clase}" 
                        style="cursor:pointer" onclick="CambiarEstadoUsuario(${nuevoEstado}, '${params.data.idUsuario}')">
                        ${params.value}
                    </span>
                `;
            }
        },
        {
            headerName: "Acción",
            field: "idUsuario",
            width: 120,
            cellRenderer: (params) => {
                return `
                    <div class="d-flex flex-column align-items-center">
                        <button class="btn btn-primary btn-sm btnActualizarUsuario" onclick="ListarUsuarioGerente('${params.value}',true)">
                          <i class="bi bi-eye"></i>
                        </button>
                    </div>
                `;
            }
        }
    ];

    const gridOptions = {
        columnDefs: columnDefs,
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: true
        },
        rowData: [],
        pagination: true,
        paginationPageSize: 100,
        paginationPageSizeSelector: [25, 50, 100],
        enableCellTextSelection: true,
        rowClassRules: {
            'highlight-warning': params => params.data.isHighlighted
        },
        onCellDoubleClicked: params => {
            const textToCopy = params.value;
            if (textToCopy != null) {
                navigator.clipboard.writeText(textToCopy)
                    .catch(err => console.error('Error al copiar:', err));
            }
        },
        suppressHorizontalScroll: false,
        localeText: window.localeTextEs
    };

    const gridDiv = document.getElementById('clientesGrid');

    const gridApi = agGrid.createGrid(gridDiv, gridOptions);

    window.gridApi = gridApi;

    ListarUsuarioGerente();

    ListarRoles("#txtRol_AR");
    ListarRoles("#txtFiltrarRol");
    ListarSupervisores();

    $('#selectTDocumento_AR').on('change', function () {
        const tipo = $(this).val();
        const $input = $('#txtDNI_AR');

        if (tipo == 'DNI') {
            $input.attr('maxlength', 8);
            if ($input.val().length > 8) {
                $input.val($input.val().substring(0, 8));
            }
        } else if (tipo == 'CE') {
            $input.attr('maxlength', 11);
        }
    });

    $('#txtDNI_AR').on('input', function () {
        this.value = this.value.replace(/\D/g, '');
    });
});

$('#btnAgregarUsuario').on('click', function () {
    LimpiarFormularioUsuario();
    $('#titlemodalARGerente').text('REGISTRAR USUARIO');
    $('#modalARGerente').modal('show');
    $('.solo-update-input').hide();
    $('#sltEstado_AR').val('ACTIVO');

});

$('#btnGuardarCambios').on('click', function () {
    let idUsuario = $(this).attr('data-id') || null;
    if (idUsuario) {
        AgregarActualizarCliente(true, idUsuario);
    } else {
        AgregarActualizarCliente(false); 
    }
});

$('#modalARGerente').on('hide.bs.modal', function () {
    $('#btnGuardarCambios').removeAttr('data-id');
});


$('#modalARGerente').on('show.bs.modal', function () {
    $('#txtRol_AR').change();
});

$(document).on('click', '.btnActualizarUsuario', function () {
    $('.solo-update-input').show();
});

$(document).on('change', '#txtRol_AR', function () {
    const rol = $(this).val();
    if (rol == '3') {
        $('#txtSupervisor_AR').parent().show();
    } else {
        $('#txtSupervisor_AR').parent().hide();
    }
});

function CambiarEstadoUsuario(estado, idUsuario) {

    $.ajax({
        url: "/Usuarios/ActualizarEstadoUsuario",
        type: "POST",
        contentType: "application/json", 
        data: JSON.stringify({
            IdUsuario: idUsuario,
            estado: estado.toString() 
        }),
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cambiar el estado del Usuario',
                    text: response.mensaje,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            } else {
                Swal.fire({
                    title: 'Actualizado Correctamente',
                    text: response.mensaje,
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false,
                    willClose: () => {
                        ListarUsuarioGerente();
                    }
                });
            }
        },
        error: function (xhr) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: xhr.responseJSON?.detalle || "Error inesperado.",
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}

function ListarUsuarioGerente(id = null, editar = false) {
    $.ajax({
        url: '/Usuarios/ListarUsuarioAdministrador?idUsuario=' + (id ?? ''),
        type: 'GET',
        success: function (data) {

            if (!editar) {

                let rowData = (data || []).map(usuario => ({
                    dni: usuario.dni ?? '',
                    usuarioNombre: usuario.nombresCompletos ?? '',
                    rol: usuario.rol ?? '',
                    supervisor: (!usuario.responsablesup || usuario.responsablesup.trim() === '')
                        ? 'EL USUARIO NO TIENE SUPERVISOR'
                        : usuario.responsablesup,
                    campania: usuario.nombrecampania ?? '',
                    correo: usuario.correo ?? '',
                    telefono: usuario.telefono ?? '',
                    fechaInicio: usuario.fechaInicio ? FechaFormat(usuario.fechaInicio) : '',
                    estado: usuario.estado ?? '',
                    fechaActualizacion: usuario.fechaActualizacion ? FechaFormat(usuario.fechaActualizacion) : '',
                    idUsuario: usuario.idUsuario
                }));

                if (window.gridApi) {
                    window.gridApi.setGridOption('rowData', rowData); 
                } else {
                    console.warn("⚠️ gridApi no está lista todavía");
                }

            } else {
                $('#titlemodalARGerente').text('ACTUALIZAR USUARIO');
                $('#modalARGerente').modal('show');
                
                $('#txtDNI_AR').val(data[0].dni ?? '');
                $('#selectTDocumento_AR').val(data[0].tipoDocumento ?? '');
                $('#txtUser_AR').val(data[0].usuario ?? '');
                $('#txtPassword_AR').val(data[0].contrasenia ?? '');

                $('#txtApellidosP_AR').val(data[0].apellido_Paterno ?? '');
                $('#txtApellidosM_AR').val(data[0].apellido_Materno ?? '');
                $('#txtNombres_AR').val(data[0].nombres ?? '');
                $('#txtRegion_AR').val(data[0].region ?? '');
                $('#txtCampania_AR').val(data[0].nombrecampania);
                $('#txtCorreo_AR').val(data[0].correo ?? '');
                $('#txtRol_AR').val(data[0].idRol?.toString() ?? '').change();
                $('#sltEstado_AR').val(data[0].estado ?? '')
                $('#txtTelefono_AR').val(data[0].telefono ?? '');
                $('#txtDepartamento_AR').val(data[0].departamento ?? '');
                $('#txtProvincia_AR').val(data[0].provincia ?? '');
                $('#txtDistrito_AR').val(data[0].distrito ?? '');
                $('#txtSupervisor_AR').val(data[0].idusuariosup?.toString() ?? '').change();

                $('#btnGuardarCambios').attr('data-id', data[0].idUsuario);
            }

        },
        error: function () {
            alert('Error al cargar la lista de usuarios');
        }
    });
}


function AgregarActualizarCliente(actualizar = false, idUsuario = null) {
    var validaciones = [];

    if (!actualizar) {
        const reglas = [
            { selector: '#selectTDocumento_AR', mensaje: 'Tipo de Documento es requerido.' },
            { selector: '#txtDNI_AR', mensaje: 'El número del documento es requerido.' },
            { selector: '#txtApellidosP_AR', mensaje: 'El apellido paterno es requerido.' },
            { selector: '#txtApellidosM_AR', mensaje: 'El apellido materno es requerido.' },
            { selector: '#txtNombres_AR', mensaje: 'El nombre es requerido.' },
            { selector: '#txtCampania_AR', mensaje: 'La campaña es requerida.' },
/*            { selector: '#txtRegion_AR', mensaje: 'La región es requerida.' },*/
        /*    { selector: '#txtDepartamento_AR', mensaje: 'El departamento es requerido.' },*/
            //{ selector: '#txtProvincia_AR', mensaje: 'La provincia es requerida.' },
            //{ selector: '#txtDistrito_AR', mensaje: 'El distrito es requerido.' },
         /*   { selector: '#txtTelefono_AR', mensaje: 'El teléfono es requerido.' },*/
            { selector: '#txtCorreo_AR', mensaje: 'El correo es requerido.' },
            { selector: '#sltEstado_AR', mensaje: 'El estado es requerido.' },
            { selector: '#txtRol_AR', mensaje: 'El rol es requerido.' }
        ];

        reglas.forEach(regla => {
            if (!$(regla.selector).val()) {
                validaciones.push(regla.mensaje);
            }
        });

        const tel = $('#txtTelefono_AR').val();
        if (tel && !/^\d{9}$/.test(tel)) {
            validaciones.push('El teléfono debe tener exactamente 9 dígitos numéricos.');
        }

        const correo = $('#txtCorreo_AR').val();
        if (correo && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo)) {
            validaciones.push('El correo ingresado no cumple con los estándares.');
        }

        if ($('#txtRol_AR').val() === '3' && !$('#txtSupervisor_AR').val()) {
            validaciones.push('El supervisor es requerido.');
        }
    }
    console.log(actualizar)
    // Validaciones solo para actualizar
    //if (actualizar) {
    //    [
    //        { selector: '#txtUser_AR', mensaje: 'El usuario es requerido.' },
    //        { selector: '#txtPassword_AR', mensaje: 'La contraseña es requerida.' }
    //    ].forEach(regla => {
    //        if (!$(regla.selector).val()) {
    //            validaciones.push(regla.mensaje);
    //        }
    //    });
    //}


    if (validaciones.length > 0) {
        Swal.fire({
            title: 'Campos incompletos',
            html: validaciones.map(v => `• ${v}`).join('<br>'),
            icon: 'info'
        });
        return;
    }
    const data = {
        IdUsuario: idUsuario || 0,
        Dni: $('#txtDNI_AR').val().trim() || '',
        TipoDocumento: $('#selectTDocumento_AR').val() || null,
        Apellido_Paterno: $('#txtApellidosP_AR').val().trim() || '',
        Apellido_Materno: $('#txtApellidosM_AR').val().trim() || '',
        Nombres: $('#txtNombres_AR').val().trim() || '',
        Departamento: $('#txtDepartamento_AR').val().trim() || '',
        Provincia: $('#txtProvincia_AR').val().trim() || '',
        Distrito: $('#txtDistrito_AR').val().trim() || '',
        Telefono: $('#txtTelefono_AR').val().trim() || '',
        Estado: $('#sltEstado_AR').val() || 'ACTIVO',
        REGION: $('#txtRegion_AR').val().trim() || '',
        NOMBRECAMPANIA: $('#txtCampania_AR').val().trim() || '',
        IdRol: parseInt($('#txtRol_AR').val()),
        Usuario: actualizar ?
            $('#txtUser_AR').val().trim() || null :
            // Al generar un nuevo usuario, se utiliza el formato "nombre.apellido"
            $('#txtNombres_AR').val().split(' ')[0] + '.' + $('#txtApellidosP_AR').val().split(' ')[0],
        Correo: $('#txtCorreo_AR').val().trim() || ''
    };

    if (actualizar) {
        data.Contrasenia = $('#txtPassword_AR').val().trim() || null;
    }

    // Si el rol es asesor (3), añadir IDUSUARIOSUP y RESPONSABLESUP
    if (data.IdRol === 3) {
        data.IDUSUARIOSUP = parseInt($('#txtSupervisor_AR').val()) || 0;
        data.RESPONSABLESUP = $('#txtSupervisor_AR option:selected').text() || '';
    }

    const url = actualizar ? '/Usuarios/ActualizarUsuario' : '/Usuarios/CrearUsuario';
    const mensajeExito = actualizar ? 'Actualizado Correctamente' : 'Registrado Correctamente';

    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (res) {
            if (res.success) {
                Swal.fire(mensajeExito, 'Se ha actualizado el usuario de manera correcta', 'success');
                ListarUsuarioGerente();
                $('#modalARGerente').modal('hide');
                LimpiarFormularioUsuario();

            } else if (res.message && res.message.includes('DNI ingresado ya se encuentra insertado')) {
                Swal.fire('DNI Existente', 'El Usuario ya ha sido creado anteriormente', 'warning');
            } else {
                Swal.fire('Error', res.message || 'No se pudo guardar', 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Error de servidor o red', 'error');
        }
    });
}

function ListarSupervisores() {
    $.ajax({
        url: "/Usuarios/ListarSupervisores",
        type: "GET",
        success: function (response) {
            const $select = $('#txtSupervisor_AR');
            $select.empty();
            $select.append($("<option>").val("").text("--Seleccione--"));
            response.forEach(function (supervisor) {
                $select.append(
                    $("<option>")
                        .val(supervisor.idUsuario)
                        .text(supervisor.nombresCompletos)
                );
            });

            // Logica para manejar roles específicos
            if ($('#rolUser').val() == 2) { // Para supervisor
                // Preseleccionar al el mismo
                $select.val($('#usuarioId').val()).change();
                $select.prop('disabled', true);
            }
        },
        error: function () {
            Swal.fire('Error', 'Error al cargar los supervisores', 'error');
        }
    });
}

function ListarRoles(idSelect = "#txtFiltrarRol") {
    const $select = $(idSelect);
    $.ajax({
        url: "/Usuarios/ListarRoles",
        type: "GET",
        success: function (response) {
            response.forEach(function (rol) {
                $select.append(
                    $("<option>")
                        .val(rol.idRol)
                        .text(rol.rol)
                );
            });

            // Lógica para manejar roles específicos
            if ($('#rolUser').val() == 2) { // Para supervisor
                // Preseleccionar a Asesor y deshabilitar
                $select.val('3').change();
                $select.prop('disabled', true);
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al obtener los roles',
                text: error.responseText,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}
function FechaFormat(fechaString) {
    const fecha = new Date(fechaString);
    const dia = fecha.getDate().toString().padStart(2, '0');
    const mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    const anio = fecha.getFullYear();
    return `${dia}/${mes}/${anio}`;
}

// Filtro por DNI
document.getElementById('filtroDni').addEventListener('input', function () {
    const v = this.value;
    const model = window.gridApi.getFilterModel() || {};
    if (!v) {
        delete model.dni; // 'dni' debe coincidir con el field en columnDefs
    } else {
        model.dni = { filterType: 'text', type: 'contains', filter: v };
    }
    window.gridApi.setFilterModel(model);
});

// Filtro por Usuario
document.getElementById('filtroUsuario').addEventListener('input', function () {
    const v = this.value;
    const model = window.gridApi.getFilterModel() || {};
    if (!v) {
        delete model.usuarioNombre; 
    } else {
        model.usuarioNombre = { filterType: 'text', type: 'contains', filter: v };
    }
    window.gridApi.setFilterModel(model);
});

    const rolesMap = {
    1: 'ADMINISTRADOR',
    2: 'SUPERVISOR',
    3: 'ASESOR',
    4: 'GERENTE ZONAL'
};

document.getElementById('txtFiltrarRol').addEventListener('change', function () {
    const v = this.value;
    const model = window.gridApi.getFilterModel() || {};

    if (!v) {
        delete model.rol; 
    } else {
        model.rol = {
            filterType: 'text',
            type: 'equals',
            filter: rolesMap[v] || ''
        };
    }

    window.gridApi.setFilterModel(model);
});

document.getElementById('txtFiltrarEstado').addEventListener('change', function () {
    const v = this.value;
    const model = window.gridApi.getFilterModel() || {};
    if (!v) {
        delete model.estado;
    } else {
        model.estado = { filterType: 'text', type: 'equals', filter: v };
    }
    window.gridApi.setFilterModel(model);
});
function DescargarResumenExcel() {

    let dni = $('#filtroDni').val();
    let usuario = $('#filtroUsuario').val();
    let txtFiltrarRol = $('#txtFiltrarRol').val();
    let txtFiltrarEstado = $('#txtFiltrarEstado').val();
    
    Swal.fire({
        title: 'Cargando...',
        timerProgressBar: true,
        allowEscapeKey: false,
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => Swal.showLoading()
    });

    $.ajax({
        url: '/Usuarios/ExportarUsuariosExcel?dni=' + encodeURIComponent(dni)
            + '&usuario=' + encodeURIComponent(usuario)
            + '&idRol=' + encodeURIComponent(txtFiltrarRol)
            + '&estado=' + encodeURIComponent(txtFiltrarEstado),
        type: 'GET',

        xhrFields: { responseType: 'blob' },
        success: function (data, status, xhr) {
            let filename = 'Usuarios_' + new Date().toISOString().slice(0, 10) + '.xlsx';

            const disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('filename=') !== -1) {
                const match = disposition.match(/filename\*?=(?:UTF-8'')?["']?([^"';]+)["']?/i);
            }
            const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            const url = window.URL.createObjectURL(blob);

            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);

            Swal.close();
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error en la operación',
                text: 'Hubo un problema al generar el archivo Excel',
                confirmButtonText: 'Ok'
            });
            Swal.close();
        }
    });
}

function LimpiarFormularioUsuario() {
    $('#txtDNI_AR').val('');
    $('#selectTDocumento_AR').val('');
    $('#txtUser_AR').val('');
    $('#txtPassword_AR').val('');
    $('#txtApellidosP_AR').val('');
    $('#txtApellidosM_AR').val('');
    $('#txtNombres_AR').val('');
    $('#txtRegion_AR').val('');
    $('#txtDepartamento_AR').val('');
    $('#txtProvincia_AR').val('')
    $('#txtDistrito_AR').val('')
    $('#txtTelefono_AR').val('')
    $('#txtCampania_AR').val('');
    $('#txtCorreo_AR').val('');
    $('#txtRol_AR').val('');
    $('#sltEstado_AR').val('');

    // Logica para manejar roles específicos
    if ($('#rolUser').val() == 2) { // Para supervisor
        preseleccionarRolSupervisor();
    }
}

function preseleccionarRolSupervisor() {
    // Preseleccionar al el mismo
    $('#txtSupervisor_AR').val($('#usuarioId').val()).change();
    $('#txtSupervisor_AR').prop('disabled', true);
    // Preseleccionar a Asesor y deshabilitar
    $('#txtRol_AR').val('3').change();
    $('#txtRol_AR').prop('disabled', true);
}
