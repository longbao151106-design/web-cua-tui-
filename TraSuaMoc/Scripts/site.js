(function () {
    'use strict';
    document.querySelectorAll('[data-step]').forEach(function (button) {
        button.addEventListener('click', function () {
            var input = button.parentElement.querySelector('input');
            var value = parseInt(input.value, 10) || 1;
            input.value = Math.max(1, Math.min(20, value + parseInt(button.dataset.step, 10)));
        });
    });
    document.querySelectorAll('[data-toggle-password]').forEach(function (button) {
        button.addEventListener('click', function () {
            var input = button.parentElement.querySelector('input');
            var show = input.type === 'password';
            input.type = show ? 'text' : 'password';
            button.textContent = show ? 'Ẩn' : 'Hiện';
            button.setAttribute('aria-label', show ? 'Ẩn mật khẩu' : 'Hiện mật khẩu');
            button.setAttribute('aria-pressed', String(show));
        });
    });
    document.querySelectorAll('[data-print]').forEach(function (button) { button.addEventListener('click', function () { window.print(); }); });
    document.querySelectorAll('form[data-checkout]').forEach(function (form) {
        form.addEventListener('submit', function () { var button = form.querySelector('button[type="submit"]'); button.disabled = true; button.textContent = 'Đang tạo hóa đơn…'; });
    });
}());
