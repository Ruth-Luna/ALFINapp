let agenciasDisponibles = [];
let tipificacionesLista = [];

async function mostrarDataCliente(data) {
    console.log('Datos del cliente:', data);
    await mostrarAgencias().then(agencias => {
        agenciasDisponibles = agencias;
    });

    await mostrarTipificaciones().then(tipificaciones => {
        tipificacionesLista = tipificaciones;
    });

    mostrarDatosProspecto(data);
    mostrarTendenciaProspecto(data);
    mostrarTelefonosManuales(data);
    mostrarTelefonosBD(data);
}

function mostrarDatosProspecto(data) {
    const map = {
        DNI: data.dni,
        X_APPATERNO: data.xappaterno,
        X_APMATERNO: data.xapmaterno,
        X_NOMBRE: data.xnombre,
        EDAD: data.edad,
        DEPARTAMENTO: data.departamento,
        PROVINCIA: data.provincia,
        DISTRITO: data.distrito,
        IDBASEUSUARIO: data.id_base,
        CAMPAÑA: data.campaña,
        OFERTA_MAX: data.oferta_max,
        TASA_MINIMA: data.tasa_minima,
        SUCURSAL_COMERCIAL: data.agencia_comercial, // Combinado en HTML como 'Sucursal AgenciaComercial'
        PLAZO: data.plazo,
        CUOTA: data.cuota,
        GRUPO_TASA: data.grupo_tasa,
        GRUPO_MONTO: data.grupo_monto,
        PERFILRO: data.perfil_ro,
        OFERTA12M: data.oferta_12m,
        TASA12M: data.tasa_12m + ' %',
        CUOTA12M: data.cuota_12m,
        OFERTA18M: data.oferta_18m,
        TASA18M: data.tasa_18m + ' %',
        CUOTA18M: data.cuota_18m,
        OFERTA24M: data.oferta_24m,
        TASA24M: data.tasa_24m + ' %',
        CUOTA24M: data.cuota_24m,
        OFERTA36M: data.oferta_36m,
        TASA36M: data.tasa_36m + ' %',
        CUOTA36M: data.cuota_36m
    };

    for (let id in map) {
        const input = document.getElementById(id);
        if (input) {
            input.value = map[id] ?? '';
        } else {
            console.warn(`No se encontró el campo con id: ${id}`);
        }
    }
}

const mapaColores = {
    "verde": "#008000",
    "amarillo": "#FFFF00",
    "naranja": "#FFA500",
    "rojo": "#FF0000",
    "verde claro": "#90EE90",
    "verde oscuro": "#006400",
    "amarillo claro": "#FFFFE0",
    "amarillo oscuro": "#FFD700",
    "naranja claro": "#FFDAB9",
    "naranja oscuro": "#FF8C00",
    "rojo claro": "#FF7F7F",
    "rojo oscuro": "#8B0000"
};

function mostrarTendenciaProspecto(data) {
    const map = {
        PROPENSION: data.propension,
        TIPO_CLIENTE: data.tipo_cliente,
        CLIENTE_NUEVO: data.cliente_nuevo,
        COLOR: data.color,
        COLOR_FINAL: data.color_final,
        TIPO_USUARIO: data.usuario
    };

    for (let id in map) {
        const input = document.getElementById(id);
        if (input) {
            input.value = map[id] ?? '';
        } else {
            console.warn(`No se encontró el campo con id: ${id}`);
        }
    }

    // Asignar colores visuales (los inputs más pequeños que no tienen id)
    const colorPrincipalHex = mapaColores[(data.color || '').toLowerCase()] || "#FFFFFF";
    const colorFinalHex = mapaColores[(data.color_final || '').toLowerCase()] || "#FFFFFF";

    const colorInputs = document.querySelectorAll('input[style*="width: 10%"]');
    if (colorInputs.length >= 2) {
        colorInputs[0].style.backgroundColor = colorPrincipalHex;
        colorInputs[1].style.backgroundColor = colorFinalHex;
    }
}

async function mostrarTelefonosManuales(data) {
    const telefonos_manuales = data.lista_telefonos_del_cliente_manual || [];
    const telefonosContainer = document.getElementById('telefonos-form-container-manual');
    const agregarTelefonoBtn = document.getElementById('agregarTelefonoBtn');

    agregarTelefonoBtn.addEventListener('click', () => {
        cargarEdicionDeTelefonos(data.id_cliente);
    });

    // Limpiar contenido previo si hay
    telefonosContainer.innerHTML = `
        <input type="hidden" name="IdAsignacion" value="${data.id_asignacion}" id="IdAsignacionCliente">
        <h4>Agregue las Tipificaciones a los clientes asignados</h4>
        <hr>
    `;

    if (telefonos_manuales.length === 0 || !Array.isArray(telefonos_manuales) || telefonos_manuales[0].telefono === null) {
        telefonosContainer.innerHTML += `
            <div class="alert alert-info" role="alert">
                No tiene números de teléfonos asignados por el usuario.
            </div>
        `;
        return;
    }
    try {
        for (let i = 0; i < telefonos_manuales.length; i++) {
            const telefonoObj = telefonos_manuales[i];
            const telefono = telefonoObj.telefono?.trim() || '';
            const comentario = telefonoObj.comentario != null ? telefonoObj.comentario : '';
            const id_tipificacion_input = `tipificacionSelect_${i}`;
            const id_fecha_visita = `fechaVisita_${i}`;
            const id_agencia = `agencia_manual_${i}`;
            const id_textarea = `comentarioAsesor_${i}`;
            const id_telefono_input = `Telefono_${i}`;

            let agenciasOptions = agenciasDisponibles.map(agencia => {
                const label = `${agencia.sucursal_comercial} - ${agencia.agencia_comercial}`;
                return `<option value="${label}">${label}</option>`;
            }).join('');

            let tipificacionOptions = tipificacionesLista.map(t => {
                return `<div class="custom-option" 
                            onclick="seleccionarOpcion(
                                '${id_tipificacion_input}'
                                , '${t.idtip}'
                                , '${t.nombretip}'
                                , '${i}')">
                                ${t.nombretip}
                        </div>`;
            }).join('');

            telefonosContainer.innerHTML += `
            <div class="form-group row align-items-center mb-4">
                <div class="col-12 col-md-4">
                    <label class="form-label">Teléfono ${i + 1}</label>
                    <input 
                        class="form-control" 
                        name="tipificaciones[${i}].Telefono"
                        id="${id_telefono_input}" 
                        type="text" 
                        value="${telefono}" 
                        readonly />

                    <div class="mt-2">
                        <textarea 
                            class="form-control" 
                            id="${id_textarea}" 
                            data-id-cliente="${data.id_cliente}"
                            name="tipificaciones[${i}].Comentario">
                            ${comentario}
                        </textarea>
                    </div>

                    <div class="mt-2">
                        <a href="javascript:void(0);" 
                            class="btn btn-primary" 
                            onclick="enviarComentario('${data.id_cliente}', '${telefono}', '${id_textarea}')">
                            <i class="fa fa-pencil"></i>
                        </a>
                    </div>
                </div>

                <div class="col-12 col-md-8 mt-2 mt-md-0">
                    <label class="form-label mt-2">Tipificación Asignada:</label>
                    <input 
                        type="hidden" 
                        id="${id_tipificacion_input}" 
                        name="tipificaciones[${i}].TipificacionId"
                        style="display: none;" />

                    <div class="custom-select">
                        <div class="custom-selected-option" id="selectedOption_${id_tipificacion_input}" 
                            onclick="mostrarOpciones('${id_tipificacion_input}')">
                            Seleccione una Tipificación
                        </div>
                        <div class="custom-options" id="opciones_${id_tipificacion_input}">
                            <input type="text" class="custom-search" id="busqueda_${id_tipificacion_input}" onkeyup="filtrarOpciones('${id_tipificacion_input}')" placeholder="Buscar...">
                            ${tipificacionOptions}
                        </div>
                    </div>
                    <div class="form-group mt-3" id="fechaVisitaContainer_${i}" style="display: none;">
                        <div class="row">
                            <div class="col">
                                <label class="form-label" for="${id_fecha_visita}">Fecha de Visita:</label>
                                <input 
                                    class="form-control" 
                                    type="date" 
                                    id="${id_fecha_visita}" 
                                    name="tipificaciones[${i}].FechaVisita" />
                            </div>
                            <div class="col">
                                <label 
                                    class="form-label" 
                                    for="${id_agencia}">
                                    Agencia Comercial:
                                </label>
                                <select 
                                    class="form-select" 
                                    name="tipificaciones[${i}].AgenciaAsignada"
                                    id="${id_agencia}">
                                    <option value="">Seleccione la Agencia</option>
                                    ${agenciasOptions}
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="row mt-1">
                        <div class="col">
                            <input 
                                class="form-control" 
                                id="id-nombre-ultima-tipificacion-${i}" 
                                type="text" 
                                value="${telefonoObj.ultima_tipificacion || ''}"
                                readonly>
                        </div>
                        <div class="col-auto">
                            <a class="btn btn-primary" href="javascript:void(0)"
                                id="buttonDerivacionContainer_${i}"
                                onclick="enviarFormularioDerivacion(
                                    '${id_agencia}',
                                    '${id_fecha_visita}',
                                    ${data.id_base},
                                    '${id_telefono_input}',
                                    1,
                                    '${data.id_asignacion}')"
                                style="display: none;">
                                <i class="fa fa-upload" aria-hidden="true"></i> Envie la Derivación
                            </a>
                        </div>
                    </div>
                </div>
            </div>
            <hr>
        `;
        }

    } catch (error) {
        console.error('Error al procesar los teléfonos manuales:', error);
    }

    // Mostrar botón de guardar
    telefonosContainer.innerHTML += `
        <div class="form-group">
            <div class="alert alert-info" role="alert">
                Use este botón para agregar las Tipificaciones de los números de TELÉFONOS ASIGNADOS POR USTED.
            </div>
            <button class="btn btn-success" type="submit">Guardar Tipificaciones</button>
        </div>
    `;
}

async function mostrarTelefonosBD(data) {
    const telefonos_bd = data.lista_telefonos_del_cliente_bd || [];
    const telefonosContainer = document.getElementById('telefonos-form-container-bd');

    telefonosContainer.innerHTML = `
        <input type="hidden" name="IdAsignacion" value="${data.id_asignacion}" id="IdAsignacion">
        <h4>Agregue las Tipificaciones a los clientes asignados</h4>
        <hr>
    `;

    if (telefonos_bd.length === 0 || !Array.isArray(telefonos_bd) || telefonos_bd[0].telefono === null) {
        telefonosContainer.innerHTML += `
            <div class="alert alert-info" role="alert">
                No tiene números de teléfonos asignados por la base de datos. Llene algún teléfono en la sección de arriba.
            </div>
        `;
        return;
    }

    try {
        for (let i = 0; i < telefonos_bd.length; i++) {
            const telefonoObj = telefonos_bd[i];
            const telefono = telefonoObj.telefono?.trim() || '';
            if (!telefono || telefono === '' || telefono === 'NULL' || telefono === '0' || telefono <= 0) {
                telefonosContainer.innerHTML += `
                `;
                continue;
            }

            const j = i + 1;
            const comentario = telefonoObj.comentario ?? '';
            const id_tipificacion_input = `tipificacionSelect_db_${j}`;
            const id_fecha_visita = `fechaVisita_db_${j}`;
            const id_agencia = `AgenciaComercialDb_${j}`;
            const id_textarea = `comentarioAsesor_teldb_${j}`;
            const id_telefono_input = `Telefono_db_${j}`;
            const descripcion_tip = telefonoObj.ultima_tipificacion ?? '';

            let agenciasOptions = agenciasDisponibles.map(agencia => {
                const label = `${agencia.sucursal_comercial} - ${agencia.agencia_comercial}`;
                return `<option value="${label}">${label}</option>`;
            }).join('');

            let tipificacionOptions = tipificacionesLista.map(t => {
                return `<div class="custom-option" 
                            id="opcion_${t.idtip}_${id_tipificacion_input}"
                            onclick="seleccionarOpcion(
                                '${id_tipificacion_input}'
                                , '${t.idtip}'
                                , '${t.nombretip}'
                                , 'db_${j}')">
                            ${t.nombretip}
                        </div>`;
            }).join('');

            console.log('Tipificaciones disponibles:', tipificacionesLista);

            // Tooltip logic
            let tipInfo = '';
            const fechaTip = telefonoObj.fecha_tipificacion;
            const añoActual = new Date().getFullYear();
            const mesActual = new Date().getMonth() + 1;
            if (fechaTip) {
                const [año, mes] = fechaTip.split('-').map(Number);
                if (año !== añoActual || mes !== mesActual) {
                    tipInfo = `
                    <div class="col-auto">
                        <span class="tooltip-symbol" title="Esta tipificación fue extraída del anterior mes">
                            <i class="fa fa-info-circle" aria-hidden="true"></i>
                        </span>
                    </div>`;
                }
            } else if (descripcion_tip) {
                tipInfo = `<div class="col-auto">
                    <span class="tooltip-symbol" title="Esta tipificación fue extraída del anterior mes">
                        <i class="fa fa-info-circle" aria-hidden="true"></i>
                    </span>
                </div>`;
            } else {
                tipInfo = `<div class="col-auto">
                    <span class="tooltip-symbol" title="Este cliente no tiene una tipificación Asignada a este número">
                        <i class="fa fa-info-circle" aria-hidden="true"></i>
                    </span>
                </div>`;
            }

            telefonosContainer.innerHTML += `
            <div class="form-group row align-items-center mb-4">
                <div class="col-12 col-md-4">
                    <label for="Telefono_${j}" class="form-label">Teléfono ${j}</label>
                    <input class="form-control" name="tipificaciones[${i}].Telefono" id="${id_telefono_input}" type="text"
                        value="${telefono}" readonly>
                    <div class="mt-2">
                        <textarea 
                            class="form-control" 
                            id="${id_textarea}" 
                            data-id-cliente="${data.id_cliente}"
                            name="tipificaciones[${i}].Comentario">
                            ${comentario}
                        </textarea>
                    </div>
                    <div class="mt-2">
                        <a href="javascript:void(0);" class="btn btn-primary" onclick="enviarComentarioTelefonoDB(
                            '${data.id_cliente}', '${telefono}', '${id_textarea}', ${j})">
                            <i class="fa fa-pencil"></i>
                        </a>
                    </div>
                </div>

                <div class="col-12 col-md-8 mt-2 mt-md-0">
                    <label for="${id_tipificacion_input}" class="form-label mt-2">Tipificación Asignada ${j}:</label>
                    <input type="hidden" name="tipificaciones[${i}].TipificacionId" id="${id_tipificacion_input}">
                    <div class="custom-select">
                        <div class="custom-selected-option" id="selectedOption_${id_tipificacion_input}" onclick="mostrarOpciones('${id_tipificacion_input}')">
                            Seleccione una Tipificación
                        </div>
                        <div class="custom-options" id="opciones_${id_tipificacion_input}">
                            <input type="text" class="custom-search" id="busqueda_${id_tipificacion_input}"
                                onkeyup="filtrarOpciones('${id_tipificacion_input}')" placeholder="Buscar...">
                            ${tipificacionOptions}
                        </div>
                    </div>

                    <div class="form-group mt-3" id="fechaVisitaContainer_db_${j}" style="display: none;">
                        <div class="row">
                            <div class="col">
                                <label for="${id_fecha_visita}" class="form-label">Fecha de Visita:</label>
                                <input class="form-control" type="date" name="tipificaciones[${i}].FechaVisita" id="${id_fecha_visita}">
                            </div>
                            <div class="col">
                                <label for="${id_agencia}" class="form-label">Agencia Comercial:</label>
                                <select class="form-select" name="tipificaciones[${i}].AgenciaAsignada" id="${id_agencia}">
                                    <option value="">Seleccione la Agencia</option>
                                    ${agenciasOptions}
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="row align-items-center mt-1">
                        <div class="col">
                            <input class="form-control" id="id-nombre-ultima-tipificacion-${j}" type="text"
                                value="${descripcion_tip}" readonly>
                        </div>
                        <div class="col-auto">
                            <a class="btn btn-primary" href="javascript:void(0)" id="buttonDerivacionContainer_db_${j}"
                                onclick="enviarFormularioDerivacion(
                                    '${id_agencia}'
                                    , '${id_fecha_visita}'
                                    , ${data.id_base}
                                    , '${id_telefono_input}'
                                    , 2
                                    , '${data.id_asignacion}')" style="display: none;">
                                <i class="fa fa-upload" aria-hidden="true"></i> Envie la Derivación
                            </a>
                        </div>
                        ${tipInfo}
                    </div>
                </div>
            </div>
            <hr>
            `;
        }
    } catch (error) {
        console.error('Error al procesar los teléfonos de la bd:', error);
    }

    // Botones finales del formulario
    telefonosContainer.innerHTML += `
        <div class="form-group">
            <div class="alert alert-info" role="alert">
                Use este botón para agregar las tipificaciones de los números de TELEFONOS ASIGNADOS POR LA EMPRESA
            </div>
            <button class="btn btn-success" type="submit">Guardar Tipificaciones</button>
            <button class="btn btn-danger" type="button" data-bs-dismiss="modal" aria-label="Close">Cancelar</button>
        </div>
    `;
}

async function mostrarAgencias() {
    const urlbase = window.location.origin;
    const url = `${urlbase}/Miscelaneos/getAgencias`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            console.error('Error al cargar las agencias:', response.statusText);
            return;
        } else {
            const result = await response.json();
            if (result.success === false) {
                console.error('Error al cargar las agencias:', result.message);
                return;
            } else {
                return result.data;
            }
        }
    } catch (error) {
        console.error('Error al cargar las agencias:', error);
    }
}

async function mostrarTipificaciones() {
    const urlbase = window.location.origin;
    const url = `${urlbase}/Miscelaneos/getTipificaciones`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            console.error('Error al cargar las tipificaciones:', response.statusText);
            return;
        } else {
            const result = await response.json();
            if (result.success === false) {
                console.error('Error al cargar las tipificaciones:', result.message);
                return;
            } else {
                return result.data;
            }
        }
    } catch (error) {
        console.error('Error al cargar las tipificaciones:', error);
    }
}