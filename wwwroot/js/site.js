document.addEventListener("DOMContentLoaded", function () {

    // Toast
    document.querySelectorAll('.toast').forEach(toastEl => {
        const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
        toast.show();
    });

    window.showToast = function (message, type) {
        const container = document.querySelector('.toast-container');
        if (!container) return;

        const toastEl = document.createElement('div');
        toastEl.className = `toast text-bg-${type}`;
        toastEl.role = "alert";

        toastEl.innerHTML = `<div class="toast-body">${message}</div>`;
        container.appendChild(toastEl);

        const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
        toast.show();

        toastEl.addEventListener('hidden.bs.toast', () => {
            toastEl.remove();
        });
    };

    wireDeleteModal();
});

// Modal
window.wireDeleteModal = function () {

    const openDeleteBtn = document.getElementById("openDeleteModal");
    const confirmDeleteBtn = document.getElementById("confirmDeleteBtn");
    const modalElement = document.getElementById("confirmDeleteModal");
    const deleteForm = document.getElementById("deleteForm");

    if (!openDeleteBtn || !confirmDeleteBtn || !modalElement || !deleteForm)
        return;

    const deleteModal = new bootstrap.Modal(modalElement);

    function updateDeleteButton() {
        const anyChecked = document.querySelectorAll("input[name='ids']:checked").length > 0;
        openDeleteBtn.disabled = !anyChecked;
    }

    updateDeleteButton();

    document.querySelectorAll("input[name='ids']").forEach(cb => {
        cb.addEventListener("change", updateDeleteButton);
    });

    openDeleteBtn.onclick = () => {
        if (document.querySelectorAll("input[name='ids']:checked").length === 0) {
            showToast("Please select at least one product", "danger");
            return;
        }
        deleteModal.show();
    };

    confirmDeleteBtn.onclick = () => deleteForm.submit();
};
