async function reagendarCliente(nuevaFechaVisita, idDerivacion) {
    const fechaVisita = document.getElementById(nuevaFechaVisita).value;
    const dto = {
        FechaReagendamiento: fechaVisita,
        IdDerivacion: idDerivacion
    };
    Swal.fire({
        title: 'Reagendar cita',
        text: 'Reagendar la cita del cliente, volvera a enviar el formulario y los correos al banco.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, reagendar',
        cancelButtonText: 'No, cancelar'
    }).then(async (result) => {
        if (result.isConfirmed) {
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
        }
    });
}

async function reagendarView(IdDerivacion) {
    const modal = document.getElementById('GeneralTemplateModal');
    const modalTitle = document.getElementById('GeneralTemplateTitleModalLabel');
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

async function reagendarPorFiltro(params) {

}