function cargarDerivacionesXAsesorSistema(DniAsesor) {
    tablaDerivacionesSistema = document.getElementById("tablaDerivacionesSistema")
    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    if (DniAsesor == "") {
        tablaGeneralSistema.style = "display: block;"
        tablaDerivacionesSistema.style = "display: none;"
        tablaDerivacionesSistema.loadedfield = "false"
        tablaDerivacionesSistema.currentDni = ""
        tablaDerivacionesGestion.style = "display: none;"
        tablaDerivacionesGestion.loadedfield = "false"
        tablaGeneralGestion.style = "display: none;"
        return;
    } else {
        $.ajax({
            type: 'GET',
            url: '/Derivacion/ObtenerDerivacionesXAsesor',
            data: {
                DniAsesor: DniAsesor
            },
            success: function (response) {
                if (response.success === false) {
                    Swal.fire({
                        title: 'Error al cargar las derivaciones',
                        text: response.message,
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                } else {
                    tablaDerivacionesSistema.style = "display: block;"
                    tablaDerivacionesSistema.loadedfield = "true"
                    tablaDerivacionesSistema.currentDni = DniAsesor
                    tablaDerivacionesGestion.style = "display: none;"
                    tablaGeneralSistema.style = "display: none;"
                    tablaGeneralGestion.style = "display: none;"
                    $('#tablaDerivacionesSistema').html(response);
                }
            },
            error: function (error) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: 'Hubo un error inesperado al cargar las derivaciones.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        });
    }
}

function cargarDerivacionesGestion(DniAsesor) {
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaDerivacionesSistema = document.getElementById("tablaDerivacionesSistema")
    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    if (tablaGeneralGestion.loadedfield === "true") {
        tablaGeneralGestion.style = "display: block;"
        tablaGeneralSistema.style = "display: none;"
        tablaDerivacionesSistema.style = "display: none;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    }
    $.ajax({
        type: 'GET',
        url: '/Derivacion/ObtenerDerivacionesGestion',
        data: {
            DniAsesor: DniAsesor
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#tablaGeneralGestion').html(response);
                tablaGeneralGestion.style = "display: block;"
                tablaGeneralGestion.loadedfield = "true"
                tablaGeneralSistema.style = "display: none;"
                tablaDerivacionesSistema.style = "display: none;"
                tablaDerivacionesGestion.style = "display: none;"
                return;
            }
        },
        error: function (error) {
            Swal.fire({
                title: 'Error al cargar las derivaciones',
                text: 'Hubo un error inesperado al cargar las derivaciones.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function cargarDerivacionesSistema() {
    tablaDerivacionesSistema = document.getElementById("tablaDerivacionesSistema")
    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    tablaGeneralSistema.style = "display: block;"
    tablaGeneralGestion.style = "display: none;"
    tablaDerivacionesSistema.style = "display: none;"
    tablaDerivacionesGestion.style = "display: none;"
}

function cargarDerivacionesGestionSupervisor(idAsesor) {
    tablaDerivacionesSistema = document.getElementById("tablaDerivacionesSistema")
    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    asesorDni = document.getElementById(idAsesor).value.toString();
    if (asesorDni === "") {
        if (tablaGeneralGestion.innerHTML.trim() !== "") {
            tablaGeneralGestion.style = "display: block;"
            tablaGeneralSistema.style = "display: none;"
            tablaDerivacionesSistema.style = "display: none;"
            tablaDerivacionesGestion.style = "display: none;"
            return;
        }
        else {
            $.ajax({
                type: 'GET',
                url: '/Derivacion/ObtenerDerivacionesGestionSupervisor',
                data: {
                },
                success: function (response) {
                    if (response.success === false) {
                        Swal.fire({
                            title: 'Error al cargar las derivaciones',
                            text: response.message,
                            icon: 'error',
                            confirmButtonText: 'Aceptar'
                        });
                        return;
                    } else {
                        $('#tablaGeneralGestion').html(response);
                        tablaGeneralGestion.style = "display: block;"
                        tablaGeneralSistema.style = "display: none;"
                        tablaDerivacionesSistema.style = "display: none;"
                        tablaDerivacionesGestion.style = "display: none;"
                        return;
                    }
                },
                error: function (error) {
                    Swal.fire({
                        title: 'Error al cargar las derivaciones',
                        text: 'Hubo un error inesperado al cargar las derivaciones.',
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                }
            });
        }
    }
    else {
        $.ajax({
            type: 'GET',
            url: '/Derivacion/ObtenerDerivacionesGestionSupervisor',
            data: {
                DniAsesor: asesorDni
            },
            success: function (response) {
                if (response.success === false) {
                    Swal.fire({
                        title: 'Error al cargar las derivaciones',
                        text: response.message,
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                } else {
                    $('#tablaDerivacionesGestion').html(response);
                    tablaGeneralGestion.style = "display: none;"
                    tablaGeneralSistema.style = "display: none;"
                    tablaDerivacionesSistema.style = "display: none;"
                    tablaDerivacionesGestion.style = "display: block;"
                    return;
                }
            },
            error: function (error) {
                Swal.fire({
                    title: 'Error al cargar las derivaciones',
                    text: 'Hubo un error inesperado al cargar las derivaciones.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        });
    }
}

function cargarDerivacionesSistemaSupervisor(idAsesor) {
    tablaDerivacionesSistema = document.getElementById("tablaDerivacionesSistema")
    tablaDerivacionesGestion = document.getElementById("tablaDerivacionesGestion")
    tablaGeneralSistema = document.getElementById("tablaGeneralSistema")
    tablaGeneralGestion = document.getElementById("tablaGeneralGestion")
    asesorDni = document.getElementById(idAsesor).value.toString();
    if (asesorDni === "") {
        tablaGeneralSistema.style = "display: block;"
        tablaGeneralGestion.style = "display: none;"
        tablaDerivacionesSistema.style = "display: none;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    } else {
        tablaGeneralSistema.style = "display: none;"
        tablaGeneralGestion.style = "display: none;"
        tablaDerivacionesSistema.style = "display: block;"
        tablaDerivacionesGestion.style = "display: none;"
        return;
    }
}