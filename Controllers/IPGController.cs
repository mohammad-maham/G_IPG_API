
using BankPay.Services.Global;
using G_IPG_API.BusinessLogic.Interfaces;
using G_IPG_API.Common;
using G_IPG_API.Interfaces;
using G_IPG_API.Models;
using G_IPG_API.Models.Mellat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace G_IPG_API.Controllers;

[ApiController]
[Route("api/[controller]")]


public class IPGController : Controller
{
    private readonly ILogger<IPGController> _logger;
    private readonly ISaman _saman;
    private readonly IIranKish _iranKish;
    private readonly IMellat _mellat;
    private readonly IConfiguration _configuration;
    private readonly GIpgDbContext _pay;


    public IPGController(ILogger<IPGController> logger, ISaman saman, IConfiguration configuration,
        IIranKish iranKish, GIpgDbContext pay, IMellat mellat)
    {
        _logger = logger;
        _saman = saman;
        _iranKish = iranKish;

        _configuration = configuration;
        _pay = pay;
        _mellat = mellat;
    }

    [HttpPost]
    [Route("Pay/GetCodeForLink")]
    public string GetCodeForLink(PaymentLinkRequest request)
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
                Price = request.Price,
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

                //var blr = new PaymentLinkRequestResult
                //{
                //    PaymentId = lr.Guid
                //};

                //return Ok(blr);

                return lr.Guid;
            }
            oldLR.Status = 1;
            _pay.SaveChanges();

            //var blr = new PaymentLinkRequestResult
            //{
            //    PaymentId = oldLR.Guid
            //};
            //return Ok(blr);
            return oldLR.Guid;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    [HttpGet]
    [Route("Pay/GetToken")]
    public IActionResult GetToken(string guid)
    {
        try
        {

            var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault();

            if (lr != null)
            {
                if (lr.ExpireDate < DateTime.Now || lr.Status != 1)
                {
                    return BadRequest("اطلاعات پرداخت نامعتبر است");
                }

                if (_configuration.GetSection("Configuration:Mellat:Active").Value == "true")
                {
                    #region Mellat
                    var resp = _mellat.bpPayRequest(new MellatPayment
                    {
                        Amount = lr.Price,
                        OrderId = lr.RequestId,
                        AdditionalData=lr.FactorDetail
                    });

                    var lc = new LinkCall
                    {
                        Id = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_pay, "seq_linkcall"),
                        RequestId = lr.RequestId,
                        InsertDate = DateTime.Now,
                        TokenInfo = JsonConvert.SerializeObject(resp)
                    };

                    _pay.LinkCalls.Add(lc);
                    _pay.SaveChanges();

                    if (resp == "0")
                    {
                        return Ok(resp);
                    }
                    else
                    {
                        BankStatus bankStatus = _pay.BankStatuses.FirstOrDefault(x => x.Code == Convert.ToInt32(resp) && x.BankId == 10003);
                        if (bankStatus != null)
                            return BadRequest(bankStatus.Description);
                        else
                            return BadRequest("ارتباط با بانک برقرار نشد. دقایقی بعد مجدد تلاش کنید");
                    }
                    #endregion
                }

                else if (_configuration.GetSection("Configuration").GetSection("Saman").Value == "true")
                {
                    #region Saman
                                       if (lr != null)
                    {
                        var result = _saman.SendViaToken(new SepTxn
                        {

                            Amount = lr.Price,
                            CellNumber = string.IsNullOrEmpty(lr.ClientMobile) ? long.Parse(lr.ClientMobile) : 0,
                            RedirectUrl = _configuration.GetSection("Configuration").GetSection("RedirectUrl").Value!.ToString()
                        });

                        result.Token = "123";
                        result.Status = 1;
                        result.Type = _configuration.GetSection("Configuration").GetSection("Saman").Key.ToLower();

                        return Ok(result);
                    }
                    #endregion
                }
                else if (_configuration.GetSection("Configuration").GetSection("IranKish").Value == "true")
                {
                    #region IranKish
                    var resp = _iranKish.GetToken(new IPG_IrKish
                    {
                        Amount = lr.Price,
                        RequestId = lr.RequestId.ToString()
                    });
                    resp.Type = _configuration.GetSection("Configuration").GetSection("IranKish").Key.ToLower();

                    var lc = new LinkCall
                    {
                        Id = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_pay, "seq_linkcall"),
                        RequestId = lr.RequestId,
                        InsertDate = DateTime.Now,
                        TokenInfo = JsonConvert.SerializeObject(resp)
                    };

                    _pay.LinkCalls.Add(lc);
                    _pay.SaveChanges();

                    if (resp.responseCode == "00")
                    {
                        return Ok(resp);
                    }
                    else
                    {
                        BankStatus bankStatus = _pay.BankStatuses.FirstOrDefault(x => x.Code == Convert.ToInt32(resp.responseCode) && x.BankId == 10002);
                        if (bankStatus != null)
                            return BadRequest(bankStatus.Description);
                        else
                            return BadRequest("ارتباط با بانک برقرار نشد. دقایقی بعد مجدد تلاش کنید");
                    }
                    #endregion
                }
            }
            return BadRequest("اطلاعات پرداخت یافت نشد");
        }
        catch (Exception ex)
        {
            throw;
            //return BadRequest(Helper.getErrorMessage(ex));
        }
    }

    [HttpGet]
    [Route("Pay/GetLinkRequestDetail")]
    public IActionResult GetLinkRequestDetail(string guid)
    {
        try
        {
            var lr = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault();
            return Ok(lr);
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse(500));
        }
    }

    [HttpGet]
    public IActionResult ShowBill(string guid)
    {
        var detail = _pay.LinkRequests.Where(w => w.Guid == guid).FirstOrDefault();

        ViewBag.TokenInfo = GetToken(guid);
        ViewBag.confirmInfo = GetBankResult(guid);

        return View(detail);
    }

    [HttpGet]
    [Route("Pay/DeleteOrder")]
    public IActionResult DeleteOrder(string orderId)
    {
        try
        {
            var lr = _pay.LinkRequests
                            .Where(w => w.OrderId == orderId && w.Status == 1).FirstOrDefault();

            if (lr == null)
                return Ok("اطلاعات پرداخت نامعتبر است");

            lr.Status = 0;

            _pay.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse(500));
        }
    }

    [HttpGet]
    [Route("Pay/GetBankResult")]
    public IActionResult GetBankResult(string guid)
    {
        try
        {
            var lr = _pay.LinkRequests
            .Where(w => w.Guid == guid).FirstOrDefault();

            VerifyInquiryResult confirmInfo = new VerifyInquiryResult();
            confirmInfo.result = new SubResult();
            var b_Res = _pay.BankResults.Where(x => x.BankId == 10002 && x.RequestId == lr.RequestId && (x.ConfirmInfo != null && x.ConfirmInfo != ""));
            foreach (var item in b_Res)
            {
                if (item.ConfirmInfo != null && item.ConfirmInfo != "")
                {
                    confirmInfo = JsonConvert.DeserializeObject<VerifyInquiryResult>(item.ConfirmInfo);
                    if (confirmInfo.result?.responseCode == "00")
                        return Ok(confirmInfo);
                }

            }

            return Ok(confirmInfo);
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse(500));
        }
    }

    /// <summary>
    /// درخواست استعلام از بانک
    /// </summary>
    /// <param name="br"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Pay/InqueryRequest")]
    public IActionResult InqueryRequest(BackResult br)
    {
        try
        {
            var inq = new Inquery
            {
                requestId = br.RequestId,
                retrievalReferenceNumber = br.retrievalReferenceNumber,
                tokenIdentity = br.token
            };
            var ins = _iranKish.InquirySingle(inq);

            var bankInq = new BankInquiry
            {
                ReqId = int.Parse(br.RequestId),
                InsDate = DateTime.Now,
                VerifyInfo = JsonConvert.SerializeObject(ins)
            };

            _pay.Add(bankInq);
            _pay.SaveChanges();

            return Ok(ins);
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse(500));
        }
    }

    /// <summary>
    /// از طرف مشاغل برای تاییدیه دادن به بانک صدا زده میشود
    /// </summary>
    /// <param name="cpm"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Pay/ConfirmationPurchase")]
    public IActionResult ConfirmationPurchase(ConfirmPaymentModel cpm)
    {
        try
        {
            var ReqId = _pay.LinkRequests.FirstOrDefault(w => w.Guid == cpm.PaymentId).RequestId;

            var brBack = _pay.BankResults.Where(f => f.RequestId == ReqId).OrderByDescending(o => o.InsertDate).FirstOrDefault();

            var br = JsonConvert.DeserializeObject<BackResult>(brBack.BankInfo);

            var rv = new RequestVerify
            {
                systemTraceAuditNumber = br.systemTraceAuditNumber,
                retrievalReferenceNumber = br.retrievalReferenceNumber,
                tokenIdentity = br.token
            };

            var vir = _iranKish.ConfirmationPurchase(rv);

            brBack.ConfirmInfo = JsonConvert.SerializeObject(vir);
            _pay.SaveChanges();

            //تراکنش موفق

            ///
            /// ارایه سرویس
            var virm = new ConfirmationResultModel
            {
                description = vir.description,
                responseCode = vir.responseCode,
                status = vir.status
            };

            return Ok(virm);
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse(500));
        }
    }

    /// <summary>
    /// بانک اطلاعات پرداخت را به این اکشن ارسال میکند و
    /// از طریق این اکشن به سامانه مشاغل ارسال میشود
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("Pay/PaymentResult")]
    public IActionResult PaymentResult()
    {
        //var guid = GetCodeForLink(new PaymentLinkRequest
        //{
        //    Title = "test",
        //    Price = 1000,
        //    AccLinkReqConf = "",
        //    CallBackType = 1,
        //    ClientMobile = "09",
        //    CallBackURL = "www.google.com",
        //    ExpDate = DateTime.Now,
        //    OrderId = "12",


        //});

        //var token = GetToken(guid) is OkObjectResult tokenResult;




        ////if (token is OkObjectResult okResult && okResult.Value is PaymentLinkRequestResult result)
        ////{

        ////}


        //var br = new BackResult
        //{
        //    token = Request.Form["token"][0],
        //    acceptorId = Request.Form["acceptorId"][0],
        //    maskedPan = Request.Form["maskedPan"][0],
        //    paymentId = Request.Form["paymentId"][0],
        //    RequestId = Request.Form["RequestId"][0],
        //    responseCode = Request.Form["responseCode"][0],
        //    retrievalReferenceNumber = Request.Form["retrievalReferenceNumber"][0],
        //    sha256OfPan = Request.Form["sha256OfPan"][0],
        //    systemTraceAuditNumber = Request.Form["systemTraceAuditNumber"][0]
        //};


        var br = new BackResult
        {
            token = "1",
            acceptorId = "2",
            maskedPan = "3",
            paymentId = "4",
            RequestId = "1000000000",
            responseCode = "00",
            retrievalReferenceNumber = "7",
            sha256OfPan = "8",
            systemTraceAuditNumber = "9"
        };

        var vr = new VerifyInquiryResult();

        try
        {
            var lr = _pay.LinkRequests.FirstOrDefault(w => w.RequestId.ToString() == br.RequestId);
            ViewBag.GUID = lr.Guid;
            var bres = new BankResult();
            var bankStatus = _pay.BankStatuses.Where(w => w.BankId == 10002 && w.Code == int.Parse(br.responseCode)).FirstOrDefault();
            vr = new VerifyInquiryResult
            {
                description = bankStatus.Description,
                responseCode = bankStatus.Code.ToString(),
                status = br.responseCode == "00" ? true : false,
                result = new SubResult()
            };


            if (!string.IsNullOrEmpty(br.RequestId))
            {
                bres.Id = (int)DataBaseHelper.GetPostgreSQLSequenceNextVal(_pay, "seq_bankresults");
                bres.RequestId = int.Parse(br.RequestId);
                bres.BankInfo = JsonConvert.SerializeObject(br);
                bres.BankId = 10002;//irankish
                bres.InsertDate = DateTime.Now;

                _pay.BankResults.Add(bres);
                _pay.SaveChanges();



                var gateway = _pay.Banks.Find(10002);
                if (br.responseCode == "00" && lr.CallBackType == 1)//Get
                {
                    Uri url = new Uri(lr.CallbackUrl + string.Format("?OrderId={0}&PaymentId={1}&TransactionId={2}&TrasactionResultText={3}&TrasactionResult={4}&GatewayName={5}",
                        lr.OrderId, lr.Guid, br.RequestId, bankStatus.Description, br.responseCode, gateway.Name));
                    string jresponse = Helper.Get(url);

                    if (jresponse != null)
                    {
                        //VerifyInquiryResult jResult = JsonConvert.DeserializeObject<VerifyInquiryResult>(jresponse);
                        vr.result.systemTraceAuditNumber = lr.RequestId.ToString();

                        return View(vr);
                    }
                }

                return View(vr);
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            vr.description = Helper.getErrorMessage(ex);
            vr.status = false;

            return BadRequest(vr);
        }
    }



    //===============================================



}
