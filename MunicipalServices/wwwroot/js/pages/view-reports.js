
document.addEventListener('DOMContentLoaded', function () {
    // Add hover effects to file items
    document.querySelectorAll('.file-item').forEach(item => {
        item.addEventListener('mouseenter', function () {
            this.style.backgroundColor = '#e9ecef';
            this.style.transform = 'scale(1.02)';
            this.style.transition = 'all 0.2s ease';
        });

        item.addEventListener('mouseleave', function () {
            this.style.backgroundColor = '#f8f9fa';
            this.style.transform = 'scale(1)';
        });
    });

    // Add loading effect for images
    document.querySelectorAll('.preview-thumbnail').forEach(img => {
        img.addEventListener('load', function () {
            this.style.opacity = '1';
        });

        img.addEventListener('error', function () {
            this.style.opacity = '0.5';
            this.alt = 'Image not found';
            this.title = 'Image could not be loaded';
        });
    });

    // Add click feedback for image links
    document.querySelectorAll('a[target="_blank"]').forEach(link => {
        link.addEventListener('click', function () {
            this.style.opacity = '0.7';
            setTimeout(() => {
                this.style.opacity = '1';
            }, 200);
        });
    });

    // Add smooth card animations on page load
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';

        setTimeout(() => {
            card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Enhanced button hover effects
    document.querySelectorAll('.btn-primary').forEach(btn => {
        btn.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-2px)';
            this.style.boxShadow = '0 5px 15px rgba(0,123,255,0.3)';
        });

        btn.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
            this.style.boxShadow = '';
        });
    });
});