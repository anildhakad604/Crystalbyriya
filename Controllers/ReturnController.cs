
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CrystalByRiya.Controllers
{
    public class ReturnController : Controller
    {
        ApplicationDbContext context;
        public ReturnController(ApplicationDbContext _context)
        {
            context = _context;
        }
        //
        // GET: /Return/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Return(string responses)
        {
            string xverify = PhonePeCredientials.xverify;
            var PhonePeGatewayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";
            using (var httpClient = new HttpClient())
            {
                var uri = new Uri($"{PhonePeGatewayURL}/pg/v1/status/{PhonePeCredientials.Merchantid}/{PhonePeCredientials.OrderId}");
                // Add headers
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("X-VERIFY", xverify);
                httpClient.DefaultRequestHeaders.Add("X-MERCHANT-ID", PhonePeCredientials.Merchantid);

                // Send GET request
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                Instagram i = new Instagram()
                {
                    InstagramUrl = responses
                };
                await context.TblInstagram.AddAsync(i);
                await context.SaveChangesAsync();

                // Return a response
                return Json(new { Success = true, Message = "Verification successful", phonepeResponse = responseContent });
            }


        }

        public IActionResult Error()
        {
            string url = "https://www.makelenofficial.com/Error";
            return Redirect(url);

        }



        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }










    }
}
