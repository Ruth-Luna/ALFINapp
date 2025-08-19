
document.addEventListener("DOMContentLoaded", () => {
    if (App && App.derivaciones) {
        App.derivaciones.init();
    }

    if (App && App.reagendamientos) {
        App.reagendamientos.init();
    }
})