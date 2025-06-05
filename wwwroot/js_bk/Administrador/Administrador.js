function crearGrafico(supervisores, clientesDerivados, idPastel) {
    var ctx = document.getElementById(idPastel).getContext('2d'); // 🖌️ Usamos idPastel dinámicamente

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: supervisores,
            datasets: [{
                data: clientesDerivados,
                backgroundColor: [
                    '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40',
                    '#FF5733', '#C70039', '#900C3F', '#581845' // Más colores por si hay más datos
                ],
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'bottom',
                }
            }
        }
    });
}
