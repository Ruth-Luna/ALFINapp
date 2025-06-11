document.addEventListener('DOMContentLoaded', () => {
    lucide.createIcons();

    const sidebarToggle = document.getElementById('sidebar-toggle');
    const sidebarOverlay = document.getElementById('sidebar-overlay');
    const triggers = document.querySelectorAll('.collapsible-trigger');

    const isDesktopCollapsed = () => sidebarToggle.checked && window.innerWidth >= 576;

    const toggleSubmenu = (t) => {
        const submenu = document.getElementById(t.dataset.target);
        const chevron = t.querySelector('.chevron');
        const open = submenu.classList.contains('open');

        if (!isDesktopCollapsed()) {
            document
                .querySelectorAll('.sidebar__submenu.open')
                .forEach(sm => sm !== submenu && sm.classList.remove('open'));
            document
                .querySelectorAll('.chevron.rotated')
                .forEach(ch => ch !== chevron && ch.classList.remove('rotated'));
        }

        submenu.classList.toggle('open', !open);
        chevron.classList.toggle('rotated', !open);
    };

    triggers.forEach(t => {
        t.addEventListener('click', e => {
            if (isDesktopCollapsed()) {
                return;
            }
            e.preventDefault();
            toggleSubmenu(t);
        });
    });

    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', () => {
            sidebarToggle.checked = false;
            sidebarToggle.dispatchEvent(new Event('change'));
        });
    }

    sidebarToggle.addEventListener('change', () => {
        if (isDesktopCollapsed()) {
            document
                .querySelectorAll('.sidebar__submenu.open')
                .forEach(sm => sm.classList.remove('open'));
            document
                .querySelectorAll('.chevron.rotated')
                .forEach(ch => ch.classList.remove('rotated'));
        }
        localStorage.setItem('sidebarCollapsed', sidebarToggle.checked);
        document.cookie = `sidebarCollapsed=${sidebarToggle.checked};path=/;max-age=${60 * 60 * 24 * 365}`;
    });
});