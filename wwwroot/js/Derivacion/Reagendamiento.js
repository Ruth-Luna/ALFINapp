async function reagendarCliente(nuevaFechaVisita, idDerivacion) {
    // console.log('Reagendar cliente:', files);
    Swal.fire({
        title: 'Reagendar cita',
        text: 'Reagendar la cita del cliente, volvera a enviar el formulario y los correos al banco.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, reagendar',
        cancelButtonText: 'No, cancelar'
    }).then(async (result) => {
        if (result.isConfirmed) {
            const fechaVisitaInput = document.getElementById(nuevaFechaVisita).value;
            const fechaVisita = new Date(fechaVisitaInput).toISOString(); // Convert to ISO 8601 format for C#

            if (!fechaVisita || isNaN(new Date(fechaVisita))) {
                Swal.fire({
                    icon: 'error',
                    title: 'Fecha inválida',
                    text: 'Por favor, ingresa una fecha válida.',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }

            const formData = [];
            const evidencias = [];
            for (const file of files) {
                const hex = await archivoAHex(file);
                formData.push(hex);
                evidencias.push({
                    fileName: file.name,
                    fileType: file.name.split('.').pop(), // Obtener la extensión del archivo
                    fileContent: hex,
                    idDerivacion: idDerivacion,
                    type: 0 // Asumiendo que el tipo es 0 para evidencia
                });
            }

            if (formData.length === 0) {
                Swal.fire({
                    icon: 'warning',
                    title: 'No hay evidencias',
                    text: 'Se subira el reagendamiento sin evidencias. Sin embargo, si desea subir evidencias posteriormente puede usar el otro boton encargado netamente de las evidencias.',
                    confirmButtonText: 'Aceptar',
                    showCancelButton: true,
                    cancelButtonText: 'Cancelar'
                }).then(async (result) => {
                    if (result.isConfirmed) {
                        const loadingSwal = Swal.fire({
                            title: 'Enviando...',
                            text: 'Por favor, espere mientras se procesa el reagendamiento.',
                            allowOutsideClick: false,
                            didOpen: () => {
                                Swal.showLoading();
                            }
                        });
                        const dto = {
                            FechaReagendamiento: fechaVisita,
                            IdDerivacion: idDerivacion,
                            evidencias: evidencias // Enviamos las evidencias como parte del DTO
                        };
                        const baseUrl = window.location.origin;
                        const url = `${baseUrl}/Reagendamiento/Reagendar`;
                        try {
                            const response = await fetch(url, {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify(dto)
                            });
                            const result = await response.json();
                            Swal.close();
                            if (!response.ok || result.success === false) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Error al reagendar la cita',
                                    text: result.message || 'Ocurrió un error desconocido',
                                    confirmButtonText: 'Aceptar'
                                });
                                return;
                            }
                            
                            if (result.success === false) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Error al reagendar la cita',
                                    text: result.message || 'Ocurrió un error desconocido',
                                    confirmButtonText: 'Aceptar'
                                });
                            } else {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Cita reagendada',
                                    text: result.message || 'La cita ha sido reagendada con éxito.',
                                    confirmButtonText: 'Aceptar'
                                });
                            }
                        } catch (error) {
                            console.error('Error:', error);
                            Swal.fire({
                                icon: 'error',
                                title: 'Error al reagendar la cita',
                                text: error.message || 'Ocurrió un error desconocido',
                                confirmButtonText: 'Aceptar'
                            });
                        }
                    } else {
                        Swal.fire({
                            icon: 'info',
                            title: 'Reagendamiento cancelado',
                            text: 'El reagendamiento ha sido cancelado.',
                            confirmButtonText: 'Aceptar'
                        });
                    }
                });
            }
        }
    });
}

async function reagendarView(IdDerivacion, puedeSerReagendado) {
    activeId = IdDerivacion;
    fileInput.value = ''; // Limpiar el input file
    files = []; // Limpiar la lista de archivos

    updateFileList(); // Actualizar la lista visualmente
    if (puedeSerReagendado === 'False') {
        Swal.fire({
            icon: 'warning',
            title: 'No se puede reagendar la cita',
            text: 'Aun no se pueden realizar acciones.',
            confirmButtonText: 'Aceptar'
        });
        return;
    }
    const modal = document.getElementById('GeneralTemplateModal');
    const modalTitle = document.getElementById('GeneralTemplateTitleModalLabel');
    modalTitle.textContent = `Evidencia para la derivación: ${activeId}`;
    const modalBody = document.getElementById('modalContentGeneralTemplate');
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Reagendamiento/Reagendamiento?id=${encodeURIComponent(IdDerivacion)}`;
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espere mientras se busca los Datos del Reagendamiento.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    try {
        const response = await fetch(url, {
            method: 'GET'
        });
        const contentType = response.headers.get("content-type");
        Swal.close();
        if (contentType && contentType.includes("application/json")) {
            const result = await response.json();
            if (!result.success) {
                Swal.fire({
                    title: 'Error',
                    text: result.message || 'Ocurrió un error desconocido',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                modalTitle.innerHTML = "Reagendamiento";
                modalBody.innerHTML = result;
                $(modal).modal('show');
            }
        } else {
            const result = await response.text();
            modalTitle.innerHTML = "Reagendamiento";
            modalBody.innerHTML = result;
            $(modal).modal('show');
        }

    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: 'Error al reagendar la cita',
            text: error.message || 'Ocurrió un error desconocido',
            confirmButtonText: 'Aceptar'
        });
    }
}