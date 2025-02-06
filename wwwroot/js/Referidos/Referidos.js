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
    console.log(FechaVisita);

    // Conversión manual de la fecha en formato 'dd/mm/yyyy hh:mm:ss' a formato ISO
    let fechaParts = FechaVisita.split(' '); // Separar la fecha y la hora
    let dateParts = fechaParts[0].split('/'); // Separar el día, mes y año
    let timeParts = fechaParts[1].split(':'); // Separar las horas, minutos y segundos

    // Crear una nueva fecha en formato ISO (yyyy-mm-ddTHH:mm:ss)
    let fechaISO = new Date(dateParts[2], dateParts[1] - 1, dateParts[0], timeParts[0], timeParts[1], timeParts[2]).toISOString();

    console.log(fechaISO);

    /*OfertaEnviada = parseFloat(OfertaEnviada);
    if (!isNaN(OfertaEnviada)) {
        Swal.fire({
            title: 'Error al derivar',
            text: 'El campo Oferta Enviada debe ser un número decimal válido',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }*/



    let oferta = OfertaEnviada;  // Este es el valor como string con coma
    oferta = oferta.replace(",", ".");  // Convertir coma a punto
    oferta = parseFloat(oferta);  // Convertir a número decimal


    $.ajax({
        type: 'GET',
        url: '/Referidos/DatosEnviarDerivacion',
        data: {
            FechaVisitaDerivacion: fechaISO,
            AgenciaDerivacion: Agencia,
            NombreAsesorDerivacion: NombreCompletoAsesor,
            DNIAsesorDerivacion: DniAsesor,
            TelefonoDerivacion: Telefono,
            DNIClienteDerivacion: DniCliente,
            NombreClienteDerivacion: NombreCliente,
            OfertaEnviadaDerivacion: oferta,
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

    let fechaParts = FechaVisitaDerivacion.split(' '); // Separar la fecha y la hora
    let dateParts = fechaParts[0].split('/'); // Separar el día, mes y año
    let timeParts = fechaParts[1].split(':'); // Separar las horas, minutos y segundos

    // Crear una nueva fecha en formato ISO (yyyy-mm-ddTHH:mm:ss)
    let fechaISO = new Date(dateParts[2], dateParts[1] - 1, dateParts[0], timeParts[0], timeParts[1], timeParts[2]).toISOString();


    $.ajax({
        type: 'POST',
        url: '/Referidos/EnviarDerivacionPorReferencia',
        data: {
            AgenciaDerivacion: AgenciaDerivacion,
            AsesorDerivacion: AsesorDerivacion,
            DNIAsesorDerivacion: DNIAsesorDerivacion,
            DNIClienteDerivacion: DNIClienteDerivacion,
            FechaVisitaDerivacion: fechaISO,
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