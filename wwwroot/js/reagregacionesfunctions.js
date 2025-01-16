function TomarEsteCliente(DNIdatos) {
    // Obtener el DNI del cliente desde el campo oculto
    const dni = DNIdatos;

    // Identificar la TipoBase activa
    const activeTab = document.querySelector('.nav-link.active');
    const tipoBase = activeTab ? activeTab.id.replace('fuenteBaseTab-', '') : null;

    // Validar si se encontraron datos necesarios
    if (!dni || !tipoBase) {
        alert('No se pudo identificar el cliente o la base activa.');
        return;
    }

    // Enviar la llamada AJAX
    const data = {
        dni: dni,
        tipoBase: tipoBase
    };

    console.log(data);

    $.ajax({
        url: '/Reagregaciones/ReAsignarClienteAUsuario',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        headers: {
            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value // Si usas AntiForgeryToken
        },
        success: function (result) {
            alert('Cliente registrado exitosamente.');
            console.log(result);
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            alert('Hubo un error al registrar el cliente.');
        }
    });
}