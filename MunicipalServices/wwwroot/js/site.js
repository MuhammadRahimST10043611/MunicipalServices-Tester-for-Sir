$(document).ready(function () {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Add loading animation to buttons on form submission
    $('form').on('submit', function () {
        $(this).find('button[type="submit"]').prop('disabled', true);
    });

    // Animate statistics cards
    if ($('.stats-card').length) {
        animateStatsCards();
    }

    // Initialize service card hover effects
    initializeServiceCards();
});

// Animate statistics cards with counting effect
function animateStatsCards() {
    $('.stats-card h3').each(function () {
        var $this = $(this);
        var countTo = parseInt($this.text());

        if (!isNaN(countTo)) {
            $({ countNum: 0 }).animate({
                countNum: countTo
            }, {
                duration: 2000,
                easing: 'swing',
                step: function () {
                    $this.text(Math.floor(this.countNum));
                },
                complete: function () {
                    $this.text(countTo);
                }
            });
        }
    });
}

// Initialize service card interactions
function initializeServiceCards() {
    $('.service-card:not(.disabled)').on('mouseenter', function () {
        $(this).find('.card-icon').addClass('animate-bounce');
    }).on('mouseleave', function () {
        $(this).find('.card-icon').removeClass('animate-bounce');
    });

    // Add click effect for disabled cards
    $('.service-card.disabled').on('click', function (e) {
        e.preventDefault();
        showComingSoonMessage();
    });
}

// Show coming soon message for disabled features
function showComingSoonMessage() {
    if ($('#comingSoonModal').length === 0) {
        $('body').append(`
            <div class="modal fade" id="comingSoonModal" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content border-0 shadow">
                        <div class="modal-header border-0 bg-primary text-white">
                            <h5 class="modal-title">
                                <i class="fas fa-clock me-2"></i>Coming Soon
                            </h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body text-center py-4">
                            <i class="fas fa-tools fa-3x text-muted mb-3"></i>
                            <h6>This feature is under development</h6>
                            <p class="text-muted mb-0">We're working hard to bring you this functionality in the next update!</p>
                        </div>
                        <div class="modal-footer border-0 justify-content-center">
                            <button type="button" class="btn btn-primary" data-bs-dismiss="modal">
                                <i class="fas fa-check me-2"></i>Got it
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `);
    }

    var modal = new bootstrap.Modal(document.getElementById('comingSoonModal'));
    modal.show();
}