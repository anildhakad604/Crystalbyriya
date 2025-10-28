var baseUrl = '@Model.Currenturl';
var selectedSizeFromServer = '@Model.SelectedSize';
var selectedMaterialFromServer = '@Model.SelectedMaterial';
var selectedAddOnFromServer = '@Model.SelectedAddOn';

$(document).ready(function () {
   
    var sizeSelected = false;
    var materialSelected = false;
    var addonSelected = false;
    var sizePrice = 0;
    var materialPrice = 0;
    var addonPrice = 0;
    // Utility function to update the URL with query parameters
    function updateQueryString(params) {
        const url = new URL(window.location);
        Object.keys(params).forEach(key => {
            if (params[key] !== null) {
                url.searchParams.set(key, params[key]);
            } else {
                url.searchParams.delete(key);
            }
        });
        window.history.replaceState({}, '', url);
    }
    function calculateTotalPrice() {
        let totalPrice = 0;

        if (addonSelected) {
            // If addon is selected, add size, material, and addon prices
            totalPrice = sizePrice + materialPrice + addonPrice;
        }
        else if (materialSelected && sizeSelected) {
            totalPrice += materialPrice + sizePrice;
        }
        else {
            // If no addon is selected, sum size and material prices only if selected
            if (sizeSelected) {
                totalPrice += sizePrice;
            }
            if (materialSelected) {
                totalPrice += materialPrice;
            }

        }
        $('#selectedPrice').val(totalPrice);
        $(".amount").html(totalPrice); // Ensure price is displayed with 2 decimal places

    }




    // Function to initialize the page with pre-selected options from the server
    function initializeSelection() {
        if (selectedSizeFromServer) {
            const sizeElement = $(`.product-action-size ul li a[data-size="${selectedSizeFromServer}"]`);
            if (sizeElement.length) {
                sizeElement.click(); // Simulate click for size
            }
        }

        if (selectedMaterialFromServer) {
            const materialElement = $(`.product-action-material-type ul li a[data-material="${selectedMaterialFromServer}"]`);
            if (materialElement.length) {
                materialElement.click(); // Simulate click for material
            }
        }
        if (selectedAddOnFromServer) {
            const addonElement = $(`.product-action-addon-type ul li a[data-addon="${selectedAddOnFromServer}"]`);
            if (addonElement.length) {
                addonElement.click(); // Simulate click for material
            }
        }

    }

    // Handle Size Selection
    $('.product-action-size ul li a').on('click', function () {
        // Remove 'active' class from all size options
        $(this).closest('ul').find('li').removeClass('active');

        // Add 'active' class to the selected size option
        $(this).parent().addClass('active');

        // Get the selected size, image URL, and other data from the data attributes
        var imageUrl = $(this).data('image-url');
        sizePrice = parseFloat($(this).data('price'));
        var stock = $(this).data('stock');
        var selectedSize = $(this).data('size');
        // Update the product image
        $('#product-image').attr('src', imageUrl);
        $(".slick-slide.slick-current.slick-active img").attr('src', imageUrl);
        $(".zoomImg").attr('src', imageUrl);
        // Update the price display


        // Update the hidden input for selected size
        $('#selectedSize').val(selectedSize);


        // Mark size as selected
        sizeSelected = true;

        calculateTotalPrice();

        // Update the URL with the selected size, preserving material if it's selected
        updateQueryString({ size: selectedSize, material: $('#selectedMaterial').val() || null, addon: $('#selectedaddon').val() || null });
    });
    $(".single-product-slider").click(function () {
        let currentimgurl = $('.slick-slide.slick-current.slick-active img').attr('src');
        $(".zoomImg").attr('src', currentimgurl);

    });

    // Handle Material Selection
    $('.product-action-material-type ul li a').on('click', function () {
        // Remove 'active' class from all material options
        $(this).closest('ul').find('li').removeClass('active');

        // Add 'active' class to the selected material option
        $(this).parent().addClass('active');

        // Get the selected material name from the data attribute
        var imageUrl = $(this).data('image-url');
        var selectedMaterial = $(this).data('material');
        materialPrice = parseFloat($(this).data('price'));

        $('#product-image').attr('src', imageUrl);
        $(".slick-slide.slick-current.slick-active img").attr('src', imageUrl);
        $(".zoomImg").attr('src', imageUrl);
        // Update the hidden input for selected material
        $('#selectedMaterial').val(selectedMaterial);

        // Mark material as selected
        materialSelected = true;
        calculateTotalPrice();






        updateQueryString({ material: selectedMaterial, size: $('#selectedSize').val() || null, addon: $('#selectedaddon').val() || null });
    });
    $(".single-product-slider").click(function () {
        let currentimgurl = $('.slick-slide.slick-current.slick-active img').attr('src');
        $(".zoomImg").attr('src', currentimgurl);

    });
    // Handle AddOn Selection
    $('.product-action-addon-type ul li a').on('click', function () {
        // Remove 'active' class from all material options
        $(this).closest('ul').find('li').removeClass('active');

        // Add 'active' class to the selected material option
        $(this).parent().addClass('active');

        // Get the selected material name from the data attribute
        var selectedaddon = $(this).data('addon');
        addonPrice = parseFloat($(this).data('price'));

        // Update the hidden input for selected material
        $('#selectedaddon').val(selectedaddon);


        // Mark material as selected
        addonSelected = true;
        calculateTotalPrice();




        updateQueryString({ addon: selectedaddon, size: $('#selectedSize').val() || null, material: $('#selectedMaterial').val() || null });
    });
    $(document).ready(function () {
        // Check if neither size nor material options exist
        if ($('.product-action-size').length === 0 && $('.product-action-material-type').length === 0) {
            var productPriceString = @Model.Pr
            var productPrice = parseFloat(productPriceString);

 
            $('#selectedPrice').val(productPrice);
            $('.amount').html(productPrice);
         
        }
    });

    // Initialize the selections based on the server-side values
    initializeSelection();

    // Click Event: Buy Now and Add to Cart (button 1)
    $('#buynowandaddtocart1').on('click', function (e) {




        var selectedSize = $('#selectedSize').val();
        var selectedMaterial = $('#selectedMaterial').val();


        // If neither size nor material exists, prevent the action
        if ($('.product-action-size').length === 0 && $('.product-action-material-type').length === 0) {

            return true;
        }

        // Check if both size and material are unselected
        if (!selectedSize && !selectedMaterial) {
            alert("Please select either a size or a material.");
            e.preventDefault();
            return false;
        }
    });

    // Click Event: Buy Now and Add to Cart (button 2)
    $('#buynowandaddtocart2').on('click', function (e) {
        var selectedSize = $('#selectedSize').val();
        var selectedMaterial = $('#selectedMaterial').val();


        // If neither size nor material exists, prevent the action
        if ($('.product-action-size').length === 0 && $('.product-action-material-type').length === 0) {

            return true;
        }

        // Check if both size and material are unselected
        if (!selectedSize && !selectedMaterial) {
            alert("Please select either a size or a material.");
            e.preventDefault();
            return false;
        }
    });
});
document.addEventListener('DOMContentLoaded', function () {
    const stars = document.querySelectorAll('.star');
    const ratingValue = document.getElementById('rating-value');

    stars.forEach((star, index) => {
        star.addEventListener('mouseover', function () {
            highlightStars(index + 1);
        });

        star.addEventListener('click', function () {
            ratingValue.value = index + 1; // Set the rating value
            highlightStars(index + 1); // Highlight stars based on selection
        });

        star.addEventListener('mouseout', function () {
            highlightStars(ratingValue.value); // Keep stars highlighted based on selected rating
        });
    });

    function highlightStars(rating) {
        stars.forEach((star, index) => {
            if (index < rating) {
                star.style.color = '#ffc107'; // Yellow color for selected stars
            } else {
                star.style.color = '#ddd'; // Default color for unselected stars
            }
        });
    }
});
document.addEventListener("DOMContentLoaded", function () {
    const submitButton = document.getElementById("submitbtn");
    const ratingField = document.getElementById("rating-value");
    submitButton.addEventListener("click", function (e) {
        if (ratingField.value === "0") {
            alert("Please select a rating before submitting your review.");

            e.preventDefault();
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    // Check if "success" is present in the query string
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('success')) {
        alert("Review submitted successfully!");
    }
});