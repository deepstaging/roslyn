document.addEventListener("DOMContentLoaded", function () {
  const nav = document.querySelector(".md-nav--primary");
  if (!nav) return;

  nav.addEventListener("click", function (e) {
    const toggle = e.target.closest(".md-nav__item--nested > .md-nav__link");
    if (!toggle) return;

    const parent = toggle.closest(".md-nav__item--nested");
    const siblings = parent.parentElement.querySelectorAll(":scope > .md-nav__item--nested");

    siblings.forEach(function (sibling) {
      if (sibling !== parent) {
        const input = sibling.querySelector(":scope > input");
        if (input) input.checked = false;
      }
    });
  });
});
