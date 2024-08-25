using BankPay.Services.Global;
using G_IPG_API.Common;
using G_IPG_API.Interfaces;
using G_IPG_API.Models;
using Newtonsoft.Json;
using RestSharp;
using zarinpalasp.netcorerest.Models;

namespace G_IPG_API.BusinessLogic
{
    public class Zarrinpal : IZarrinpal
    {
        private readonly IConfiguration _configuration;
        private readonly GIpgDbContext _pay;

        public Zarrinpal(IConfiguration iConfig, GIpgDbContext pay)
        {
            _configuration = iConfig;
            _pay = pay;
        }
        public string AddPaymentData(PaymentLinkRequest request)
        {
            try
            {
                var lr = new LinkRequest
                {
                    RequestId = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_pay, "seq_linkrequest"),
                    //AccLinkReqConf = request.AccLinkReqConf,
                    //CallBackType = (short)request.CallBackType,
                    UserId=request.FactorData.Header.CustomerId,
                    ClientMobile = request.ClientMobile,
                    CallbackUrl = request.CallBackURL,
                    ExpireDate = request.ExpDate,
                    OrderId = request.OrderId,
                    Price =(long) request.Price,
                    Title = request.Title,
                    Status = 1,
                    Guid = Helper.IdGenerator(),
                    InsertDate = DateTime.Now,
                    FactorDetail = JsonConvert.SerializeObject(request.FactorData)
                };
                LinkRequest oldLR = _pay.LinkRequests.FirstOrDefault(x => x.Price == request.Price
                && x.OrderId == request.OrderId
                && x.CallbackUrl == request.CallBackURL
                && x.ClientMobile == request.ClientMobile);

                if (oldLR == null)
                {
                    _pay.LinkRequests.Add(lr);
                    _pay.SaveChanges();

                    return lr.Guid;
                }
                oldLR.Status = 1;
                _pay.SaveChanges();
                return oldLR.Guid;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string Payment(LinkRequest model)
        {

            string amount = model.Price.ToString();
            string description = model.Title;
            string mobile = model.ClientMobile;
            string callbackUrl = _configuration.GetSection("Configuration:Zarrinpal:CallbackUrl").Value!;
            var merchantId = _configuration.GetSection("Configuration:Zarrinpal:Merchant").Value!;

            var Parameters = new ZarrinRequestParameters(merchantId, amount, description, callbackUrl, mobile, "");
            var requestUrl = _configuration.GetSection("Configuration:Zarrinpal:RequestUrl").Value!;
            var client = new RestClient(requestUrl);
            var request = new RestRequest("", Method.Post);

            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");
            request.AddJsonBody(Parameters);

            var response = client.ExecuteAsync(request);
            return response.Result.Content;
        }

        public string VerifyPayment(string authority, string amount)
        {
            var merchantId = _configuration.GetSection("Configuration:Zarrinpal:Merchant").Value!;
            var verifyUrl = _configuration.GetSection("Configuration:Zarrinpal:VerifyUrl").Value!;
            var client = new RestClient(verifyUrl);
            var request = new RestRequest("", Method.Post);

            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");
            request.AddJsonBody(new VerifyParameters
            {
                authority = authority,
                amount = amount,
                merchant_id = merchantId
            });

            var response = client.ExecuteAsync(request);
            return response.Result.Content;
        }
    }
}