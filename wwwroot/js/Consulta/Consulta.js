function deriveEsteCliente() {
    const fechaVisitaDerivacion = document.getElementById('fechaVisitaDerivacion').value;
    const agenciaDerivacion = document.getElementById('agenciaDerivacion').value;
    const asesorDerivacion = document.getElementById('asesorDerivacion').value;
    const DNIAsesorDerivacion = document.getElementById('DNIAsesorDerivacion').value;
    const telefonoDerivacion = document.getElementById('telefonoDerivacion').value;
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
                Swal.fire ({
                    title: 'Derivación generada',
                    text: data.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                location.href = '/Consulta/Consulta';
            } else {
                Swal.fire ({
                    title: 'Error en la operación. Lea el mensaje a continuación',
                    text: data.message,
                    icon: 'warning',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function validarTelefono() {
    const campobusqueda = document.getElementById('dnicliente').value;

    if (!campobusqueda) {
        Swal.fire({
            title: 'Error',
            text: 'El campo del cliente es obligatorio.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/Consulta/VerificarTelefono',
        data: {
            telefono: campobusqueda
        },
        success: function (data) {
            if (data.success === false) {
                Swal.fire({
                    title: 'Error',
                    text: data.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });

                return;
            } else {
                Swal.fire({
                    title: 'Cliente encontrado',
                    text: 'El cliente ha sido encontrado en la base de datos.',
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                const datosClienteExistente = document.getElementById("datos-cliente-existente");
                datosClienteExistente.style.display = "block";
                datosClienteExistente.innerHTML = data; // Carga la vista parcial
            }
        },
        error: function (error) {
            console.error(error);
            Swal.fire({
                title: 'Error',
                text: error,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
    });
}