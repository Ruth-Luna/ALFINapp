function BuscarDNIAReferir(idDniLabel) {
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
    $.ajax({
        url: '/Referido/BuscarDNIReferido',
        type: 'GET',
        data: {
            dniBusqueda: dni
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al buscar',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Busqueda Exitosa',
                    text: "El DNI ha sido encontrado",
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                datosClienteExistente.innerHTML = response;
                datosClienteExistente.style.display = 'block';
            }
        },
        error: function () {
            Swal.fire({
                title: 'Error al buscar',
                text: 'Ocurrió un error al buscar el DNI.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

function ReferirDNI(dni, fuenteBase) {
    const baseFuente = fuenteBase;
    
    const NombresCompletosUsuario = document.getElementById('NombresCompletosUsuario');
    const ApellidosCompletosUsuario = document.getElementById('ApellidosCompletosUsuario');
    const DNIUsuario = document.getElementById('DNIUsuario');
    const NombresCompletosCliente = document.getElementById('NombresCompletosCliente');
    const CelularCliente = document.getElementById('CelularCliente');
    const AgenciaAtencion = document.getElementById('AgenciaAtencion');
    const FechaVisitaAgencia = document.getElementById('FechaVisitaAgencia');
    //MAS CAMPOS
    const CelularUsuario = document.getElementById('CelularUsuario');
    const CorreoUsuario = document.getElementById('CorreoUsuario');
    const CCIUsuario = document.getElementById('CCIUsuario');
    const DepartamentoUsuario = document.getElementById('DepartamentoUsuario');
    const UbigeoUsuario = 'NO DEFINIDO';
    const BancoUsuario = document.getElementById('BancoUsuario');

    const dniRegex = /^\d{8,11}$/;
    const celularRegex = /^9\d{8}$/;

    if (!dniRegex.test(DNIUsuario.value)) {
        Swal.fire({
            title: 'Error al referir',
            text: 'El DNI debe contener exactamente 8 dígitos numéricos.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (!celularRegex.test(CelularCliente.value) || !celularRegex.test(CelularUsuario.value)) {
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
        NombresCompletosUsuario.value == "" || 
        ApellidosCompletosUsuario.value == "" || 
        DNIUsuario.value == "" || 
        CelularCliente.value == "" || 
        AgenciaAtencion.value == "" || 
        FechaVisitaAgencia.value == "" || 
        CelularUsuario.value == "" ||
        CorreoUsuario.value == "" ||
        CCIUsuario.value == "" ||
        DepartamentoUsuario.value == "" ||
        UbigeoUsuario.value == "" ||
        BancoUsuario.value == "") {
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
            nombresUsuario: NombresCompletosUsuario.value.toUpperCase(),
            apellidosUsuario: ApellidosCompletosUsuario.value.toUpperCase(),
            nombrescliente: NombresCompletosCliente.value.toUpperCase(),
            dniUsuario: DNIUsuario.value,
            telefono: CelularCliente.value,
            agencia: AgenciaAtencion.value,
            fechaVisita: FechaVisitaAgencia.value,

            celular: CelularUsuario.value,
            correo: CorreoUsuario.value,
            cci: CCIUsuario.value,
            departamento: DepartamentoUsuario.value.toUpperCase(),
            ubigeo: UbigeoUsuario,
            banco: BancoUsuario.value
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