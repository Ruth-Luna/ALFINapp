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

// Función para enviar los cambios de datos personales de Clientes traidos por el Asesor
function guardarCambiosPorAsesor() {
    const formData = {};
    console.log('Guardando cambios...');

    // Recolectar datos del formulario
    document.querySelectorAll(".form-control-editable").forEach(input => {
        if (!input.hasAttribute("readonly")) {
            formData[input.id] = input.value;
        }
    });

    // Recolectar datos de los campos ocultos

    const idBase = document.getElementById("IDBASEUSUARIO").value;

    console.log('Datos enviados:', { formData, idBase });

    // Enviar datos como JSON
    $.ajax({
        url: '/Datos/ActualizarDatosProspecto',
        type: 'POST',
        data: {
            formData: formData,
            idBase: idBase,
        },
        success: function (result) {
            if (result.success === true) {
                Swal.fire({
                    title: 'Cambios guardados',
                    text: result.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            }
            else {
                Swal.fire({
                    title: 'Error en la operación',
                    text: result.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }

        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error al guardar los cambios',
                text: 'Hubo un error al intentar guardar los cambios. Por favor, inténtalo nuevamente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            console.error('Error al guardar los cambios:', error);
        }
    });
}

function enviarFormularioDerivacion(idAgenciaComercial, idFechaDeVisita, idBase, idTelefonoEnviado) {
    agenciaComercial = document.getElementById(idAgenciaComercial).value;
    FechaVisita = document.getElementById(idFechaDeVisita).value;
    Telefono = document.getElementById(idTelefonoEnviado).value;
    Swal.fire({
        title: 'Derivaciones',
        text: 'Al enviar la derivación, el cliente será contactado por el banco y se dara el seguimiento correspondiente. Procure no cerrar esta pagina. Este proceso puede durar varios segundos.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Continuar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
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
                },
                success: function (result) {
                    Swal.close();
                    if (result.success === true) {
                        Swal.fire({
                            title: 'Derivación enviada',
                            text: result.message,
                            icon: 'success',
                            confirmButtonText: 'Aceptar'
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
    });
}