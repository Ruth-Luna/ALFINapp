actualpage = 1;
function actPagGestion(page) {
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Gestion/GestionPage/${page}`;
        
}

function filterTableMain (id = 'busqueda-campo-gestion-leads-texto', typefilter = 'busqueda-campo-gestion-leads') {
    const input = document.getElementById(id);
    const typefilter = document.getElementById(typefilter);
    const filter = input.value.toUpperCase();
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Leads/FilterGestion?text_camp=${encodeURIComponent(dniValue)}`;
}