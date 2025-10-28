$(document).ready(function () {
    // Check email or phone number when input loses focus
    $('#email, #phone').on('blur', function () {
        const email = $('#email').val();

        const phone = $('#phone').val();
        console.log(email);

        if (email || phone) {
            $.ajax({
                url: '/api/CheckRegister/CheckIfExists', // API endpoint
                type: 'POST',
                data: { email: email, phone: phone },
                success: function (response) {
                    console.log(response);
                    // Handle email existence
                    if (response.emailExists) {
                        $('#existemail').text('Email already exists.').show();
                        $('#registerbtn').hide();
                    } else {
                        $('#existemail').hide();
                    }

                    // Handle phone existence
                    if (response.phoneExists) {
                        $('#existphone').text('Phone number already exists.').show();
                        $('#registerbtn').hide();

                    } else {
                        $('#existphone').hide();
                    }
                    if (!response.emailExists && !response.phoneExists) {
                        $('#registerbtn').show();
                    }


                },
                error: function () {
                    console.error('Error connecting to the API.');
                }
            });
        }
    });
});