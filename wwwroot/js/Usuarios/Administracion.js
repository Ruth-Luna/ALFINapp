
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
                <button class="btn btn-primary btn-sm" onclick="ListarUsuarioGerente('${params.value}',true)">
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
    $('#selectTDocumento_AR').on('change', function () {
        const tipo = $(this).val();
        const $input = $('#txtDNI_AR');

        if (tipo == '1') {
            $input.attr('maxlength', 8);
            if ($input.val().length > 8) {
                $input.val($input.val().substring(0, 8)); 
            }
        } else if (tipo == '2') {
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
                    usuarioNombre: `${usuario.nombres ?? ''} ${usuario.apellido_Paterno ?? ''} ${usuario.apellido_Materno ?? ''}`,
                    rol: usuario.rol ?? '',
                    supervisor: usuario.responsablesup ?? 'EL USUARIO NO TIENE SUPERVISOR',
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
                $('#txtCampania_AR').val(data[0].nombrecampania ?? '');
                $('#txtCorreo_AR').val(data[0].correo ?? '');
                $('#txtRol_AR').val(data[0].rol ?? '');
                $('#selectEstado').val(data[0].estado ?? '')

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

    const data = {
        IdUsuario: idUsuario || null,
        Dni: $('#txtDNI_AR').val() || '',
        Apellido_Paterno: $('#txtApellidosP_AR').val() || '',
        Apellido_Materno: $('#txtApellidosM_AR').val() || '',
        Nombres: $('#txtNombres_AR').val() || '',
        Departamento: $('#txtDepartamento_AR').val() || '',
        Provincia: $('#txtProvincia_AR').val() || '',
        Distrito: $('#txtDistrito_AR').val() || '',
        Telefono: $('#txtTelefono_AR').val() || '',
        Estado: $('#selectEstado').val() || 'ACTIVO',
        IDUSUARIOSUP: 0, // o el valor real si lo tienes
        RESPONSABLESUP: '',
        REGION: $('#txtRegion_AR').val() || '',
        NOMBRECAMPAÑA: $('#txtCampania_AR').val() || '',
        IdRol: $('#txtRol_AR').val() || '',
        Usuario: $('#txtUser_AR').val() || '',
        Correo: $('#txtCorreo_AR').val() || ''
    };
    console.log(data)
    if (correo && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo)) {
        Swal.fire('Formato Inválido', 'El correo ingresado no cumple con los estándares', 'warning');
        return;
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
    $('#selectEstado').val('');
}
