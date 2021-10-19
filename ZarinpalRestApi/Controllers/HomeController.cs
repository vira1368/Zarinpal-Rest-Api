using Microsoft.AspNetCore.Mvc;
using ZarinpalRestApi.Helpers;
using ZarinpalRestApi.Models;

namespace ZarinpalRestApi.Controllers
{
    public class HomeController : Controller
    {
        // Any random number work with Sandbox URL in v1
        const string TestMerchantIdV1 = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
        
        // v4 does not provide SandBox API, so we should use designate Test Merchant-ID for testing propose
        // This Test Merchant ID only works for first step, for bank payment, you need to get a real Merchant ID :|
        const string TestMerchantIdV4 = "4ced0a1e-4ad8-4309-9668-3ea3ae8e8897";
        

        public IActionResult Index()
        {
            //Simulation for get data from the database.
            var viewModel = TestDatabase.Data;

            return View(viewModel);
        }

        #region V1
        
        public IActionResult Payment(int id)
        {
            var product = TestDatabase.GetById(id);

            var request = new ZarinpalModel.Payment.Request
            {
                MerchantId = TestMerchantIdV1,
                Amount = product.Amount, // V1 in Toman
                CallbackUrl = $"{Request.Scheme}://{Request.Host}/Home/Callback/{product.Id}",
                Description = $"Pay for {product.Name} product."
            };

            var response = RestApi.PaymentRequest(request);

            if (response.Status == 100)
            {
                //For debug and test.
                return Redirect($"https://sandbox.zarinpal.com/pg/StartPay/{response.Authority}");

                //For publish.
                //return Redirect($"https://www.zarinpal.com/pg/StartPay/{response.Authority}");
            }

            TempData["Message"] = response.Status;
            return View("Index");
        }

        public IActionResult Callback(int id, string authority, string status)
        {
            var viewModel = new MessageModel();

            if (status == "NOK")
            {
                viewModel.IsError = true;
                viewModel.Text = "Transaction unsuccessful.";
            }
            else if (status == "OK")
            {
                var product = TestDatabase.GetById(id);
                var request = new ZarinpalModel.PaymentVerification.Request
                {
                    MerchantId = TestMerchantIdV1,
                    Authority = authority,
                    Amount = product.Amount
                };

                var response = RestApi.PaymentVerification(request);

                if (response.Status == 100)
                {
                    viewModel.IsError = false;
                    viewModel.Text = $"Transaction successful. RefId: {response.RefId}";
                }
                else
                {
                    viewModel.IsError = true;
                    viewModel.Text = $"Transaction unsuccessful. Status: {response.Status}";
                }
            }

            return RedirectToAction("ShowResult", viewModel);
        }

        #endregion V1
        
        #region V4
        
        public IActionResult PaymentV4(int id)
        {
            var product = TestDatabase.GetById(id);

            var request = new ZarinpalModelV4.Payment.Request
            {
                MerchantId = TestMerchantIdV4,
                Amount = product.Amount * 10, // V4 in Rials
                CallbackUrl = $"{Request.Scheme}://{Request.Host}/Home/CallbackV4/{product.Id}",
                Description = $"Pay for {product.Name} product.",
                Metadata = new ZarinpalModelV4.Payment.RequestMetadata // Either none, or all
                {
                    Email = "hassan.faghihi@outlook.com",
                    Mobile = null
                }
            };

            var response = RestApiV4.PaymentRequest(request);

            if (response.Data.Code == 100)
            {
                var gatewayLink = RestApiV4.GenerateGatewayLink(response.Data.Authority);
                return Redirect(gatewayLink);
            }

            TempData["Message"] = response.Data.Code;
            return View("Index");
        }

        /// <summary>
        /// The URL of which the Zarinpal will call after a successful or failure payment operation
        /// </summary>
        /// <param name="id">ProductID: It is the ID you previously send to the Zarinpal, Like Factor ID, Order ID, an ID to track what user is paying for.<br/>
        /// Here we used Product ID, which in real scenario mostly will be wrong, unless you are showing something to user that is disposable.</param>
        /// <param name="authority">
        /// A unique 32 characters length identifier of type `UUID` (Universal Unique Identifier) that Zarinpal
        /// Sent to client for each payment request. The Identifier always start with 'A' character.
        /// Sample: A 36 character lenght string, starting with A, like: A00000000000000000000000000217885159
        /// </param>
        /// <param name="status">
        /// Either `OK` or `NOK`, of which the `OK` represent the successful payment and `NOK` represent a failure. <br />
        /// Whenever the status is `OK`, and only when it is `OK`, we should also verify the incoming request with Zarinpal, Otherwise it may be an attacker issuing false request
        /// </param>
        /// <returns></returns>
        public IActionResult CallbackV4(int id, string authority, string status)
        {
            var viewModel = new MessageModel();
        
            if (status == "NOK")
            {
                viewModel.IsError = true;
                viewModel.Text = "Transaction unsuccessful.";
            }
            else if (status == "OK")
            {
                var product = TestDatabase.GetById(id);
                var request = new ZarinpalModelV4.Verify.Request
                {
                    MerchantId = TestMerchantIdV4,
                    Authority = authority,
                    Amount = product.Amount * 10
                };
        
                var response = RestApiV4.Verify(request);
        
                if (response.Data.Code == 100) // Successful
                {
                    viewModel.IsError = false;
                    viewModel.Text = $"Transaction successful. RefId: {response.Data.RefId}";
                }
                else if (response.Data.Code == 101) // Repeated successful
                {
                    viewModel.IsError = false;
                    viewModel.Text = $"Transaction repeated with success response. RefId: {response.Data.RefId}";
                }
                else // Error
                {
                    viewModel.IsError = true;
                    viewModel.Text = $"Transaction unsuccessful. Status: {response.Data.Code}";
                }
            }
        
            return RedirectToAction("ShowResult", viewModel);
        }
        
        #endregion V4
        
        public IActionResult ShowResult(MessageModel viewModel)
        {
            return View(viewModel);
        }
    }
}
