flatpickr("#mes-selector", {
  plugins: [
    new monthSelectPlugin({
      shorthand: true,
      dateFormat: "Y-m",
      altFormat: "F Y"
    })
  ]
});

function show_options_refac(id) {
    let opcionesDiv = document.getElementById(id);
    opcionesDiv.style.display = opcionesDiv.style.display === "block" ? "none" : "block";
}

function select_option_refac(id, idSelect, idOptions, value, text, callback = null) {
    document.getElementById(idSelect).innerText = text;
    document.getElementById(id).value = value;
    let opciones = document.getElementById(idOptions);
    opciones.style.display = "none";
    if (typeof callback === "function") {
        callback();  // Ejecutas la función pasada
    }
}

function filter_option_refac(idInput, idOptions) {
    let input = document.getElementById(idInput);
    let filter = input.value.toLowerCase();
    let opcionesDiv = document.getElementById(idOptions);
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
