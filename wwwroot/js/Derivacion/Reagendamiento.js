async function reagendarCliente(nuevaFechaVisita, idDerivacion) {
    Swal.fire({
        title: 'Reagendar cita',
        text: 'Reagendar la cita del cliente. Esto volverá a enviar el formulario y los correos al banco.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, reagendar',
        cancelButtonText: 'No, cancelar'
    }).then(async (result) => {
        if (!result.isConfirmed) return;

        const fechaVisitaInput = document.getElementById(nuevaFechaVisita).value;
        const fechaVisita = new Date(fechaVisitaInput).toISOString();

        if (!fechaVisita || isNaN(new Date(fechaVisita))) {
            Swal.fire({
                icon: 'error',
                title: 'Fecha inválida',
                text: 'Por favor, ingresa una fecha válida.',
                confirmButtonText: 'Aceptar'
            });
            return;
        }

        const evidencias = [];
        for (const file of files) {
            const hex = await archivoAHex(file);
            evidencias.push({
                fileName: file.name,
                fileType: file.name.split('.').pop(),
                fileContent: hex,
                idDerivacion: idDerivacion,
                type: 0
            });
        }

        // Si no hay evidencias, mostrar advertencia
        if (evidencias.length === 0) {
            const warningResult = await Swal.fire({
                icon: 'warning',
                title: 'No hay evidencias',
                text: 'Se subirá el reagendamiento sin evidencias. ¿Desea continuar?',
                confirmButtonText: 'Sí',
                cancelButtonText: 'No',
                showCancelButton: true
            });

            if (!warningResult.isConfirmed) {
                Swal.fire({
                    icon: 'info',
                    title: 'Reagendamiento cancelado',
                    text: 'El reagendamiento ha sido cancelado.',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }
        }

        // Enviar reagendamiento (con o sin evidencias)
        const loadingSwal = Swal.fire({
            title: 'Enviando...',
            text: 'Por favor, espere mientras se procesa el reagendamiento.',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

        const dto = {
            FechaReagendamiento: fechaVisita,
            IdDerivacion: idDerivacion,
            evidencias: evidencias
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

            Swal.fire({
                icon: 'success',
                title: 'Cita reagendada',
                text: result.message || 'La cita ha sido reagendada con éxito.',
                confirmButtonText: 'Aceptar'
            }).then(() => location.reload());
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
            if (result.success === false) {
                Swal.fire({
                    title: 'Error',
                    text: result.message || 'Ocurrió un error desconocido',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                cargarReagendacion(result.data);
            }
        } else {
            console.error('Error: La respuesta no es JSON');
            Swal.fire({
                title: 'Error',
                text: 'La respuesta del servidor no es válida.',
                icon: 'error',
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
}

function cargarReagendacion(data) {
    fileInput.value = ''; // Limpiar el input file
    files = []; // Limpiar la lista de archivos
    updateFileList(); // Actualizar la lista visualmente
    const modalTitle = document.getElementById('evidencia-derivacion-title');
    modalTitle.textContent = `Reagendamiento de cita para ${data["nombresCompletos"].toUpperCase()}`;

    const reagendacioncontent = document.getElementById('reagendacion-de-derivacion-content');
    reagendacioncontent.innerHTML = ''; // Limpiar el contenido previo

    reagendacioncontent.innerHTML = `
        <h5>Reagendamiento</h5>
        <div class="col-md-6">
        <div class="form-group mb-2">
            <label for="dni-reagendamiento">Dni Cliente: </label>
            <input type="text" class="form-control" id="dni-reagendamiento" name="dni-reagendamiento"
            value="${data["dni"]?.toUpperCase?.() || ''}" readonly />
        </div>
        <div class="form-group mb-2">
            <label for="nombre-reagendamiento">Nombre Cliente: </label>
            <input type="text" class="form-control" id="nombre-reagendamiento" name="nombre-reagendamiento"
            value="${data["nombresCompletos"]?.toUpperCase?.() || ''}" readonly />
        </div>
        <div class="form-group mb-2">
            <label for="telefono-reagendamiento">Teléfono Cliente: </label>
            <input type="text" class="form-control" id="telefono-reagendamiento" name="telefono-reagendamiento"
            value="${data["telefono"]?.toUpperCase?.() || ''}" readonly />
        </div>
        </div>

        <div class="col-md-6">
        <div class="form-group mb-2">
            <label for="oferta-reagendamiento">Oferta a enviar: </label>
            <input type="text" class="form-control" id="oferta-reagendamiento" name="oferta-reagendamiento"
            value="S/. ${Math.trunc(data["ofertaMax"] || 0)}" readonly />
        </div>
        <div class="form-group mb-2">
            <label for="agencia-reagendamiento">Agencia Asignada: </label>
            <input type="text" class="form-control" id="agencia-reagendamiento" name="agencia-reagendamiento"
            value="${data["agenciaAsignada"]?.toUpperCase?.() || ''}" readonly />
        </div>
        <div class="form-group">
            <label for="fecha-reagendamiento-previo">Fecha Previa de Agendamiento</label>
            <input type="text" class="form-control" id="fecha-reagendamiento-previo"
            name="fecha-reagendamiento-previo" value="${data["fechaVisitaPrevia"] || ''}" readonly />
        </div>
        </div>

        <div class="col-md-12">
        <div class="form-group mb-3">
            <label for="fecha-reagendamiento-nueva">Nueva Fecha de Visita</label>
            <input type="date" class="form-control" id="fecha-reagendamiento-nueva"
            name="fecha-reagendamiento-nueva" />
        </div>
        </div>
    `;
    const submitButtonContainer = document.getElementById('evidencia-derivacion-submit-button-container');
    submitButtonContainer.innerHTML = ''; // Limpiar el contenedor de botones
    submitButtonContainer.innerHTML = `
        <a class="btn btn-success" href="javascript:void(0)"
            onclick="reagendarCliente('fecha-reagendamiento-nueva', ${data["idDerivacion"]})">Reagendar</a>
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" aria-label="Close">Cancelar</button>
    `;
}
