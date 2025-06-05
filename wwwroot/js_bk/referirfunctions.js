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

    // Mostrar el mensaje de carga
    let loadingSwal = Swal.fire({
        title: 'Buscando...',
        text: 'Por favor, espera mientras se procesa la búsqueda.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Activa la animación de carga
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

    // $.ajax({
    //     url: '/Referido/BuscarDNIReferido',
    //     type: 'GET',
    //     data: {
    //         dniBusqueda: dni
    //     },
    //     success: function (response) {
    //         if (response.success === false) {
    //             Swal.fire({
    //                 title: 'Error al buscar',
    //                 text: response.message,
    //                 icon: 'error',
    //                 confirmButtonText: 'Aceptar'
    //             });
    //         } else {
    //             Swal.fire({
    //                 title: 'Busqueda Exitosa',
    //                 text: "El DNI ha sido encontrado",
    //                 icon: 'success',
    //                 confirmButtonText: 'Aceptar'
    //             });
    //             datosClienteExistente.innerHTML = response;
    //             datosClienteExistente.style.display = 'block';
    //         }
    //     },
    //     error: function () {
    //         Swal.fire({
    //             title: 'Error al buscar',
    //             text: 'Ocurrió un error al buscar el DNI.',
    //             icon: 'error',
    //             confirmButtonText: 'Aceptar'
    //         });
    //     }
    // });
}

function ReferirDNI(dni, fuenteBase) {
    const baseFuente = fuenteBase;
    
    const NombresCompletosAsesor = document.getElementById('NombresCompletosUsuario');
    const ApellidosCompletosAsesor = document.getElementById('ApellidosCompletosUsuario');
    const DNIAsesor = document.getElementById('DNIUsuario');
    const NombresCompletosCliente = document.getElementById('NombresCompletosCliente');
    const CelularCliente = document.getElementById('CelularCliente');
    const AgenciaAtencion = document.getElementById('AgenciaAtencion');
    const FechaVisitaAgencia = document.getElementById('FechaVisitaAgencia');
    //MAS CAMPOS
    const CelularAsesor = document.getElementById('CelularUsuario');
    const CorreoAsesor = document.getElementById('CorreoUsuario');
    const CCIAsesor = document.getElementById('CCIUsuario');
    const DepartamentoAsesor = document.getElementById('DepartamentoUsuario');
    const UbigeoAsesor = 'NO DEFINIDO';
    const BancoAsesor = document.getElementById('BancoUsuario');

    const dniRegex = /^\d{8,11}$/;
    const celularRegex = /^9\d{8}$/;

    if (!dniRegex.test(DNIAsesor.value)) {
        Swal.fire({
            title: 'Error al referir',
            text: 'El DNI debe contener exactamente 8 dígitos numéricos.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (!celularRegex.test(CelularCliente.value) || !celularRegex.test(CelularAsesor.value)) {
        Swal.fire({
            title: 'Error al referir',
            text: 'El número de celular debe contener exactamente 9 dígitos numéricos, y no puede estar vacio.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (dni == "" || 
        fuenteBase == "" || 
        NombresCompletosAsesor.value == "" || 
        ApellidosCompletosAsesor.value == "" || 
        DNIAsesor.value == "" || 
        CelularCliente.value == "" || 
        AgenciaAtencion.value == "" || 
        FechaVisitaAgencia.value == "" || 
        CelularAsesor.value == "" ||
        CorreoAsesor.value == "" ||
        CCIAsesor.value == "" ||
        DepartamentoAsesor.value == "" ||
        UbigeoAsesor.value == "" ||
        BancoAsesor.value == "") {
        Swal.fire({
            title: 'Error al referir',
            text: 'No se han completado todos los campos. Asegurese de llenar todos los campos',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    $.ajax({
        url: '/Referido/ReferirCliente',
        type: 'POST',
        data: {
            dniReferir: dni,
            fuenteBase: baseFuente,
            nombresUsuario: NombresCompletosAsesor.value.toUpperCase(),
            apellidosUsuario: ApellidosCompletosAsesor.value.toUpperCase(),
            nombrescliente: NombresCompletosCliente.value.toUpperCase(),
            dniUsuario: DNIAsesor.value,
            telefono: CelularCliente.value,
            agencia: AgenciaAtencion.value,
            fechaVisita: FechaVisitaAgencia.value,

            celular: CelularAsesor.value,
            correo: CorreoAsesor.value,
            cci: CCIAsesor.value,
            departamento: DepartamentoAsesor.value.toUpperCase(),
            ubigeo: UbigeoAsesor,
            banco: BancoAsesor.value
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al referir',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });

            } else {
                Swal.fire({
                    title: 'Referido exitoso',
                    text: response.message + ". Puede cerrar esta pagina.",
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error details:", textStatus, errorThrown);
            Swal.fire({
                title: 'Error al referir',
                text: 'Ocurrió un error al referir el cliente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}