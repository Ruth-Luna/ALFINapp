actualpage = 1;
function actPagGestion(page) {
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Gestion/GestionPage/${page}`;

}

function filterTablaByDetails(
    searchinput = 'busqueda-campo-gestion-leads-texto',
    typefilterId = 'filtro-busqueda-campo-gestion-leads'
) {
    const searchInput = document.getElementById(searchinput);
    const filterInput = document.getElementById(typefilterId);
    const search = searchInput.value.toLowerCase();
    const filter = filterInput.value.toLowerCase();
    const baseUrl = window.location.origin;
    let url = `${baseUrl}/Leads/Gestion?searchfield=${encodeURIComponent(search)}&filter=${encodeURIComponent(filter)}&paginaInicio=0&paginaFinal=1`;
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


function sortTableGestionLeads(filter) {
    const baseUrl = window.location.origin;
    url = `${baseUrl}/Leads/Gestion?&filter=${encodeURIComponent(filter)}paginaInicio=0&paginaFinal=1`;
    window.location.href = url;
}