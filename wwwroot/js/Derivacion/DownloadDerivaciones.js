
async function downloadDerivaciones() {
    const instervaloInicial = document.getElementById("intervalo-inicio-de-fechas").value;
    const intervaloFinal = document.getElementById("intervalo-final-de-fechas").value;
    const filtroDni = document.getElementById("filtroDNI").value;
    const filtroFecha = document.getElementById("filtroFecha").value;

    let filtro = null;
    let campo = null;
    if (filtroDni) {
        filtro = 'dni';
        campo = filtroDni;
    } else if (filtroFecha) {
        filtro = 'fecha';
        campo = filtroFecha;
    }

    const url = window.location.origin + `/Excel/DownloadDerivaciones?filtro=${filtro}&campo=${campo}&fecha_inicio=${instervaloInicial}&fecha_final=${intervaloFinal}`;
    try {
        const response = await fetch(url, {
            method: 'GET',
        });
        if (!response.ok) throw new Error("Error al descargar el archivo.");
        
    } catch (error) {
        console.error("Error al descargar las derivaciones:", error);
        Swal.fire({
            title: 'Error',
            text: 'Ocurrió un error al descargar las derivaciones.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }
}