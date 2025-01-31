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
            <div class="row btn btn-danger m-1 d-inline-block" onclick="clearAllTags()">
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
                <div class="row btn btn-primary m-1 d-inline-block">
                    <span>
                        ${value}
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

$(document).ready(function () {
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
        traditional: true,
        data: {
            base_busqueda: TipoBase,
            rango_edad: RangoEdad,
            rango_tasas: RangoTasas,
            oferta: OfertaMaxima,
            tipo_cliente: TipoCliente,
            cliente_estado: ClienteEstado,
            grupo_tasa: GrupoTasa,
            grupo_monto: GrupoMonto,
            deudas: Deudas,
            "campaña": filterGroups['CampañasCreadas'],
            "usuario": filterGroups['Usuarios'],
            "propension": filterGroups['Propension'],
            "color": filterGroups['Color'],
            "color_final": filterGroups['ColorFinal'],
            "frescura": filterGroups['Frescura']
        },
        success: function (response) {
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
        error: function (xhr, status, error) {
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

function MostrarSupervisoresYDestinos() {
    $.ajax({
        type: "GET",
        url: "/Asignacion/BuscarSupervisoresYDestinos",
        traditional: true,
        data: {
        },
        success: function (response) {
            if (response.success === false) {
                Swal.fire({
                    title: 'Error al mostrar la vista',
                    text: `${response.message}`,
                    icon: 'warning',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                $('#SupervisoresYDestinosModal').modal('show');
                $("#SupervisoresYDestinos").html(response);
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error al mostrar la vista',
                text: `Error: ${error}`,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
    });
}

function InsertarAsignacionBases() {
    let baseIds = [];
    $("td[id^='idBase_']").each(function () {
        baseIds.push($(this).text());
    });
    TipoBase = document.getElementById("TipoBase").value;
    Destino = document.getElementById("destinosBasesInsercion").value;

    let supervisoresData = [];
    $("input[id^='NumeroClientesAsignados_']").each(function () {
        let id = $(this).attr('id').split('_')[1];
        let nombre = $(`#NombresCompletos_${id}`).val();
        let numeroClientes = $(this).val();
        supervisoresData.push({
            idUsuario: parseInt(id, 10), // Convertir a número
            nombresCompletos: nombre,
            numeroClientesAsignados: parseInt(numeroClientes, 10) // Convertir a número entero
        });
    });
    console.log(supervisoresData);
    console.log(baseIds);

    $.ajax({
        type: "POST",
        url: "/Asignacion/InsertarAsignacionASupervisores",
        contentType: "application/json", // Especificar el tipo de contenido
        data: JSON.stringify({ // Convertir los datos a JSON
            idClientes: baseIds,
            SupervisoresData: supervisoresData,
            fuenteBase: TipoBase,
            destino: Destino
        }),
        success: function (response) {
            if (response.success === true) {
                Swal.fire({
                    title: 'Asignacion realizada con exito',
                    text: `${response.message}`,
                    icon: 'success',
                    confirmButtonText: 'Aceptar'
                });
                return;
            } else {
                Swal.fire({
                    title: 'Error al realizar la asignacion',
                    text: `${response.message}`,
                    icon: 'error',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error al realizar la asignacion',
                text: `Error: ${error}`,
                icon: 'error',
                confirmButtonText: 'Aceptar'
            });
            return;
        }
    });
}