async function cargarReporteAsesor(idUsuario) {
    if (idUsuario === undefined || idUsuario === null || idUsuario === '') {
        return;
    }
    var asesorElement = document.getElementById('div-derivaciones-asesor');
    asesorElement.innerHTML = ''; // Limpiar el contenido previo
    $(asesorElement).parent().addClass('d-none'); // Ocultar el elemento inicialmente

    var fecha = document.getElementById('fecha-filtro').getAttribute('data');
    year = fecha.split('-')[0];
    month = fecha.split('-')[1];

    if (year === undefined || year === null || year === '') {
        year = null;
    }
    if (month === undefined || month === null || month === '') {
        month = null;
    }

    const baseUrl = window.location.origin;
    const userId = parseInt(idUsuario, 10);
    const url = `${baseUrl}/Reportes/AsesorReportes?idAsesor=${encodeURIComponent(userId)}&anio=${encodeURIComponent(year)}&mes=${encodeURIComponent(month)}`;
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
            $(asesorElement).parent().removeClass('d-none');
            cargarDerivacionesAsesorFecha();
            cargarGestionInforme();
            cargarReporteAsignacionVsGestion();
            cargarReporteDerivacionVsDesembolso();
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
    supervisorElement.innerHTML = ''; // Limpiar el contenido previo
    $(supervisorElement).parent().addClass('d-none'); // Ocultar el elemento inicialmente

    var fecha = document.getElementById('fecha-filtro').getAttribute('data');
    year = fecha.split('-')[0];
    month = fecha.split('-')[1];
    if (year === undefined || year === null || year === '') {
        year = null;
    }
    if (month === undefined || month === null || month === '') {
        month = null;
    }
    const baseUrl = window.location.origin;
    const userId = parseInt(idUsuario, 10);
    const url = `${baseUrl}/Reportes/SupervisorReportes?idSupervisor=${encodeURIComponent(userId)}&anio=${encodeURIComponent(year)}&mes=${encodeURIComponent(month)}`;
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
            $(supervisorElement).parent().removeClass('d-none');
            cargarReportesSupervisor();
            cargarReportesDerivacionesSupervisor();
            cargarGraficoDerivacionesVsDesembolsos();
            cargarDerivacionesSupervisorFecha();
            cargarDesembolsosSupervisorFecha();
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

async function cargarReportePorFechas(fecha) {
    if (fecha === undefined || fecha === null || fecha === '') {
        return;
    }
    var fechaElement = document.getElementById('div-reporteria-fechas');
    fechaElement.innerHTML = '';
    $(fechaElement).parent().addClass('d-none');
    const baseUrl = window.location.origin;
    const fechaToSend = new Date(fecha);
    const formattedDate = fechaToSend.toISOString().split('T')[0]; // Formato YYYY-MM-DD
    fecha = formattedDate;
    const url = `${baseUrl}/Reportes/ReportesFechas?fecha=${encodeURIComponent(fecha)}`;
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud de reporteria.',
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
                    icon: 'error',
                    title: 'Error al obtener el reporte',
                    text: result.message || 'Ocurrió un error desconocido',
                    confirmButtonText: 'Aceptar'
                });
            }
        } else {
            const html = await response.text();
            fechaElement.innerHTML = html;
            $(fechaElement).parent().removeClass('d-none');
            gpieasignacionFecha();
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

async function cargarReportePorMetas(fecha) {
    if (fecha === undefined || fecha === null || fecha === '') {
        return;
    }
    var fechaElement = document.getElementById('div-reporteria-fechas-por-meses');
    fechaElement.innerHTML = '';
    $(fechaElement).parent().addClass('d-none');
    const baseUrl = window.location.origin;
    const fechaToSend = new Date(fecha);
    const formattedDate = fechaToSend.toISOString().split('T')[0]; // Formato YYYY-MM-DD
    fecha = formattedDate;
    const url = `${baseUrl}/Reportes/ReportesPorMes?fecha=${encodeURIComponent(fecha)}`;
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud de reporteria.',
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
                    icon: 'error',
                    title: 'Error al obtener el reporte',
                    text: result.message || 'Ocurrió un error desconocido',
                    confirmButtonText: 'Aceptar'
                });
            }
        } else {
            const html = await response.text();
            fechaElement.innerHTML = html;
            $(fechaElement).parent().removeClass('d-none');
            gpieasignacionFecha();
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

async function cargarReportePorMeses(date) {
    console.log(date);    
    if (date === undefined || date === null || date === '') {
        return;
    }
    let dateParts = date.split('-');

    let year = parseInt(dateParts[0], 10);
    let month = parseInt(dateParts[1], 10);

    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Reportes/Reportes?anio=${encodeURIComponent(year)}&mes=${encodeURIComponent(month)}`;
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud de reporteria.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    window.location.href = url;
}

async function cargarReporteParcialMeses(mes, anio) {
    
    var fechaElement = document.getElementById('div-reporteria-fechas-por-meses');
    fechaElement.innerHTML = '';
    $(fechaElement).parent().addClass('d-none');

    let year = parseInt(anio, 10);
    let month = parseInt(mes, 10);

    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Reportes/ReportesPorMes?mes=${encodeURIComponent(month)}&año=${encodeURIComponent(year)}`;
    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud de reporteria.',
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
                    icon: 'error',
                    title: 'Error al obtener el reporte',
                    text: result.message || 'Ocurrió un error desconocido',
                    confirmButtonText: 'Aceptar'
                });
            }
        } else {
            const html = await response.text();
            fechaElement.innerHTML = html;
            $(fechaElement).parent().removeClass('d-none');
            gtablamesinforme();
            gpiemeses();
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