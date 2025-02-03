function deriveEsteCliente() {
    const fechaVisitaDerivacion = document.getElementById('FechaVisitaDerivacion').value;
    const agenciaDerivacion = document.getElementById('AgenciaDerivacion').value;
    const asesorDerivacion = document.getElementById('AsesorDerivacion').value;
    const DNIAsesorDerivacion = document.getElementById('DNIAsesorDerivacion').value;
    const telefonoDerivacion = document.getElementById('TelefonoDerivacion').value;
    const DNIClienteDerivacion = document.getElementById('DNIClienteDerivacion').value;
    const nombreClienteDerivacion = document.getElementById('nombreClienteDerivacion').value;

    if (!fechaVisitaDerivacion) {
        alert('La fecha de visita es obligatoria.');
        return;
    }
    if (!agenciaDerivacion) {
        alert('La agencia es obligatoria.');
        return;
    }
    if (!asesorDerivacion) {
        alert('El asesor es obligatorio.');
        return;
    }
    if (!DNIAsesorDerivacion) {
        alert('El DNI del asesor es obligatorio.');
        return;
    }
    if (!telefonoDerivacion) {
        alert('El teléfono es obligatorio.');
        return;
    }
    if (!DNIClienteDerivacion) {
        alert('El DNI del cliente es obligatorio.');
        return;
    }
    if (!nombreClienteDerivacion) {
        alert('El nombre del cliente es obligatorio.');
        return;
    }

    // Continue with the rest of the function logic
    $.ajax({
        type: 'POST',
        url: '/Derivacion/GenerarDerivacion',
        data: {
            FechaVisitaDerivacion: fechaVisitaDerivacion,
            AgenciaDerivacion: agenciaDerivacion,
            AsesorDerivacion: asesorDerivacion,
            DNIAsesorDerivacion: DNIAsesorDerivacion,
            TelefonoDerivacion: telefonoDerivacion,
            DNIClienteDerivacion: DNIClienteDerivacion,
            NombreClienteDerivacion: nombreClienteDerivacion
        },
        success: function (data) {
            if (data.success) {
                alert('Derivación creada correctamente.');
                location.href = '/Consulta/Consulta';
            } else {
                alert(data.message);
            }
        },
        error: function (error) {
            console.error(error);
        }
    });
}