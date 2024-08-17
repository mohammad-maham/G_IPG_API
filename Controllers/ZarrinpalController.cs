using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using zarinpalasp.netcorerest.Models;
using G_IPG_API.Models;
using ZarinPal.Class;
using G_IPG_API.Interfaces;

namespace G_IPG_API.Controllers
{
    public class ZarrinpalController : Controller
    {
        private readonly ILogger<ZarrinpalController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IZarrinpal _zarrinpal;
        private readonly GIpgDbContext _pay;

        public ZarrinpalController(ILogger<ZarrinpalController> logger, GIpgDbContext pay, IConfiguration configuration, IZarrinpal zarrinpal)
        {
            _logger = logger;
            _pay = pay;
            _configuration = configuration;
            _zarrinpal = zarrinpal;
        }


        [HttpPost]
        [Route("Pay/ShowBill")]
        public IActionResult ShowBill( PaymentLinkRequest model)
        {
            var guid = _zarrinpal.AddPaymentData(model);
            model.Guid=guid;





            return View(model);
        }

        [HttpGet]
        public IActionResult Payment(string guid)
        {
            try
            {
                var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault()!;
                if (lr == null)
                {
                    return BadRequest(404);
                }

                var res = _zarrinpal.Payment(lr);

                JObject jo = JObject.Parse(res);

                string errorscode = jo["errors"].ToString();

                JObject jodata = JObject.Parse(res);

                string dataauth = jodata["data"].ToString();


                if (dataauth != "[]")
                {

                   var authority = jodata["data"]["authority"].ToString();

                    string gatewayUrl = URLs.gateWayUrl + authority;

                    return Redirect(gatewayUrl);

                }
                else
                {
                    return BadRequest("error " + errorscode);
                }
            }

            catch (Exception ex)
            {
                  throw new Exception(ex.Message);
            }
            return null;
        }

        public IActionResult VerifyPayment()
        {
            try
            {
                var authority = "";
                if (HttpContext.Request.Query["Authority"] != "")
                    authority = HttpContext.Request.Query["Authority"];

                var res = _zarrinpal.VerifyPayment(authority,"1000");

                JObject jodata = JObject.Parse(res);

                string data = jodata["data"].ToString();

                JObject jo = JObject.Parse(res);

                string errors = jo["errors"].ToString();

                if (data != "[]")
                {
                    string refid = jodata["data"]["ref_id"].ToString();

                    ViewBag.code = refid;

                    return View();
                }
                else if (errors != "[]")
                {

                    string errorscode = jo["errors"]["code"].ToString();

                    return BadRequest($"error code {errorscode}");

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
