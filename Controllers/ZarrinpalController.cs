using G_IPG_API.Common;
using G_IPG_API.Interfaces;
using G_IPG_API.Models;
using G_IPG_API.Models.Wallet;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            try
            {
                var authority = "";
                if (HttpContext.Request.Query["Authority"] != "")
                    authority = HttpContext.Request.Query["Authority"];

                if (string.IsNullOrEmpty(authority))
                    return BadRequest(new ApiResponse(703));

                var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault()!;

                if (lr == null)
                    return BadRequest(new ApiResponse(706));

                var res = _zarrinpal.VerifyPayment(authority, lr);

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
