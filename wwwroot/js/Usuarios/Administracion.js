
document.addEventListener('DOMContentLoaded', function () {
    ListarUsuarioAdministrador();
});


function ListarUsuarioAdministrador() {
    $.ajax({
        url: '/Usuarios/ListarUsuarioAdministrador',
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
                                        <button class="btn btn-primary btn-sm" style="cursor: pointer;" onclick="CargarModalModificarUsuario('${usuario.idUsuario}')">
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
                            <td>${usuario.nombrecampania ?? ''}</td>
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


function CambiarEstadoUsuario(accion, idUsuario) {
    $.ajax({
        url: "/Usuarios/CambiarEstadoUsuario",
        type: "POST",
        data: {
            IdUsuario: idUsuario,
            accion: accion
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cambiar el estado del Usuario. ',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            } else {
                Swal.fire({
                    title: 'Se ha cambiado el estado de manera exitosa',
                    text: response.message,
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false,
                    willClose: () => {
                        ListarUsuarioAdministrador();
                    }
                });
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al actualizar los datos',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}

function CargarModalModificarUsuario(idUsuario) {
    $.ajax({
        url: "/Usuarios/ModificarUsuarioVista",
        type: "GET",
        data: {
            IdUsuario: idUsuario
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al obtener los datos del Usuario',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            } else {
                $('#modalContentGeneralTemplate').html(response); // Inserta el contenido de la vista en el modal
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#GeneralTemplateTitleModalLabel').text("Modificar Derivacion"); // Inserta el contenido de la vista en el modal
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


function FechaFormat(fechaString) {
    const fecha = new Date(fechaString);
    const dia = fecha.getDate().toString().padStart(2, '0');
    const mes = (fecha.getMonth() + 1).toString().padStart(2, '0');
    const anio = fecha.getFullYear();
    return `${dia}/${mes}/${anio}`;
}