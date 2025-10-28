
        $(document).ready(function () {
            // This handles the typing event for the search input field
            $('#search-input1').on('input', function () {
                var query = $(this).val();
                if (query.length > 0) {
                    $('#clear-search').show();
                } else {
                    $('#clear-search').hide();
                    $('#search-results-inline').hide().empty();
                }

                // Only trigger search if more than 2 characters are typed
                if (query.length > 2) {
                    $.ajax({
                        url: '/api/Search',  // API to fetch matching products
                        type: 'GET',
                        data: { name: query },  // Send the search term
                        success: function (data) {
                            // Clear the existing results and show the container
                            $('#search-results-inline').empty().show();

                            // If there are matching products, append their names and SKU codes
                            if (data.length > 0) {
                                $.each(data, function (index, product) {
                                    $('#search-results-inline').append(`
            <div>
                           <a href="/SearchResult?name=${encodeURIComponent(query)}"
                   class="submit-btn"
                   asp-controller="Search"
                   asp-action="SearchResult">
                    ${product.productName}
                </a>
            </div>
        `);
                                });
                            } else {
                                $('#search-results-inline').append('<p>No results found</p>');  // Corrected the quotes here
                            }
                        },
                        error: function (xhr, status, error) {
                            console.error('Error: ' + error);
                        }
                    });
                } else {
                    // If input is less than 3 characters, hide and clear the results
                    $('#search-results-inline').hide().empty();
                }
            });
        });



