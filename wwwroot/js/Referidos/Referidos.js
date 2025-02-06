function cargarDerivacionManual(NombreCompletoAsesor, DniCliente, NombreCliente, OfertaEnviada, Telefono, Agencia, FechaVisita, DniAsesor) {
    if (!DniCliente || !Telefono || !DniAsesor) {
        Swal.fire({
            title: 'Error al derivar',
            text: 'Error al cargar los datos de las derivaciones',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    OfertaEnviada = parseFloat(OfertaEnviada);
    if (!isNaN(OfertaEnviada)) {
        Swal.fire({
            title: 'Error al derivar',
            text: 'El campo Oferta Enviada debe ser un número decimal válido',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/Referidos/DatosEnviarDerivacion',
        data: {
            FechaVisitaDerivacion: FechaVisita,
            AgenciaDerivacion: Agencia,
            NombreAsesorDerivacion: NombreCompletoAsesor,
            DNIAsesorDerivacion: DniAsesor,
            TelefonoDerivacion: Telefono,
            DNIClienteDerivacion: DniCliente,
            NombreClienteDerivacion: NombreCliente,
            OfertaEnviadaDerivacion: OfertaEnviada,
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las referencias a derivar',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#modalContentGeneralTemplate').html(response); // Inserta el contenido de la vista en el modal
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#GeneralTemplateTitleModalLabel').text("Precaucion se mandara esta Derivacion"); // Inserta el contenido de la vista en el modal
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las Referencias',
                text: 'Hubo un error inesperado al cargar las Referencias a enviar.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function enviarDerivacionPorReferencia(AgenciaDerivacion, 
    AsesorDerivacion, 
    DNIAsesorDerivacion, 
    DNIClienteDerivacion, 
    FechaVisitaDerivacion, 
    NombreClienteDerivacion, 
    TelefonoDerivacion) {
    
    $.ajax({
        type: 'POST',
        url: '/Referidos/EnviarDerivacionPorReferencia',
        data: {
            AgenciaDerivacion: AgenciaDerivacion,
            AsesorDerivacion: AsesorDerivacion,
            DNIAsesorDerivacion: DNIAsesorDerivacion,
            DNIClienteDerivacion: DNIClienteDerivacion,
            FechaVisitaDerivacion: FechaVisitaDerivacion,
            NombreClienteDerivacion: NombreClienteDerivacion,
            TelefonoDerivacion: TelefonoDerivacion,
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al enviar la derivacion',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                Swal.fire({
                    title: 'Derivacion enviada',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                $('#GeneralTemplateModal').modal('hide');
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al enviar la derivacion',
                text: 'Hubo un error inesperado al enviar la derivacion.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });    
}