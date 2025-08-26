document.addEventListener('DOMContentLoaded', async () => {
    data = await getAllDerivaciones();
    rol = data.rolUsuario || 0;
    usuarioAsesores = data.asesores || [];
    usuarioSupervisores = data.supervisores || [];

    derivaciones = data.derivaciones || [];
    if (App && App.derivaciones) {
        App.derivaciones.init(derivaciones, rol, usuarioAsesores, usuarioSupervisores);
    }

    data2 = await getAllReagendamientos();
    reagendamientos = data2.reagendamientos || [];
    if (App && App.reagendamientos) {
        App.reagendamientos.init(reagendamientos, rol, usuarioAsesores, usuarioSupervisores);
    }
});

async function downloadDerivaciones() {
    const intervaloInicialInput = document.getElementById("fechaDerivacion").value;
    const intervaloFinalInput = document.getElementById("fechaVisitaDerivacion").value;
    const filtroDni = document.getElementById("dniClienteDerivaciones").value;
    const filtroSupervisor = document.getElementById("supervisorDerivaciones").value;

    let filtro = "";
    let campo = "";
    if (filtroDni) {
        filtro = 'dni';
        campo = filtroDni;
    } else if (filtroSupervisor && filtroSupervisor !== "Todos") {
        filtro = 'supervisor';
        campo = filtroSupervisor;
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
