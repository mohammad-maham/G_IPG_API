using BankPay.Services.Global;
using G_IPG_API.Common;
using G_IPG_API.Interfaces;
using G_IPG_API.Models;
using G_IPG_API.Models.Wallet;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace G_IPG_API.Controllers
{
    public class IPGController : Controller
    {

        private readonly ILogger<IPGController> _logger;
        private readonly IConfiguration _configuration;
        private readonly GIpgDbContext _pay;
        private readonly GWalletDbContext _wallet;

        public IPGController(ILogger<IPGController> logger, GIpgDbContext pay, IConfiguration configuration,   GWalletDbContext wallet)
        {
            _logger = logger;
            _pay = pay;
            _configuration = configuration;
            _wallet = wallet;
        }

        [HttpPost]
        [Route("IPG/AddPaymentData")]
        public IActionResult AddPaymentData([FromBody] PaymentLinkRequest model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new ApiResponse(500));

                var wc = _wallet.WalletCurrencies.FirstOrDefault(x => x.Id == model.WallectCurrencyId);
                if (wc == null)
                    return BadRequest(new ApiResponse(702));

                Transaction transaction = new Transaction
                {
                    Id = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_wallet, "seq_transactionid"),
                    Amount = model.Price,
                    OrderId = model.OrderId,
                    TransactionDate = DateTime.Now,
                    TransactionTypeId = (short)Enums.TransactionType.Deposit,
                    TransactionModeId = (short)Enums.TransactionMode.Online,
                    Status = 0,
                    WalletCurrencyId = wc.Id,
                    WalletId = model.WalletId
                };

                string callbackUrl = _configuration.GetSection("Configuration:Zarrinpal:CallbackUrl").Value!;
                LinkRequest lr = new LinkRequest
                {
                    RequestId = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_pay, "seq_linkrequest"),
                    UserId = model.UserId,
                    ClientMobile = model.ClientMobile,
                    CallbackUrl = callbackUrl,
                    ExpireDate = model.ExpDate,
                    OrderId = model.OrderId,
                    Price = model.Price,
                    Title = model.Title,
                    Status = 1,
                    Guid = Helper.IdGenerator(),
                    InsertDate = DateTime.Now,
                    FactorDetail = JsonConvert.SerializeObject(model.FactorData)
                };

                LinkRequest oldLR = _pay.LinkRequests.FirstOrDefault(x => x.Price == model.Price
                && x.OrderId == model.OrderId
                && x.CallbackUrl == model.CallBackURL
                && x.ClientMobile == model.ClientMobile)!;

                if (oldLR == null)
                {
                    _wallet.Transactions.Add(transaction);
                    _wallet.SaveChanges();

                    _pay.LinkRequests.Add(lr);
                    _pay.SaveChanges();

                    return Ok(new ApiResponse { StatusCode = 200, Data = JsonConvert.SerializeObject(lr.Guid) });
                }

                oldLR.Status = 1;
                _pay.SaveChanges();
                return Ok(new ApiResponse { StatusCode = 200, Data = JsonConvert.SerializeObject(oldLR.Guid) });

            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        [Route("IPG/ShowBill")]
        public IActionResult ShowBill(string guid)
        {
            var model = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault()!;
            return View(model);
        }

    }
}
