window.StockCoreTour = (() => {

    let tour;

    function createTour() {
        tour = new Shepherd.Tour({
            useModalOverlay: true,
            defaultStepOptions: {
                cancelIcon: { enabled: true },
                scrollTo: { behavior: 'smooth', block: 'center' },
                classes: 'stockcore-tour'
            }
        });

        tour.addStep({
            id: 'dashboard-title',
            text: `
                <strong>Dashboard</strong><br/><br/>
                This is your main workspace. From here you can quickly
                see how your inventory is doing.
            `,
            attachTo: { element: '.dashboard-header h1', on: 'bottom' },
            buttons: [
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'dashboard-summary',
            text: `
                These cards give you a quick summary of products,
                categories, and stock movements.
            `,
            attachTo: { element: '.stats-grid', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'navigation',
            text: `
                Use this menu to move through the system and manage
                your inventory.
            `,
            attachTo: { element: '.navbar-nav-left', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'products',
            text: `
                In Products, you can create and manage all the items
                in your inventory.
            `,
            attachTo: { element: 'a[href="/Products"]', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'categories',
            text: `
                Categories help you organize products and keep
                everything easy to find.
            `,
            attachTo: { element: 'a[href="/Categories"]', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'stock',
            text: `
                In Stock, you register incoming and outgoing items
                and review stock movements.
            `,
            attachTo: { element: 'a[href="/Stock"]', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'charts',
            text: `
                These charts help you understand trends and see
                how your inventory changes over time.
            `,
            attachTo: { element: '.dashboard-analytics', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'login',
            text: `
                To add, edit, or delete information, you must be logged in.
                This keeps your inventory secure.
            `,
            attachTo: { element: '.user-link', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'privacy',
            text: `
                The Privacy page explains how user data is handled
                inside the application.
            `,
            attachTo: { element: 'a[href="/Home/Privacy"]', on: 'bottom' },
            buttons: [
                { text: 'Back', action: () => tour.back() },
                { text: 'Next', action: () => tour.next() }
            ]
        });

        tour.addStep({
            id: 'finish',
            text: `
                You are ready to use StockCore.<br/><br/>
                You can restart this tour anytime using the help icon.
            `,
            buttons: [
                { text: 'Finish', action: () => tour.complete() }
            ]
        });
    }

    function start() {
        createTour();
        tour.start();
    }

    function resumeIfNeeded() { }

    return {
        start,
        resumeIfNeeded
    };

})();
