const menuButton = document.getElementById("mobileMenuButton");

const sidebar = document.querySelector(".sidebar");

const overlay = document.getElementById("sidebarOverlay");

menuButton.addEventListener("click", () => {

    sidebar.classList.toggle("open");

    overlay.classList.toggle("show");

});

overlay.addEventListener("click", () => {

    sidebar.classList.toggle("open");

    overlay.classList.toggle("show");

    document.body.classList.toggle("menu-open");

});


document.querySelectorAll(".sidebar-link")
    .forEach(link => {

        link.addEventListener("click", () => {

            sidebar.classList.remove("open");

            overlay.classList.remove("show");

            document.body.classList.remove("menu-open");

        });

    });