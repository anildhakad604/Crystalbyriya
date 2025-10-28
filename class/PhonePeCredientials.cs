/*namespace CrystalByRiya
{
    public static class PhonePeCredientials
    {
        public readonly static string RedirectUrl = "https://www.crystalsbyriya.com/thankyou";
        //public readonly static string RedirectUrl = "https://localhost:44333/thankyou";
        public readonly static string CallbackUrl = "https://www.crystalsbyriya.com/thankyou";
        //public readonly static string CallbackUrl = "https://localhost:44333/api/response";
        //public readonly static string SaltKey = "14fa5465-f8a7-443f-8477-f986b8fcfde9";
        public readonly static string SaltKey = "66222d9f-cfd6-41fd-8b83-d495f887a63e";
        public readonly static string PostUrl = "https://api.phonepe.com/apis/hermes/pg/v1/pay";
        //  public readonly static string PostUrl = "https://api-preprod.phonepe.com/apis/pg-sandbox/pg/v1/pay";
        public readonly static string Merchantid = "CRYSTALSONLINE";
        public readonly static int saltIndex = 1;
        public static string OrderId { get; set; }
        public static string xverify { get; set; }


        public static string checkstatusPhonePeGatewayURL = "https://api.phonepe.com/apis/hermes";
        //8468977350
    }
}*/
namespace CrystalByRiya
{
    public static class PhonePeCredientials
    {
        public readonly static string RedirectUrl = "https://www.crystalsbyriya.com/thankyou";
      //  public readonly static string RedirectUrl = "https://localhost:44333/thankyou";
       public readonly static string CallbackUrl = "https://www.crystalsbyriya.com/api/response";
     //   public readonly static string CallbackUrl = "https://localhost:44333/api/response";
        public readonly static string SaltKey = "66222d9f-cfd6-41fd-8b83-d495f887a63e";
        public readonly static string PostUrl = "https://api.phonepe.com/apis/hermes/pg/v1/pay";
        // public readonly static string Merchantid = "CRYSTALSONLINE";
        public readonly static int saltIndex = 1;
        public static string OrderId { get; set; }
        public static string xverify { get; set; }
      //  public static string Merchantid { get; set; } = "PGTESTPAYUAT77";
        public readonly static string Merchantid = "CRYSTALSONLINE";
    }
}
