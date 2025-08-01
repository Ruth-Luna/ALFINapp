async function downloadDerivaciones() {
    const intervaloInicialInput = document.getElementById("intervalo-inicio-de-fechas").value;
    const intervaloFinalInput = document.getElementById("intervalo-final-de-fechas").value;
    const filtroDni = document.getElementById("filtroDNI").value;
    const filtroFecha = document.getElementById("filtroFecha").value;

    let filtro = "";
    let campo = "";
    if (filtroDni) {
        filtro = 'dni';
        campo = filtroDni;
    } else if (filtroFecha) {
        filtro = 'fecha';
        campo = filtroFecha;
    }

    const loadingSwal = Swal.fire({
        title: 'Descargando datos',
        text: 'Por favor, espere mientras se generan los datos.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    const params = new URLSearchParams();

    if (filtro) params.append("filtro", filtro);
    if (campo) params.append("campo", campo);
    if (intervaloInicialInput) params.append("fecha_inicio", intervaloInicialInput);
    if (intervaloFinalInput) params.append("fecha_final", intervaloFinalInput);

    const url = window.location.origin;

    try {
        const response = await fetch(`${url}/Excel/DownloadDerivacionesAsync?${params.toString()}`);

        if (!response.ok) {
            throw new Error("No se pudo descargar el archivo");
        }

        const blob = await response.blob();
        const contentDisposition = response.headers.get("Content-Disposition");
        let filename = "derivaciones.xlsx"; // fallback

        // Extraer el nombre del archivo si viene en el header
        if (contentDisposition && contentDisposition.includes("filename=")) {
            filename = contentDisposition.split("filename=")[1].replace(/['"]/g, "");
        }

        // Crear enlace para descarga
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        Swal.close();
        Swal.fire({
            title: 'Descarga completada',
            text: 'Los datos han sido descargados exitosamente.',
            icon: 'success',
            confirmButtonText: 'Aceptar'
        });
    } catch (error) {
        Swal.close();
        Swal.fire({
            title: 'Error',
            text: 'Hubo un problema al descargar el archivo.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
    }
}
