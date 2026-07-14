// ========================================
// Mobile Sidebar
// ========================================

const menuButton = document.getElementById("mobileMenuButton");
const sidebar = document.querySelector(".sidebar");
const overlay = document.getElementById("sidebarOverlay");

if (menuButton) {

    menuButton.addEventListener("click", () => {

        sidebar.classList.add("open");
        overlay.classList.add("show");
        document.body.classList.add("menu-open");

    });

}

if (overlay) {

    overlay.addEventListener("click", () => {

        sidebar.classList.remove("open");
        overlay.classList.remove("show");
        document.body.classList.remove("menu-open");

    });

}


// ========================================
// Close sidebar when normal menu clicked
// ========================================

document.querySelectorAll(".sidebar-link:not(.sidebar-dropdown-toggle)")
    .forEach(link => {

        link.addEventListener("click", () => {

            if (window.innerWidth <= 768) {

                sidebar.classList.remove("open");
                overlay.classList.remove("show");
                document.body.classList.remove("menu-open");

            }

        });

    });


// ========================================
// Quick Actions (Mobile)
// ========================================

const quickActionToggle = document.getElementById("quickActionToggle");
const quickActionMenu = document.getElementById("quickActionMenu");

if (quickActionToggle && quickActionMenu) {

    quickActionToggle.addEventListener("click", function (e) {

        if (window.innerWidth > 768)
            return;

        e.preventDefault();
        e.stopPropagation();

        quickActionToggle.classList.toggle("active");
        quickActionMenu.classList.toggle("open");

    });

}


// ========================================
// Prevent submenu click from bubbling
// ========================================

if (quickActionMenu) {

    quickActionMenu.addEventListener("click", function (e) {

        e.stopPropagation();

    });

}


// ========================================
// Close submenu if clicking outside
// ========================================

document.addEventListener("click", function () {

    if (window.innerWidth > 768)
        return;

    quickActionToggle?.classList.remove("active");
    quickActionMenu?.classList.remove("open");

});


const profile = document.getElementById("profileDropdown");
const profileMenu = document.getElementById("profileMenu");

if (profile) {

    profile.addEventListener("click", function (e) {

        e.stopPropagation();

        profileMenu.classList.toggle("show");

    });

}

document.addEventListener("click", function () {

    profileMenu?.classList.remove("show");
    notificationMenu?.classList.remove("show");

});


const notificationToggle = document.getElementById("notificationDropdown");
const notificationMenu = document.getElementById("notificationMenu");

if (notificationToggle) {

    notificationToggle.addEventListener("click", function (e) {

        e.stopPropagation();

        notificationMenu.classList.toggle("show");

    });

}