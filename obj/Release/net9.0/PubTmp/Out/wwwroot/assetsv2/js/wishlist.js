    document.addEventListener('DOMContentLoaded', function () {
        fetch('/api/WishlistUpdateQty/getWishlistCount')
            .then(response => response.json())
            .then(data => {
                // Update the cart count span dynamically
                document.querySelector('.wishlist-count').innerText = data.count;
            })
            .catch(error => {
                console.error('Error fetching cart count:', error);
            });
        });
