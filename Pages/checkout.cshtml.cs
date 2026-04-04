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
        private static readonly List<string> CountryOptionsData = new()
        {
            "Australia",
            "Austria",
            "Belgium",
            "Canada",
            "Czech Republic",
            "Denmark",
            "Finland",
            "France",
            "Germany",
            "United States",
            "United Kingdom",
            "India",
            "Japan",
            "Mexico",
            "South Korea",
            "Spain",
            "Italy",
            "Vietnam"
        };

        private static readonly Dictionary<string, List<string>> StatesByCountry = new(StringComparer.OrdinalIgnoreCase)
        {
            ["India"] = new List<string>
            {
                "Andhra Pradesh",
                "Arunachal Pradesh",
                "Assam",
                "Bihar",
                "Chhattisgarh",
                "Goa",
                "Gujarat",
                "Haryana",
                "Himachal Pradesh",
                "Jharkhand",
                "Karnataka",
                "Kerala",
                "Madhya Pradesh",
                "Maharashtra",
                "Manipur",
                "Meghalaya",
                "Mizoram",
                "Nagaland",
                "Odisha",
                "Punjab",
                "Rajasthan",
                "Sikkim",
                "Tamil Nadu",
                "Telangana",
                "Tripura",
                "Uttar Pradesh",
                "Uttarakhand",
                "West Bengal",
                "Andaman and Nicobar Islands",
                "Chandigarh",
                "Dadra and Nagar Haveli and Daman and Diu",
                "Delhi",
                "Jammu and Kashmir",
                "Ladakh",
                "Lakshadweep",
                "Puducherry"
            },
            ["Australia"] = new List<string>
            {
                "New South Wales",
                "Queensland",
                "South Australia",
                "Tasmania",
                "Victoria",
                "Western Australia",
                "Australian Capital Territory",
                "Northern Territory"
            },
            ["Austria"] = new List<string>
            {
                "Burgenland",
                "Carinthia",
                "Lower Austria",
                "Upper Austria",
                "Salzburg",
                "Styria",
                "Tyrol",
                "Vorarlberg",
                "Vienna"
            },
            ["Belgium"] = new List<string>
            {
                "Antwerp",
                "Brussels-Capital Region",
                "East Flanders",
                "Flemish Brabant",
                "Hainaut",
                "Liege",
                "Limburg",
                "Luxembourg",
                "Namur",
                "Walloon Brabant",
                "West Flanders"
            },
            ["Canada"] = new List<string>
            {
                "Alberta",
                "British Columbia",
                "Manitoba",
                "New Brunswick",
                "Newfoundland and Labrador",
                "Nova Scotia",
                "Ontario",
                "Prince Edward Island",
                "Quebec",
                "Saskatchewan",
                "Northwest Territories",
                "Nunavut",
                "Yukon"
            },
            ["Czech Republic"] = new List<string>
            {
                "Prague",
                "Central Bohemian",
                "South Bohemian",
                "Plzen",
                "Karlovy Vary",
                "Usti nad Labem",
                "Liberec",
                "Hradec Kralove",
                "Pardubice",
                "Vysocina",
                "South Moravian",
                "Olomouc",
                "Zlin",
                "Moravian-Silesian"
            },
            ["Denmark"] = new List<string>
            {
                "Capital Region",
                "Central Denmark",
                "North Denmark",
                "Region Zealand",
                "Region of Southern Denmark"
            },
            ["Finland"] = new List<string>
            {
                "Uusimaa",
                "Southwest Finland",
                "Satakunta",
                "Hame",
                "Pirkanmaa",
                "Pohjanmaa",
                "North Ostrobothnia",
                "Lapland",
                "Aland"
            },
            ["France"] = new List<string>
            {
                "Auvergne-Rhone-Alpes",
                "Bourgogne-Franche-Comte",
                "Brittany",
                "Centre-Val de Loire",
                "Corsica",
                "Grand Est",
                "Hauts-de-France",
                "Ile-de-France",
                "Normandy",
                "Nouvelle-Aquitaine",
                "Occitanie",
                "Pays de la Loire",
                "Provence-Alpes-Cote d'Azur"
            },
            ["Germany"] = new List<string>
            {
                "Baden-Wurttemberg",
                "Bavaria",
                "Berlin",
                "Brandenburg",
                "Bremen",
                "Hamburg",
                "Hesse",
                "Lower Saxony",
                "Mecklenburg-Vorpommern",
                "North Rhine-Westphalia",
                "Rhineland-Palatinate",
                "Saarland",
                "Saxony",
                "Saxony-Anhalt",
                "Schleswig-Holstein",
                "Thuringia"
            },
            ["United States"] = new List<string>
            {
                "Alabama",
                "Alaska",
                "Arizona",
                "Arkansas",
                "California",
                "Colorado",
                "Connecticut",
                "Delaware",
                "Florida",
                "Georgia",
                "Hawaii",
                "Idaho",
                "Illinois",
                "Indiana",
                "Iowa",
                "Kansas",
                "Kentucky",
                "Louisiana",
                "Maine",
                "Maryland",
                "Massachusetts",
                "Michigan",
                "Minnesota",
                "Mississippi",
                "Missouri",
                "Montana",
                "Nebraska",
                "Nevada",
                "New Hampshire",
                "New Jersey",
                "New Mexico",
                "New York",
                "North Carolina",
                "North Dakota",
                "Ohio",
                "Oklahoma",
                "Oregon",
                "Pennsylvania",
                "Rhode Island",
                "South Carolina",
                "South Dakota",
                "Tennessee",
                "Texas",
                "Utah",
                "Vermont",
                "Virginia",
                "Washington",
                "West Virginia",
                "Wisconsin",
                "Wyoming",
                "District of Columbia"
            },
            ["United Kingdom"] = new List<string>
            {
                "England",
                "Scotland",
                "Wales",
                "Northern Ireland"
            },
            ["Japan"] = new List<string>
            {
                "Tokyo",
                "Osaka",
                "Kanagawa",
                "Aichi",
                "Hokkaido",
                "Fukuoka",
                "Hyogo",
                "Saitama",
                "Chiba",
                "Kyoto"
            },
            ["Mexico"] = new List<string>
            {
                "Aguascalientes",
                "Baja California",
                "Baja California Sur",
                "Campeche",
                "Chiapas",
                "Chihuahua",
                "Coahuila",
                "Colima",
                "Durango",
                "Guanajuato",
                "Guerrero",
                "Hidalgo",
                "Jalisco",
                "Mexico City",
                "Michoacan",
                "Morelos",
                "Nayarit",
                "Nuevo Leon",
                "Oaxaca",
                "Puebla",
                "Queretaro",
                "Quintana Roo",
                "San Luis Potosi",
                "Sinaloa",
                "Sonora",
                "Tabasco",
                "Tamaulipas",
                "Tlaxcala",
                "Veracruz",
                "Yucatan",
                "Zacatecas"
            },
            ["South Korea"] = new List<string>
            {
                "Seoul",
                "Busan",
                "Daegu",
                "Incheon",
                "Gwangju",
                "Daejeon",
                "Ulsan",
                "Sejong",
                "Gyeonggi-do",
                "Gangwon-do",
                "Chungcheongbuk-do",
                "Chungcheongnam-do",
                "Jeollabuk-do",
                "Jeollanam-do",
                "Gyeongsangbuk-do",
                "Gyeongsangnam-do",
                "Jeju-do"
            },
            ["Spain"] = new List<string>
            {
                "Andalusia",
                "Aragon",
                "Asturias",
                "Balearic Islands",
                "Basque Country",
                "Canary Islands",
                "Cantabria",
                "Castile and Leon",
                "Castilla-La Mancha",
                "Catalonia",
                "Extremadura",
                "Galicia",
                "La Rioja",
                "Madrid",
                "Murcia",
                "Navarre",
                "Valencian Community"
            },
            ["Italy"] = new List<string>
            {
                "Abruzzo",
                "Aosta Valley",
                "Apulia",
                "Basilicata",
                "Calabria",
                "Campania",
                "Emilia-Romagna",
                "Friuli Venezia Giulia",
                "Lazio",
                "Liguria",
                "Lombardy",
                "Marche",
                "Molise",
                "Piedmont",
                "Sardinia",
                "Sicily",
                "Trentino-Alto Adige",
                "Tuscany",
                "Umbria",
                "Veneto"
            },
            ["Vietnam"] = new List<string>
            {
                "Hanoi",
                "Ho Chi Minh City",
                "An Giang",
                "Ba Ria - Vung Tau",
                "Bac Giang",
                "Bac Kan",
                "Bac Lieu",
                "Bac Ninh",
                "Ben Tre",
                "Binh Dinh",
                "Binh Duong",
                "Binh Phuoc",
                "Binh Thuan",
                "Ca Mau",
                "Can Tho",
                "Da Nang"
            }
        };


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
        public IReadOnlyList<string> CountryOptions => CountryOptionsData;
        public IReadOnlyList<string> StateOptions => GetStatesForCountry(Detail?.Country);

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

            EnsureCheckoutDefaults();
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
                    return LocalRedirect(redirectUrl);
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
                EnsureCheckoutDefaults();
                return Page();
            }
        }



        public async Task<IActionResult> OnPostPlaceOrder(string Phone, string comment, string paymentMethod, bool shipment, string email)
        {
            EnsureCheckoutDefaults();
            Phone = string.IsNullOrWhiteSpace(Phone) ? Detail?.ContactNumber : Phone;
            email = string.IsNullOrWhiteSpace(email) ? Detail?.Emailid : email;

            if (string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Please provide a valid email and phone number.");
                return Page();
            }

            // Debug: Check what fields are actually being received
            if (Detail == null)
            {
                ModelState.AddModelError(string.Empty, "Billing details object is null.");
                return Page();
            }

            // Debug: Log field values for troubleshooting
            var debugInfo = new List<string>();
            if (string.IsNullOrWhiteSpace(Detail.Name)) debugInfo.Add("Name is empty");
            if (string.IsNullOrWhiteSpace(Detail.LastName)) debugInfo.Add("LastName is empty");
            if (string.IsNullOrWhiteSpace(Detail.Emailid)) debugInfo.Add("Emailid is empty");
            if (string.IsNullOrWhiteSpace(Detail.ContactNumber)) debugInfo.Add("ContactNumber is empty");
            if (string.IsNullOrWhiteSpace(Detail.Address)) debugInfo.Add("Address is empty");
            if (string.IsNullOrWhiteSpace(Detail.City)) debugInfo.Add("City is empty");
            if (string.IsNullOrWhiteSpace(Detail.State)) debugInfo.Add("State is empty");
            if (string.IsNullOrWhiteSpace(Detail.Country)) debugInfo.Add("Country is empty");
            if (string.IsNullOrWhiteSpace(Detail.PinCode)) debugInfo.Add("PinCode is empty");

            if (debugInfo.Any())
            {
                var errorMessage = "Missing required fields: " + string.Join(", ", debugInfo);
                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }

            // If we get here, all required fields are filled
            // No need for additional ModelState.IsValid check since we already validated manually

            double subtotal = 0;
            double shipping = 0;
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

            if (subtotal <= 0)
            {
                ModelState.AddModelError(string.Empty, "Your cart is empty.");
                return Page();
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
            
            // Generate proper Order ID using stored procedure before payment
            string order_id = await GenerateOrderIdForPayment(email, comment, paymentMethod, Total);
            PhonePeCredientials.OrderId = order_id;
            HttpContext.Session.SetString("PhonePeTransactionId", order_id);
            HttpContext.Session.SetString("PreGeneratedOrderId", order_id); // Store for thank you page

            await SavePendingOrderDetails(order_id, email, isBuyNow);

            Random rnd = new Random();
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

        private async Task<string> GenerateOrderIdForPayment(string email, string comment, string paymentMethod, double totalAmount)
        {
            var date = DateTime.Now;
            var couponCodeValue = HttpContext.Session.GetInt32("Discount") ?? 0; // Get discount as integer
            var status = "PENDING";
            var paymentFrom = paymentMethod;
            var paymentStatus = "PENDING";
            var orderNotes = comment ?? "NA";

            // Define output parameter for the generated Order ID
            var generatedOrderId = new SqlParameter
            {
                ParameterName = "@ProdID",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Size = 50,
                Direction = System.Data.ParameterDirection.Output
            };

            // Execute the stored procedure with correct data types
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC SpOrderId @EmailID, @Date, @CouponCode, @Status, @TotalAmount, @PaymentFrom, @PaymentStatus, @OrderNotes, @ProdID OUTPUT",
                new SqlParameter("@EmailID", email),
                new SqlParameter("@Date", date),
                new SqlParameter("@CouponCode", couponCodeValue), // Changed to integer
                new SqlParameter("@Status", status),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@PaymentFrom", paymentFrom),
                new SqlParameter("@PaymentStatus", paymentStatus),
                new SqlParameter("@OrderNotes", orderNotes),
                generatedOrderId
            );

            // Retrieve the generated Order ID
            string orderId = generatedOrderId.Value.ToString();

            // Verify the order was actually created in database
            var verifyOrder = await _context.TblBillingDetails
                .FirstOrDefaultAsync(o => o.Orderid == orderId);

            if (verifyOrder == null)
            {
                // If order not found, create a basic order record manually
                var newOrder = new TblBillingDetail
                {
                    Orderid = orderId,
                    Emailid = email,
                    Name = Detail?.Name ?? "",
                    LastName = Detail?.LastName ?? "",
                    ContactNumber = Detail?.ContactNumber ?? "",
                    Address = Detail?.Address ?? "",
                    City = Detail?.City ?? "",
                    State = Detail?.State ?? "",
                    Country = Detail?.Country ?? "",
                    PinCode = Detail?.PinCode ?? "",
                    FullName = (Detail?.Name ?? "") + " " + (Detail?.LastName ?? ""),
                    Gst = "0"
                };

                _context.TblBillingDetails.Add(newOrder);
                await _context.SaveChangesAsync();
            }

            return orderId;
        }

        private async Task SavePendingOrderDetails(string orderId, string email, bool isBuyNow)
        {
            if (isBuyNow)
            {
                foreach (var item in childskuCodes)
                {
                    var orderDetail = new TblCustomerOrderDetails
                    {
                        OrderCode = orderId,
                        SkuCode = item.SKUCode ?? "NA",
                        Qty = item.Quantity,
                        Price = item.Price,
                        Status = "Pending",
                        Gst = item.Gst,
                        Material = item.Material ?? "NA",
                        Size = item.Size ?? "NA",
                        ProductId = item.ProductID ?? "NA",
                        AddOn = item.Addon ?? "NA",
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
                        Qty = item.Qty,
                        Price = item.Price,
                        Status = "Pending",
                        Gst = item.Gst,
                        Material = item.MaterialName ?? "NA",
                        Size = item.Size ?? "NA",
                        ProductId = item.ProductId ?? "NA",
                        AddOn = item.Addon ?? "NA",
                        Email = email ?? "NA"
                    };

                    await _context.TblCustomerOrderDetails.AddAsync(orderDetail);
                }
            }

            await _context.SaveChangesAsync();
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

        private void EnsureCheckoutDefaults()
        {
            Detail ??= new TblBillingDetail();

            if (string.IsNullOrWhiteSpace(Detail.Country))
            {
                Detail.Country = "India";
            }

            if (string.IsNullOrWhiteSpace(Detail.State))
            {
                var states = GetStatesForCountry(Detail.Country);
                if (states.Count > 0)
                {
                    Detail.State = states[0];
                }
            }
        }

        private static List<string> GetStatesForCountry(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                country = "India";
            }

            return StatesByCountry.TryGetValue(country, out var states)
                ? states
                : new List<string>();
        }
    }

}
