function cargarBaseDisponible(destino) {
    $.ajax({
        url: "/Asignacion/ObtenerBaseDisponibleDelDestino",
        type: "GET",
        data: {
            destino: destino != '' ? destino : ''
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al cargar las bases disponibles',
                    text: `${response.message}`,
                    icon: 'warning',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else if (response.success === true) {
                // Usar .val() para actualizar los valores de los inputs
                $("#NumTotalLeads").val(response.data.totalClientes);
                $("#NumLeadsDisponible").val(response.data.clientesPendientesSupervisor);
                $("#NumLeadsAsignados").val(response.data.clientesAsignadosSupervisor);
                
                // Opcional: Añadir logging para debuggear
                console.log("Datos actualizados:", {
                    total: response.data.totalClientes,
                    disponibles: response.data.clientesPendientesSupervisor,
                    asignados: response.data.clientesAsignadosSupervisor
                });
            }
        },
        error: function (xhr, status, error) {
            console.error("Error en la petición AJAX:", error);
            Swal.fire({
                title: 'Error al cargar las bases disponibles',
                text: `Error: ${error}`,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}