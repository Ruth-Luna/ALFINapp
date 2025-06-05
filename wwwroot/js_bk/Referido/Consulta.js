async function BuscarReferidosDeDNI(idDNI) {
    const dni = document.getElementById(idDNI);
    if (dni === "") {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Debe ingresar un DNI.',
            confirmButtonText: 'Aceptar'
        });
        $("#divReferidos").css("display", "none");
        return;
    }

    let loadingSwal = Swal.fire({
        title: 'Enviando...',
        text: 'Por favor, espera mientras se procesa la solicitud.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading(); // Activa la animación de carga
        }
    });

    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Referido/BuscarReferidosDeDNI?DNI=${dni.value}`;

    try {
        const response = await fetch(url, {
            method: 'GET'
        });
        const contentType = response.headers.get("Content-Type");
        Swal.close();
        if (contentType.includes("application/json") && contentType) {
            const result = await response.json();
            if (result.success === false) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: result.message,
                    confirmButtonText: 'Aceptar'
                });
                document.getElementById("divReferidos").style.display = "none";
                return;
            } else {
                document.getElementById("divReferidos").innerHTML = result.data;
                document.getElementById("divReferidos").style.display = "block";
            }
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Ocurrió un error al buscar los referidos.',
            confirmButtonText: 'Aceptar'
        });
        document.getElementById("divReferidos").style.display = "none";
        return;
    }
    
    // $.ajax({
    //     url: "/Referido/BuscarReferidosDeDNI",
    //     data: {
    //         DNI: document.getElementById(idDNI).value
    //     },
    //     type: "GET",
    //     success: function (response) {
    //         if (response.success === false) {
    //             Swal.fire({
    //                 icon: 'error',
    //                 title: 'Error',
    //                 text: response.message,
    //                 confirmButtonText: 'Aceptar'
    //             });
    //             $("#divReferidos").css("display", "none");
    //             return;
    //         } else {
    //             $("#divReferidos").html(response);
    //             $("#divReferidos").css("display", "block");
    //         }
    //     },
    //     error: function (response) {
    //         Swal.fire({
    //             icon: 'error',
    //             title: 'Error',
    //             text: 'Error al buscar referidos',
    //             confirmButtonText: 'Aceptar'
    //         });
    //         $("#divReferidos").css("display", "none");
    //     }
    // });
}