namespace CrystalByRiya
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public string merchantId { get; set; }
        public string merchantTransactionId { get; set; }
        public string transactionId { get; set; }
        public int amount { get; set; }
        public string state { get; set; }
        public string responseCode { get; set; }
        public PaymentInstrument paymentInstrument { get; set; }
    }

    public class PaymentInstrument
    {
        public string type { get; set; }
        public string utr { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

}
