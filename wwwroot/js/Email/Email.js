var myModal = new bootstrap.Modal(document.getElementById('GeneralTemplateModal'));
myModal.show();

document.getElementById('GeneralTemplateModal').addEventListener('hidden.bs.modal', function () {
    myModal.show();
});

async function registrarEmail(idForm) {
    const form = document.getElementById(idForm);
    const inputs = form.querySelectorAll('input');
    const regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    let data = {};
    inputs.forEach(input => {
        data[input.id] = input.value;
    });

    if (!data['email_update_users']) {
        Swal.fire({
            icon: 'error',
            title: 'Error al enviar el email',
            text: 'El email no puede estar vacío',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    if (!regex.test(data['email_update_users'])) {
        Swal.fire({
            icon: 'error',
            title: 'Error al enviar el email',
            text: 'El email no es válido',
            confirmButtonText: 'Aceptar'
        });
        return;
    }

    let dataJson = JSON.stringify(data);
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Email/RegisterEmail`;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: dataJson
        });

        const result = await response.json();

        if (!response.ok || result.success === false) {
            Swal.fire({
                icon: 'error',
                title: 'Error al enviar el email',
                text: result.message || 'Ocurrió un error desconocido',
                confirmButtonText: 'Aceptar'
            });
        } else {
            Swal.fire({
                icon: 'success',
                title: 'Email enviado',
                text: result.message || 'El email se ha registrado con éxito',
                confirmButtonText: 'Aceptar'
            }).then(() => {
                window.location.href = `${baseUrl}/Administrador/Inicio`;
            });
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error al enviar el email',
            text: 'No se pudo enviar el email. Inténtelo de nuevo más tarde.',
            confirmButtonText: 'Aceptar'
        });
    }
}
