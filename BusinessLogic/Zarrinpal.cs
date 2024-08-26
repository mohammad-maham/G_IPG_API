using G_IPG_API.Common;
using G_IPG_API.Interfaces;
using G_IPG_API.Models;
using G_IPG_API.Models.Wallet;
using RestSharp;
using zarinpalasp.netcorerest.Models;

namespace G_IPG_API.BusinessLogic
{
    public class Zarrinpal : IZarrinpal
    {
        private readonly IConfiguration _configuration;
        private readonly GIpgDbContext _pay;
        private readonly GWalletDbContext _wallet;

        public Zarrinpal(IConfiguration iConfig, GIpgDbContext pay, GWalletDbContext wallet)
        {
            _configuration = iConfig;
            _pay = pay;
            _wallet = wallet;
        }
      

        public string Payment(LinkRequest model)
        {

            string amount = model.Price.ToString()!;
            string description = model.Title;
            string mobile = model.ClientMobile!;
            string requestUrl = _configuration.GetSection("Configuration:Zarrinpal:RequestUrl").Value!;
            string callbackUrl = _configuration.GetSection("Configuration:Zarrinpal:CallbackUrl").Value!;
            string merchantId = _configuration.GetSection("Configuration:Zarrinpal:Merchant").Value!;

            var @params = new ZarrinRequestParameters(merchantId, amount, description, callbackUrl, mobile, "");
            var res = new GoldApi(callbackUrl,@params).Post();

            return res;
        }

        public string VerifyPayment(string authority,  LinkRequest model)
        {
            var tr = _wallet.Transactions.FirstOrDefault(x => x.OrderId == model.OrderId);
            tr.Status = 1;
            _wallet.Transactions.Update(tr);

            var wc = _wallet.WalletCurrencies.FirstOrDefault(x => x.Id == model.WallectCurrencyId);
            wc.Amount += model.Price;
            _wallet.WalletCurrencies.Update(wc);

            string merchantId = _configuration.GetSection("Configuration:Zarrinpal:Merchant").Value!;
            string verifyUrl = _configuration.GetSection("Configuration:Zarrinpal:VerifyUrl").Value!;
            RestClient client = new RestClient(verifyUrl);
            RestRequest request = new RestRequest("", Method.Post);


            var @params = new VerifyParameters{
                authority = authority,
                amount = model.Price.ToString()!,
                merchant_id = merchantId
            };
            var res = new GoldApi(verifyUrl, @params).Post();

            return res;

        }
    }
}