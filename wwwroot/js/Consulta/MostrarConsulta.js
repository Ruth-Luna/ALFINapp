let dniactivo = "";
const colores = {
    "verde": "#28a745",
    "amarillo": "#ffc107",
    "naranja": "#fd7e14",
    "rojo": "#dc3545",
    "verde claro": "#d4edda",
    "verde oscuro": "#155724",
    "amarillo claro": "#fff3cd",
    "amarillo oscuro": "#856404",
    "naranja claro": "#ffeeba",
    "naranja oscuro": "#c69500",
    "rojo claro": "#f8d7da",
    "rojo oscuro": "#721c24"
};

async function validarDNI() {
    const dniInput = document.getElementById("dnicliente");
    const datosClienteExistente = document.getElementById("datos-cliente-existente");

    const dniValue = dniInput.value;

    datosClienteExistente.classList.add("d-none");
    let data = {};
    if (dniValue === "") {
        Swal.fire({
            title: 'Error',
            text: 'El campo del Cliente es obligatorio.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    } else if (!/^\d{8}$/.test(dniValue)) {
        Swal.fire({
            title: 'Error',
            text: 'El DNI debe contener exactamente 8 dígitos y solo números.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    } else {
        let loadingSwal = Swal.fire({
            title: 'Enviando...',
            text: 'Por favor, espera mientras se busque el Dni.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        const baseUrl = window.location.origin;
        const url = `${baseUrl}/Consulta/VerificarDNIenBDoBanco?dni=${encodeURIComponent(dniValue)}`;
        try {
            const response = await fetch(url, {
                method: 'GET'
            });
            const contentType = response.headers.get("content-type");
            Swal.close();
            if (contentType && contentType.includes("application/json")) {
                const result = await response.json();
                if (result.error === true) {
                    Swal.fire({
                        title: 'Error',
                        text: result.message || 'Ocurrió un error desconocido',
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                } else if (result.existe === false) {
                    Swal.fire({
                        title: 'Cliente No Encontrado',
                        text: result.message || 'Ocurrió un error desconocido',
                        icon: 'info',
                        confirmButtonText: 'Aceptar'
                    });
                } else if (result.existe === true && result.error === false) {
                    Swal.fire({
                        title: 'Cliente Encontrado',
                        text: `El cliente ha sido encontrado exitosamente. El cliente fue traido de ${result.data["traidoDe"]}.`,
                        icon: 'success',
                        confirmButtonText: 'Aceptar'
                    });
                    datosClienteExistente.classList.remove("d-none");
                    dniactivo = dniValue; // Guardar el DNI activo
                    data = result.data;
                }
            } else {
                Swal.fire({
                    title: 'Envio Incorrecto',
                    text: 'Se esta enviando mal los datos.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        } catch (error) {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Hubo un error al verificar el DNI.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
        mostrarConsultas(data);
    }
}

function mostrarConsultas(data) {
    console.log(data);
    const mensajeBase = document.getElementById("mensaje-traido-de-base");
    if (data["traidoDe"] === "BDALFIN") {
        mensajeBase.innerHTML = `
            <div class="alert alert-primary mt-2" role="alert">
                El cliente fue extraido de la base de datos de <strong>ALFIN</strong>
            </div>
        `;
    } else if (data["traidoDe"] === "BDA365") {
        mensajeBase.innerHTML = `
            <div class="alert alert-primary mt-2" role="alert">
                El cliente fue extraido de la base de datos de <strong>A365</strong>
            </div>
        `;
    } else {
        mensajeBase.innerHTML = `
            <div class="alert alert-primary mt-2" role="alert">
                No se pudo determinar la base de datos de origen del cliente. Solo podra visualizar los datos del cliente.
            </div>
        `;
    }

    const camposTexto = {
        "nombres-completos-cliente": (d) => {
            const apePat = d.apellidoPaterno?.toUpperCase() || "";
            const apeMat = d.apellidoMaterno?.toUpperCase() || "";
            const nombres = d.nombres?.toUpperCase() || "";
            return `${apePat} ${apeMat} ${nombres}`.trim() != "" ?
                `${apePat} ${apeMat} ${nombres}`.trim() :
                "No se encontraron nombres completos";
        },
        "color-inicial-consulta-nombre": (d) => d.color.toUpperCase(),
        "color-final-consulta-nombre": (d) => d.colorFinal.toUpperCase(),
        "campaña": (d) => d.campaña.toUpperCase(),
        "oferta_maxima": (d) => "S/. " + d.ofertaMax,
        "plazo-maximo": (d) => d.plazo + " meses",
        "capacidad-de-pago": (d) => d.capacidadMax,
        "deuda-con-alfin": (d) => d.saldoDiferencialReeng,
        "nuevo-en-base": (d) => d.clienteNuevo,
        "deuda": (d) => d.deuda1,
        "nro-entidades": (d) => d.entidad1,
        "usuario-v3": (d) => d.userV3,
        "hasta_s_3k": (d) => d.tasa1 ? Math.trunc(d.tasa1) + "%" : "",
        "hasta_s_6_1k": (d) => d.tasa2 ? Math.trunc(d.tasa2) + "%" : "",
        "hasta_s_8_6k": (d) => d.tasa3 ? Math.trunc(d.tasa3) + "%" : "",
        "hasta_s_12_4k": (d) => d.tasa4 ? Math.trunc(d.tasa4) + "%" : "",
        "hasta_s_15_1k": (d) => d.tasa5 ? Math.trunc(d.tasa5) + "%" : "",
        "hasta_s_24_6k": (d) => d.tasa6 ? Math.trunc(d.tasa6) + "%" : "",
        "mas_de_s_24_6k": (d) => d.tasa7 ? Math.trunc(d.tasa7) + "%" : "",
        "nueva_tasa": (d) => d.grupoTasa || "",
        "usuario_tipo": (d) => d.usuario || "",
        "usuario_segmento": (d) => d.segmentoUser || "",
        "flag_deuda_v_oferta": (d) => d.flagDeudaVOferta || ""
    };

    Object.entries(camposTexto).forEach(([id, fn]) => {
        const el = document.getElementById(id);
        if (el) el.value = fn(data);
    });
    // Color de fondo
    document.getElementById("color-inicial-consulta").style.backgroundColor = colores[data.color?.toLowerCase()] || "#ffffff";
    document.getElementById("color-final-consulta").style.backgroundColor = colores[data.colorFinal?.toLowerCase()] || "#ffffff";
    // Botón
    if (data.idrol === 3) {
        document.getElementById("botones-consulta").innerHTML = `
            <a class="btn btn-success" href="javascript:void(0);" onclick="TakeThisClient('${data.dni}', '${data.traidoDe}')">
                <i class="fas fa-check"></i> Tipifique Este Cliente
            </a>
        `;
    } else {
        // Agregar mensaje que no puede asignar el cliente
        document.getElementById("botones-consulta").innerHTML = `
            <div class="alert alert-warning mt-2" role="alert">
                <strong>Atención:</strong> No tiene permisos para autoasignarse este cliente. Solo puede visualizar los datos.
            </div>
        `;
    }
    
}

async function validarTelefono() {
    const telefonoInput = document.getElementById("dnicliente");
    const datosClienteExistente = document.getElementById("datos-cliente-existente");

    const telefonoValue = telefonoInput.value;

    datosClienteExistente.classList.add("d-none");
    let data = {};
    if (telefonoValue === "") {
        Swal.fire({
            title: 'Error',
            text: 'El campo del Cliente es obligatorio.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    } else if (!/^\d{7,15}$/.test(telefonoValue)) {
        Swal.fire({
            title: 'Error',
            text: 'El teléfono debe contener entre 7 y 15 dígitos y solo números.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    } else {
        let loadingSwal = Swal.fire({
            title: 'Enviando...',
            text: 'Por favor, espera mientras se busque el Telefono.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        const baseUrl = window.location.origin;
        const url = `${baseUrl}/Consulta/VerificarTelefono?telefono=${encodeURIComponent(telefonoValue)}`;
        try {
            const response = await fetch(url, {
                method: 'GET'
            });
            const contentType = response.headers.get("content-type");
            Swal.close();
            if (contentType && contentType.includes("application/json")) {
                const result = await response.json();
                if (result.error === true) {
                    Swal.fire({
                        title: 'Error',
                        text: result.message || 'Ocurrió un error desconocido',
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                } else if (result.existe === false) {
                    Swal.fire({
                        title: 'Cliente No Encontrado',
                        text: result.message || 'Ocurrió un error desconocido',
                        icon: 'info',
                        confirmButtonText: 'Aceptar'
                    });
                } else if (result.existe === true && result.error === false) {
                    Swal.fire({
                        title: 'Cliente Encontrado',
                        text: `El cliente ha sido encontrado exitosamente. El cliente fue traido de ${result.data["traidoDe"]}.`,
                        icon: 'success',
                        confirmButtonText: 'Aceptar'
                    });
                    datosClienteExistente.classList.remove("d-none");
                    dniactivo = telefonoValue; // Guardar el DNI activo
                    data = result.data;
                }
            } else {
                Swal.fire({
                    title: 'Envio Incorrecto',
                    text: 'Se esta enviando mal los datos.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        } catch (error) {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Hubo un error al verificar el DNI.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
        mostrarConsultas(data);
    }
}
