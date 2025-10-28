using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CrystalByRiya.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext context;
        public HomeController(ApplicationDbContext _context, IHttpContextAccessor contextAccessor)
        {
            context = _context;
        }
       
       
        public IActionResult Navigate()
        {
            return RedirectPermanent("https://www.makelenofficial.com/success");
        }
        public void Demo()
        {
          string firstName =  HttpContext.Session.GetString("Name");
          string phone =  HttpContext.Session.GetString("mobile");
          string email =  HttpContext.Session.GetString("email");
          string amount =   HttpContext.Session.GetString("amount");
          string productInfo = HttpContext.Session.GetString("orderId");         

           
           

            //posting all the parameters required for integration.

           /* Dictionary<string, string> myremotepost = new Dictionary<string, string>();
            string Url = "https://secure.payu.in/_payment";
            myremotepost.Add("key", PayuCrediential.Pkey);
            string txnid = Generatetxnid();
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", amount);
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            myremotepost.Add("surl", "https://www.makelenofficial.com/Home/Return");//Success url.
            myremotepost.Add("furl", "https://www.makelenofficial.com/Home/Error");//Failure url
            myremotepost.Add("service_provider", "payu_paisa");
            string hashString = PayuCrediential.Pkey + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + PayuCrediential.salt;
           
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);
            string outputHTML = "<html><head>";
            outputHTML += "</head><body onload=\"document.form1.submit()\">";
            outputHTML += "<form name=\"form1\" method=\"post\" action=\""+ Url + "\">";//, FormName, Method, Url));
           
            foreach (string key in myremotepost.Keys)
            {
                outputHTML += "<input type='hidden' name='" + key + "' value='" + myremotepost[key] + "'>";
            }


            outputHTML += "</form></body></html>";
            HttpContext.Response.WriteAsync(outputHTML);*/


        }

           


        //Hash generation Algorithm

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


        public string Generatetxnid()
        {

            Random rnd = new Random();
            string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);

            return txnid1;
        }
        
        public async Task<IActionResult> Return()
        {

            string[] merc_hash_vars_seq;
            string merc_hash_string = string.Empty;
            string merc_hash = string.Empty;
            string order_id = string.Empty;
            string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

            if (Request.Form["status"].ToString() == "success")
            {
                string Orderid = Request.Form["productinfo"];
                merc_hash_vars_seq = hash_seq.Split('|');
                Array.Reverse(merc_hash_vars_seq);
                // merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();


                foreach (string merc_hash_var in merc_hash_vars_seq)
                {
                    merc_hash_string += "|";
                    // merc_hash_string = merc_hash_string + (form[merc_hash_var] != null ? form[merc_hash_var] : "");

                }
                // Response.Write(merc_hash_string);
                merc_hash = Generatehash512(merc_hash_string).ToLower();



                if (merc_hash != Request.Form["hash"])
                {
                   // Response.WriteAsync("Hash value did not matched");

                }
                else
                {
                    order_id = Request.Form["txnid"];

                    ViewData["Message"] = "Status is successful. Hash value is matched";
                    //Response.WriteAsync("<br/>Hash value matched");

                    //Hash value did not matched
                }

                string firstName = Request.Form["firstname"];//HttpContext.Session.GetString("Name");
                string phone = Request.Form["phone"];
                string email = Request.Form["email"];
                string amount = Request.Form["amount"];
                string Neworderid = Request.Form["productinfo"];

              /*  TblOrderId paymentfrom =await context.TblOrderIds.SingleOrDefaultAsync(orderid => orderid.Orderid == Neworderid);
                paymentfrom.PaymentFrom = "Payu";
                paymentfrom.PaymentStatus = "Completed";
                context.SaveChanges();
                Sms.SendSMS(phone, Neworderid, Convert.ToString(amount));    */            
                return LocalRedirect("/Home/Navigate");


            }

            else
            {                
                 
                return LocalRedirect("/Home/Error");
                // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

            }
          
        }

        public IActionResult Error()
        {
            string url = "https://www.makelenofficial.com/PaymentFailed";
            return Redirect(url);

        }

        public IActionResult SuccessRedirect()
        {
            string url = "https://www.makelenofficial.com/success";
            return Redirect(url);
        }



      
    }
}

   