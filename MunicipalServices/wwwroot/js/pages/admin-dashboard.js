// Admin Dashboard JavaScript
document.addEventListener('DOMContentLoaded', function () {
    // Initialize tooltips
    initializeTooltips();

    // Animate stat numbers
    animateStatNumbers();

    // Add interactive effects
    initializeInteractiveEffects();

    // Auto-refresh functionality
    initializeAutoRefresh();

    // Real-time updates
    initializeRealTimeUpdates();
});

// Initialize Bootstrap tooltips
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Animate stat numbers with counting effect
function animateStatNumbers() {
    const statNumbers = document.querySelectorAll('.stat-number');

    statNumbers.forEach((stat, index) => {
        const target = parseInt(stat.textContent);
        if (isNaN(target)) return;

        stat.textContent = '0';

        setTimeout(() => {
            animateCounter(stat, target);
        }, index * 200);
    });
}

// Counter animation function
function animateCounter(element, target) {
    let current = 0;
    const increment = target / 50;
    const timer = setInterval(() => {
        current += increment;
        if (current >= target) {
            element.textContent = target;
            clearInterval(timer);
        } else {
            element.textContent = Math.floor(current);
        }
    }, 30);
}

// Initialize interactive effects
function initializeInteractiveEffects() {
    // Add hover effects to cards
    const cards = document.querySelectorAll('.stat-card, .action-card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-8px) scale(1.02)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Add click feedback
    const actionCards = document.querySelectorAll('.action-card');
    actionCards.forEach(card => {
        card.addEventListener('click', function () {
            this.style.transform = 'translateY(-5px) scale(0.98)';
            setTimeout(() => {
                this.style.transform = 'translateY(-8px) scale(1.02)';
            }, 150);
        });
    });

    // Add button ripple effect
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('click', function (e) {
            createRippleEffect(e, this);
        });
    });
}

// Create ripple effect for buttons
function createRippleEffect(event, button) {
    const ripple = document.createElement('span');
    const rect = button.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = event.clientX - rect.left - size / 2;
    const y = event.clientY - rect.top - size / 2;

    ripple.style.cssText = `
        position: absolute;
        width: ${size}px;
        height: ${size}px;
        left: ${x}px;
        top: ${y}px;
        background: rgba(255, 255, 255, 0.4);
        border-radius: 50%;
        transform: scale(0);
        animation: ripple 0.6s linear;
        pointer-events: none;
    `;

    // Add ripple animation if not exists
    if (!document.querySelector('#rippleStyle')) {
        const style = document.createElement('style');
        style.id = 'rippleStyle';
        style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(2);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }

    button.style.position = 'relative';
    button.style.overflow = 'hidden';
    button.appendChild(ripple);

    setTimeout(() => {
        ripple.remove();
    }, 600);
}

// Initialize auto-refresh functionality
function initializeAutoRefresh() {
    // Auto-refresh stats every 5 minutes
    setInterval(refreshDashboardStats, 5 * 60 * 1000);

    // Add manual refresh button
    addRefreshButton();
}

// Add refresh button to header
function addRefreshButton() {
    const adminWelcome = document.querySelector('.admin-welcome');
    if (adminWelcome) {
        const refreshBtn = document.createElement('button');
        refreshBtn.className = 'btn btn-sm btn-outline-light ms-3';
        refreshBtn.innerHTML = '<i class="fas fa-sync-alt"></i>';
        refreshBtn.title = 'Refresh Dashboard';
        refreshBtn.setAttribute('data-bs-toggle', 'tooltip');
        refreshBtn.addEventListener('click', refreshDashboardStats);

        adminWelcome.appendChild(refreshBtn);

        // Initialize tooltip for the new button
        new bootstrap.Tooltip(refreshBtn);
    }
}

// Refresh dashboard statistics
async function refreshDashboardStats() {
    const refreshBtn = document.querySelector('.admin-welcome .btn');
    if (refreshBtn) {
        refreshBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
        refreshBtn.disabled = true;
    }

    try {
        // Simulate API call - replace with actual endpoint
        await new Promise(resolve => setTimeout(resolve, 1000));

        // Update stat numbers with animation
        updateStatNumbers();

        // Show success feedback
        showNotification('Dashboard refreshed successfully!', 'success');
    } catch (error) {
        showNotification('Failed to refresh dashboard', 'error');
    } finally {
        if (refreshBtn) {
            refreshBtn.innerHTML = '<i class="fas fa-sync-alt"></i>';
            refreshBtn.disabled = false;
        }
    }
}

// Update stat numbers (simulate new data)
function updateStatNumbers() {
    const statNumbers = document.querySelectorAll('.stat-number');
    statNumbers.forEach(stat => {
        const current = parseInt(stat.textContent);
        const variation = Math.floor(Math.random() * 3) - 1; // -1, 0, or 1
        const newValue = Math.max(0, current + variation);

        if (newValue !== current) {
            animateCounter(stat, newValue);
        }
    });
}

// Show notification
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = `
        top: 20px;
        right: 20px;
        z-index: 1050;
        min-width: 300px;
    `;
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    document.body.appendChild(notification);

    // Auto-remove after 3 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 3000);
}

// Initialize real-time updates
function initializeRealTimeUpdates() {
    // Add real-time badges for recent items
    addRealTimeBadges();

    // Highlight new items
    highlightNewItems();

    // Add activity indicators
    addActivityIndicators();
}

// Add real-time badges
function addRealTimeBadges() {
    const recentItems = document.querySelectorAll('.recent-item');
    recentItems.forEach((item, index) => {
        if (index < 2) { // Mark first 2 as recent
            const header = item.querySelector('.recent-item-header');
            if (header) {
                const badge = document.createElement('span');
                badge.className = 'badge bg-success ms-2';
                badge.innerHTML = '<i class="fas fa-circle pulse"></i> New';
                header.appendChild(badge);
            }
        }
    });

    // Add pulsing animation
    const style = document.createElement('style');
    style.textContent = `
        .pulse {
            animation: pulse 2s infinite;
        }
        @keyframes pulse {
            0% { opacity: 1; }
            50% { opacity: 0.5; }
            100% { opacity: 1; }
        }
    `;
    document.head.appendChild(style);
}

// Highlight new items
function highlightNewItems() {
    const newItems = document.querySelectorAll('.recent-item:first-child');
    newItems.forEach(item => {
        item.style.background = 'linear-gradient(90deg, rgba(72, 187, 120, 0.1) 0%, transparent 100%)';
        item.style.borderLeft = '4px solid #48bb78';
        item.style.animation = 'fadeIn 0.5s ease';
    });
}

// Add activity indicators
function addActivityIndicators() {
    const statCards = document.querySelectorAll('.stat-card');
    statCards.forEach((card, index) => {
        const indicator = document.createElement('div');
        indicator.className = 'activity-indicator';
        indicator.style.cssText = `
            position: absolute;
            top: 10px;
            right: 10px;
            width: 8px;
            height: 8px;
            background: #48bb78;
            border-radius: 50%;
            animation: pulse 2s infinite;
        `;
        card.style.position = 'relative';
        card.appendChild(indicator);

        // Different delays for each card
        indicator.style.animationDelay = `${index * 0.5}s`;
    });
}

// Keyboard shortcuts
document.addEventListener('keydown', function (e) {
    if (e.ctrlKey || e.metaKey) {
        switch (e.key) {
            case 'r':
                e.preventDefault();
                refreshDashboardStats();
                break;
            case '1':
                e.preventDefault();
                window.location.href = document.querySelector('a[href*="CreateEvent"]')?.href;
                break;
            case '2':
                e.preventDefault();
                window.location.href = document.querySelector('a[href*="Events"]')?.href;
                break;
            case '3':
                e.preventDefault();
                window.location.href = document.querySelector('a[href*="ServiceRequests"]')?.href;
                break;
        }
    }
});

// Add keyboard shortcuts help
function showKeyboardShortcuts() {
    const helpModal = document.createElement('div');
    helpModal.className = 'modal fade';
    helpModal.innerHTML = `
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Keyboard Shortcuts</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-6"><kbd>Ctrl + R</kbd></div>
                        <div class="col-6">Refresh Dashboard</div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-6"><kbd>Ctrl + 1</kbd></div>
                        <div class="col-6">Create Event</div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-6"><kbd>Ctrl + 2</kbd></div>
                        <div class="col-6">Manage Events</div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-6"><kbd>Ctrl + 3</kbd></div>
                        <div class="col-6">Service Requests</div>
                    </div>
                </div>
            </div>
        </div>
    `;

    document.body.appendChild(helpModal);
    const modal = new bootstrap.Modal(helpModal);
    modal.show();

    helpModal.addEventListener('hidden.bs.modal', () => {
        helpModal.remove();
    });
}

// Add help button
document.addEventListener('DOMContentLoaded', function () {
    const helpBtn = document.createElement('button');
    helpBtn.className = 'btn btn-outline-secondary position-fixed';
    helpBtn.style.cssText = 'bottom: 20px; right: 20px; z-index: 1000; border-radius: 50%; width: 50px; height: 50px;';
    helpBtn.innerHTML = '<i class="fas fa-question"></i>';
    helpBtn.title = 'Keyboard Shortcuts (?)';
    helpBtn.setAttribute('data-bs-toggle', 'tooltip');
    helpBtn.addEventListener('click', showKeyboardShortcuts);

    document.body.appendChild(helpBtn);
    new bootstrap.Tooltip(helpBtn);
});

// Help with ? key
document.addEventListener('keydown', function (e) {
    if (e.key === '?' && !e.ctrlKey && !e.metaKey) {
        e.preventDefault();
        showKeyboardShortcuts();
    }
});