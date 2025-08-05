
let gridOptions;

document.addEventListener('DOMContentLoaded', function () {
    ListarUsuarioGerente();
    ListarRoles();
    $('#selectTDocumento_EditarUsuario').on('change', function () {
        const tipo = $(this).val();
        const $input = $('#txtDNI_EditarUsuario');

        if (tipo == '1') {
            $input.attr('maxlength', 8);
            if ($input.val().length > 8) {
                $input.val($input.val().substring(0, 8)); 
            }
        } else if (tipo == '2') {
            $input.attr('maxlength', 11);
        }
    });

    $('#txtDNI_EditarUsuario').on('input', function () {
        this.value = this.value.replace(/\D/g, '');
    });
});



function ListarUsuarioGerente(id = null) {
    $.ajax({
        url: '/Usuarios/ListarUsuarioAdministrador?id=' + (id ?? ''),
        type: 'GET',
        success: function (data) {
            const rowData = data.map(usuario => ({
                dni: usuario.dni ?? '',
                usuarioNombre: `${usuario.nombres ?? ''} ${usuario.apellido_Paterno ?? ''} ${usuario.apellido_Materno ?? ''}`,
                rol: usuario.rol ?? '',
                supervisor: usuario.responsablesup ?? 'EL USUARIO NO TIENE SUPERVISOR',
                campaña: usuario.nombrecampaña ?? '',
                correo: usuario.correo ?? '',
                telefono: usuario.telefono ?? '',
                fechaInicio: usuario.fechaInicio ? FechaFormat(usuario.fechaInicio) : '',
                fechaCese: usuario.fechaCese ? FechaFormat(usuario.fechaCese) : '',
                estado: usuario.estado ?? '',
                fechaActualizacion: usuario.fechaActualizacion ? FechaFormat(usuario.fechaActualizacion) : '',
                idUsuario: usuario.idUsuario // para acciones
            }));

            if (gridOptions.api) {
                gridOptions.api.setRowData(rowData);
            }
        },
        error: function () {
            alert('Error al cargar la lista de usuarios');
        }
    });
}

// Inicializa AG Grid al cargar
document.addEventListener('DOMContentLoaded', function () {
    const columnDefs = [
        { headerName: "DNI", field: "dni" },
        { headerName: "Usuario", field: "usuarioNombre" },
        { headerName: "Rol", field: "rol" },
        { headerName: "Supervisor Asignado", field: "supervisor" },
        { headerName: "Campaña", field: "campaña" },
        { headerName: "Correo", field: "correo" },
        { headerName: "Teléfono", field: "telefono" },
        { headerName: "Fecha Inicio", field: "fechaInicio" },
        { headerName: "Fecha Cese", field: "fechaCese" },
        {
            headerName: "Estado",
            field: "estado",
            cellRenderer: (params) => {
                const clase = params.value === 'ACTIVO' ? 'bg-success' : 'bg-danger';
                return `<span class="badge rounded-pill w-100 ${clase}">${params.value}</span>`;
            }
        },
        {
            headerName: "Acción",
            field: "idUsuario",
            cellRenderer: (params) => {
                const checked = params.data.estado === 'ACTIVO' ? 'checked' : '';
                return `
            <div class="d-flex flex-column align-items-center">
              <div class="d-flex justify-content-center gap-2 mb-2">
                <div class="form-check form-switch">
                  <input class="form-check-input" type="checkbox" ${checked} 
                         onchange="CambiarEstadoUsuario(this.checked ? 1 : 0, '${params.value}')">
                </div>
                <button class="btn btn-primary btn-sm" onclick="ListarUsuarioxID('${params.value}')">
                  <i class="bi bi-pencil"></i>
                </button>
              </div>
            </div>
          `;
            }
        }
    ];

    gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        pagination: true,
        paginationPageSize: 10,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true
        },
        domLayout: 'autoHeight'
    };

    new agGrid.Grid(document.getElementById('clientesGrid'), gridOptions);

    // Llamar por primera vez
    ListarUsuarioGerente();
});

function CambiarEstadoUsuario(estado, idUsuario) {

    console.log(idUsuario)
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

function ListarUsuarioxID(idUsuario) {
    $.ajax({
        url: "/Usuarios/ListarUsuarioAdministrador",
        type: "GET",
        data: {
            idUsuario: idUsuario
        },
        success: function (response) {
            console.log(response)
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al obtener los datos del Usuario',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            } else {
                $('#modalEditarUsuarioGerente').modal('show');

                $('#txtDNI_EditarUsuario').val(response[0].dni ?? '');
                $('#selectTDocumento_EditarUsuario').val(response[0].tipoDocumento ?? '');
                $('#txtUser_EditarUsuario').val(response[0].usuario ?? '');
                $('#txtPassword_EditarUsuario').val(response[0].contrasenia ?? '');
                
                $('#txtApellidosP_EditarUsuario').val(response[0].apellido_Paterno ?? '');
                $('#txtApellidosM_EditarUsuario').val(response[0].apellido_Materno ?? '');
                $('#txtNombres_EditarUsuario').val(response[0].nombres ?? '');
                $('#txtRegion_EditarUsuario').val(response[0].region ?? '');
                $('#txtCampania_EditarUsuario').val(response[0].nombrecampania ?? '');
                $('#txtCorreo_EditarUsuario').val(response[0].correo ?? '');
                $('#txtRol_EditarUsuario').val(response[0].rol ?? '');
                $('#selectEstado').val(response[0].estado ?? '')

                $('#btnGuardarCambios').attr('data-id', response[0].idUsuario);
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al obtener los datos del Usuario',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}


$('#btnGuardarCambios').on('click', function () {
    const idUsuario = $(this).attr('data-id');
    const correo = $('#txtCorreo_EditarUsuario').val();

    const data = {
        IdUsuario: idUsuario,
        dni: $('#txtDNI_EditarUsuario').val() || null,
        tipo_doc: $('#selectTDocumento_EditarUsuario').val() || null,
        usuario: $('#txtUser_EditarUsuario').val() || null,
        contrasenia: $('#txtPassword_EditarUsuario').val() || null,
        apellido_paterno: $('#txtApellidosP_EditarUsuario').val() || null,
        apellido_materno: $('#txtApellidosM_EditarUsuario').val() || null,
        nombres: $('#txtNombres_EditarUsuario').val() || null,
        region: $('#txtRegion_EditarUsuario').val() || null,
        NOMBRECAMPAÑA: $('#txtCampania_EditarUsuario').val() || null,
        correo: $('#txtCorreo_EditarUsuario').val() || null,
        rol: $('#txtRol_EditarUsuario').val() || null,
        estado: $('#selectEstado').val() || null
    };

        if (correo && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo)) {
            Swal.fire('Formato Inválido', 'El correo ingresado no cumple con los estándares', 'warning');
            return; 
        }
    $.ajax({
        url: '/Usuarios/ActualizarUsuario',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (res) {
            console.log(res)
            if (res.success) {
                Swal.fire('Actualizado Correctamente', 'Se ha editado los datos del usuario', 'success');
                ListarUsuarioGerente()
                $('#modalEditarUsuarioGerente').modal('hide');
            } else {
                Swal.fire('Error', res.message || 'No se pudo actualizar', 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Error de servidor o red', 'error');
        }
    });
});
function ListarRoles() {
    const $select = $("#txtRol_EditarUsuario");

    $select.empty(); // Limpia por si acaso
    $select.append('<option value="" selected disabled>--Seleccione--</option>');

    $.ajax({
        url: "/Usuarios/ListarRoles",
        type: "GET",
        success: function (response) {
            response.forEach(function (rol) {
                $select.append(
                    $("<option>")
                        .val(rol.rol)
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