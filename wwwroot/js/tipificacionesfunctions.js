function verificarTipificacion(index) {
    console.log('Verificando tipificación...');
    console.log(`tipificacionSelect_${index}`);
    const tipificacionInput = document.getElementById(`tipificacionSelect_${index}`);
    const fechaVisitaContainer = document.getElementById(`fechaVisitaContainer_${index}`);
    const buttonDerivacionContainer = document.getElementById(`buttonDerivacionContainer_${index}`);

    if (tipificacionInput.value == 2) {
        console.log('Tipificación seleccionada: Derivación');
        fechaVisitaContainer.style.display = "block";
        buttonDerivacionContainer.style.display = "block";
    } else {
        fechaVisitaContainer.style.display = "none";
        buttonDerivacionContainer.style.display = "none";
    }
}

// Función para actualizar el ID de la tipificación cuando se selecciona una opción del datalist
function actualizarIdTipificacion(contadorNumeros) {
    var inputText = document.getElementById('tipificacionSelect_' + contadorNumeros);
    var datalist = document.getElementById('tipificacionesList_' + contadorNumeros);
    var hiddenInput = document.getElementById('tipificacionId_' + contadorNumeros);

    // Buscar la opción correspondiente en el datalist
    for (var option of datalist.options) {
        if (option.value === inputText.value) {
            hiddenInput.value = option.getAttribute('data-id'); // Actualizar el ID oculto
            return;
        }
    }

    hiddenInput.value = ''; // Si no hay coincidencia, limpiar el ID
}

function enviarFormularioDerivacion(
    idAgenciaComercial,
    idFechaDeVisita,
    idBase,
    idTelefonoEnviado,
    typeTip,
    IdAsignacion
) {
    agenciaComercial = document.getElementById(idAgenciaComercial).value;
    FechaVisita = document.getElementById(idFechaDeVisita).value;
    Telefono = document.getElementById(idTelefonoEnviado).value;
    Asignacion = document.getElementById(IdAsignacion).value;
    Swal.fire({
        title: 'Derivaciones',
        text: 'Al enviar la derivación, el cliente será contactado por el banco y se dara el seguimiento correspondiente. Procure no cerrar esta pagina. Este proceso puede durar varios segundos.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Continuar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            // Verificar si tiene Nombre el cliente
            
            if (document.getElementById("X_APPATERNO").value == "" && document.getElementById("X_APMATERNO").value == "" && document.getElementById("X_NOMBRE").value == "") {
                Swal.fire({
                    title: 'Error',
                    text: 'El cliente no tiene nombre. Ingrese el nombre a continuación (Este nombre no será guardado en la Base de datos pero será usado para la derivación).',
                    icon: 'warning',
                    input: 'text',  // Genera un cuadro de texto
                    inputPlaceholder: 'Ingrese el nombre del cliente',
                    showCancelButton: true,
                    confirmButtonText: 'Aceptar',
                    cancelButtonText: 'Cancelar',
                    inputValidator: (value) => {
                        if (!value.trim()) {
                            return 'Por favor, ingrese un nombre válido.';
                        }
                    }
                }).then((inputResult) => {
                    if (inputResult.isConfirmed) {
                        let nombreIngresado = inputResult.value;
                        if (nombreIngresado) {
                            document.getElementById("X_NOMBRE").value = nombreIngresado;
                            Swal.fire({
                                title: 'Éxito',
                                text: 'Nombre ingresado correctamente. Ahora puede continuar con la derivación.',
                                icon: 'info',
                                confirmButtonText: 'Aceptar'
                            }).then(() => {
                                enviarDerivacion( 
                                    agenciaComercial,
                                    FechaVisita,
                                    Telefono,
                                    idBase,
                                    typeTip,
                                    Asignacion,
                                    nombreIngresado
                                );
                            });
                        }
                    }
                });
            } else {
                // Si el cliente tiene nombre, proceder con la derivación
                enviarDerivacion( 
                    agenciaComercial,
                    FechaVisita,
                    Telefono,
                    idBase,
                    typeTip,
                    Asignacion
                );
            }
        }
    });
}

function enviarDerivacion(agenciaComercial, FechaVisita, Telefono, idBase, typeTip, Asignacion, NombresCompletos = null) {
    console.log('Enviando derivación...');
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Activa la animación de carga
        }
    });
    $.ajax({
        url: '/Tipificaciones/GenerarDerivacion',
        type: 'POST',
        data: {
            agenciaComercial: agenciaComercial,
            FechaVisita: FechaVisita,
            Telefono: Telefono,
            idBase: idBase,
            type: typeTip,
            idAsignacion: Asignacion,
            NombresCompletos: NombresCompletos
        },
        success: function (result) {
            Swal.close();
            if (result.success === true) {
                Swal.fire({
                    title: 'Derivación enviada',
                    text: result.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                }).then(() => {
                    location.reload();
                });
            }
            else {
                Swal.fire({
                    title: 'Error en la operación. Lea con cuidado',
                    text: result.message,
                    icon: 'warning',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            Swal.fire({
                title: 'Error al enviar la derivación',
                text: 'Hubo un error al intentar enviar la derivación. Por favor, inténtalo nuevamente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            console.error('Error al enviar la derivación:', error);
        }
    });
}