// alerts.js
// API pública: showLoading(text), hideLoading(), withLoading(asyncFn)
// Usa SweetAlert2 si está cargado, si no crea un overlay DOM simple.

(function (global) {
    'use strict';

    function createOverlay() {
        var ov = document.createElement('div');
        ov.id = 'global-loading-overlay';
        ov.style.position = 'fixed';
        ov.style.left = '0';
        ov.style.top = '0';
        ov.style.width = '100%';
        ov.style.height = '100%';
        ov.style.background = 'rgba(0,0,0,0.5)';
        ov.style.display = 'flex';
        ov.style.alignItems = 'center';
        ov.style.justifyContent = 'center';
        ov.style.zIndex = '20000';
        ov.innerHTML = '<div style="text-align:center;color:#fff;font-size:1.15rem"><div class="spinner-border text-light" role="status" style="width:3rem;height:3rem"><span class="visually-hidden">Loading...</span></div><div id="global-loading-text" style="margin-top:.75rem"></div></div>';
        return ov;
    }

    var overlay = null;

    function showOverlay(text) {
        if (!overlay) {
            overlay = createOverlay();
            document.body.appendChild(overlay);
        }
        var txt = overlay.querySelector('#global-loading-text');
        if (txt) txt.textContent = text || 'Procesando...';
        overlay.style.display = 'flex';
    }

    function hideOverlay() {
        if (overlay) {
            overlay.style.display = 'none';
        }
    }

    function showLoading(text) {
        // Prefer Swal if available
        if (window.Swal) {
            // SweetAlert2 loading modal (non-blocking)
            window.__swalLoadingInstance = Swal.fire({
                title: text || 'Procesando...',
                allowOutsideClick: false,
                allowEscapeKey: false,
                didOpen: function () {
                    Swal.showLoading();
                },
                showConfirmButton: false,
                background: '#141420',
                color: '#fff'
            });
        } else {
            showOverlay(text);
        }
    }

    function hideLoading() {
        if (window.Swal && window.__swalLoadingInstance) {
            try { Swal.close(); } catch { }
            window.__swalLoadingInstance = null;
        } else {
            hideOverlay();
        }
    }

    // Helper para envolver una promesa/async function mostrando el loading
    async function withLoading(asyncFn, text) {
        try {
            showLoading(text);
            var result = await asyncFn();
            return result;
        } finally {
            hideLoading();
        }
    }

    // Evitar múltiples envíos
    function attachFormSubmitPreventDouble(form) {
        if (!form) return;
        form.addEventListener('submit', function (e) {
            if (form.dataset.submitted === 'true') {
                e.preventDefault();
                return;
            }
            form.dataset.submitted = 'true';
        });
    }

    // Exponer API global
    global.AppAlerts = {
        showLoading: showLoading,
        hideLoading: hideLoading,
        withLoading: withLoading,
        attachFormSubmitPreventDouble: attachFormSubmitPreventDouble
    };

})(window);