let filterGroups = {
    'CampañasCreadas': [],
    'Usuarios': [],
    'Propension': [],
    'Color': [],
    'ColorFinal': [],
    'Frescura': []
};

function updateFilterTags() {
    const campana = $("#CampañasCreadas").val();
    const usuario = $("#Usuarios").val();
    const propension = $("#Propension").val();
    const color = $("#Color").val();
    const colorFinal = $("#ColorFinal").val();
    const frescura = $("#Frescura").val();

    // Add new values if not already present
    if (campana && !filterGroups['CampañasCreadas'].includes(campana)) {
        filterGroups['CampañasCreadas'].push(campana);
    }
    if (usuario && !filterGroups['Usuarios'].includes(usuario)) {
        filterGroups['Usuarios'].push(usuario);
    }
    if (propension && !filterGroups['Propension'].includes(propension)) {
        filterGroups['Propension'].push(propension);
    }
    if (color && !filterGroups['Color'].includes(color)) {
        filterGroups['Color'].push(color);
    }
    if (colorFinal && !filterGroups['ColorFinal'].includes(colorFinal)) {
        filterGroups['ColorFinal'].push(colorFinal);
    }
    if (frescura && !filterGroups['Frescura'].includes(frescura)) {
        filterGroups['Frescura'].push(frescura);
    }
    $("#CampañasCreadas").val('');
    $("#Usuarios").val('');
    $("#Propension").val('');
    $("#Color").val('');
    $("#ColorFinal").val('');
    $("#Frescura").val('');

    renderTags();
}

function renderTags() {
    const tagContainer = $("#idEtiquetasLabel");
    tagContainer.empty();

    const hasTags = Object.values(filterGroups).some(group => group.length > 0);
    if (hasTags) {
        const clearAllButton = `
            <div class="col-md-2 btn btn-danger m-1 d-inline-block" onclick="clearAllTags()">
                <span>
                    Borrar Todo
                    <i class="fa fa-trash ml-2" style="cursor: pointer;"></i>
                </span>
            </div>`;
        tagContainer.append(clearAllButton);
    }

    Object.entries(filterGroups).forEach(([type, values]) => {
        values.forEach(value => {
            const tag = `
                <div class="col-md-2 btn btn-primary m-1 d-inline-block">
                    <span>
                        ${type}: ${value}
                        <i class="fa fa-times ml-2" onclick="removeFilter('${type}', '${value}')" style="cursor: pointer;"></i>
                    </span>
                </div>`;
            tagContainer.append(tag);
        });
    });
}

function removeFilter(type, value) {
    filterGroups[type] = filterGroups[type].filter(v => v !== value);
    renderTags();
}

function clearAllTags() {
    Object.keys(filterGroups).forEach(key => {
        filterGroups[key] = [];
    });
    renderTags();
}

$(document).ready(function() {
    $("#CampañasCreadas, #Usuarios, #Propension, #Color, #ColorFinal, #Frescura")
        .on('change', updateFilterTags);
});

function BuscarAsignacionFiltrarBases() {
    TipoBase = document.getElementById("TipoBase").value;
    RangoEdad = document.getElementById("RangoEdad").value;
    RangoTasas = document.getElementById("RangoTasas").value;
    OfertaMaxima = document.getElementById("OfertaMaxima").value;
    TipoCliente = document.getElementById("TipoCliente").value;
    ClienteEstado = document.getElementById("ClienteEstado").value;
    GrupoTasa = document.getElementById("GrupoTasa").value;
    GrupoMonto = document.getElementById("GrupoMonto").value;
    Deudas = document.getElementById("Deudas").value;

    console.log(filterGroups);

    $.ajax({
        type: "GET",
        url: "/Asignacion/BuscarAsignacionFiltrarBases",
        data: {
            base_busqueda: TipoBase,
            campaña: filterGroups['CampañasCreadas'],
            oferta: OfertaMaxima,
        },
        success: function(response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al mostrar la tabla',
                    text: `${response.message}`,
                    icon: 'warning',
                    confirmButtonText: 'Aceptar'
                });
                return;                
            } else {
                $("#idTablaAsignacionLabel").html(response);
            }
        },
        error: function(xhr, status, error) {
            Swal.fire({
                title: 'Error al mostrar la tabla',
                text: `Error: ${error}`,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
    });
}