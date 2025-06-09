document.addEventListener('DOMContentLoaded', () => {
    lucide.createIcons();

    const sidebarToggle = document.getElementById('sidebar-toggle');
    const collapsibleTriggers = document.querySelectorAll('.collapsible-trigger');

    const isSidebarCollapsed = () => sidebarToggle?.checked;

    const toggleSubmenu = (trigger) => {
        const targetId = trigger.getAttribute('data-target');
        const submenu = document.getElementById(targetId);
        const chevron = trigger.querySelector('.chevron');

        const isOpen = submenu.classList.contains('open');

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
            if (isSidebarCollapsed()) return;
            e.preventDefault();
            toggleSubmenu(trigger);
        });
    });

    sidebarToggle?.addEventListener('change', () => {
        if (isSidebarCollapsed()) {
            document.querySelectorAll('.sidebar__submenu.open').forEach(sm => sm.classList.remove('open'));
            document.querySelectorAll('.chevron.rotated').forEach(ch => ch.classList.remove('rotated'));
        }
    });

    // 👇 Submenu flotante con retardo al ocultar
    const submenuTimeouts = new Map();

    document.querySelectorAll('.sidebar__menu-item').forEach(item => {
        const button = item.querySelector('.sidebar__menu-button');
        const submenu = item.querySelector('.sidebar__submenu');

        if (!submenu || !button) return;

        item.addEventListener('mouseenter', () => {
            if (!isSidebarCollapsed()) return;

            clearTimeout(submenuTimeouts.get(submenu));
            submenu.classList.add('floating');
        });

        item.addEventListener('mouseleave', () => {
            if (!isSidebarCollapsed()) return;

            const timeoutId = setTimeout(() => {
                submenu.classList.remove('floating');
            }, 300); // Tiempo de espera para ocultar
            submenuTimeouts.set(submenu, timeoutId);
        });

        submenu.addEventListener('mouseenter', () => {
            clearTimeout(submenuTimeouts.get(submenu));
        });

        submenu.addEventListener('mouseleave', () => {
            const timeoutId = setTimeout(() => {
                submenu.classList.remove('floating');
            }, 300);
            submenuTimeouts.set(submenu, timeoutId);
        });
    });
});
