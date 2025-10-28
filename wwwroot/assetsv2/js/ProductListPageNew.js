// Function to add event listeners for sorting functionality
function addSortEventListener(ids, inputId) {
    ids.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener('click', function () {
                // Set the corresponding hidden input value to true
                document.getElementById(inputId).value = 'true';

                // Submit the form to the backend
                document.getElementById('sortForm').submit();
            });
        }
    });
}

// Add event listeners for each sorting option
addSortEventListener(['sortByPopularity', 'sortByPopularitymob'], 'isSortByPopularity1');
addSortEventListener(['sortbylatest', 'sortbylatestmob'], 'isbylatest1');
addSortEventListener(['sortbyrating', 'sortbyratingmob'], 'isbyrating1');
addSortEventListener(['sortbydefault', 'sortbydefaultmob'], 'isbydefault1');
addSortEventListener(['sortbypriceasc', 'sortbypriceascmob'], 'isbypriceasc1');
addSortEventListener(['sortbypricedesc', 'sortbypricedescmob'], 'isbypricedesc1');


// Elements
const rangeMin = document.getElementById("rangeMin") || document.getElementById("mobileRangeMin");
const rangeMax = document.getElementById("rangeMax") || document.getElementById("mobileRangeMax");
const minPriceInput = document.getElementById("minPriceInput") || document.getElementById("mobileMinPriceInput");
const maxPriceInput = document.getElementById("maxPriceInput") || document.getElementById("mobileMaxPriceInput");
const progress = document.querySelector(".progress") || document.querySelector(".mobile-progress");
const productListContainer = document.getElementById("productListContainer");

let debounceTimeout;

// Update slider from input fields
function updateSliderFromInput() {
    const minValue = parseInt(minPriceInput.value) || 0;
    const maxValue = parseInt(maxPriceInput.value) || 0;

    if (minValue <= maxValue) {
        rangeMin.value = minValue;
        rangeMax.value = maxValue;
        updateSliderProgress();
    }
}

// Update input fields from sliders
function updateInputFromSlider() {
    const minValue = parseInt(rangeMin.value) || 0;
    const maxValue = parseInt(rangeMax.value) || 0;

    if (minValue <= maxValue) {
        minPriceInput.value = minValue;
        maxPriceInput.value = maxValue;
        updateSliderProgress();
        debounceFetchProducts();
    }
}

// Update the slider progress bar
function updateSliderProgress() {
    const minPercent = (rangeMin.value / rangeMin.max) * 100;
    const maxPercent = (rangeMax.value / rangeMax.max) * 100;
    progress.style.left = `${minPercent}%`;
    progress.style.right = `${100 - maxPercent}%`;
}

// Debounced function to fetch products
function debounceFetchProducts() {
    clearTimeout(debounceTimeout);
    debounceTimeout = setTimeout(fetchProducts, 300); // Adjust delay as needed
}

// Fetch products based on price range
async function fetchProducts() {
    const minPrice = minPriceInput.value;
    const maxPrice = maxPriceInput.value;

    try {
        // Fetch products from the API
        const response = await fetch(`/api/PriceSlider/GetProductsByPriceRange?catname=${catname}&subcatname=${subcatname}&minPrice=${minPrice}&maxPrice=${maxPrice}`);

        const products = await response.json();

        // Clear existing products
        productListContainer.innerHTML = "";

        // Populate with new products
        if (products.length > 0) {
            products.forEach(product => {
                const productHtml = `
                            <div class="col-6 col-md-4 col-lg-3">
                                <div class="product-item">
                                    <div class="product-thumb">
                                        <a href="/detail/${product.skuCode}/${product.productName.replace(/ /g, "-")}">
                                            <img data-src="https://www.crystalsbyriya.com/images/${product.thumbnail}" alt="${product.productName}" class="lazyload">
                                            <span class="bg-thumb" data-bg-img="https://www.crystalsbyriya.com/images/${product.thumbnail}" style="background-image: url('https://www.crystalsbyriya.com/images/${product.thumbnail}');"></span>
                                            <span class="thumb-overlay"></span>
                                        </a>
                                     
                                    <div class="product-footer position-relative">
                                        <div class="product-info d-block">
                                            <div class="content-inner text-center">
                                                <h4 class="title"><a href="/detail/${product.skuCode}/${product.productName.replace(/ /g, "-")}">${product.productName}</a></h4>
                                                <div class="prices">
                                                    <span class="price">₹ ${product.price}</span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="product-action-button-parent">
                                            <a href="/detail/${product.skuCode}/${product.productName.replace(/ /g, "-")}" class="btn product-action-button"><i class="lastudioicon-bag-3 me-1"></i> ADD TO CART</a>
                                        </div>
                                    </div>
                                </div>
                            </div>`;
                productListContainer.innerHTML += productHtml;
            });
        } else {
            // Show a message if no products match the criteria
            productListContainer.innerHTML = `<div class="col-12"><p>No products found in this price range.</p></div>`;
        }
    } catch (error) {
        console.error("Error fetching products:", error);

        // Handle errors gracefully
        productListContainer.innerHTML = `
                    <div class="col-12">
                        <p>There was an error loading the products. Please try again later.</p>
                    </div>`;
    }
}

// Event listeners for synchronization
minPriceInput.addEventListener("input", () => {
    updateSliderFromInput();
    debounceFetchProducts();
});

maxPriceInput.addEventListener("input", () => {
    updateSliderFromInput();
    debounceFetchProducts();
});

rangeMin.addEventListener("input", updateInputFromSlider);
rangeMax.addEventListener("input", updateInputFromSlider);

// Initialize the slider progress on page load
updateSliderProgress();