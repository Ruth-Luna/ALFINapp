async function descargarDatos() {
    const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin').value;
    const selectElement = document.getElementById('filtro-descarga-general');
    const selectedOption = selectElement.options[selectElement.selectedIndex];
    const optgroup = selectedOption.parentElement;

    const type_filter = optgroup.getAttribute('data-type');
    const filtroData = selectedOption.value;

    let messages = [];

    if (fechaInicio === "" || fechaFin === "") {
        messages.push("No se ha seleccionado un rango de fechas.");
    }

    if (filtroData === "") {
        messages.push("No ha seleccionado una base destino.");
    }

    if (messages.length > 0) {
        Swal.fire({
            title: 'Precaución',
            html: messages.join("<br>") + "<br>Se descargarán todos los clientes asignados segun los filtros seleccionado.",
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
    }

    $('#filtro-descarga-general').select2('close');
    const loadingSwal = Swal.fire({
        title: 'Descargando datos',
        text: 'Por favor, espere mientras se generan los datos.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    const baseUrl = window.location.origin;

    const params = new URLSearchParams();


    if (fechaInicio) params.append("fechaInicio", fechaInicio);
    if (fechaFin) params.append("fechaFin", fechaFin);
    if (filtroData) params.append("filtroDescarga", filtroData);
    if (type_filter) params.append("typeFilter", type_filter);
    
    try {
        const response = await fetch(`${baseUrl}/Excel/DescargarClientesAsignados?${params.toString()}`);

        if (!response.ok) {
            throw new Error("Error al descargar el archivo.");
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

        Swal.close(); // cerrar loading
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
    $('#filtro-descarga-general').val('').trigger('change');
}


$(document).ready(function () {
    $('#filtro-descarga-general').select2({
        theme: 'bootstrap-5',
        placeholder: "Seleccione una base o lista",
        width: '100%',
        allowClear: true
    });
});

function filtrar_datos_en_tabla() {
    const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin').value;
    const selectElement = document.getElementById('filtro-descarga-general');
    const selectedOption = selectElement.options[selectElement.selectedIndex];
    const optgroup = selectedOption.parentElement;

    const type_filter = optgroup.getAttribute('data-type');
    const search = selectedOption.value;
    const baseUrl = window.location.origin;
    url = `${baseUrl}/Leads/Gestion?filter=${encodeURIComponent(type_filter)}&searchfield=${encodeURIComponent(search)}`;
    if (search === '') {
        Swal.fire({
            title: 'Precaución',
            text: 'No se ha seleccionado una base destino. Se mostrarán todos los clientes asignados esto puede demorar un buen rato.',
            icon: 'warning',
            confirmButtonText: 'Aceptar'
        });
        url = `${baseUrl}/Leads/Gestion?paginaInicio=0&paginaFinal=1`;
    }
    window.location.href = url;
}

function restart_table() {
    const baseUrl = window.location.origin;
    url = `${baseUrl}/Leads/Gestion?paginaInicio=0&paginaFinal=1`;
    window.location.href = url;
}