document.addEventListener('DOMContentLoaded', () => {
    lucide.createIcons();

    const sidebarToggle = document.getElementById('sidebar-toggle');
    const collapsibleTriggers = document.querySelectorAll('.collapsible-trigger');

    // Estado actual del sidebar (expandido o colapsado)
    const isSidebarCollapsed = () => sidebarToggle?.checked;

    // Función para abrir/cerrar submenu por click
    const toggleSubmenu = (trigger) => {
        const targetId = trigger.getAttribute('data-target');
        const submenu = document.getElementById(targetId);
        const chevron = trigger.querySelector('.chevron');

        const isOpen = submenu.classList.contains('open');

        // Cierra todos los demás submenus si el sidebar no está colapsado
        if (!isSidebarCollapsed()) {
            document.querySelectorAll('.sidebar__submenu.open').forEach(sm => {
                if (sm !== submenu) sm.classList.remove('open');
            });
            document.querySelectorAll('.chevron.rotated').forEach(ch => {
                if (ch !== chevron) ch.classList.remove('rotated');
            });
        }

        submenu.classList.toggle('open', !isOpen);
        chevron.classList.toggle('rotated', !isOpen);
    };

    collapsibleTriggers.forEach(trigger => {
        trigger.addEventListener('click', (e) => {
            // Si el sidebar está colapsado, no usar JS (CSS hover lo maneja)
            if (isSidebarCollapsed()) return;
            e.preventDefault();
            toggleSubmenu(trigger);
        });
    });

    // Opcional: actualiza los íconos si el sidebar cambia de estado
    sidebarToggle?.addEventListener('change', () => {
        // Cierra todos los submenús abiertos si colapsa
        if (isSidebarCollapsed()) {
            document.querySelectorAll('.sidebar__submenu.open').forEach(sm => sm.classList.remove('open'));
            document.querySelectorAll('.chevron.rotated').forEach(ch => ch.classList.remove('rotated'));
        }
    });
});