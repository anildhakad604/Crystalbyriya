function HandleAddToWishlist2(skuCode, productName) {
    event.preventDefault();
    document.getElementById('SkuCode').value = skuCode;
    document.getElementById('ProductName').value = productName;
    document.getElementById('buynow').value = 'true';
    document.getElementById('wishlistForm').submit();
}
document.querySelectorAll('.quview').forEach(button => {
    button.addEventListener('click', async function () {
        const skuCode = this.getAttribute('id'); // SKU Code from the button
        const url = `/api/QuickView/GetQuickView`; // API endpoint

        try {
            // Show loading spinner or placeholder content in the modal
            document.querySelector('.product-quick-view-modal').classList.add('active');
            document.querySelector('.product-quick-view-modal .title').textContent = "Loading...";
            document.querySelector('.product-quick-view-modal .price').textContent = "";
            document.querySelector('.product-quick-view-modal .product-desc').textContent = "Fetching product details...";
            document.querySelector('.product-quick-view-modal .product-sku').textContent = "N/A";
            document.querySelector('.product-quick-view-modal .widget-tags').textContent = "";

            const sizesContainer = document.querySelector('.product-quick-view-modal .sizes');
            const sizesTitle = sizesContainer.querySelector('h6'); // Title element for sizes
            const materialsContainer = document.querySelector('.product-quick-view-modal .material');
            const materialsTitle = materialsContainer.querySelector('h6');

            const swiperWrapper = document.querySelector('.product-quick-view-modal .swiper-wrapper');
            swiperWrapper.innerHTML = "";

            sizesContainer.querySelectorAll('.btn').forEach(btn => btn.remove());
            materialsContainer.querySelectorAll('.btn').forEach(btn => btn.remove());

            // Hide titles initially
            sizesTitle.style.display = "none";
            materialsTitle.style.display = "none";






            // Fetch product details from the API
            const response = await fetch(`${url}?skucode=${skuCode}`);
            const data = await response.json();
            console.log(data);

            if (response.ok && data.success) {
                const product = data.data;

                // Populate the modal with the fetched product data
                document.querySelector('.product-quick-view-modal .title').textContent = product.productName;

                document.querySelector('.product-quick-view-modal .price').textContent = `₹ ${product.price}`;
                document.querySelector('.product-quick-view-modal .product-desc').innerHTML = product.description;
                document.querySelector('.product-quick-view-modal .product-sku').textContent = product.productId;
                document.querySelector('.product-quick-view-modal .widget-tags').textContent = product.tags;
                document.getElementById('Skucode').value = product.productId;
                document.getElementById('Productname').value = product.productName;
                document.getElementById('Skucode1').value = product.productId;
                document.getElementById('Productname1').value = product.productName;
                document.getElementById('Buynow').value = 'true';

                // Dynamic Images
                if (product.image && product.image.length > 0) {
                    product.image.forEach(imageName => {
                        const slide = document.createElement('div');
                        slide.className = 'swiper-slide';
                        slide.innerHTML = `
                            <img data-src="https://www.crystalsbyriya.com/images/${imageName}" alt="Product Image" class="lazy-load">
                        `;
                        swiperWrapper.appendChild(slide);
                    });

                    // Reinitialize Swiper for new slides
                    const swiperInstance = new Swiper('.quick-view-slider-container', {
                        navigation: {
                            nextEl: '.swiper-button-next',
                            prevEl: '.swiper-button-prev',
                        },
                        pagination: {
                            el: '.swiper-pagination',
                            clickable: true,
                        },
                        loop: true,
                    });

                    // Trigger lazy loading
                    document.querySelectorAll('.lazy-load').forEach(img => {
                        img.setAttribute('src', img.getAttribute('data-src'));
                        img.removeAttribute('data-src');
                    });
                } else {
                    swiperWrapper.innerHTML = `<div class="swiper-slide">No images available</div>`;
                }

                /// Hidden field to store the total price
                const totalPriceField = document.getElementById('totalPrice'); // Ensure this hidden field exists in your HTML
                let selectedSizePrice = 0;
                let selectedMaterialPrice = 0;

                // Populate Sizes
                if (product.size && product.size.length > 0) {
                    sizesTitle.style.display = ""; // Show title if sizes exist

                    product.size.forEach(sizeString => {
                        const [sizeName, sizePrice, sizeImage] = sizeString.split(":");

                        const sizeButton = document.createElement('button');
                        sizeButton.className = 'btn btn-sizes';
                        sizeButton.textContent = sizeName.trim();
                        sizeButton.setAttribute('data-price', sizePrice.trim()); // Add price as a data attribute
                        sizeButton.setAttribute('data-image', sizeImage.trim()); // Add image attribute

                        // Add click event for size selection
                        sizeButton.addEventListener('click', function () {
                            // Remove 'active' class from all size buttons
                            document.querySelectorAll('.product-quick-view-modal .sizes .btn-sizes').forEach(btn => {
                                btn.classList.remove('active');
                            });

                            // Add 'active' class to the clicked button
                            this.classList.add('active');

                            // Update selected size price
                            selectedSizePrice = parseFloat(sizePrice.trim());

                            // Update total price
                            const totalPrice = selectedSizePrice + selectedMaterialPrice;
                            document.querySelector('.product-quick-view-modal .price').textContent = `₹ ${totalPrice}`;
                            totalPriceField.value = totalPrice;

                            // Update Swiper with the size-specific image
                            swiperWrapper.innerHTML = `
                            <div class="swiper-slide">
                                <img data-src="https://www.crystalsbyriya.com/images/${sizeImage.trim()}" alt="Size Image" class="lazy-load">
                            </div>
                        `;

                            // Reinitialize Swiper for new slides
                            const swiperInstance = new Swiper('.quick-view-slider-container', {
                                navigation: {
                                    nextEl: '.swiper-button-next',
                                    prevEl: '.swiper-button-prev',
                                },
                                pagination: {
                                    el: '.swiper-pagination',
                                    clickable: true,
                                },
                                loop: true,
                            });

                            // Trigger lazy loading for the new image
                            document.querySelectorAll('.lazy-load').forEach(img => {
                                img.setAttribute('src', img.getAttribute('data-src'));
                                img.removeAttribute('data-src');
                            });
                        });

                        sizesContainer.appendChild(sizeButton);
                    });
                }

                if (product.materialName && product.materialName.length > 0) {
                    materialsTitle.style.display = "";
                    product.materialName.forEach(materialString => {
                        const [materialName, materialPrice, materialImage] = materialString.split(":");

                        const materialButton = document.createElement('button');
                        materialButton.className = 'btn btn-sizes';
                        materialButton.textContent = materialName.trim();
                        materialButton.setAttribute('data-price', materialPrice.trim()); // Add price as a data attribute
                        materialButton.setAttribute('data-image', materialImage.trim()); // Add image attribute

                        // Add click event for material selection
                        materialButton.addEventListener('click', function () {
                            // Remove 'active' class from all material buttons
                            document.querySelectorAll('.product-quick-view-modal .material .btn-sizes').forEach(btn => {
                                btn.classList.remove('active');
                            });

                            // Add 'active' class to the clicked button
                            this.classList.add('active');

                            // Update selected material price
                            selectedMaterialPrice = parseFloat(materialPrice.trim());

                            // Update total price
                            const totalPrice = selectedSizePrice + selectedMaterialPrice;
                            document.querySelector('.product-quick-view-modal .price').textContent = `₹ ${totalPrice}`;
                            totalPriceField.value = totalPrice; // Set the total price in the hidden field


                            // Update Swiper with the material-specific image
                            swiperWrapper.innerHTML = `
                            <div class="swiper-slide">
                                <img data-src="https://www.crystalsbyriya.com/images/${materialImage.trim()}" alt="Material Image" class="lazy-load">
                            </div>
                        `;

                            // Reinitialize Swiper for new slides
                            const swiperInstance = new Swiper('.quick-view-slider-container', {
                                navigation: {
                                    nextEl: '.swiper-button-next',
                                    prevEl: '.swiper-button-prev',
                                },
                                pagination: {
                                    el: '.swiper-pagination',
                                    clickable: true,
                                },
                                loop: true,
                            });

                            // Trigger lazy loading for the new image
                            document.querySelectorAll('.lazy-load').forEach(img => {
                                img.setAttribute('src', img.getAttribute('data-src'));
                                img.removeAttribute('data-src');
                            });
                        });

                        materialsContainer.appendChild(materialButton);
                    });
                }


            } else {
                // Handle API error
                document.querySelector('.product-quick-view-modal .title').textContent = "Error";
                document.querySelector('.product-quick-view-modal .product-desc').textContent = data.message || "Failed to load product details.";
            }
        } catch (error) {
            console.error('Error fetching Quick View data:', error);
            document.querySelector('.product-quick-view-modal .title').textContent = "Error";
            document.querySelector('.product-quick-view-modal .product-desc').textContent = "An unexpected error occurred.";
        }
    });
});

// Close modal when the close button is clicked
document.querySelector('.product-quick-view-modal .btn-close').addEventListener('click', function () {
    document.querySelector('.product-quick-view-modal').classList.remove('active');
});
function HandleAddToWishlist3() {
    event.preventDefault();
    document.getElementById('wishlistForms').submit();
}
function HandleAddToCart3() {
    event.preventDefault(); // Prevent default form submission behavior

    let sizeSelected = false;
    let materialSelected = false;
    let hasSizes = false;
    let hasMaterials = false;

    const availableSizes = document.querySelectorAll('.product-quick-view-modal .sizes .btn-sizes');
    const availableMaterials = document.querySelectorAll('.product-quick-view-modal .material .btn-sizes');


    if (availableSizes.length > 0) {
        hasSizes = true;
    }
    if (availableMaterials.length > 0) {
        hasMaterials = true;
    }

    // Check for selected size
    const selectedSize = document.querySelector('.product-quick-view-modal .sizes .btn-sizes.active');
    if (selectedSize) sizeSelected = true;

    // Check for selected material
    const selectedMaterial = document.querySelector('.product-quick-view-modal .material .btn-sizes.active');
    if (selectedMaterial) materialSelected = true;

    // If no sizes or materials are available, proceed without alert
    if (!hasSizes && !hasMaterials) {
        const basePrice = parseFloat(document.querySelector('.product-quick-view-modal .price').textContent.replace('₹', '').trim());
        document.getElementById('totalPrice').value = basePrice;
        const quantityInputs = document.getElementById('quantity1');
        document.getElementById('quantity').value = quantityInputs.value;

        // Submit the form
        document.getElementById('addtocartForms').submit();
        return;
    }

    // Validate size and material selection
    if ((hasSizes && !sizeSelected)) {
        alert("Please select a size or material before adding to cart.");
        return;

    }
    if ((hasMaterials && !materialSelected)) {
        alert("Please select a size or material before adding to cart.");
        return;

    }


    // Set selected size and material in the form
    if (selectedSize) document.getElementById('selectedSize').value = selectedSize.textContent.trim();
    if (selectedMaterial) document.getElementById('selectedMaterial').value = selectedMaterial.textContent.trim();

    // Set quantity
    const quantityInput = document.getElementById('quantity1');
    if (!quantityInput || !quantityInput.value) {
        alert("Please specify a quantity before adding to cart.");
        return;
    }
    document.getElementById('quantity').value = quantityInput.value;

    // Submit the form
    document.getElementById('addtocartForms').submit();
} 