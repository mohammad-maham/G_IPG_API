using G_APIs.Services;
using G_IPG_API.Models;
using G_IPG_API.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;
using zarinpalasp.netcorerest.Models;

namespace G_IPG_API.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/AddPaymentData")]
        public string AddPaymentData(PaymentLinkRequest model)
        {

            var factorHeader = new FactorHeader
            {
                CustomerId=100000080,
                CustomerMobile="09397371193",
                CutomerName="سعید",
                FactorTitle="تست بانک",
                CreateDate = DateTime.Now,

                
            };
            var factorItems = new List<FactorItem>
            {
                new FactorItem
                {
                    ItemTitle="واریز پول",
                    ItemUnitPrice=10000,
                    ItemUnitType="ریال",
                    ItemCount=4,
                    ItemDiscount=0,


                }
            };

            var factorFooter = new FactorFooter
            {
                FactorSumPrice = factorItems.Sum(x => x.ItemUnitPrice),
                SellerAddress = new List<string> { "آدرس فروشگاه" },
                SellerName = "نام فروشنده",
                SellerTelephone = new List<string> { "تلفن فروشنده  " },
                FactorVAT = 1

            };

            model.FactorData = new FactorDataModel
            {
                Header = factorHeader,
                Items = factorItems,
                Footer = factorFooter
                
            };


            string amount = model.Price.ToString();
            string description = model.Title;
            string mobile = model.ClientMobile;
            string callbackurl = model.CallBackURL;


            var res = new GoldApi("http://localhost:5171/Pay/AddPaymentData", model).Post();
            return res;
        }
    }
}
