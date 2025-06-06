
document.addEventListener('DOMContentLoaded', function () {
    lucide.createIcons();
});


// Funcionalidad para los menús colapsables
document.addEventListener('DOMContentLoaded', function () {
    const triggers = document.querySelectorAll('.collapsible-trigger');

    triggers.forEach(trigger => {
        trigger.addEventListener('click', function () {
            const targetId = this.getAttribute('data-target');
            const submenu = document.getElementById(targetId);
            const chevron = this.querySelector('.chevron');

            if (submenu.classList.contains('open')) {
                submenu.classList.remove('open');
                chevron.classList.remove('rotated');
            } else {
                submenu.classList.add('open');
                chevron.classList.add('rotated');
            }
        });
    });
});