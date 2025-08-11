async function BuscarDNIAReferir(idDniLabel) {
    const dniBusqueda = document.getElementById(idDniLabel);
    const datosClienteExistente = document.getElementById('datosClienteExistente');

    if (dniBusqueda.value == "") {
        Swal.fire({
            title: 'Error al buscar',
            text: 'Debe ingresar un DNI.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    const dni = dniBusqueda.value;

    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Referido/BuscarDNIReferido?dniBusqueda=${dni}`;

    let loadingSwal = Swal.fire({
        title: 'Buscando...',
        text: 'Por favor, espera mientras se procesa la búsqueda.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    try {
        const response = await fetch(url, {
            method: 'GET'
        });
        const contentType = response.headers.get("Content-Type");
        Swal.close(); // Cierra el mensaje de carga
        if (contentType.includes("application/json") && contentType) {
            const result = await response.json();
            if (result.success === false) {
                Swal.fire({
                    title: 'Error al buscar',
                    text: result.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                datosClienteExistente.style.display = 'none';
                return;
            } 
        }
        else if (contentType.includes("text/html")) {
            Swal.fire({
                title: 'Búsqueda exitosa',
                text: "El DNI ha sido encontrado",
                icon: 'success',
                confirmButtonText: 'Aceptar'
            });
            const result = await response.text();
            datosClienteExistente.innerHTML = result;
            datosClienteExistente.style.display = 'block';
            return;
        } else {
            Swal.fire({
                title: 'Error al buscar',
                text: 'Formato de respuesta inesperado.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            datosClienteExistente.style.display = 'none';
            return;
        }
    } catch (error) {
        Swal.fire({
            title: 'Error al buscar',
            text: 'Ocurrió un error al buscar el DNI.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }
}

function ReferirDNI(dni, fuenteBase) {
    const campos = {
        NombresCompletosAsesor: 'NombresCompletosUsuario',
        ApellidosCompletosAsesor: 'ApellidosCompletosUsuario',
        DNIAsesor: 'DNIUsuario',
        NombresCompletosCliente: 'NombresCompletosCliente',
        CelularCliente: 'CelularCliente',
        AgenciaAtencion: 'AgenciaAtencion',
        FechaVisitaAgencia: 'FechaVisitaAgencia',
        CelularAsesor: 'CelularUsuario',
        CorreoAsesor: 'CorreoUsuario',
        CCIAsesor: 'CCIUsuario',
        DepartamentoAsesor: 'DepartamentoUsuario',
        BancoAsesor: 'BancoUsuario'
    };

    const valores = {};
    for (let key in campos) {
        valores[key] = document.getElementById(campos[key]).value.trim();
    }
    const UbigeoAsesor = 'NO DEFINIDO';

    const mostrarError = (mensaje) => {
        Swal.fire({
            title: 'Error al referir',
            text: mensaje,
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
    };

    const dniRegex = /^\d{8,11}$/;
    const celularRegex = /^9\d{8}$/;

    if (!dniRegex.test(valores.DNIAsesor)) {
        return mostrarError('El DNI debe contener exactamente 8 dígitos numéricos.');
    }

    if (!celularRegex.test(valores.CelularCliente) || !celularRegex.test(valores.CelularAsesor)) {
        return mostrarError('El número de celular debe contener exactamente 9 dígitos numéricos, y no puede estar vacío.');
    }

    if ([dni, fuenteBase, UbigeoAsesor, ...Object.values(valores)].some(v => v === "")) {
        return mostrarError('No se han completado todos los campos. Asegúrese de llenar todos los campos.');
    }

    $.ajax({
        url: '/Referido/ReferirCliente',
        type: 'POST',
        data: {
            dni_cliente: dni,
            fuente_base: fuenteBase,
            nombres_vendedor: valores.NombresCompletosAsesor.toUpperCase(),
            apellidos_vendedor: valores.ApellidosCompletosAsesor.toUpperCase(),
            nombres_clientes: valores.NombresCompletosCliente.toUpperCase(),
            dni_vendedor: valores.DNIAsesor,
            telefono: valores.CelularCliente,
            agencia: valores.AgenciaAtencion,
            fecha_visita: valores.FechaVisitaAgencia,
            celular: valores.CelularAsesor,
            correo: valores.CorreoAsesor,
            cci: valores.CCIAsesor,
            departamento: valores.DepartamentoAsesor.toUpperCase(),
            ubigeo: UbigeoAsesor,
            banco: valores.BancoAsesor
        },
        success: function (response) {
            Swal.fire({
                title: response.success ? 'Referido exitoso' : 'Error al referir',
                text: response.message + (response.success ? ". Puede cerrar esta página." : ""),
                icon: response.success ? 'success' : 'error',
                confirmButtonText: 'Aceptar'
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error details:", textStatus, errorThrown);
            mostrarError('Ocurrió un error al referir el cliente.');
        }
    });
}
