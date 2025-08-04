
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
            console.log(data)
            const tbody = $('#clientesTable tbody');
            tbody.empty();
            const filaFiltro = `
                        <tr>
                            <td></td>
                            <td>
                                <div class="d-flex justify-content-center">
                                    <button class="btn btn-warning btn-sm" onclick="sortTable('clientesTable', 1, 'string')" data-sort-ignore>
                                        <i class="bi bi-filter"></i>
                                    </button>
                                </div>
                            </td>
                            <td colspan="8"></td>
                        </tr>
                    `;
            tbody.append(filaFiltro);
            $.each(data, function (i, usuario) {

                const row = `
                      
                        <tr>
                            <td>
                                <div class="d-flex flex-column align-items-center">
                                    <div class="d-flex justify-content-center gap-2 mb-2">
                                       <div class="form-check form-switch">
                                            <input class="form-check-input " style="cursor: pointer;" type="checkbox" role="switch"
                                                ${usuario.estado === 'ACTIVO' ? 'checked' : ''} 
                                                onchange="CambiarEstadoUsuario(this.checked ? 1 : 0, '${usuario.idUsuario}')">
                                        </div>
                                        <button class="btn btn-primary btn-sm" style="cursor: pointer;" onclick="ListarUsuarioxID('${usuario.idUsuario}')">
                                            <i class="bi bi-pencil"></i>
                                        </button>
                                    </div>
                                </div>
                            </td>
                             <td>
                                <div class="w-100 text-center">
                                    <span class="badge rounded-pill w-100 ${usuario.estado === 'ACTIVO' ? 'bg-success' : 'bg-danger'}">
                                        ${usuario.estado ?? ''}
                                    </span>
                                </div>
                            </td>
                            <td>${usuario.dni ?? ''}</td>
                            <td>${usuario.nombresCompletos ?? ''}</td>
                            <td>${usuario.responsablesup ? usuario.responsablesup : 'EL USUARIO NO TIENE SUPERVISOR'}</td>
                            <td>${usuario.nombrecampaña ?? ''}</td>
                            <td>${usuario.rol ?? ''}</td>
                            <td>${usuario.fechaActualizacion ? FechaFormat(usuario.fechaActualizacion) : ''}</td>
                            <td>${usuario.fechaInicio ? FechaFormat(usuario.fechaInicio) : ''}</td>
                            <td>${usuario.fechaCese ? FechaFormat(usuario.fechaCese) : ''}</td>
                        </tr>`;
                tbody.append(row);
            });
        },
        error: function () {
            alert('Error al cargar la lista de usuarios');
        }
    });
}

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