// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// "Moja lista" dropdown – otwieranie na hover.
// Uwaga: element istnieje tylko dla zalogowanego użytkownika.
const myList = document.getElementById("myList");

if (myList) {
    const menu = myList.querySelector(".dropdown-menu");
    const toggle = myList.querySelector(".dropdown-toggle");

    myList.addEventListener("mouseenter", () => {
        menu?.classList.add("show");
        toggle?.setAttribute("aria-expanded", "true");
    });

    myList.addEventListener("mouseleave", () => {
        menu?.classList.remove("show");
        toggle?.setAttribute("aria-expanded", "false");
    });
}
