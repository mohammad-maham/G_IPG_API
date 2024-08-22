using BankPay.Services.Global;
using G_IPG_API.BusinessLogic.Interfaces;
using G_IPG_API.Common;
using G_IPG_API.Controllers;
using G_IPG_API.Interfaces;
using G_IPG_API.Models.Mellat;
using G_IPG_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace G_IPG_API.BusinessLogic;

public class IPG : IIPG
{
    private readonly IConfiguration _configuration;
    private readonly GIpgDbContext _pay;
    private readonly ILogger<IPGController> _logger  ;

    public IPG(ILogger<IPGController> logger, IConfiguration configuration, GIpgDbContext pay)
    {
        _logger = logger;
        _configuration = configuration;
        _pay = pay;
    }

    public string AddPaymentData(PaymentLinkRequest request)
    {
        var lr = new LinkRequest
        {
            RequestId = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_pay, "seq_linkrequest"),
            UserId = request.FactorData!.Header.CustomerId,
            ClientMobile = request.ClientMobile,
            CallbackUrl = request.CallBackURL,
            ExpireDate = request.ExpDate,
            OrderId = request.OrderId,
            Price = (long)request.Price!,
            Title = request.Title!,
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
        return oldLR.Guid!;
    }

}