function BuscarReferidosDeDNI(idDNI) {
    $.ajax({
        url: "/Referido/BuscarReferidosDeDNI",
        data: {
            DNI: document.getElementById(idDNI).value
        },
        type: "GET",
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message,
                    confirmButtonText: 'Aceptar'
                });
                $("#divReferidos").css("display", "none");
                return;
            } else {
                $("#divReferidos").html(response);
                $("#divReferidos").css("display", "block");
            }
        },
        error: function (response) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error al buscar referidos',
                confirmButtonText: 'Aceptar'
            });
            $("#divReferidos").css("display", "none");
        }
    });
}