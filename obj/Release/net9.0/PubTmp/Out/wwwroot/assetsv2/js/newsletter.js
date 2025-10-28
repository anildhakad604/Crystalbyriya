                                    $(document).ready(function () {
                                        $('#newsletterForm').on('submit', function (e) {
                                            e.preventDefault(); 

                                            var email = $('#email').val(); 



                                            // Ensure the email is sent as a JSON object
                                            $.ajax({
                                                url: '/api/Newsletter', // API endpoint
                                                type: 'POST',
                                                contentType: 'application/json', // Specify JSON format
                                                dataType: 'json', // Expect a JSON response
                                                data: JSON.stringify({ email: email }), // Convert the email into a JSON string
                                                success: function (response) {
                                                    alert('Subscription successful!');
                                                    $('#email').val(''); // Clear the input field
                                                },
                                                error: function (xhr, status, error) {
                                                    // Display detailed error information
                                                    console.error("Error details: ", {
                                                        status: xhr.status,              // HTTP status code
                                                        statusText: xhr.statusText,      // HTTP status text
                                                        responseText: xhr.responseText,  // Server's response text
                                                        errorMessage: error              // Error message (if available)
                                                    });

                                                    // Show user-friendly error message
                                                    alert('Subscription failed: ' + xhr.status + ' ' + xhr.statusText + '\n' + xhr.responseText);
                                                }
                                            });
                                        });
                                    });
