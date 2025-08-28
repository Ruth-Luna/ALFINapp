function cargarReagendacion(data) {
    document.getElementById('reagendacion-de-derivacion-content').classList.remove('d-none');
    document.getElementById('dni-reagendamiento').value = data.dniCliente;
    document.getElementById('oferta-reagendamiento').value = data.ofertaMax != undefined ? data.ofertaMax : data.oferta;
    document.getElementById('nombre-reagendamiento').value = data.nombreCliente;
    document.getElementById('agencia-reagendamiento').value = data.nombreAgencia != undefined ? data.nombreAgencia : data.agencia;
    document.getElementById('telefono-reagendamiento').value = data.telefonoCliente != undefined ? data.telefonoCliente : data.telefono;
    document.getElementById('fecha-reagendamiento-previo').value = formatDateTime(data.fechaVisita, 'dd/mm/yyyy');
    // document.getElementById('fecha-reagendamiento-nueva').value = data.fechaVisita;
}

// async function reagendarView(IdDerivacion, puedeSerReagendado) {
//     activeId = IdDerivacion;
//     fileInput.value = ''; // Limpiar el input file
//     files = []; // Limpiar la lista de archivos

//     updateFileList(); // Actualizar la lista visualmente
//     if (puedeSerReagendado === 'False') {
//         Swal.fire({
//             icon: 'warning',
//             title: 'No se puede reagendar la cita',
//             text: 'Aun no se pueden realizar acciones.',
//             confirmButtonText: 'Aceptar'
//         });
//         return;
//     }
//     const baseUrl = window.location.origin;
//     const url = `${baseUrl}/Reagendamiento/Reagendamiento?id=${encodeURIComponent(IdDerivacion)}`;
//     let loadingSwal = Swal.fire({
//         title: 'Enviando...',
//         text: 'Por favor, espere mientras se busca los Datos del Reagendamiento.',
//         allowOutsideClick: false,
//         didOpen: () => {
//             Swal.showLoading();
//         }
//     });
//     try {
//         const response = await fetch(url, {
//             method: 'GET'
//         });
//         const contentType = response.headers.get("content-type");
//         Swal.close();
//         if (contentType && contentType.includes("application/json")) {
//             const result = await response.json();
//             if (result.success === false) {
//                 Swal.fire({
//                     title: 'Error',
//                     text: result.message || 'Ocurrió un error desconocido',
//                     icon: 'error',
//                     confirmButtonText: 'Aceptar'
//                 });
//             } else {
//                 cargarReagendacion(result.data);
//             }
//         } else {
//             console.error('Error: La respuesta no es JSON');
//             Swal.fire({
//                 title: 'Error',
//                 text: 'La respuesta del servidor no es válida.',
//                 icon: 'error',
//                 confirmButtonText: 'Aceptar'
//             });
//         }

//     } catch (error) {
//         console.error('Error:', error);
//         Swal.fire({
//             icon: 'error',
//             title: 'Error al reagendar la cita',
//             text: error.message || 'Ocurrió un error desconocido',
//             confirmButtonText: 'Aceptar'
//         });
//     }
// }

async function enviarReagendacion(nuevaFechaVisita, idDerivacion) {
    Swal.fire({
        title: 'Reagendar cita',
        text: 'Reagendar la cita del cliente. Esto volverá a enviar el formulario y los correos al banco.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, reagendar',
        cancelButtonText: 'No, cancelar'
    }).then(async (result) => {
        if (!result.isConfirmed) return;

        let fechaVisitaInput;
        let fechaVisita;
        try {
            fechaVisitaInput = document.getElementById(nuevaFechaVisita).value;
            fechaVisita = new Date(fechaVisitaInput).toISOString();
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error al obtener la fecha',
                text: 'Por favor, asegúrate de que la fecha esté en el formato correcto.',
                confirmButtonText: 'Aceptar'
            });
            return;
        }

        if (!fechaVisita || isNaN(new Date(fechaVisita))) {
            Swal.fire({
                icon: 'error',
                title: 'Fecha inválida',
                text: 'Por favor, ingresa una fecha válida.',
                confirmButtonText: 'Aceptar'
            });
            return;
        }

        if (files.length === 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Sin evidencias',
                text: 'No se han adjuntado archivos. No puede continuar',
                confirmButtonText: 'Aceptar'
            })
            return;
        }

        const loadingSwal = Swal.fire({
            title: 'Enviando...',
            text: 'Por favor, espere mientras se procesa el reagendamiento.',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });
        const baseUrl = window.location.origin;
        

        // Crear FormData para enviar archivos
        // var formdata = new FormData();
        // formdata.append("files", fileInput.files[0], "GerenteZonalController.cs");
        // formdata.append("dnicliente", "73393133");

        // var requestOptions = {
        //     method: 'POST',
        //     body: formdata,
        //     redirect: 'follow'
        // };

        // fetch("http://localhost:5051/Download/UploadImage", requestOptions)
        //     .then(response => response.text())
        //     .then(result => console.log(result))
        //     .catch(error => console.log('error', error));
        // Ejemplo de respuesta esperada del servidor
        // {"url":"http://localhost:5051/temp-images/3213f8ee-d7f5-4491-806c-57f6cb0691db.png"}
        const downloadURL = `${baseUrl}/Download/UploadImage`;

        var urls_string = [];
        for (const file of files) {
            try {
                const formdata = new FormData();
                formdata.append("files", file, file.name);

                const requestOptions = {
                    method: 'POST',
                    body: formdata,
                    redirect: 'follow'
                };

                const response1 = await fetch(downloadURL, requestOptions);
                if (response1.ok) {
                    const result1 = await response1.json();
                    urls_string.push(result1.url);
                } else {
                    console.error('Error al subir el archivo:', response1.statusText);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error al subir archivos',
                        text: `No se pudo subir el archivo ${file.name}. Por favor, intenta de nuevo.`,
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                }
            } catch (error) {
                console.error('Error al subir archivo:', file.name, error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error al subir archivos',
                    text: `No se pudo subir el archivo ${file.name}. Por favor, intenta de nuevo.`,
                    confirmButtonText: 'Aceptar'
                });
            }
        }

        const dto = {
            FechaReagendamiento: fechaVisita,
            IdDerivacion: idDerivacion,
            urlEvidencias: urls_string
        };

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
                    icon: 'warning',
                    title: 'Error al reagendar la cita',
                    text: `${result.message} Si desea ver los cambios puede recargar la pagina.` || 'Ocurrió un error desconocido',
                    confirmButtonText: 'Aceptar'
                }).then(() => {

                    const modal = document.getElementById('evidencia-derivacion-modal');
                    // Simulate a click on the close button
                    if (modal) {
                        const closeButton = modal.classList.contains('btn-close') ? modal : modal.querySelector('.btn-close');
                        if (closeButton) {
                            closeButton.click();
                        }
                    }
                });
                return;
            }

            Swal.fire({
                icon: 'success',
                title: 'Cita reagendada',
                text: `${result.message} Si desea ver los cambios puede recargar la pagina.` || 'La cita ha sido reagendada con éxito.',
                confirmButtonText: 'Aceptar'
            }).then(() => {
                // Only close the modal if the user confirms
                const modal = document.getElementById('evidencia-derivacion-modal');
                // Simulate a click on the close button
                if (modal) {
                    const closeButton = modal.classList.contains('btn-close') ? modal : modal.querySelector('.btn-close');
                    if (closeButton) {
                        closeButton.click();
                    }
                }
            });
            return;
        } catch (error) {
            Swal.close();
            console.error('Error:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error al reagendar la cita',
                text: error.message || 'Ocurrió un error desconocido',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}