async function cargarReporteAsesor(idUsuario) {
    if (idUsuario === undefined || idUsuario === null || idUsuario === '') {
        return;
    }
    var asesorElement = document.getElementById('div-derivaciones-asesor');
    const baseUrl = window.location.origin;
    const userId = parseInt(idUsuario, 10);
    const url = `${baseUrl}/Reportes/AsesorReportes?idAsesor=${encodeURIComponent(userId)}`;
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Activa la animación de carga
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
                    icon: 'error',
                    title: 'Error al obtener el reporte',
                    text: result.message || 'Ocurrió un error desconocido',
                    confirmButtonText: 'Aceptar'
                });
            }
        } else {
            const html = await response.text();
            asesorElement.innerHTML = html;
            asesorElement.style.display = 'block';
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'No se pudo cargar el reporte',
            confirmButtonText: 'Aceptar'
        });
    }
}

async function cargarReporteSupervisor(idUsuario) {
    if (idUsuario === undefined || idUsuario === null || idUsuario === '') {
        return;
    }
    var supervisorElement = document.getElementById('div-derivaciones-supervisor');
    const baseUrl = window.location.origin;
    const userId = parseInt(idUsuario, 10);
    const url = `${baseUrl}/Reportes/SupervisorReportes?idSupervisor=${encodeURIComponent(userId)}`;
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Activa la animación de carga
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
                    icon: 'error',
                    title: 'Error al obtener el reporte',
                    text: result.message || 'Ocurrió un error desconocido',
                    confirmButtonText: 'Aceptar'
                });
            }
        } else {
            const html = await response.text();
            supervisorElement.innerHTML = html;
            supervisorElement.style.display = 'block';
        }
    } catch (error) {
        Swal.close();
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'No se pudo cargar el reporte',
            confirmButtonText: 'Aceptar'
        });
    }
}