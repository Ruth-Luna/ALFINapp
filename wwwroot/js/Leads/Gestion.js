actualpage = 1;
function actPagGestion(page) {
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Gestion/GestionPage/${page}`;

}

function filterTablaByDetails(
    searchinput = 'busqueda-campo-gestion-leads-texto',
    typefilterId = 'filtro-busqueda-campo-gestion-leads',
    order = 'tipificacion',
    orderAsc = false
) {
    const searchInput = document.getElementById(searchinput);
    const filterInput = document.getElementById(typefilterId);
    const search = searchInput.value.toLowerCase();
    const filter = filterInput.value.toLowerCase();
    const baseUrl = window.location.origin;
    url = `${baseUrl}/Leads/Gestion?&filter=${encodeURIComponent(filter)}&searchfield=${encodeURIComponent(search)}&order=${encodeURIComponent(order)}&orderAsc=${encodeURIComponent(orderAsc)}&paginaInicio=0&paginaFinal=1`;
    if (searchInput.value === '') {
        url = `${baseUrl}/Leads/Gestion?paginaInicio=0&paginaFinal=1`;
    }
    window.location.href = url;
}

function restartDefaultTable() {
    const baseUrl = window.location.origin;
    url = `${baseUrl}/Leads/Gestion?paginaInicio=0&paginaFinal=1`;
    window.location.href = url;
}


function sortTableGestionLeads(filter, searchfield, order, orderAsc) {
    const baseUrl = window.location.origin;
    url = `${baseUrl}/Leads/Gestion?&filter=${encodeURIComponent(filter)}&searchfield=${encodeURIComponent(searchfield)}&order=${encodeURIComponent(order)}&orderAsc=${encodeURIComponent(orderAsc)}&paginaInicio=0&paginaFinal=1`;
    window.location.href = url;
}

async function cargarDataCliente(idBase, idAsignacion, traidoDe) {
    const baseUrl = window.location.origin;
    // let url
    // if (traidoDe === 'A365') {
    //     url = `${baseUrl}/Vendedor/TipificarClienteView`;
    // }
    // if (traidoDe === 'ALFIN') {
    //     url = `${baseUrl}/Vendedor/TipificarClienteDBALFINView`;
    // }
    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ id_base: idBase })
        });

        if (!response.ok) {
            const errorData = await response.json();
            Swal.fire({
                title: 'Error al cargar los datos',
                text: errorData.message || 'Hubo un error al intentar cargar los datos. Por favor, inténtalo nuevamente.',
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        } else {
            const result = await response.json();
        }
    } catch (error) {
        Swal.fire({
            title: 'Error al cargar los datos',
            text: 'Hubo un error al intentar cargar los datos. Por favor, inténtalo nuevamente.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
    }
}

function loadTipificarCliente(idBase, traidoDe) {
    let functionToExecute = '';
    if (traidoDe === 'A365') {
        functionToExecute = 'TipificarClienteView';
    }
    if (traidoDe === 'ALFIN') {
        functionToExecute = 'TipificarClienteDBALFINView';
    }
    try {
        $.ajax({
            url: `/Vendedor/${functionToExecute}`, // Controlador y acción
            type: 'GET',
            data: { id_base: idBase },
            success: function (result) {
                if (result.success === false) {
                    Swal.fire({
                        title: 'Error al cargar los datos',
                        text: result.message,
                        icon: 'error',
                        confirmButtonText: 'Aceptar'
                    });
                    return;

                }
                $('#modalContentGeneralTemplate').html(result); // Inserta el contenido de la vista en el modal
                $('#GeneralTemplateModal').modal('show'); // Muestra el modal
                $('#GeneralTemplateTitleModalLabel').text("Tipificaciones al Usuario"); // Inserta el contenido de la vista en el modal
            },
            error: function () {
                Swal.fire({
                    title: 'Error al cargar los datos',
                    text: 'Hubo un error al intentar cargar los datos. Por favor, inténtalo nuevamente.',
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
            }
        });
    } catch (error) {
        Swal.fire({
            title: 'Error al cargar los datos',
            text: 'Hubo un error al intentar cargar los datos. Por favor, inténtalo nuevamente.',
            icon: 'error',
            confirmButtonText: 'Aceptar'
        });
    }
}