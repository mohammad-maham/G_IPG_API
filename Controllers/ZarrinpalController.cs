using G_IPG_API.Common;
using G_IPG_API.Interfaces;
using G_IPG_API.Models;
using G_IPG_API.Models.Wallet;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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


        [HttpGet]
        [Route("Pay/Payment")]
        public IActionResult Payment([FromQuery] string guid)
        {
            try
            {
                var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault();

                if (lr == null)
                    return BadRequest(new ApiResponse(706));

                if (lr.ExpireDate < DateTime.Now || lr.Status != 1)
                    return BadRequest(new ApiResponse(705));

                var resp = _zarrinpal.Payment(lr);

                if (string.IsNullOrEmpty(resp))
                    return BadRequest(new ApiResponse(707));

                var lc = new LinkCall
                {
                    Id = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_pay, "seq_linkcall"),
                    RequestId = lr.RequestId,
                    InsertDate = DateTime.Now,
                    TokenInfo = JsonConvert.SerializeObject(resp)
                };

                _pay.LinkCalls.Add(lc);
                _pay.SaveChanges();

                JObject jo = JObject.Parse(resp.ToString());
                string dataAuth = jo["data"]!.ToString();

                if (dataAuth != "[]")
                {
                    string authority = jo["data"]["authority"].ToString();
                    string gatewayUrl = _configuration.GetSection("Configuration:Zarrinpal:GatewayUrl").Value! + authority;
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
            using (var context = new GWalletDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var authority = "";
                        if (HttpContext.Request.Query["Authority"] != "")
                            authority = HttpContext.Request.Query["Authority"];
                        else
                            return BadRequest(new ApiResponse(703));

                        var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault()!;
                        var trc = _wallet.TransactionConfirmations.FirstOrDefault(x => x.Id == lr.TransactionConfirmId);
                        var wc = _wallet.WalletCurrencies.FirstOrDefault(x => x.Id == lr.WallectCurrencyId);

                        if (lr == null || trc == null || wc == null)
                            return BadRequest(new ApiResponse(703));

                        #region Verfy Payment
                        var res = _zarrinpal.VerifyPayment(authority, lr);
                        JObject jo = JObject.Parse(res);
                        string errors = jo["errors"].ToString();
                        string data = jo["data"].ToString();
                        #endregion

                        #region Verify Confirmation
                        trc.ConfirmationDate = DateTime.Now;
                        trc.ResponceDescription = res;
                        #endregion

                        if (data != "[]")
                        {
                            trc.Status = 1;
                            wc.Amount += lr.Price;

                            string refid = jo["data"]["ref_id"].ToString();
                            ViewBag.RefId = refid;
                        }
                        else if (errors != "[]")
                        {
                            string errorscode = jo["errors"]["code"].ToString();
                            ViewBag.ErrorCode = errorscode;
                        }

                        _wallet.WalletCurrencies.Update(wc);
                        _wallet.TransactionConfirmations.Update(trc);
                        _wallet.SaveChanges();
                        transaction.Commit();

                        return View("ShowBill", lr);

                    }
                    catch (Exception ex)
                    {
                        transaction.Dispose();

                        var st = new StackTrace(ex, true);
                        var frame = st.GetFrame(0);
                        var line = frame.GetFileLineNumber();
                        throw new Exception("Error on line " + line + Environment.NewLine + ex.Message);
                    }
                }
            }
        }

    }
}
