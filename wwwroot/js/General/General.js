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
    const opcionesDiv = document.getElementById(id);
    const select = opcionesDiv.closest(".custom-select");

    // Si no encuentra el contenedor, salir
    if (!select || !opcionesDiv) return;

    const isVisible = opcionesDiv.style.display === "block";

    // Cerrar todos los demás dropdowns antes
    document.querySelectorAll(".custom-options").forEach(opt => {
        opt.style.display = "none";
        opt.classList.remove("open-up");
    });

    if (!isVisible) {
        opcionesDiv.style.display = "block";

        // Medir espacio disponible
        const rect = select.getBoundingClientRect();
        const spaceBelow = window.innerHeight - rect.bottom;
        const spaceAbove = rect.top;

        if (spaceBelow < 250 && spaceAbove > 250) {
            opcionesDiv.classList.add("open-up"); // Abrir hacia arriba
        } else {
            opcionesDiv.classList.remove("open-up"); // Abrir hacia abajo
        }
    } else {
        opcionesDiv.style.display = "none";
        opcionesDiv.classList.remove("open-up");
    }
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
