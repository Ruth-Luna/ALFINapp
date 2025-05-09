document.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.getElementById('sidebar');
    const toggle = document.getElementById('sidebarToggle');
    let expandedByHover = false;

    // --- Función para detectar escritorio ---
    function isDesktop() {
        return window.innerWidth >= 992;
    }

    // --- Hover para expandir/contraer (solo en escritorio) ---
    sidebar.addEventListener('mouseenter', () => {
        if (!isDesktop()) return;
        if (!toggle.checked) {
            toggle.checked = true;
            expandedByHover = true;
        }
    });

    sidebar.addEventListener('mouseleave', () => {
        if (!isDesktop()) return;
        if (expandedByHover) {
            toggle.checked = false;
            expandedByHover = false;
        }
    });

    // Cuando el usuario hace clic manual en el toggle, desactivamos el hover
    document.getElementById('toggleSidebar').addEventListener('click', () => {
        expandedByHover = false;
    });

    // --- Gestión de clase .active en los enlaces ---
    sidebar.addEventListener('click', (e) => {
        const link = e.target.closest('.nav-link');
        if (!link) return;

        // Si es un enlace de toggle de colapso, no cambiamos la navegación
        if (link.hasAttribute('data-bs-toggle')) return;

        // Quitamos .active de todos los nav-links que no sean toggles de colapso
        sidebar.querySelectorAll('.nav-link:not([data-bs-toggle="collapse"])').forEach(el => el.classList.remove('active'));

        // Añadimos .active al que clicamos
        link.classList.add('active');

        // Si es un sub-enlace dentro de un collapse, activamos también el toggle padre
        activateParentCollapses(link);
    });

    // --- Sincronizar .active con eventos de Bootstrap Collapse ---
    sidebar.querySelectorAll('.nav-link[data-bs-toggle="collapse"]').forEach(toggleLink => {
        // Obtenemos el selector del collapse (href o data-bs-target)
        const targetSelector = toggleLink.getAttribute('href') || toggleLink.dataset.bsTarget;
        const collapseEl = document.querySelector(targetSelector);
        if (!collapseEl) return;

        // Al abrir, marcamos el toggle como activo
        collapseEl.addEventListener('show.bs.collapse', () => {
            toggleLink.classList.add('active');
        });

        // Al cerrar, lo desmarcamos si no hay sub-enlaces activos dentro
        collapseEl.addEventListener('hide.bs.collapse', () => {
            if (!collapseEl.querySelector('.nav-link.active')) {
                toggleLink.classList.remove('active');
            }
        });
    });

    // --- Detectar URL actual y activar enlace correspondiente ---
    function activateCurrentPageLink() {
        // Obtenemos la ruta actual
        const currentPath = window.location.pathname;

        // Comparamos con cada enlace del sidebar
        sidebar.querySelectorAll('.nav-link:not([data-bs-toggle="collapse"])').forEach(link => {
            const href = link.getAttribute('href');

            // Comprobamos si el href coincide con la ruta actual
            if (href && (href === currentPath || currentPath.endsWith(href))) {
                // Quitamos active de todos los enlaces que no sean toggles
                sidebar.querySelectorAll('.nav-link:not([data-bs-toggle="collapse"])').forEach(el => el.classList.remove('active'));

                // Activamos este enlace
                link.classList.add('active');

                // Activamos también los padres collapse
                activateParentCollapses(link);
            }
        });
    }

    // Función para activar todos los collapse padres de un elemento
    function activateParentCollapses(element) {
        let parentCollapse = element.closest('.collapse');
        while (parentCollapse) {
            // Expandimos el collapse
            const bsCollapse = new bootstrap.Collapse(parentCollapse, { toggle: false });
            bsCollapse.show();

            // Activamos el toggle que controla este collapse
            const selector = `[data-bs-toggle="collapse"][href="#${parentCollapse.id}"], [data-bs-toggle="collapse"][data-bs-target="#${parentCollapse.id}"]`;
            const toggleLink = sidebar.querySelector(selector);
            if (toggleLink) toggleLink.classList.add('active');

            // Buscamos el siguiente padre collapse (si existe)
            parentCollapse = parentCollapse.parentElement.closest('.collapse');
        }
    }

    // Necesitamos ejecutar la detección después de que el contenido del sidebar sea cargado por AJAX
    const observer = new MutationObserver((mutations) => {
        for (const mutation of mutations) {
            if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                // Verificamos si se agregó contenido al sidebar
                if (document.querySelector('#sidebarToggleRender ul')) {
                    activateCurrentPageLink();
                    observer.disconnect(); // Desconectamos el observer una vez activado el enlace
                    break;
                }
            }
        }
    });

    // Configuramos el observer para monitorear cambios en el contenedor del sidebar
    observer.observe(document.getElementById('sidebarToggleRender'), { childList: true, subtree: true });
});