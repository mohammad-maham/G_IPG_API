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
                    AccLinkReqConf = request.AccLinkReqConf,
                    CallBackType = (short)request.CallBackType,
                    ClientMobile = request.ClientMobile,
                    CallbackUrl = request.CallBackURL,
                    ExpireDate = request.ExpDate,
                    OrderId = request.OrderId,
                    Price =(long) request.Price,
                    Title = request.Title,
                    Status = 1,
                    Guid = Helper.IdGenerator(),
                    InsertDate = DateTime.Now,
                    //FactorDetail = JsonConvert.SerializeObject(request.FactorData)
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
            var merchant_id = _configuration.GetSection("Configuration:Zarrinpal:merchant").Value!;

            string amount = model.Price.ToString();
            string description = model.Title;
            string mobile = model.ClientMobile;
            string callbackurl = model.CallbackUrl;

            var Parameters = new ZarrinRequestParameters(merchant_id, amount, description, callbackurl, mobile, "");

            var client = new RestClient(URLs.requestUrl);

            Method method = Method.Post;

            var request = new RestRequest("", method);

            request.AddHeader("accept", "application/json");

            request.AddHeader("content-type", "application/json");

            request.AddJsonBody(Parameters);

            var response = client.ExecuteAsync(request);

            return response.Result.Content;
        }

        public string VerifyPayment(string authority, string amount)
        {
            VerifyParameters parameters = new VerifyParameters();

            parameters.authority = authority;
            parameters.amount = amount;
            parameters.merchant_id = _configuration.GetSection("Configuration:Zarrinpal:merchant").Value!;

            var client = new RestClient(URLs.verifyUrl);
            Method method = Method.Post;
            var request = new RestRequest("", method);

            request.AddHeader("accept", "application/json");

            request.AddHeader("content-type", "application/json");
            request.AddJsonBody(parameters);

            var response = client.ExecuteAsync(request);

            return response.Result.Content;
        }
    }
}