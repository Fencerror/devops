(function () {
    function getUser() {
        return (localStorage.getItem("userName") || "").trim();
    }

    function setUser(name) {
        localStorage.setItem("userName", (name || "").trim());
    }

    function clearUser() {
        localStorage.removeItem("userName");
    }

    function syncUserUI() {
        var u = getUser();

        var badge = document.getElementById("userBadge");
        if (badge) {
            if (!u) {
                badge.style.display = "none";
                badge.textContent = "";
            } else {
                badge.style.display = "inline-flex";
                badge.textContent = u;
            }
        }

        var footerUser = document.getElementById("footerUser");
        if (footerUser) {
            footerUser.textContent = u ? ("Пользователь: " + u) : "";
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        syncUserUI();

        var loginInput = document.getElementById("userName");
        if (loginInput) {
            var u = getUser();
            if (u && !loginInput.value) loginInput.value = u;

            var startForm = document.getElementById("startForm");
            if (startForm) {
                startForm.addEventListener("submit", function () {
                    var name = (loginInput.value || "").trim();
                    if (name) setUser(name);
                    syncUserUI();
                });
            }
        }

        var hiddenUserName = document.getElementById("hiddenUserName");
        if (hiddenUserName) {
            var u2 = getUser();
            if (u2) hiddenUserName.value = u2;
        }

        var clearBtn = document.getElementById("clearUser");
        if (clearBtn) {
            clearBtn.addEventListener("click", function () {
                clearUser();
                var inp = document.getElementById("userName");
                if (inp) inp.value = "";
                var hidden = document.getElementById("hiddenUserName");
                if (hidden) hidden.value = "";
                syncUserUI();
            });
        }
    });
})();
