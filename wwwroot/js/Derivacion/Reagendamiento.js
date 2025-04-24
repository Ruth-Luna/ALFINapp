async function reagendarCliente(IdDerivacion) {
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
            const url = `${baseUrl}/Derivacion/ReagendarCliente`;
            try {
                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(params)
                });
                const result = await response.json();
                if (!response.ok || result.success === false) {
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

async function reagendarPorFiltro(params) {
    
}