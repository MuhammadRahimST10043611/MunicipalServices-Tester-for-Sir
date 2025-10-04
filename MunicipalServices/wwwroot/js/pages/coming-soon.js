// Coming Soon Pages JavaScript
document.addEventListener('DOMContentLoaded', function () {
    // Animate progress bars on page load
    animateProgressBars();

    // Add interactive effects to feature cards
    initializeFeatureCards();

    // Initialize button hover effects
    initializeButtonEffects();

    // Add sparkle animations
    initializeSparkleEffects();

    // Initialize timeline animations
    initializeTimelineAnimations();

    // Add floating animation to main icon
    initializeIconAnimations();
});

// Animate progress bars with delay
function animateProgressBars() {
    const progressBars = document.querySelectorAll('.progress-bar');

    progressBars.forEach((bar, index) => {
        const targetWidth = bar.style.width;
        bar.style.width = '0%';

        setTimeout(() => {
            bar.style.transition = 'width 2s cubic-bezier(0.4, 0, 0.2, 1)';
            bar.style.width = targetWidth;
        }, 500 + (index * 200));
    });
}

// Initialize feature card interactions
function initializeFeatureCards() {
    const featureCards = document.querySelectorAll('.feature-card');

    featureCards.forEach((card, index) => {
        // Staggered animation on load
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';

        setTimeout(() => {
            card.style.transition = 'all 0.6s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 200 + (index * 150));

        // Enhanced hover effects
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-8px) scale(1.02)';
            this.style.boxShadow = '0 15px 35px rgba(0,0,0,0.15)';

            // Animate the icon
            const icon = this.querySelector('.feature-icon i');
            if (icon) {
                icon.style.transform = 'scale(1.1) rotate(5deg)';
                icon.style.transition = 'all 0.3s ease';
            }
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
            this.style.boxShadow = '';

            // Reset icon
            const icon = this.querySelector('.feature-icon i');
            if (icon) {
                icon.style.transform = 'scale(1) rotate(0deg)';
            }
        });
    });
}

// Initialize button effects
function initializeButtonEffects() {
    const buttons = document.querySelectorAll('.btn');

    buttons.forEach(button => {
        button.addEventListener('click', function (e) {
            // Create ripple effect
            createRippleEffect(e, this);

            // Add click animation
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = '';
            }, 150);
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
        background: rgba(255, 255, 255, 0.3);
        border-radius: 50%;
        transform: scale(0);
        animation: ripple 0.6s linear;
        pointer-events: none;
    `;

    // Add ripple animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes ripple {
            to {
                transform: scale(2);
                opacity: 0;
            }
        }
    `;
    document.head.appendChild(style);

    button.style.position = 'relative';
    button.style.overflow = 'hidden';
    button.appendChild(ripple);

    setTimeout(() => {
        ripple.remove();
        if (button.querySelectorAll('span').length === 0) {
            style.remove();
        }
    }, 600);
}

// Initialize sparkle effects
function initializeSparkleEffects() {
    const sparkles = document.querySelectorAll('.icon-decoration i');

    sparkles.forEach((sparkle, index) => {
        // Add random twinkling effect
        setInterval(() => {
            sparkle.style.opacity = Math.random() * 0.8 + 0.2;
        }, 1000 + (index * 300));

        // Add random scale changes
        setInterval(() => {
            const scale = Math.random() * 0.3 + 0.8;
            sparkle.style.transform = `scale(${scale})`;
        }, 2000 + (index * 500));
    });
}

// Initialize timeline animations
function initializeTimelineAnimations() {
    const timelineSteps = document.querySelectorAll('.timeline-step');

    // Animate timeline steps sequentially
    timelineSteps.forEach((step, index) => {
        step.style.opacity = '0';
        step.style.transform = 'translateX(-20px)';

        setTimeout(() => {
            step.style.transition = 'all 0.5s ease';
            step.style.opacity = step.classList.contains('completed') || step.classList.contains('active') ? '1' : '0.5';
            step.style.transform = 'translateX(0)';
        }, 300 + (index * 200));
    });

    // Add hover effects to timeline steps
    timelineSteps.forEach(step => {
        step.addEventListener('mouseenter', function () {
            if (this.classList.contains('completed') || this.classList.contains('active')) {
                const stepContent = this.querySelector('.step-content');
                if (stepContent) {
                    stepContent.style.transform = 'translateX(5px)';
                    stepContent.style.boxShadow = '0 8px 25px rgba(0,0,0,0.15)';
                }
            }
        });

        step.addEventListener('mouseleave', function () {
            const stepContent = this.querySelector('.step-content');
            if (stepContent) {
                stepContent.style.transform = 'translateX(0)';
                stepContent.style.boxShadow = '';
            }
        });
    });
}

// Initialize icon animations
function initializeIconAnimations() {
    const mainIcon = document.querySelector('.coming-soon-icon i');

    if (mainIcon) {
        // Add subtle rotation on hover
        const iconContainer = document.querySelector('.coming-soon-icon');

        iconContainer.addEventListener('mouseenter', function () {
            mainIcon.style.transform = 'scale(1.1) rotate(5deg)';
            mainIcon.style.transition = 'all 0.3s ease';
        });

        iconContainer.addEventListener('mouseleave', function () {
            mainIcon.style.transform = 'scale(1) rotate(0deg)';
        });
    }
}

// Add floating particles effect
function createFloatingParticles() {
    const container = document.querySelector('.coming-soon-background');
    if (!container) return;

    for (let i = 0; i < 20; i++) {
        setTimeout(() => {
            const particle = document.createElement('div');
            particle.style.cssText = `
                position: absolute;
                width: ${Math.random() * 6 + 2}px;
                height: ${Math.random() * 6 + 2}px;
                background: ${['#667eea', '#764ba2', '#48bb78', '#f6ad55'][Math.floor(Math.random() * 4)]};
                border-radius: 50%;
                left: ${Math.random() * 100}%;
                top: 100%;
                opacity: ${Math.random() * 0.5 + 0.2};
                animation: floatUp ${Math.random() * 3 + 2}s linear infinite;
                pointer-events: none;
            `;

            container.appendChild(particle);

            setTimeout(() => {
                if (particle.parentNode) {
                    particle.remove();
                }
            }, 5000);
        }, i * 200);
    }

    // Add float up animation
    if (!document.querySelector('#floatUpStyle')) {
        const style = document.createElement('style');
        style.id = 'floatUpStyle';
        style.textContent = `
            @keyframes floatUp {
                0% {
                    transform: translateY(0) rotate(0deg);
                    opacity: 0;
                }
                10% {
                    opacity: 1;
                }
                90% {
                    opacity: 1;
                }
                100% {
                    transform: translateY(-100vh) rotate(360deg);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
}

// Start floating particles effect
setTimeout(createFloatingParticles, 1000);

// Repeat floating particles every 10 seconds
setInterval(createFloatingParticles, 10000);

// Add scroll reveal animations for elements that come into view
function initializeScrollReveal() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Observe elements for scroll reveal
    const revealElements = document.querySelectorAll('.progress-section, .notification-card, .alternative-card');
    revealElements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(30px)';
        el.style.transition = 'all 0.6s ease';
        observer.observe(el);
    });
}

// Initialize scroll reveal
initializeScrollReveal();

// Add keyboard navigation for accessibility
document.addEventListener('keydown', function (e) {
    if (e.key === 'Tab') {
        document.body.classList.add('keyboard-navigation');
    }
});

document.addEventListener('mousedown', function () {
    document.body.classList.remove('keyboard-navigation');
});

// Performance optimization: Reduce animations on low-end devices
if (navigator.hardwareConcurrency && navigator.hardwareConcurrency < 4) {
    document.body.classList.add('reduced-animations');

    const style = document.createElement('style');
    style.textContent = `
        .reduced-animations * {
            animation-duration: 0.5s !important;
            transition-duration: 0.2s !important;
        }
    `;
    document.head.appendChild(style);
}