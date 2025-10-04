
// Handle image load errors
function handleImageError(img) {
    img.style.display = 'none';
    var errorDiv = document.createElement('div');
    errorDiv.className = 'preview-error';
    errorDiv.innerHTML = '<i class="fas fa-exclamation-triangle"></i><br>Image not available';
    img.parentNode.appendChild(errorDiv);
}

// Add smooth animations
document.addEventListener('DOMContentLoaded', function () {
    // Animate cards on load
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';

        setTimeout(() => {
            card.style.transition = 'all 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Add hover effects to file cards
    const fileCards = document.querySelectorAll('.file-card');
    fileCards.forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-2px)';
            this.style.boxShadow = '0 8px 25px rgba(0,0,0,0.15)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
            this.style.boxShadow = '0 2px 10px rgba(0,0,0,0.1)';
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
});