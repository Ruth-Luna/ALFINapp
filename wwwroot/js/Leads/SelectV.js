function mostrarOpciones(id) {
    let opcionesDiv = document.getElementById("opciones_" + id);
    opcionesDiv.style.display = opcionesDiv.style.display === "block" ? "none" : "block";
}

function seleccionarOpcion(id, value, text, index = null) {
    document.getElementById("selectedOption_" + id).innerText = text;
    document.getElementById(id).value = value;
    let opciones = document.getElementById("opciones_" + id);
    opciones.style.display = "none";
    if (index != null) {
        verificarTipificacion(index);
    }
}

function filtrarOpciones(id) {
    let input = document.getElementById("busqueda_" + id);
    let filter = input.value.toLowerCase();
    let opcionesDiv = document.getElementById("opciones_" + id);
    let opciones = opcionesDiv.getElementsByClassName("custom-option");

    for (let i = 0; i < opciones.length; i++) {
        let texto = opciones[i].innerText.toLowerCase();
        opciones[i].style.display = texto.includes(filter) ? "block" : "none";
    }
}

document.addEventListener("click", function (event) {
    let selects = document.querySelectorAll(".custom-select");

    selects.forEach(select => {
        let opcionesDiv = select.querySelector(".custom-options");

        // Si el menú está visible y el clic NO es dentro del select, ocultarlo
        if (opcionesDiv.style.display === "block" && !select.contains(event.target)) {
            opcionesDiv.style.display = "none";
        }
    });
});
