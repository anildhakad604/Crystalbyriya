using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrystalByRiya;
using CrystalByRiya.Models;
using static CrystalByRiya.Pages.checkoutModel;
using Newtonsoft.Json;
using Astaberry.Helpers;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Mono.TextTemplating;
using System.Diagnostics.Metrics;
using MailKit.Search;
using MailKit.Security;
using MimeKit;


namespace CrystalByRiya.Pages
{
    public class thankyouModel : PageModel
    {
        ApplicationDbContext _context;
        public thankyouModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public OrderDetails OrderDetails { get; private set; }
        public TblOrderId OrderId { get; set; }
        public List<Item> Carts { get; set; } = new List<Item>();
        public MailCredentials MailCredentials { get; private set; }

        public double shipping { get; set; }
        public string Currenturl { get; private set; }
        public double Total { get; set; }
        public double subtotal { get; set; }

        public TblBillingDetail Detail { get; set; }
        public TblShippingDetail ShippingDetail { get; set; }

        public TblCustomerOrderDetails CustomerOrderDetails { get; set; }
        public List<ChildskuCode> childskuCodes { get; set; }= new List<ChildskuCode>();
       public List<OrderMail> orderDetails { get; set; } = new List<OrderMail>();
        public DateTime date { get; set; }

        public string paymentMethod { get; set; }
        public double CouponDiscount { get; set; } 
        public double DiscountAmount { get; set; }
        public double AfterDiscount { get; set; }
        public string Phone { get; set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                // Determine if it's a Buy Now or regular cart checkout
                bool isBuyNow = HttpContext.Session.GetString("isBuyNow") == "True";
                if (isBuyNow)
                {
                    childskuCodes = SessionHelper.GetObjectFromJson<List<ChildskuCode>>(HttpContext.Session, "buynowItem") ?? new List<ChildskuCode>();
                }
                else
                {
                    Carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") ?? new List<Item>();
                }

                // Prepare for PhonePe API call
                string merchantId = PhonePeCredientials.Merchantid;
                string merchantTransactionId = HttpContext.Session.GetString("PhonePeTransactionId")
                    ?? PhonePeCredientials.OrderId;

                if (string.IsNullOrWhiteSpace(merchantTransactionId))
                {
                    return RedirectToPage("/Error");
                }

                string sha256 = Sha256Hash($"/pg/v1/status/{merchantId}/{merchantTransactionId}{PhonePeCredientials.SaltKey}") + $"###{PhonePeCredientials.saltIndex}";
           //     string apiEndpoint = "https://api-preprod.phonepe.com/apis/pg-sandbox";
                var apiEndpoint = "https://api.phonepe.com/apis/hermes";
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", sha256);
                httpClient.DefaultRequestHeaders.Add("X-MERCHANT-ID", merchantId);

                // Call PhonePe API
                var response = await httpClient.GetAsync(new Uri($"{apiEndpoint}/pg/v1/status/{merchantId}/{merchantTransactionId}"));
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(responseContent);

                if (paymentResponse == null || !new[] { "PAYMENT_SUCCESS", "PAYMENT_ERROR", "INTERNAL_SERVER_ERROR" }.Contains(paymentResponse.Code))
                {
                    return RedirectToPage("/Error");
                }
              var discountvalue = HttpContext.Session.GetInt32("Discount");
                string check = HttpContext.Session.GetString("AppliedCoupon");
                // Calculate totals
                CalculateTotals(isBuyNow,discountvalue,check);

                // Retrieve session data
                string email = HttpContext.Session.GetString("Email");
                string comment = HttpContext.Session.GetString("Comment");
                paymentMethod = HttpContext.Session.GetString("PaymentMethod");
               Phone = HttpContext.Session.GetString("Phone");
                bool isShipToDifferentAddress = HttpContext.Session.GetString("IsShipToDifferentAddress") == "True";

                Detail = JsonConvert.DeserializeObject<TblBillingDetail>(HttpContext.Session.GetString("BillingDetails"));
                ShippingDetail = JsonConvert.DeserializeObject<TblShippingDetail>(HttpContext.Session.GetString("ShippingDetails"));

                // Check if Order ID was pre-generated during checkout
                string preGeneratedOrderId = HttpContext.Session.GetString("PreGeneratedOrderId");
                string orderId;
                
                if (!string.IsNullOrEmpty(preGeneratedOrderId))
                {
                    // Use the pre-generated Order ID from checkout
                    orderId = preGeneratedOrderId;
                    // Clear the session variable
                    HttpContext.Session.Remove("PreGeneratedOrderId");
                }
                else
                {
                    // Generate new Order ID using stored procedure (fallback)
                    orderId = await GenerateOrderId(email, comment, paymentMethod, paymentResponse.Code);
                }

                // Save Billing and Shipping Details
                await SaveBillingAndShippingDetails(orderId, email, isShipToDifferentAddress,Phone);

                // Save Order Details
                await SaveOrderDetails(orderId, email, isBuyNow);

                // Handle post-payment response
                return await HandlePaymentResponse(paymentResponse.Code, orderId);
            }
            catch (Exception ex)
            {
                // Log exception (if logging is implemented)
                return RedirectToPage("/Error");
            }
        }

        private void CalculateTotals(bool isBuyNow,int? discountvalue,string check)
        {
            subtotal = isBuyNow
                ? childskuCodes.Sum(item => (double)item.Price * item.Quantity)
                : Carts.Sum(item => (double)item.Price * item.Qty);
            if (isBuyNow == true) {
                shipping = subtotal < 3000 ? 80 : 0;
                Total = subtotal + shipping;

            }
            else
            {
                if (check == "True")
                {
                    DiscountAmount = subtotal * (double)discountvalue / 100;
                    AfterDiscount = subtotal - DiscountAmount;
                    shipping = AfterDiscount < 3000 ? 80 : 0;
                    Total = AfterDiscount + shipping;
                }
                else
                {
                    shipping = subtotal < 3000 ? 80 : 0;
                    Total = subtotal + shipping;

                }
            }

           ;
        }



        private async Task<string> GenerateOrderId(string email, string comment, string paymentMethod, string paymentStatus)
        {
            string Email = email; // Replace with actual email if available
            date = DateTime.Now;
            float couponCode = 0; // Assuming 0 if no coupon code is applied; update accordingly
            string status = "Pending";
            float totalAmount = (float)Total;// Will be calculated later based on cart
            string paymentFrom = paymentMethod;
            string PaymentStatus = paymentStatus == "PAYMENT_SUCCESS" ? "Completed" : "Pending";
            string OrderNotes = comment;

            // Define output parameter for the generated Order ID
            var generatedOrderId = new SqlParameter
            {
                ParameterName = "@ProdID",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Size = 15,
                Direction = System.Data.ParameterDirection.Output
            };

            // Execute the stored procedure
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC SpOrderId @EmailID, @Date, @CouponCode, @Status, @TotalAmount, @PaymentFrom, @PaymentStatus,@OrderNotes, @ProdID OUTPUT",
                new SqlParameter("@EmailID", Email),
                new SqlParameter("@Date", date), // Pass as DateTime
                new SqlParameter("@CouponCode", couponCode), // Pass as float
                new SqlParameter("@Status", status),
                new SqlParameter("@TotalAmount", totalAmount), // Pass as float
                new SqlParameter("@PaymentFrom", paymentFrom),
                new SqlParameter("@PaymentStatus", PaymentStatus),
                new SqlParameter("@OrderNotes", OrderNotes),
                generatedOrderId
            );

            // Retrieve the generated Order ID
            return generatedOrderId.Value.ToString();

        }

        private async Task SaveBillingAndShippingDetails(string orderId, string email, bool isShipToDifferentAddress, string Phone)
        {
            Detail.ContactNumber = Phone;
            Detail.Orderid = orderId;
            Detail.Emailid = email;
            Detail.FillDefaults();
            await _context.TblBillingDetails.AddAsync(Detail);
            await _context.SaveChangesAsync();

            if (!isShipToDifferentAddress)
            {
                ShippingDetail = Detail.CloneToShipping(orderId);
            }
            else
            {
                ShippingDetail.FillDefaults(orderId, Detail);
            }

            await _context.TblShippingDetails.AddAsync(ShippingDetail);
            await _context.SaveChangesAsync();
        }

        private async Task SaveOrderDetails(string orderId, string email, bool isBuyNow)
        {
            if (isBuyNow)
            {
                foreach (var item in childskuCodes)
                {
                    var orderDetail = new TblCustomerOrderDetails
                    {
                        OrderCode = orderId,
                        SkuCode = item.SKUCode ?? "NA",
                        Qty = item.Quantity, // For Buy Now, it's Quantity
                        Price = (float)item.Price,
                        Status = "Pending",
                        Gst = (float)item.Gst,
                        Material = item.Material,
                        Size = item.Size,
                        ProductId = item.ProductID,
                        AddOn = item.Addon,
                        Email = email ?? "NA"
                    };
                    await _context.TblCustomerOrderDetails.AddAsync(orderDetail);
                }
            }
            else
            {
                foreach (var item in Carts)
                {
                    var orderDetail = new TblCustomerOrderDetails
                    {
                        OrderCode = orderId,
                        SkuCode = item.skucode ?? "NA",
                        Qty = item.Qty, // For Cart, it's Qty
                        Price = (float)item.Price,
                        Status = "Pending",
                        Gst = (float)item.Gst,
                        Material = item.MaterialName,
                        Size = item.Size,
                        ProductId = item.ProductId,
                        AddOn = item.Addon,
                        Email = email ?? "NA"
                    };
                    await _context.TblCustomerOrderDetails.AddAsync(orderDetail);
                }
            }

            await _context.SaveChangesAsync();
        }


        private async Task<IActionResult> HandlePaymentResponse(string paymentCode, string orderId)
        {
            HttpContext.Session.SetString("OrderCompleted", "true");
            HttpContext.Session.SetString("OrderId", orderId);

            if (paymentCode == "PAYMENT_SUCCESS")
            {
                await SendMail();
                return Page();
            }

            return RedirectToPage(paymentCode == "PAYMENT_ERROR" ? "/Error" : "/Error1");
        }

        public async Task<IActionResult> SendMail()
        {
            var orderid = HttpContext.Session.GetString("OrderId");
            var name = HttpContext.Session.GetString("name");
            bool isBuyNow = HttpContext.Session.GetString("isBuyNow") == "True";
            string Email = HttpContext.Session.GetString("Email");
            List<OrderMail> orderDetails = new List<OrderMail>();

            // Process BuyNow items
            if (isBuyNow)
            {
                childskuCodes = SessionHelper.GetObjectFromJson<List<ChildskuCode>>(HttpContext.Session, "buynowItem");
                if (childskuCodes != null && childskuCodes.Any())
                {
                    foreach (var item in childskuCodes)
                    {
                        orderDetails.Add(new OrderMail
                        {
                            ProductName = item.Product,
                            Qty = item.Quantity,
                            Price = item.Price,
                            MaterialName = item.Material ?? "NA",
                            Size = item.Size ?? "NA",
                            Addon = item.Addon ?? "NA"
                        });
                    }
                }
            }
            else // Process Cart items
            {
                Carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                if (Carts != null && Carts.Any())
                {
                    foreach (var item in Carts)
                    {
                        orderDetails.Add(new OrderMail
                        {
                            ProductName = item.ProductName,
                            Qty = item.Qty,
                            Price = item.Price,
                            MaterialName = item.MaterialName ?? "NA",
                            Size = item.Size ?? "NA",
                            Addon = item.Addon ?? "NA"
                        });
                    }
                }
            }
            MailCredentials = await _context.TblMailCredentials.FirstOrDefaultAsync();

            // Construct Email Content Dynamically
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(MailCredentials.MailId));
            email.To.Add(MailboxAddress.Parse(Email));
            email.To.Add(MailboxAddress.Parse(MailCredentials.MailId)); // Recipient's email
            email.Subject = $"Order Confirmation - Order ID: {orderid}";

            // Build the HTML body with order details
            StringBuilder emailBody = new StringBuilder();

            // Greeting and Welcome Message
            emailBody.Append($"<p>Dear <strong>{name}</strong>,</p>");
            emailBody.Append("<p>Thank you for choosing <strong>Crystals by Riya</strong>! We’re excited to have you in our crystal-loving community. Each piece is ethically sourced to bring positive energy and beauty into your life.</p>");

            // Order Details Table
            emailBody.Append($"<h3>Your Order Confirmation - Order ID: {orderid}</h3>");
            emailBody.Append("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>");
            emailBody.Append("<tr style='background-color: #f4f4f4; text-align: left;'><th>Product Name</th><th>Quantity</th><th>Price</th><th>Material</th><th>Size</th><th>AddOn</th></tr>");

            foreach (var detail in orderDetails)
            {
                emailBody.Append($@"
    <tr>
        <td>{detail.ProductName}</td>
        <td>{detail.Qty}</td>
        <td>₹ {detail.Price:0.00}</td>
        <td>{detail.MaterialName}</td>
        <td>{detail.Size}</td>
        <td>{detail.Addon}</td>
    </tr>");
            }
            emailBody.Append("</table>");

            // Next Steps
            emailBody.Append(@"
    <h4>What’s Next?</h4>
    <p>Your order is on its way! Check your email and registered Phone Number for tracking details.</p>");

            // Stay Connected
            emailBody.Append(@"
    <h4>Stay Connected:</h4>
    <p>Share your experience and tag us on Instagram <strong>@crystalsbyriya</strong> to inspire our community.</p>");

            // Contact Information
            emailBody.Append(@"
    <p>For any questions, reach out to us:</p>
    <ul>
        <li>Email: <a href='mailto:Info@crystalsbyriya.com'>Info@crystalsbyriya.com</a></li>
        <li>Instagram: <a href='https://www.instagram.com/crystalsbyriya'>@crystalsbyriya</a></li>
    </ul>");

            // Closing
            emailBody.Append(@"
    <p>Wishing you love, light, and positive energy,</p>
    <p>With Regards,</p>
    <p><strong>Team Crystals By Riya</strong></p>");

            // Attach email body
            BodyBuilder builder = new BodyBuilder
            {
                HtmlBody = emailBody.ToString()
            };
            email.Body = builder.ToMessageBody();

            // Send email using SMTP
            try
            {
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(MailCredentials.MailId, MailCredentials.MailPassword);

                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
                return new JsonResult(new { success = true, message = "Order details sent to your email." });
            }
            catch (Exception ex)
            {
                // Log the error and return a failure response
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return new JsonResult(new { success = false, message = "Failed to send email. Please try again." });
            }
        }


        public async Task<IActionResult> OnPostAsync(string response)
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    // Read the body of the request
                    var body = await reader.ReadToEndAsync();

                    // Process the webhook data here (e.g., log it, save it to the database, etc.)

                    // Respond with a success message
                    return new JsonResult(new { success = true, message = "Webhook received!" });
                }
            }
            catch (Exception ex)
            {
                return Page();
            }
        }
        private static string Sha256Hash(string value)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
public static class BillingDetailExtensions
{
    public static void FillDefaults(this TblBillingDetail detail)
    {
        detail.Apartment ??= "NA";
        detail.LastName ??= "NA";
        detail.Gst ??= "3";
        detail.Name ??= "NA";
        detail.Country ??= "India";
        detail.ContactNumber ??= "0000000000";
        detail.City ??= "NA";
        detail.State ??= "NA";
        detail.Address ??= "NA";
        detail.PinCode ??= "000000";
    }

    public static TblShippingDetail CloneToShipping(this TblBillingDetail billing, string orderId)
    {
        return new TblShippingDetail
        {
            Orderid = orderId,
            Apartment = billing.Apartment,
            LastName = billing.LastName,
            Name = billing.Name,
            Country = billing.Country,
            City = billing.City,
            State = billing.State,
            Address = billing.Address,
            Emailid = billing.Emailid,
            PinCode = billing.PinCode,
            ContactNumber = billing.ContactNumber
        };
    }

    public static void FillDefaults(this TblShippingDetail shipping, string orderId, TblBillingDetail billing)
    {
        shipping.Orderid = orderId;
        shipping.Apartment ??= billing.Apartment;
        shipping.LastName ??= billing.LastName;
        shipping.Name ??= billing.Name;
        shipping.Country ??= billing.Country;
        shipping.City ??= billing.City;
        shipping.State ??= billing.State;
        shipping.Address ??= billing.Address;
        shipping.Emailid ??= billing.Emailid;
        shipping.PinCode ??= billing.PinCode;
        shipping.ContactNumber ??= billing.ContactNumber;
    }
}
public class PaymentResponse
{
    public bool Success { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public PaymentData Data { get; set; }
}

public class PaymentData
{
    public string MerchantId { get; set; }
    public string MerchantTransactionId { get; set; }
    public string TransactionId { get; set; }
    public int Amount { get; set; }
    public string State { get; set; }
    public string ResponseCode { get; set; }
    public PaymentInstrument PaymentInstrument { get; set; }
}

public class PaymentInstrument
{
    public string Type { get; set; }
    public string PgTransactionId { get; set; }
    public string PgServiceTransactionId { get; set; }
    public string BankTransactionId { get; set; }
    public string BankId { get; set; }
    public string Arn { get; set; }
}
