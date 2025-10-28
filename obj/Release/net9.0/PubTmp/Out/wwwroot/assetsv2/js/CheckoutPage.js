function validatePaymentMethod() {

    // Check if any of the payment options are expanded (selected)
    var onlinePaymentSelected = document.getElementById("itemTwo").classList.contains('show');
    var cashOnDeliverySelected = document.getElementById("itemThree").classList.contains('show');

    // Set the hidden field value for the payment method
    if (onlinePaymentSelected) {
        document.getElementById("paymentMethod").value = "online";
    } else if (cashOnDeliverySelected) {
        document.getElementById("paymentMethod").value = "cod";
    }

    // If none of the options are selected, show an alert
    if (!onlinePaymentSelected && !cashOnDeliverySelected) {
        alert("Please select a payment method.");
        return false;
    }

    return true; // Allow form submission
}
function toggleRequiredFields() {
    const isChecked = document.getElementById('ship_to_different').checked;
    const requiredFields = document.querySelectorAll('#name, #street_address, #town_city, #state_region, #zip, #phone, #email');

    requiredFields.forEach(field => {
        if (isChecked) {
            field.setAttribute('required', 'required');
        } else {
            field.removeAttribute('required');
        }
    });
    document.getElementById('shipment-hidden').value = isChecked.toString();
}