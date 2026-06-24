document.addEventListener('DOMContentLoaded', () => {
    // Staggered fade-in for cards
    document.querySelectorAll('.card-glass, .stat-card, .feature-card').forEach((el, i) => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(16px)';
        el.style.transition = `opacity 0.45s ease ${i * 0.06}s, transform 0.45s ease ${i * 0.06}s`;
        requestAnimationFrame(() => {
            el.style.opacity = '1';
            el.style.transform = 'translateY(0)';
        });
    });

    // Auto-dismiss alerts after 5s
    document.querySelectorAll('.alert-glass[data-auto-dismiss]').forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.4s ease';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 400);
        }, 5000);
    });

    // Navbar scroll shadow
    const nav = document.querySelector('.navbar');
    if (nav) {
        window.addEventListener('scroll', () => {
            nav.style.boxShadow = window.scrollY > 10
                ? '0 4px 24px rgba(0,0,0,0.3)'
                : 'none';
        }, { passive: true });
    }
});
