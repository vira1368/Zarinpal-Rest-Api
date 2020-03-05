using Microsoft.AspNetCore.Mvc;
using ZarinpalRestApi.Helpers;
using ZarinpalRestApi.Models;

namespace ZarinpalRestApi.Controllers
{
    public class HomeController : Controller
    {
        const string MerchantId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        public IActionResult Index()
        {
            //Simulation for get data from the database.
            var viewModel = TestDatabase.Data;

            return View(viewModel);
        }

        public IActionResult Payment(int id)
        {
            var product = TestDatabase.GetById(id);

            var request = new ZarinpalModel.Payment.Request
            {
                MerchantId = MerchantId,
                Amount = product.Amount,
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
                    MerchantId = MerchantId,
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

        public IActionResult ShowResult(MessageModel viewModel)
        {
            return View(viewModel);
        }
    }
}
