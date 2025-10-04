
// Enhanced animations and interactions
document.addEventListener('DOMContentLoaded', function () {
    // Animate statistics counters
    const statNumbers = document.querySelectorAll('.stat-number[data-count]');
    statNumbers.forEach(stat => {
        const target = parseInt(stat.getAttribute('data-count'));
        animateCounter(stat, target);
    });

    // Service card hover effects
    const serviceCards = document.querySelectorAll('.service-card-premium');
    serviceCards.forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-5px) scale(1.02)';
        });
        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Check if celebration should be shown
    if (window.showCelebration) {
        // Add confetti effect
        setTimeout(function () {
            createConfetti();
        }, 500);

        // Auto-dismiss alert after 15 seconds
        setTimeout(function () {
            const celebrationAlert = document.querySelector('.celebration-alert');
            if (celebrationAlert) {
                celebrationAlert.remove();
            }
        }, 15000);
    }
});

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

// Confetti effect function
function createConfetti() {
    for (let i = 0; i < 50; i++) {
        setTimeout(() => {
            const confetti = document.createElement('div');
            confetti.style.cssText = `
                position: fixed;
                top: -10px;
                left: ${Math.random() * 100}%;
                width: 10px;
                height: 10px;
                background: ${['#ff6b6b', '#4ecdc4', '#45b7d1', '#96ceb4', '#feca57'][Math.floor(Math.random() * 5)]};
                z-index: 9999;
                animation: confetti-fall 3s linear forwards;
                border-radius: 50%;
            `;
            document.body.appendChild(confetti);

            setTimeout(() => confetti.remove(), 3000);
        }, i * 50);
    }
}