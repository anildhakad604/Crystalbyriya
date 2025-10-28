using Astaberry.Helpers;
using CrystalByRiya.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace CrystalByRiya.Pages
{
    public class checkoutModel : PageModel
    {
        private readonly PhonePePaymentService _paymentService;


        private readonly ApplicationDbContext _context;

        public checkoutModel(ApplicationDbContext context, PhonePePaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }
        public class OrderDetails
        {
            public string OrderId { get; set; }
            public string username { get; set; }
            public string Phone { get; set; }
            public string PaymentMethod { get; set; }
            public double Subtotal { get; set; }
            public string Shippingaddress { get; set; }
            public string Billingaddress { get; set; }
            public double Total { get; set; }
            public List<Item> Items { get; set; }

            public string comment { get; set; }

            public DateTime OrderDate { get; set; }
        }
        [BindProperty]
        public Register register { get; set; }
        [BindProperty(SupportsGet = true)]
        public TblBillingDetail Detail { get; set; }
        [BindProperty(SupportsGet = true)]
        public TblShippingDetail ShippingDetail { get; set; }
        public TblCustomerOrderDetails CustomerOrderDetails { get; set; }

        public string UserEmail { get; private set; }
        public string Password { get; private set; }

        public string Currenturl { get; private set; }
        public List<Item> Carts { get; set; } = new List<Item>();
        public List<ChildskuCode> childskuCodes { get; set; } = new List<ChildskuCode>();

        public double GstAmount { get; set; }
        public double subtotal { get; set; }
        public double shipping { get; set; }
        public double Total { get; set; }
        public double CouponDiscount { get; set; } 
        public double DiscountAmount { get; set; }
        public double AfterDiscount { get; set; }

        public bool NeedsToAddDetails { get; set; }
        public void OnGet(bool isBuyNow = false)
        {
            HttpContext.Session.SetString("isBuyNow", isBuyNow.ToString());
            // Retrieve the current URL for display purposes
            Currenturl = HttpContext.Request.GetDisplayUrl();
          var discountvalue=  HttpContext.Session.GetInt32("Discount");
          var check=  HttpContext.Session.GetString("AppliedCoupon");

            // Remove any existing coupon code from the session
            HttpContext.Session.Remove("CouponCode");
            UserEmail = HttpContext.Session.GetString("UserEmail");

            if (isBuyNow)
            {
                // Get the Buy Now item from the session
                childskuCodes = SessionHelper.GetObjectFromJson<List<ChildskuCode>>(HttpContext.Session, "buynowItem");


                if (childskuCodes != null)
                {
                    // Process the Buy Now item(s)
                    subtotal = childskuCodes.Sum(item => item.Price * item.Quantity);

                   
                    shipping = subtotal< 3000 ? 80 : 0; // Add shipping charge if subtotal is less than 300
                    Total = subtotal + shipping;

                    // Retrieve user details if the user is logged in
                    UserEmail = HttpContext.Session.GetString("UserEmail");
                    if (!string.IsNullOrEmpty(UserEmail))
                    {
                        Detail = _context.TblBillingDetails.Where(b => b.Emailid == UserEmail).FirstOrDefault();
                        if (Detail == null)
                        {
                            Detail = new TblBillingDetail { Emailid = UserEmail };
                        }
                        ShippingDetail = _context.TblShippingDetails.Where(s => s.Emailid == UserEmail).FirstOrDefault();
                        if (ShippingDetail == null)
                        {
                            ShippingDetail = new TblShippingDetail { Emailid = UserEmail };
                        }
                    }
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "buynowItem", childskuCodes);
            }
            else
            {
                // Regular cart checkout process
                Carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

                if (Carts != null && Carts.Any())
                {
                    // Calculate subtotal and shipping for the regular cart items
                    subtotal = Carts.Sum(item => item.Price * item.Qty);
                    if (check == "True")
                    {
                        DiscountAmount = subtotal * (double)discountvalue / 100;
                        AfterDiscount = subtotal;
                        AfterDiscount -= DiscountAmount;
                        shipping = AfterDiscount < 3000 ? 80 : 0; // Add shipping charge if subtotal is less than 300
                        Total = AfterDiscount + shipping;
                    }
                    else
                    {
                        shipping = subtotal < 3000 ? 80 : 0; 
                        Total = subtotal + shipping;

                    }
                }

                // Retrieve user session information if the user is logged in
                UserEmail = HttpContext.Session.GetString("UserEmail");
                if (!string.IsNullOrEmpty(UserEmail))
                {
                    Detail = _context.TblBillingDetails.Where(b => b.Emailid == UserEmail).FirstOrDefault();
                    if (Detail == null)
                    {
                        Detail = new TblBillingDetail { Emailid = UserEmail };
                    }
                    ShippingDetail = _context.TblShippingDetails.Where(s => s.Emailid == UserEmail).FirstOrDefault();
                    if (ShippingDetail == null)
                    {
                        ShippingDetail = new TblShippingDetail { Emailid = UserEmail };
                    }
                }

            }
        }
        public async Task<IActionResult> OnPostLoginAsync(string redirectUrl)
        {

            // Check if the user exists in the database with the provided email and password
            var isexist = await _context.TblRegisters
                .SingleOrDefaultAsync(e => e.Email == register.Email && e.Password == register.Password);

            if (isexist != null)
            {
                // Store login session in cookies or session
                HttpContext.Session.SetString("UserEmail", register.Email);
                HttpContext.Session.SetString("UserName", isexist.Name); // Assuming Name field exists
                HttpContext.Session.SetString("Password", register.Password);


                // If redirectUrl is provided, redirect there after login, otherwise redirect to index
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                // If login fails, reload the page and display an error message
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
        }



        public async Task<IActionResult> OnPostPlaceOrder(string Phone, string comment, string paymentMethod, bool shipment, string email)
        {

            double subtotal = 0;
            double shipping = 0;
            double Total = 0;
            bool isBuyNow = HttpContext.Session.GetString("isBuyNow") == "True";
            var Discount = HttpContext.Session.GetInt32("Discount");
            var checkvalue = HttpContext.Session.GetString("AppliedCoupon");

            if (isBuyNow)
            {
                // Retrieve Buy Now item(s)
                childskuCodes = SessionHelper.GetObjectFromJson<List<ChildskuCode>>(HttpContext.Session, "buynowItem");
            }
            else
            {
                // Retrieve cart items
                Carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            }

            // Calculate subtotal and shipping based on the selected items
            if (Carts != null && Carts.Any())
            {
                foreach (var item in Carts)
                {
                    subtotal += item.Price * item.Qty;
                }
                if (checkvalue == "True")
                {
                    DiscountAmount = subtotal * (double)Discount / 100;
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
            else if (childskuCodes != null)
            {
                subtotal += childskuCodes.Sum(item => item.Price * item.Quantity);
                shipping = subtotal < 3000 ? 80 : 0;
                Total = subtotal + shipping;

            }



            // Apply shipping if necessary

            // Calculate the final total

            HttpContext.Session.SetString("Phone", Phone);
            HttpContext.Session.SetString("isBuyNow", isBuyNow.ToString());
            SessionHelper.SetObjectAsJson(HttpContext.Session, "buynowItem", childskuCodes);


            HttpContext.Session.SetString("Comment", comment ?? "NA");
            HttpContext.Session.SetString("PaymentMethod", paymentMethod);
            HttpContext.Session.SetString("Email", email);


            HttpContext.Session.SetString("IsShipToDifferentAddress", shipment.ToString());

            // Store other form data for Billing and Shipping details in the session
            HttpContext.Session.SetString("BillingDetails", JsonConvert.SerializeObject(Detail));
            HttpContext.Session.SetString("ShippingDetails", JsonConvert.SerializeObject(ShippingDetail));


            // Check if the payment method is 'cod' (Cash on Delivery)
            decimal amount;
            if (paymentMethod == "cod")
            {
                // Set the amount to 200 for Cash on Delivery
                amount = 200;
            }
            else
            {
                // For online payment, use the total amount calculated from the cart or Buy Now item
                amount = (decimal)Total;
            }
            double integerAmount = Convert.ToDouble(Math.Round(amount, 2) * 100);
            Random rnd = new Random();
            string order_id = rnd.Next(1000, 9999).ToString();
            PhonePeCredientials.OrderId = order_id;
            int newMerchantId = rnd.Next(111111, 999999);
            string NewMid = "UM" + newMerchantId; // Use or remove as per requirement

            var data = new Dictionary<string, object>
{
    { "merchantId", PhonePeCredientials.Merchantid },
    { "merchantTransactionId", PhonePeCredientials.OrderId },
    { "merchantUserId", "Muid" + PhonePeCredientials.OrderId },
    { "amount", integerAmount.ToString() },
    { "redirectUrl", PhonePeCredientials.RedirectUrl },
    { "redirectMode", "REDIRECT" },
    { "callbackUrl", PhonePeCredientials.CallbackUrl },
    { "mobileNumber", Phone },
    { "paymentInstrument", new Dictionary<string, string> { { "type", "PAY_PAGE" } } }
};

            var encode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            var stringToHash = encode + "/pg/v1/pay" + PhonePeCredientials.SaltKey;
            var sha256 = Sha256Hash(stringToHash);
            var finalXHeader = sha256 + "###" + PhonePeCredientials.saltIndex;
            PhonePeCredientials.xverify = finalXHeader;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-VERIFY", finalXHeader);
                var requestData = new Dictionary<string, string> { { "request", encode } };
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(PhonePeCredientials.PostUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var rData = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return Redirect(rData.data.instrumentResponse.redirectInfo.url.ToString());
            }



        }


        public string Production(string orderId, string Phone, string paymentMethod)
        {

            Random rnd = new Random();
            int newMerchantId = rnd.Next(11111, 99999);
            string NewMid = "UM" + newMerchantId;
            var data = new Dictionary<string, object>
                       {
                           { "merchantId", PhonePeCredientials.Merchantid},
             {"merchantTransactionId", Guid.NewGuid().ToString()},
            { "merchantUserId", NewMid},
             {"amount", Convert.ToString(100000)},
             {"redirectUrl", "RedirectUrl"},
            { "redirectMode", "REDIRECT"},
            { "callbackUrl", "CallbackUrl"},
             {"mobileNumber", Phone},
              { "paymentInstrument", new Dictionary<string, string> { { "type", "PAY_PAGE" } } }
            };
            var encode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            var saltKey = "SaltKey";
            var saltIndex = 1;
            var stringToHash = encode + "/pg/v1/pay" + saltKey;
            var sha256 = Sha256Hash(stringToHash);
            var finalXHeader = sha256 + "###" + saltIndex;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("X-VERIFY", finalXHeader);
                var requestData = new Dictionary<string, string> { { "request", encode } };
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = client.PostAsync("PostUrl", content).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var rData = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return rData.data.instrumentResponse.redirectInfo.url.ToString();
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

        public async Task<IActionResult> OnGetVerifyPaymentAsync(string orderId)
        {
            var verificationResponse = await _paymentService.VerifyPaymentAsync(orderId);

            // Handle verification response and update order status
            return Page();
        }
    }

}


