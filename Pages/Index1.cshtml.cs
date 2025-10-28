using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace CrystalByRiya.Pages
{
    public class Index1Model : PageModel
    {
        private readonly PhonePePaymentService _paymentService;
        public Index1Model(PhonePePaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public int OrderId { get; set; }

        public void OnGet()
        {
            Random rnd = new Random();
            OrderId = rnd.Next(11111, 99999);
        }
        public async Task<IActionResult> OnPost(string orderId, decimal amount, string Phone)
        {
            Random rnd = new Random();
            OrderId = rnd.Next(11111, 99999);
            PhonePeCredientials.OrderId = OrderId.ToString();
            Random rnd1 = new Random();
            int newMerchantId = rnd1.Next(111111, 999999);
            string NewMid = "UM" + newMerchantId;
            var data = new Dictionary<string, object>
                     {

              { "merchantId", PhonePeCredientials.Merchantid },
{ "merchantTransactionId",PhonePeCredientials.OrderId },
{ "merchantUserId", "Muid"+PhonePeCredientials.OrderId},
{ "amount", 10000},
{ "redirectUrl", PhonePeCredientials.RedirectUrl},
 {"redirectMode", "REDIRECT"},
 {"callbackUrl", PhonePeCredientials.CallbackUrl},
 {"mobileNumber", "9999999999"},
 { "paymentInstrument", new Dictionary<string, string> { { "type", "PAY_PAGE" } } }

};
            var encode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));

            var stringToHash = encode + "/pg/v1/pay" + PhonePeCredientials.SaltKey;
            var sha256 = Sha256Hash(stringToHash);
            var finalXHeader = sha256 + "###" + PhonePeCredientials.saltIndex;
            PhonePeCredientials.xverify = finalXHeader;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("X-VERIFY", finalXHeader);
                var requestData = new Dictionary<string, string> { { "request", encode } };
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = client.PostAsync(PhonePeCredientials.PostUrl, content).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var rData = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return Redirect(rData.data.instrumentResponse.redirectInfo.url.ToString());
            }

        }
        public string Production(string orderId, decimal amount, string Phone)
        {
            Random rnd = new Random();
            int newMerchantId = rnd.Next(11111, 99999);
            string NewMid = "UM" + newMerchantId;
            var data = new Dictionary<string, object>
                       {
                           { "merchantId", PhonePeCredientials.Merchantid},
             {"merchantTransactionId", Guid.NewGuid().ToString()},
            { "merchantUserId", NewMid},
             {"amount", amount},
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

