document.addEventListener('DOMContentLoaded', function () {
    fetch('/api/UpdateCartQty/getCartCount')
        .then(response => response.json())
        .then(data => {
            // Update the cart count span dynamically
            document.querySelector('.cart-count').innerText = data.count;
        })
        .catch(error => {
            console.error('Error fetching cart count:', error);
        });
});