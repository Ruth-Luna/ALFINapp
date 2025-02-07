function cambiarDatos(idUsuario) {
    var datosUsuario = {
        IdUsuario: idUsuario,
        Dni: document.getElementById('DNI_' + idUsuario).value,
        NombresCompletos: document.getElementById('NombresCompletos_' + idUsuario).value,
        Departamento: document.getElementById('Departamento_' + idUsuario).value,
        Provincia: document.getElementById('Provincia_' + idUsuario).value,
        Distrito: document.getElementById('Distrito_' + idUsuario).value,
        Telefono: document.getElementById('Telefono_' + idUsuario).value,
        FechaRegistro: document.getElementById('FechaRegistro_' + idUsuario).value,
        Estado: document.getElementById('Estado_' + idUsuario).value,
        IDUSUARIOSUP: document.getElementById('IDUSUARIOSUP_' + idUsuario).value,
        RESPONSABLESUP: document.getElementById('RESPONSABLESUP_' + idUsuario).value,
        REGION: document.getElementById('REGION_' + idUsuario).value,
        NOMBRECAMPAÑA: document.getElementById('NOMBRECAMPAÑA_' + idUsuario).value,
        IdRol: document.getElementById('IdRol_' + idUsuario).value,
        Rol: document.getElementById('Rol_' + idUsuario).value
    };

    $.ajax({
        url: '/Usuarios/ModificarUsuario',
        type: 'POST',
        data: JSON.stringify(datosUsuario),
        contentType: 'application/json; charset=utf-8',
        success: function(response) {
            alert('Datos actualizados correctamente');
        },
        error: function(error) {
            alert('Error al actualizar los datos');
        }
    });
}