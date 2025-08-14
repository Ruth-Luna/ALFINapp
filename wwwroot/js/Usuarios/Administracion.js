
let gridOptions;

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
                const checked = params.data.estado === 'ACTIVO' ? 'checked' : '';
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

    gridOptions = {
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
        suppressCellSelection: true,
        rowClassRules: {
            'highlight-warning': params => params.data.isHighlighted
        },
        onCellDoubleClicked: params => {
            const textToCopy = params.value;
            if (textToCopy != null) {
                navigator.clipboard.writeText(textToCopy)
                    .then(() => {
                    })
                    .catch(err => {
                        console.error('Error al copiar:', err);
                    });
            }
        },
        suppressHorizontalScroll: false,
        localeText: window.localeTextEs
    };

    new agGrid.Grid(document.getElementById('clientesGrid'), gridOptions);

    ListarUsuarioGerente();
    ListarRoles("#txtRol_AR"); // Para formulario
    ListarRoles("#txtFiltrarRol")
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

document.querySelectorAll('#filtroDni, #filtroUsuario, #txtFiltrarRol, #txtFiltrarEstado')
    .forEach(el => {
        el.addEventListener('input', aplicarFiltroGlobal);
        el.addEventListener('change', aplicarFiltroGlobal);
    });

$('#btnAgregarUsuario').on('click', function () {
    LimpiarFormularioUsuario();
    $('#modalARGerente').modal('show');
    $('#btnGuardarCambios').data('modo', 'agregar');
    // Ocultar inputs de usuario y contraseña
    $('.solo-update-input').hide();
});

$('#btnGuardarCambios').on('click', function () {
    const modo = $(this).data('modo');
    const idUsuario = $(this).attr('data-id') || null;

    if (modo === 'agregar') {
        AgregarActualizarCliente(false); 
    } else {
        AgregarActualizarCliente(true, idUsuario); 
    }
});

$('#modalARGerente').on('show.bs.modal', function () {
    // Forzar la activacion del change del select de rol
    $('#txtRol_AR').change();
});

$(document).on('click', '.btnActualizarUsuario', function () {
    // Mostrar inputs de usuario y contraseña
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
                const rowData = data.map(usuario => ({
                    dni: usuario.dni ?? '',
                    usuarioNombre: `${usuario.nombresCompletos ?? ''}`,
                    rol: usuario.rol ?? '',
                    supervisor: (!usuario.responsablesup || usuario.responsablesup.trim() === '') ? 'EL USUARIO NO TIENE SUPERVISOR' : usuario.responsablesup,
                    campania: usuario.nombrecampania ?? '',
                    correo: usuario.correo ?? '',
                    telefono: usuario.telefono ?? '',
                    fechaInicio: usuario.fechaInicio ? FechaFormat(usuario.fechaInicio) : '',
                    estado: usuario.estado ?? '',
                    fechaActualizacion: usuario.fechaActualizacion ? FechaFormat(usuario.fechaActualizacion) : '',
                    idUsuario: usuario.idUsuario
                }));

                if (gridOptions.api) {
                    gridOptions.api.setRowData(rowData);
                }
            } else {
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
                console.debug(data[0].nombrecampania + ' - ' + $('#txtCampania_AR').val());
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
    const correo = $('#txtCorreo_AR').val();

    // Validar inputs

    var validaciones = [];

    if (!$('#selectTDocumento_AR').val()) validaciones.push('Tipo de Documento es requerido.');
    if (!$('#txtDNI_AR').val()) validaciones.push('El número del documento es requerido.');
    if (!$('#txtApellidosP_AR').val()) validaciones.push('El apellido paterno es requerido.');
    if (!$('#txtApellidosM_AR').val()) validaciones.push('El apellido materno es requerido.');
    if (!$('#txtNombres_AR').val()) validaciones.push('El nombre es requerido.');
    if (!$('#txtCampania_AR').val()) validaciones.push('La campaña es requerida.');
    if (!$('#txtRegion_AR').val()) validaciones.push('La región es requerida.');
    if (!$('#txtDepartamento_AR').val()) validaciones.push('El departamento es requerido.');
    if (!$('#txtProvincia_AR').val()) validaciones.push('La provincia es requerida.');
    if (!$('#txtDistrito_AR').val()) validaciones.push('El distrito es requerido.');
    if (!$('#txtTelefono_AR').val()) validaciones.push('El teléfono es requerido.');
    if ($('#txtTelefono_AR').val() && !/^\d{9}$/.test($('#txtTelefono_AR').val())) validaciones.push('El teléfono debe tener exactamente 9 dígitos numéricos.');
    if (!$('#txtCorreo_AR').val()) validaciones.push('El correo es requerido.');
    if ($('#txtCorreo_AR').val() && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test($('#txtCorreo_AR').val())) {
        validaciones.push('El correo ingresado no cumple con los estándares.');
    }
    if (!$('#sltEstado_AR').val()) validaciones.push('El estado es requerido.');
    if (!$('#txtRol_AR').val()) validaciones.push('El rol es requerido.');

    // Validacion exclusivas para rol asesor (3)
    if ($('#txtRol_AR').val() === '3') {
        if (!$('#txtSupervisor_AR').val()) validaciones.push('El supervisor es requerido.');
    }

    // Validacion exlusivas de actualizacion
    if (actualizar) {
        if (!$('#txtUser_AR').val()) validaciones.push('El usuario es requerido.');
        if (!$('#txtPassword_AR').val()) validaciones.push('La contraseña es requerida.');
    }

    if (validaciones.length > 0) {
        Swal.fire('Campos faltantes o inválidos', validaciones.join('<br>'), 'warning');
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
        NOMBRECAMPAÑA: $('#txtCampania_AR').val().trim() || '',
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

    console.log(actualizar)
    const url = actualizar ? '/Usuarios/ActualizarUsuario' : '/Usuarios/CrearUsuario';
    const mensajeExito = actualizar ? 'Actualizado Correctamente' : 'Registrado Correctamente';

    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (res) {
            if (res.success) {
                Swal.fire(mensajeExito, '', 'success');
                ListarUsuarioGerente();
                $('#modalARGerente').modal('hide');
                LimpiarFormularioUsuario();
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

function aplicarFiltroGlobal() {
    // Tomar todos los valores de filtros
    const texto = [
        document.getElementById('filtroDni').value,
        document.getElementById('filtroUsuario').value,
        document.getElementById('txtFiltrarRol').value,
        document.getElementById('txtFiltrarEstado').value
    ].filter(Boolean).join(' ');

    const selEstado = document.getElementById('txtFiltrarEstado');
    selEstado.addEventListener('change', function () {
        const v = this.value; 
        const model = gridOptions.api.getFilterModel();

        if (!v) {
            delete model.estado;
        } else {
            model.estado = { filterType: 'text', type: 'equals', filter: v };
        }
        gridOptions.api.setFilterModel(model);
    });

    gridOptions.api.setQuickFilter(texto);
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
    $('#txtCampania_AR').val('');
    $('#txtCorreo_AR').val('');
    $('#txtRol_AR').val('');
    $('#sltEstado_AR').val('');
}
