document.addEventListener("DOMContentLoaded", () => {

    const toggles = document.querySelectorAll(".theme-toggle");
    if (!toggles.length) return;

    const setTheme = (theme) => {
        const isDark = theme === "dark";

        if (isDark) {
            document.documentElement.setAttribute("data-theme", "dark");
        } else {
            document.documentElement.removeAttribute("data-theme");
        }

        localStorage.setItem("theme", isDark ? "dark" : "light");

        toggles.forEach(btn => {
            const icon = btn.querySelector("i");
            icon.className = isDark
                ? "bi bi-sun-fill"
                : "bi bi-moon-fill";
        });
    };

    const storedTheme = localStorage.getItem("theme") || "light";
    setTheme(storedTheme);

    toggles.forEach(btn => {
        btn.addEventListener("click", () => {
            const currentTheme =
                document.documentElement.getAttribute("data-theme") === "dark"
                    ? "dark"
                    : "light";

            const nextTheme = currentTheme === "dark" ? "light" : "dark";
            setTheme(nextTheme);
        });
    });

});

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


document.addEventListener('DOMContentLoaded', () => {
    if (!window.dashboardConfig) return;
    Dashboard.init(window.dashboardConfig);
});

const Dashboard = (() => {

    let charts = {};

    /* ---------- INIT ---------- */

    async function init(config) {
        try {
            const data = await fetchData(config.dataUrl);
            renderStats(data);
            renderLists(data);
            renderCharts(data);
            bindExport(config.exportUrl);
        } catch (err) {
            console.error('Dashboard error:', err);
        }
    }

    /* ---------- DATA ---------- */

    async function fetchData(url) {
        const res = await fetch(url);
        if (!res.ok) throw new Error('Failed to load dashboard data');
        return res.json();
    }

    /* ---------- STATS ---------- */

    function renderStats(data) {
        setText('totalProducts', data.totalProducts);
        setText('stockIn', data.stockInThisMonth);
        setText('stockOut', data.stockOutThisMonth);
        setText('totalCategories', data.totalCategories);
    }

    /* ---------- LISTS ---------- */

    function renderLists(data) {
        renderRecentMovements(data.recentMovements);
        renderLowStockProducts(data.lowStockProducts);
        renderCategoriesOverview(data.categoriesOverview);
        renderRecentProducts(data.recentProducts);
    }

    function renderRecentMovements(items) {
        renderList('recentMovements', items, m => `
            <div class="list-item">
                <div class="list-item-icon ${m.movementType === 1 ? 'icon-success' : 'icon-danger'}">
                    <i class="bi bi-arrow-${m.movementType === 1 ? 'up' : 'down'}-circle"></i>
                </div>
                <div class="list-item-content">
                    <p class="list-item-title">${m.productName}</p>
                    <p class="list-item-subtitle">${m.quantity} units • ${formatDate(m.createdAt)}</p>
                </div>
            </div>
        `, 'No recent movements');
    }

    function renderLowStockProducts(items) {
        renderList('lowStockProducts', items, p => `
            <div class="list-item">
                <div class="list-item-icon icon-warning">
                    <i class="bi bi-exclamation-triangle"></i>
                </div>
                <div class="list-item-content">
                    <p class="list-item-title">${p.name}</p>
                    <p class="list-item-subtitle">${p.sku} • Stock: ${p.stock}</p>
                </div>
            </div>
        `, 'All products have sufficient stock');
    }

    function renderCategoriesOverview(items) {
        renderList('categoriesOverview', items, c => `
            <div class="list-item">
                <div class="list-item-content">
                    <p class="list-item-title">${c.name}</p>
                    <p class="list-item-subtitle">${c.productCount} products</p>
                </div>
                <div class="list-item-badge">${c.productCount}</div>
            </div>
        `, 'No categories found');
    }

    function renderRecentProducts(items) {
        renderList('recentProducts', items, p => `
            <div class="list-item">
                <div class="list-item-content">
                    <p class="list-item-title">${p.name}</p>
                    <p class="list-item-subtitle">${p.category} • ${p.sku}</p>
                </div>
                <div class="list-item-badge ${p.isActive ? 'badge-success' : 'badge-inactive'}">
                    ${p.isActive ? 'Active' : 'Inactive'}
                </div>
            </div>
        `, 'No products found');
    }

    function renderList(containerId, items, template, emptyText) {
        const el = document.getElementById(containerId);
        if (!items || items.length === 0) {
            el.innerHTML = `<p class="no-data">${emptyText}</p>`;
            return;
        }
        el.innerHTML = items.map(template).join('');
    }

    /* ---------- CHARTS ---------- */

    function renderCharts(data) {
        renderStockInOutChart(data.stockInOutChart);
        renderProductsByCategoryChart(data.productsByCategoryChart);
        renderMonthlyStockChart(data.monthlyStockChart);
        renderStackedCategoryChart(data.stackedCategoryChart);
    }

    function createChart(key, ctx, config) {
        charts[key]?.destroy();
        charts[key] = new Chart(ctx, {
            ...config,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                ...config.options
            }
        });
    }

    function renderStockInOutChart(data) {
        const stockIn = data.find(x => x.movementType === 1)?.total ?? 0;
        const stockOut = data.find(x => x.movementType === 2)?.total ?? 0;

        createChart('stockInOut', getCtx('stockInOutChart'), {
            type: 'bar',
            data: {
                labels: ['Stock In', 'Stock Out'],
                datasets: [{
                    data: [stockIn, stockOut],
                    borderRadius: 6
                }]
            },
            options: {
                plugins: { legend: { display: false } },
                scales: { y: { beginAtZero: true } }
            }
        });
    }

    function renderProductsByCategoryChart(data) {
        createChart('productsByCategory', getCtx('productsByCategoryChart'), {
            type: 'doughnut',
            data: {
                labels: data.map(x => x.name),
                datasets: [{ data: data.map(x => x.productCount) }]
            },
            options: {
                plugins: { legend: { position: 'bottom' } }
            }
        });
    }

    function renderMonthlyStockChart(data) {
        const months = [...new Set(data.map(x => `${x.year}-${x.month}`))];

        const sum = (m, type) =>
            data.filter(x => `${x.year}-${x.month}` === m && x.movementType === type)
                .reduce((a, b) => a + b.total, 0);

        createChart('monthlyStock', getCtx('monthlyStockChart'), {
            type: 'line',
            data: {
                labels: months,
                datasets: [
                    { label: 'Stock In', data: months.map(m => sum(m, 1)), tension: 0.3 },
                    { label: 'Stock Out', data: months.map(m => sum(m, 2)), tension: 0.3 }
                ]
            }
        });
    }

    function renderStackedCategoryChart(data) {
        createChart('stackedCategory', getCtx('stackedCategoryChart'), {
            type: 'bar',
            data: {
                labels: data.map(x => x.category),
                datasets: [
                    { label: 'Stock In', data: data.map(x => x.stockIn) },
                    { label: 'Stock Out', data: data.map(x => x.stockOut) }
                ]
            },
            options: {
                scales: {
                    x: { stacked: true },
                    y: { stacked: true }
                }
            }
        });
    }

    /* ---------- EXPORT ---------- */

    function bindExport(url) {
        document.getElementById('btnExportReport')
            ?.addEventListener('click', () => window.location.href = url);
    }

    /* ---------- UTILS ---------- */

    function setText(id, value) {
        const el = document.getElementById(id);
        if (el) el.textContent = value;
    }

    function getCtx(id) {
        return document.getElementById(id);
    }

    function formatDate(dateString) {
        const d = new Date(dateString);
        const diffH = (Date.now() - d) / 36e5;
        if (diffH < 1) return 'Just now';
        if (diffH < 24) return `${Math.floor(diffH)}h ago`;
        if (diffH < 168) return `${Math.floor(diffH / 24)}d ago`;
        return d.toLocaleDateString();
    }

    return { init };
})();
