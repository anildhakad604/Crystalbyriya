function enableEditing() {
  var form = document.getElementById("accountDetailsForm");
  var inputs = form.querySelectorAll("input, select");

  // Enable the form fields
  inputs.forEach(function (input) {
    input.removeAttribute("readonly");
    input.removeAttribute("disabled");
  });

  // Show the Save button
  document.getElementById("saveButton").style.display = "block";

  // Hide the Edit button
  document.getElementById("editButton").style.display = "none";
}
document.addEventListener("DOMContentLoaded", function () {
  async function loadOrders() {
    const orderItemsContainer = document.getElementById("order-items");

    try {
      // Fetch order summary from the API
      const response = await fetch("/api/YourOrders/summary");

      if (!response.ok) {
        throw new Error(
          "Failed to fetch orders. Server responded with status " +
            response.status,
        );
      }

      const data = await response.json();

      // Clear existing content in order items container
      orderItemsContainer.innerHTML = "";

      // Check if the response contains order data
      if (!data.carts || data.carts.length === 0) {
        orderItemsContainer.innerHTML = `<tr><td colspan="7" class="text-center">Your order is empty.</td></tr>`;
        return;
      }

      // Populate order items dynamically
      data.carts.forEach((item) => {
        const row = document.createElement("tr");
        row.innerHTML = `
                                                        <td class="pro-thumbnail">
                                                            <div class="pro-info text-center">
                                                                <div class="pro-img">
                                                                    <a href="#"><img src="https://www.drastrocrystals.com/images/${item.image}" alt="${item.productName}" /></a>
                                                                </div>
                                                                <div class="pro-name"><span>${item.productName}</span></div>
                                                            </div>
                                                        </td>
                                                        <td class="pro-material text-center">${item.material || "N/A"}</td>
                                                        <td class="pro-size text-center">${item.size || "N/A"}</td>
                                                        <td class="pro-price"><span>₹${item.price.toFixed(2)}</span></td>
                                                        <td class="pro-quantity">
                                                            <div class="action-top d-flex justify-content-center">
                                                                <div class="pro-qty-area">
                                                                    <div class="pro-qty">
                                                                        <input type="text" title="Quantity" value="${item.qty}" readonly>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td class="pro-subtotal"><span>₹${(item.price * item.qty).toFixed(2)}</span></td>
                                                        <td class="pro-status text-center">${item.status || "Pending"}</td>
                                                    `;
        orderItemsContainer.appendChild(row);
      });
    } catch (error) {
      console.error("Error fetching orders:", error);
      orderItemsContainer.innerHTML = `<tr><td colspan="7" class="text-center">You have not placed any orders yet.</td></tr>`;
    }
  }

  // Load orders when the "Your Orders" tab is clicked
  document
    .getElementById("list-orders-list")
    .addEventListener("click", loadOrders);
});
const detailPageBaseUrl = "/detail";

function getWishlist() {
  $.ajax({
    url: "/dashboard?handler=Wishlist",
    type: "GET",
    success: function (response) {
      let wishlistTableBody = "";
      const imageBasePath = "/images/";

      if (response && response.length > 0) {
        response.forEach((item) => {
          const productImageUrl = imageBasePath + item.Image;

          // Construct the URL in the format /detail/ProductId/ProductName
          const productNameFormatted = item.ProductName.replace(/ /g, "-"); // Replace spaces with hyphens
          const addToCartUrl = `${detailPageBaseUrl}/${item.ProductId}/${productNameFormatted}`;

          wishlistTableBody += `
                                                            <tr>
                                                               <td class="pro-thumbnail">
                                                                    <div class="pro-info">
                                                                        <div class="pro-img">
                                                                            <a href="${addToCartUrl}">
                                                                                <img src="${productImageUrl}" alt="product-thumb">
                                                                            </a>
                                                                        </div>
                                                                    </div>
                                                                </td>
                                                                <td class="pro-name">
                                                                    <span class="ms-1">${item.ProductName}</span>
                                                                </td>
                                                                <td class="pro-stock-status">
                                                                   In Stock
                                                                </td>
                                                                <td class="pro-price">&#8377;${item.Price}</td>
                                                                                               <td style="width:30%;" class="pro-action">
                                    <a href="${addToCartUrl}" class="btn product-action-button">
                                        <i class="lastudioicon-bag-3 me-1"></i> ADD TO CART
                                    </a>
                                </td>

                                                            </tr>
                                                        `;
        });
      } else {
        wishlistTableBody = `
                                                        <tr>
                                                            <td colspan="6" class="text-center">
                                                                Your wishlist is empty.
                                                            </td>
                                                        </tr>
                                                    `;
      }

      // Update the table body with generated content
      $("#list-wishlist .wishlist-table tbody").html(wishlistTableBody);
    },
    error: function () {
      alert("Error loading wishlist. Please try again.");
    },
  });
}
function removeFromWishlist(productId) {
  const antiForgeryToken =
    document.querySelector(
      '#logoutForm input[name="__RequestVerificationToken"]',
    )?.value || "";

  $.ajax({
    url: "/dashboard?handler=RemoveFromWishlist",
    type: "POST",
    data: { productId: productId },
    headers: {
      RequestVerificationToken: antiForgeryToken,
    },
    success: function (response) {
      // Reload or dynamically update the wishlist after removing the item
      location.reload(); // Reload the page to reflect the changes
    },
    error: function (error) {
      console.log("Error removing item from wishlist:", error);
    },
  });
}
