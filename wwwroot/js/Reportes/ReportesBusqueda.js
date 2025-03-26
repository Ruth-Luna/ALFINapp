async function cargarReporteAsesor(idUsuario) {
    var asesorElement = document.getElementById('div-derivaciones-asesor');
    const baseUrl = window.location.origin;
    const userId = parseInt(idUsuario, 10);
    const url = `${baseUrl}/Reportes/AsesorReportes?idAsesor=${encodeURIComponent(userId)}`;

    try {
        const response = await fetch(url, {
            method: 'GET'
        });
        const contentType = response.headers.get("content-type");

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
            // Si no es JSON, asumimos que es HTML
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
