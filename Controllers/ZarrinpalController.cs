using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using zarinpalasp.netcorerest.Models;
using G_IPG_API.Models;
using ZarinPal.Class;
using G_IPG_API.Interfaces;
using G_IPG_API.Models.Wallet;
using G_IPG_API.Models.Wallet.VM;
using G_IPG_API.Common;
using G_IPG_API.Models.DataModels;
using G_IPG_API.BusinessLogic.Interfaces;

namespace G_IPG_API.Controllers
{
    public class ZarrinpalController : Controller
    {
        private readonly ILogger<ZarrinpalController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IZarrinpal _zarrinpal;
        private readonly GIpgDbContext _pay;
        //private readonly IFund _fund;
        private readonly GWalletDbContext _wallet;

        public ZarrinpalController(ILogger<ZarrinpalController> logger, GIpgDbContext pay, IConfiguration configuration, IZarrinpal zarrinpal, GWalletDbContext wallet)
        {
            _logger = logger;
            _pay = pay;
            _configuration = configuration;
            _zarrinpal = zarrinpal;
            _wallet = wallet;
        }


        [HttpPost]
        [Route("Pay/AddPaymentData")]
        public IActionResult AddPaymentData([FromBody] PaymentLinkRequest model)
        {
            try
            {
                if (model != null)
                {
                    var guid = _zarrinpal.AddPaymentData(model);
                    return Ok(new ApiResponse(data: guid));
                }

                return BadRequest(new ApiResponse(500));
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        [Route("Pay/ShowBill")]
        public IActionResult ShowBill(string guid)
        {
            var model = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault()!;
            return View(model);
        }


        [HttpGet]
        [Route("Pay/Payment")]
        public IActionResult Payment([FromQuery] string guid)
        {
            try
            {
                var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault()!;
                if (lr == null)
                    return BadRequest(new ApiResponse(701));

                var k = JsonConvert.DeserializeObject<FactorDataModel>(lr.FactorDetail);
                
                var wallet = _wallet.Wallets.FirstOrDefault(x => x.UserId == lr.UserId);
                if (wallet == null)
                    return BadRequest(new ApiResponse(702));

                var wc = _wallet.WalletCurrencies.FirstOrDefault(x=>x.WalletId==wallet.Id && x.CurrencyId==(short)Enums.Currency.Money);

                var transaction = new Transaction
                {
                    Id = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
                    Amount = (decimal)lr.Price,
                    OrderId = lr.RequestId.ToString(),
                    TransactionDate = DateTime.Now,
                    TransactionTypeId = (short)Enums.TransactionType.Deposit,
                    TransactionModeId = (short)Enums.TransactionMode.Online,
                    Status = 0,
                    WalletCurrencyId =wc.Id,
                    WalletId = wallet.Id
                };

                var tr = _wallet.Transactions.Add(transaction);

                var res = _zarrinpal.Payment(lr);

                JObject jo = JObject.Parse(res);
                string dataauth = jo["data"].ToString();

                if (dataauth != "[]")
                {
                    _wallet.SaveChanges();

                    var authority = jo["data"]["authority"].ToString();
                    string gatewayUrl = URLs.gateWayUrl + authority;

                    return Redirect(gatewayUrl);

                }
                else
                {
                    ViewBag.ErrorCode = jo["errors"]["message"];
                    return View("ShowBill", lr);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("Pay/VerifyPayment")]
        public IActionResult VerifyPayment(string guid)
        {
            try
            {
                var authority = "";
                if (HttpContext.Request.Query["Authority"] != "")
                    authority = HttpContext.Request.Query["Authority"];


                if (string.IsNullOrEmpty(authority))
                    return BadRequest(new ApiResponse(703));

                var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault()!;

                if (lr == null)
                    return BadRequest(new ApiResponse(703));

                var k = JsonConvert.DeserializeObject<FactorDataModel>(lr.FactorDetail);

                var wallet = _wallet.Wallets.FirstOrDefault(x => x.UserId == lr.UserId);
                if (wallet == null)
                    return BadRequest(new ApiResponse(702));

                var tr = _wallet.Transactions.FirstOrDefault(x => x.OrderId == lr.RequestId.ToString());
                tr.Status = 1;
                _wallet.Transactions.Update(tr);

                var wc = _wallet.WalletCurrencies.FirstOrDefault(x => x.WalletId == wallet.Id && x.CurrencyId == (short)Enums.Currency.Money);
                wc.Amount += (decimal)lr.Price;
                _wallet.WalletCurrencies.Update(wc);

                var res = _zarrinpal.VerifyPayment(authority, lr.Price.ToString());

                JObject jo = JObject.Parse(res);

                string errors = jo["errors"].ToString();
                string data = jo["data"].ToString();

                if (data != "[]")
                {
                    _wallet.SaveChanges();

                    string refid = jo["data"]["ref_id"].ToString();
                    ViewBag.RefId = refid;

                    return View("ShowBill", lr);
                }
                else if (errors != "[]")
                {

                    string errorscode = jo["errors"]["code"].ToString();
                    ViewBag.ErrorCode = errorscode;

                    return View("ShowBill", lr);
                }


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return NotFound();
        }

    }
}
