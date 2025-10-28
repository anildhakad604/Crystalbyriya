$(document).ready(function () {
    // Handle Quantity Selection
    $('.Quantity').on('change', function () {
        // Get the closest table row
        var $row = $(this).closest('tr');

        // Get the selected quantity and product ID
        var selectedQuantity = $(this).val();
        var price = parseFloat($row.find('.Price').data('price'));

        // Calculate the new subtotal for this product
        var subtotal = price * selectedQuantity;

        // Update the subtotal text in the corresponding Subtotal column
        $row.find('.Subtotal').text('₹ ' + subtotal.toFixed(2));

        // Update the grand total by recalculating all rows' subtotals
        updateGrandTotal();
    });

    // Function to recalculate and update the grand total
    function updateGrandTotal() {
        var grandTotal = 0;

        // Iterate through each row to accumulate the subtotal
        $('#orders').find('tr').each(function () {
            var rowSubtotal = parseFloat($(this).find('.Subtotal').text().replace('₹', '').trim());
            if (!isNaN(rowSubtotal)) {
                grandTotal += rowSubtotal;
            }
        });

        // Update the grand total display
        $('.cart-subtotal .amount').text('₹ ' + grandTotal.toFixed(2));

        // Add shipping cost if necessary
        var shippingCost = 80; // Or calculate dynamically
        var grandTotalWithShipping = grandTotal + shippingCost;

        // Update grand total with shipping
        $('.amount-total .amount').text('₹ ' + grandTotalWithShipping.toFixed(2));
    }
});

// Function to submit the form via AJAX when quantity is changed
function submitQuantityForm(selectElement) {
    var $form = $(selectElement).closest('form');
    var formData = $form.serialize(); // Serialize the form data

    $.ajax({
        url: $form.attr('action'),
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                console.log('Quantity updated successfully');
                // Optionally, you can trigger the UI update manually if needed
                updateGrandTotal();
            }
        },
        error: function (error) {
            alert('Error updating quantity.');
        }
    });

    return false; // Prevent full page reload
}