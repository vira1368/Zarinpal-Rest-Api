using Newtonsoft.Json;

namespace ZarinpalRestApi.Models
{
    public class ZarinpalModel
    {
        public class Payment
        {
            public class Request
            {
                [JsonProperty("MerchantID")]
                public string MerchantId { get; set; }

                public decimal Amount { get; set; }

                [JsonProperty("CallbackURL")]
                public string CallbackUrl { get; set; }

                public string Description { get; set; }
            }

            public class Response
            {
                public int Status { get; set; }
                public string Authority { get; set; }
            }
        }

        public class PaymentVerification
        {
            public class Request
            {
                [JsonProperty("MerchantID")]
                public string MerchantId { get; set; }

                public string Authority { get; set; }

                public decimal Amount { get; set; }
            }

            public class Response
            {
                public int Status { get; set; }

                [JsonProperty("RefID")]
                public int RefId { get; set; }
            }
        }
    }
}
