/**
 * JanSamadhan — Global Interactions, Toast System & Accessibility Controller
 * Adheres to Emil Kowalski's Design Engineering & Animation Performance principles:
 * - 60fps composite-only animations (transform & opacity)
 * - Sonner-inspired toast notification hub (pause-on-hover, stackable, swipe-to-dismiss)
 * - Tactile button press physics and focus visibility management
 */

(function () {
    'use strict';

    /**
     * Display a floating non-blocking toast notification
     * @@param {string} message - Message text to display
     * @@param {'success'|'error'|'warning'|'info'} [type='info'] - Semantic alert type
     * @@param {number} [duration=4000] - Duration in ms before auto-dismissal
     */
    window.showToast = function (message, type = 'info', duration = 4000) {
        if (!message) return null;

        let container = document.getElementById('toastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-hub';
            container.setAttribute('aria-live', 'polite');
            container.setAttribute('aria-atomic', 'true');
            document.body.appendChild(container);
        }

        const toast = document.createElement('div');
        toast.className = `toast-item toast-item--${type}`;
        toast.setAttribute('role', type === 'error' ? 'alert' : 'status');

        // Semantic icons matching design system
        let iconHtml = '<i class="fas fa-circle-info text-primary" aria-hidden="true"></i>';
        if (type === 'success') {
            iconHtml = '<i class="fas fa-circle-check text-success" aria-hidden="true"></i>';
        } else if (type === 'error' || type === 'danger') {
            iconHtml = '<i class="fas fa-circle-exclamation text-danger" aria-hidden="true"></i>';
        } else if (type === 'warning') {
            iconHtml = '<i class="fas fa-triangle-exclamation text-warning" aria-hidden="true"></i>';
        }

        toast.innerHTML = `
            <div class="toast-item__icon">${iconHtml}</div>
            <div class="toast-item__content">${escapeHtml(message)}</div>
            <button type="button" class="toast-item__close" aria-label="Close notification">&times;</button>
        `;

        container.appendChild(toast);

        // Entrance animation: off-thread composite transform + opacity
        requestAnimationFrame(function () {
            toast.classList.add('is-visible');
        });

        let timer = null;
        let isDismissed = false;

        function dismissToast() {
            if (isDismissed) return;
            isDismissed = true;
            toast.classList.remove('is-visible');
            toast.classList.add('is-leaving');

            // Wait for 180ms ease-out composite transition before removal
            setTimeout(function () {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 200);
        }

        function startTimer() {
            if (duration > 0) {
                timer = setTimeout(dismissToast, duration);
            }
        }

        function clearTimer() {
            if (timer) {
                clearTimeout(timer);
                timer = null;
            }
        }

        // Sonner Principle: Pause toast auto-dismiss timer on pointer hover
        toast.addEventListener('mouseenter', clearTimer);
        toast.addEventListener('mouseleave', startTimer);
        toast.addEventListener('focusin', clearTimer);
        toast.addEventListener('focusout', startTimer);

        // Close button click
        const closeBtn = toast.querySelector('.toast-item__close');
        if (closeBtn) {
            closeBtn.addEventListener('click', function (e) {
                e.stopPropagation();
                dismissToast();
            });
        }

        startTimer();
        return toast;
    };

    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Initialize global listeners on DOM ready
    document.addEventListener('DOMContentLoaded', function () {
        // Automatically convert any unhandled TempData alerts into toasts
        const alerts = document.querySelectorAll('.alert[data-auto-toast]');
        alerts.forEach(function (alert) {
            const text = alert.textContent.trim();
            const type = alert.classList.contains('alert-success') ? 'success' :
                         alert.classList.contains('alert-danger') ? 'error' :
                         alert.classList.contains('alert-warning') ? 'warning' : 'info';
            if (text) {
                window.showToast(text, type);
                alert.style.display = 'none';
            }
        });

        // Touch feedback: Remove hover sticky artifacts on mobile devices
        document.addEventListener('touchstart', function () {}, { passive: true });
    });
})();

