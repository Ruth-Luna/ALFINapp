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