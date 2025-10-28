    document.addEventListener('DOMContentLoaded', function () {
            const resetModal = () => {
                const modalBody = document.querySelector('#blogModal .modal-body');
    modalBody.querySelector('#blogImage').src = ''; // Reset image
    modalBody.querySelector('#blogMeta').innerHTML = ''; // Reset metadata
    modalBody.querySelector('#blogTitle').textContent = ''; // Reset title
    modalBody.querySelector('#blogDescription').innerHTML = ''; // Reset description
    modalBody.querySelector('#relatedProducts').innerHTML = ''; // Clear related products
            };

            document.querySelectorAll('.blogquickview').forEach(button => {
        button.addEventListener('click', async function () {
            const blogid = this.getAttribute('id'); // Assuming buttons have data-blogid
            if (!blogid) {
                console.error('Blog ID is missing');
                return;
            }

            const url = `/api/BlogQuickView/GetBlogQuickviews`;

            try {
                const response = await fetch(`${url}?blogid=${blogid}`);
                if (!response.ok) {
                    console.error('Error:', response.statusText);
                    return;
                }

                const data = await response.json();
                if (data.success) {
                    resetModal();

                    const blog = data.data;

                    // Update modal content dynamically
                    const modalBody = document.querySelector('#blogModal .modal-body');
                    modalBody.querySelector('#blogImage').src = blog.image || '/assets/img/placeholder.jpg';
                    modalBody.querySelector('#blogMeta').innerHTML = '<a href="#">Wealth</a>, <a href="#">Health</a>'; // Example metadata
                    modalBody.querySelector('#blogTitle').textContent = blog.blogTitle || 'Blog Title';
                    modalBody.querySelector('#blogDescription').innerHTML = blog.blogdescription || 'Blog description';

                    // Populate related products dynamically
                    const relatedProductsArea = modalBody.querySelector('#relatedProducts');
                    blog.productname.forEach(productString => {
                        const [productName, skuCode, price, thumbnail] = productString.split(':');

                        // Function to generate slug from product name
                        function slugify(text) {
                            return text
                                .toString()                  // Convert to stri              // Convert to lowercase
                                .trim()                      // Trim whitespace
                                .replace(/\s+/g, '-')        // Replace spaces with -
                                .replace(/[^\w\-]+/g, '')    // Remove all non-word characters
                                .replace(/\-\-+/g, '-');     // Replace multiple - with single -
                        }

                        // Generate the slug
                        const productSlug = slugify(productName);
                        const productHtml = `
                                    <div class="col-6 ps-0">
                                        <div class="product-item">
                                            <div class="product-thumb">
                                                <a href="/detail/${skuCode}/${productSlug}">
                                                    <img src="https://www.crystalsbyriya.com/images/${thumbnail}" alt="${productName}">
                                                </a>
                                             
                                            </div>
                                            <div class="product-footer position-relative">
                                                <div class="content-inner text-center">
                                                    <h4 class="title">${productName}</h4>
                                                    <div class="prices">
                                                        <span class="price">&#8377; ${price}</span>
                                                    </div>
                                                </div>
                                               
                                            </div>
                                        </div>
                                    </div>
                                `;
                        relatedProductsArea.insertAdjacentHTML('beforeend', productHtml);
                    });

                    // Show the modal
                    const blogModal = new bootstrap.Modal(document.getElementById('blogModal'));
                    blogModal.show();
                } else {
                    console.error('Failed to fetch blog data');
                }
            } catch (error) {
                console.error('Error fetching blog data:', error);
            }
        });
            });

    const blogModalElement = document.getElementById('blogModal');

    // Handle modal close with reload
    document.querySelector('.btn-close').addEventListener('click', function () {
                const blogModal = bootstrap.Modal.getInstance(blogModalElement);
    if (blogModal) {
        blogModal.hide(); // Hide modal
    blogModal.dispose(); // Dispose Bootstrap instance
                }
    resetModal();
    location.reload(); // Reload the page
            });

    // Ensure proper cleanup when modal is hidden
    blogModalElement.addEventListener('hidden.bs.modal', function () {
        resetModal();
            });
        });





