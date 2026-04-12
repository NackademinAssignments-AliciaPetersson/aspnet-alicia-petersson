document.addEventListener("DOMContentLoaded", () => {
    const selectors = {
        toggleButton: ".mobile-nav-toggle",
        desktopDropdownToggles: ".desktop-dropdown-trigger",
        flyoutMenu: ".mobile-flyout-menu",
        closeButtons: "[data-mobile-nav-close]",
        groupToggles: ".mobile-menu-group-toggle",
        group: ".mobile-menu-group",
        submenu: ".mobile-submenu"
    };

    const stateClasses = {
        navOpen: "mobile-nav-open",
        submenuOpen: "open"
    };

    const body = document.body;
    const toggleButton = document.querySelector(selectors.toggleButton);
    const flyoutMenu = document.querySelector(selectors.flyoutMenu);
    const closeButtons = document.querySelectorAll(selectors.closeButtons);
    const groupToggles = document.querySelectorAll(selectors.groupToggles);
    const desktopDropdownToggles = document.querySelectorAll(selectors.desktopDropdownToggles);

    //Desktop
    desktopDropdownToggles.forEach((dropdownToggle) => {
        dropdownToggle.addEventListener("click", (e) => {
            e.stopPropagation();
            const dropdownMenu = dropdownToggle.nextElementSibling;
            if (!dropdownMenu) {
                return;
            }

            const isExpanded = dropdownToggle.getAttribute("aria-expanded") === "true";
            const nextExpandedState = (!isExpanded).toString();

            dropdownToggle.setAttribute("aria-expanded", nextExpandedState);
            dropdownMenu.setAttribute("aria-hidden", nextExpandedState === "true" ? "false" : "true");

            dropdownMenu.classList.toggle(stateClasses.submenuOpen, !isExpanded);
        });
    });

    //Mobile
    if (toggleButton && flyoutMenu) {
        const isMenuOpen = () => body.classList.contains(stateClasses.navOpen);

        const openMenu = () => {
            body.classList.add(stateClasses.navOpen);
            toggleButton.setAttribute("aria-expanded", "true");
            flyoutMenu.setAttribute("aria-hidden", "false");
        };

        const closeMenu = () => {
            body.classList.remove(stateClasses.navOpen);
            toggleButton.setAttribute("aria-expanded", "false");
            flyoutMenu.setAttribute("aria-hidden", "true");
        };

        const toggleMenu = () => {
            if (isMenuOpen()) {
                closeMenu();
                return;
            }

            openMenu();
        };

        const toggleSubmenu = (button) => {
            const parent = button.closest(selectors.group);
            const submenu = parent?.querySelector(selectors.submenu);

            if (!parent || !submenu)
                return;

            const isExpanded = button.getAttribute("aria-expanded") === "true";
            const nextExpandedState = (!isExpanded).toString();

            button.setAttribute("aria-expanded", nextExpandedState);
            submenu.classList.toggle(stateClasses.submenuOpen, !isExpanded);
        };

        toggleButton.addEventListener("click", toggleMenu);

        closeButtons.forEach((button) => {
            button.addEventListener("click", closeMenu);
        });

        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && isMenuOpen()) {
                closeMenu();
            }
        });

        groupToggles.forEach((button) => {
            button.addEventListener("click", () => toggleSubmenu(button));
        });
    }


});