using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZarinpalRestApi.Models;

namespace ZarinpalRestApi.Helpers
{
    /// <summary>
    /// Implementing v4 API of ZarinPal Payments Gateway
    /// Useful Links: <br/>
    /// <a href="https://docs.zarinpal.com/paymentGateway/">Terms</a> <br />
    /// <a href="https://docs.zarinpal.com/paymentGateway/guide/">API Reference</a> <br />
    /// <a href="https://docs.zarinpal.com/paymentGateway/sandbox.html">Sandbox and Real URLs</a> <br />
    /// </summary>
    public class RestApiV4
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        // NOTE: the documentations says we should use `https://sandbox.zarinpal.com` URL for testing,
        // but only following URL will work, and it is also provided in sample provided by Zarinpal
        private static readonly string BaseApiUrl = "https://api.zarinpal.com"; //For debug and test.
        private static readonly string BaseWebUrl = "https://www.zarinpal.com"; //For debug and test.

        //private static readonly string BaseApiUrl = "https://api.zarinpal.com";//For publish.
        //private static readonly string BaseWebUrl = "https://www.zarinpal.com";//For publish.

        public static ZarinpalModelV4.Payment.Response PaymentRequest(ZarinpalModelV4.Payment.Request request)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializeObject = JsonConvert.SerializeObject(request, GetSerializerSetting());

            var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var httpResponseMessage =
                HttpClient.PostAsync($"{BaseApiUrl}/pg/v4/payment/request.json", stringContent).Result;

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadGateway)
                throw new ZarinpalException(httpResponseMessage.StatusCode, "Cannot contact Zarinpal Server");
            if ((int) httpResponseMessage.StatusCode >= 400 && (int) httpResponseMessage.StatusCode < 500)
                throw new ZarinpalException(httpResponseMessage.StatusCode,
                    "Cannot process the request due to bad request error.");
            if ((int)httpResponseMessage.StatusCode >= 500)
                throw new ZarinpalException(httpResponseMessage.StatusCode, "Zarinpal responded with an unknown error");
            

            var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<ZarinpalModelV4.Payment.Response>(result,GetSerializerSetting());
        }

        public static string GenerateGatewayLink(string authority)
        {
            return $"{BaseWebUrl}/pg/StartPay/{authority}";
        }

        public static ZarinpalModelV4.Verify.Response Verify(ZarinpalModelV4.Verify.Request request)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializeObject = JsonConvert.SerializeObject(request, GetSerializerSetting());

            var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var httpResponseMessage =
                HttpClient.PostAsync($"{BaseApiUrl}/pg/v4/payment/verify.json", stringContent).Result;

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadGateway)
                throw new ZarinpalException(httpResponseMessage.StatusCode, "Cannot contact Zarinpal Server");
            if ((int) httpResponseMessage.StatusCode >= 400 && (int) httpResponseMessage.StatusCode < 500)
                throw new ZarinpalException(httpResponseMessage.StatusCode,
                    "Cannot process the request due to bad request error.");
            if ((int)httpResponseMessage.StatusCode >= 500)
                throw new ZarinpalException(httpResponseMessage.StatusCode, "Zarinpal responded with an unknown error");
            
            var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<ZarinpalModelV4.Verify.Response>(result, GetSerializerSetting());
        }

        private static JsonSerializerSettings GetSerializerSetting()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            };
            return jsonSerializerSettings;
        }
    }
}
