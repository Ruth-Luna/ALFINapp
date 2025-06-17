function cargarBaseDisponible(destino) {
    $.ajax({
        url: "/Asignacion/ObtenerBaseDestino",
        type: "GET",
        data: {
            filtro: destino != '' ? destino : ''
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

function EnviarDatos() {
    const asignaciones = [];

    Swal.fire({
        title: 'Procesando...',
        text: 'Por favor, espera mientras se procesa la asignación.',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    document.querySelectorAll('tbody tr').forEach((row) => {
        const index = row.getAttribute('data-index');
        const numClientesInput = row.querySelector(`input[name="asignacionasesor[${index}].NumClientes"]`);
        const idVendedorInput = row.querySelector(`input[name="asignacionasesor[${index}].IdVendedor"]`);

        if (numClientesInput && idVendedorInput) {
            const numClientes = numClientesInput.value;
            const idVendedor = idVendedorInput.value;

            if (numClientes > 0) {
                asignaciones.push({
                    NumClientes: parseInt(numClientes, 10),
                    IdVendedor: parseInt(idVendedor, 10)
                });
            }
        }
    });

    const selectElement = document.getElementById('BaseDestino');
    const selectedOption = selectElement.options[selectElement.selectedIndex];
    const optgroup = selectedOption.parentElement;

    const type_filter = optgroup.getAttribute('data-type');
    const selectAsesorBase = selectedOption.value;

    $.ajax({
        url: '/Asesor/AsignarClientesAAsesores',
        type: 'POST',
        data: {
            asignacionasesor: asignaciones,
            filter: selectAsesorBase,
            type_filter: type_filter
        },
        success: function (response) {
            Swal.close();
            if (response.success === false) {
                Swal.fire({
                    title: 'Error en la asignación',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            } else {
                Swal.fire({
                    title: 'Asignación Exitosa',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                }).then(() => {
                    document.getElementById('guardarAsignacionesContainer').style.display = 'none';
                    location.reload();
                });
            }
        },
        error: function () {
            Swal.close();

            Swal.fire({
                title: 'Error en la Reasignación',
                text: 'Error en la Reasignación',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
        }
    });
}

$(document).ready(function () {
    $('#BaseDestino').select2({
        placeholder: "Seleccione una base o lista",
        allowClear: true,
        width: '100%'
    });
});
