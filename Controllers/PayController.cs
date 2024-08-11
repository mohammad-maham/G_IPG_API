using BankPay.Services.Interfaces;
using G_IPG_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace G_IPG_API.Controllers;

[ApiController]
public class PayController : Controller
{
    private readonly ILogger<PayController> _logger;
    private readonly IUnitOfWork _uow;
    private readonly ISaman _saman;
    private readonly IIranKish _iranKish;
    private readonly IConfiguration _configuration;
    private readonly DbSet<Bank> _bank;
    private readonly DbSet<BankInquiry> _bankInquiry;
    private readonly DbSet<BankStatus> _bankStatus;
    private readonly DbSet<BankResult> _bankResult;
    private readonly DbSet<LinkCall> _linkCall;
    private readonly DbSet<LinkRequest> _linkRequest;
    private readonly DbSet<Status> _status;

    public PayController(ILogger<PayController> logger, IUnitOfWork uow, ISaman saman, IConfiguration configuration,
        IIranKish iranKish)
    {
        _logger = logger;
        _uow = uow;
        _saman = saman;
        _iranKish = iranKish;

        _configuration = configuration;

        _linkRequest = _uow.Set<LinkRequest>();
        _linkCall = _uow.Set<LinkCall>();
        _bankResult = _uow.Set<BankResult>();
        _bankInquiry = _uow.Set<BankInquiry>();
        _bankStatus = _uow.Set<BankStatus>();
        _bank = _uow.Set<Bank>();
    }

    /// <summary>
    /// کاربر با استفاده از لینک و از طریق انگولار اکشن را صدا میکند
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("Pay/GetToken")]
    public IActionResult GetToken(string guid)
    {
        try
        {
            //var x = 0;
            //var y = 1 / x;


            var lr = _linkRequest
                            .Where(w => w.Guid == guid).FirstOrDefault();
            // for testing

            //TokenResult t = new TokenResult
            //{
            //    result = new Result
            //    {
            //        billInfo = new Billinfo
            //        {
            //            billId = null,
            //            billPaymentId = null
            //        },
            //        token = "38B7B8E50DA32B43B2811DF7155F187B3935",
            //        expiryTimeStamp = 1645362958,
            //        initiateTimeStamp = 1645363558,
            //        transactionType = "Purchase"
            //    },
            //    description = null,
            //    responseCode = "00",
            //    status = true,
            //    Type = "irankish",
            //};
            //return Ok(t);

            if (lr != null)
            {
                if (lr.ExpDate < DateTime.Now || lr.Status != 1)
                {
                    return BadRequest("اطلاعات پرداخت نامعتبر است");
                }
                if (_configuration.GetSection("Configuration").GetSection("Saman").Value == "true")
                {
                    #region Saman
                    //fortest
                    lr = new LinkRequest
                    {
                        Price = 10000,
                        ClientMobile = "09357375624"
                    };

                    if (lr != null)
                    {
                        var result = _saman.SendViaToken(new SepTxn
                        {

                            Amount = lr.Price,
                            CellNumber = string.IsNullOrEmpty(lr.ClientMobile) ? long.Parse(lr.ClientMobile) : 0,
                            RedirectUrl = "118.tci.ir"
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
                        RequestId = lr.ReqId.ToString()
                    });
                    resp.Type = _configuration.GetSection("Configuration").GetSection("IranKish").Key.ToLower();

                    var lc = new LinkCall
                    {
                        ReqId = lr.ReqId,
                        InsDate = DateTime.Now,
                        TokenInfo = JsonConvert.SerializeObject(resp)
                    };

                    _linkCall.Add(lc);
                    _uow.SaveChanges();

                    if (resp.responseCode == "00")
                    {
                        return Ok(resp);
                    }
                    else
                    {
                        BankStatus bankStatus = _bankStatus.FirstOrDefault(x => x.Code == Convert.ToInt32(resp.responseCode) && x.bankId == 10002);
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
    [Route("Pay/DeleteOrder")]
    public IActionResult DeleteOrder(string orderId)
    {
        try
        {
            var lr = _linkRequest
                            .Where(w => w.OrderId == orderId && w.Status == 1).FirstOrDefault();

            if (lr == null)
                return Ok("اطلاعات پرداخت نامعتبر است");

            lr.Status = 0;

            _uow.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(Helper.getErrorMessage(e));
        }
    }

    [HttpGet]
    [Route("Pay/GetLinkRequestDetail")]
    public IActionResult GetLinkRequestDetail(string guid)
    {
        try
        {
            var lr = _linkRequest
                    .Where(w => w.Guid == guid).FirstOrDefault();
            return Ok(lr);
        }
        catch (Exception e)
        {
            return BadRequest(Helper.getErrorMessage(e));
        }
    }

    [HttpGet]
    [Route("Pay/GetBankResult")]
    public IActionResult GetBankResult(string guid)
    {
        try
        {
            var lr = _linkRequest
            .Where(w => w.Guid == guid).FirstOrDefault();

            VerifyInquiryResult confirmInfo = new VerifyInquiryResult();
            confirmInfo.result = new SubResult();
            var b_Res = _bankResult.Where(x => x.BankId == 10002 && x.ReqId == lr.ReqId && (x.ConfrimInfo != null && x.ConfrimInfo != ""));
            foreach (var item in b_Res)
            {
                if (item.ConfrimInfo != null && item.ConfrimInfo != "")
                {
                    confirmInfo = JsonConvert.DeserializeObject<VerifyInquiryResult>(item.ConfrimInfo);
                    if (confirmInfo.result?.responseCode == "00")
                        return Ok(confirmInfo);
                }

            }

            return Ok(confirmInfo);
        }
        catch (Exception e)
        {
            return BadRequest(Helper.getErrorMessage(e));
        }
    }

    /// <summary>
    /// سامانه مدیریت مشاغل با ارسال پارامترهای پرداخت، کد را برای الصاق به لینک را دریافت میکند
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Pay/GetCodeForLink")]
    public IActionResult GetCodeForLink(PaymentLinkRequest request)
    {
        try
        {
            var lr = new LinkRequest
            {
                AccLinkReqConf = request.AccLinkReqConf,
                CallBackType = request.CallBackType,
                ClientMobile = request.ClientMobile,
                CallBackURL = request.CallBackURL,
                ExpDate = request.ExpDate,
                OrderId = request.OrderId,
                Price = request.Price,
                Title = request.Title,
                Status = 1,
                Guid = Helper.IdGenerator(),
                InsDate = DateTime.Now,
                FactorDetailJson = JsonConvert.SerializeObject(request.FactorData)
            };
            LinkRequest oldLR = _linkRequest.FirstOrDefault(x => x.Price == request.Price
            && x.OrderId == request.OrderId
            && x.CallBackURL == request.CallBackURL
            && x.ClientMobile == request.ClientMobile);

            if (oldLR == null)
            {
                _linkRequest.Add(lr);
                _uow.SaveChanges();

                var blr = new PaymentLinkRequestResult
                {
                    PaymentId = lr.Guid
                };

                return Ok(blr);
            }
            else
            {
                oldLR.Status = 1;
                _uow.SaveChanges();

                var blr = new PaymentLinkRequestResult
                {
                    PaymentId = oldLR.Guid
                };
                return Ok(blr);
            }
        }
        catch (Exception e)
        {
            return BadRequest(Helper.getErrorMessage(e));
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

            _bankInquiry.Add(bankInq);
            _uow.SaveChanges();

            return Ok(ins);
        }
        catch (Exception e)
        {
            return BadRequest(Helper.getErrorMessage(e));
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
            var ReqId = _linkRequest.FirstOrDefault(w => w.Guid == cpm.PaymentId).ReqId;

            var brBack = _bankResult.Where(f => f.ReqId == ReqId).OrderByDescending(o => o.InsDate).FirstOrDefault();

            var br = JsonConvert.DeserializeObject<BackResult>(brBack.BackInfo);

            var rv = new RequestVerify
            {
                systemTraceAuditNumber = br.systemTraceAuditNumber,
                retrievalReferenceNumber = br.retrievalReferenceNumber,
                tokenIdentity = br.token
            };

            var vir = _iranKish.ConfirmationPurchase(rv);

            brBack.ConfrimInfo = JsonConvert.SerializeObject(vir);
            _uow.SaveChanges();

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
            return BadRequest(Helper.getErrorMessage(e));
        }
    }

    /// <summary>
    /// بانک اطلاعات پرداخت را به این اکشن ارسال میکند و
    /// از طریق این اکشن به سامانه مشاغل ارسال میشود
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("Pay/PaymentResult")]
    public IActionResult PaymentResult()
    {
        var br = new BackResult
        {
            token = Request.Form["token"][0],
            acceptorId = Request.Form["acceptorId"][0],
            maskedPan = Request.Form["maskedPan"][0],
            paymentId = Request.Form["paymentId"][0],
            RequestId = Request.Form["RequestId"][0],
            responseCode = Request.Form["responseCode"][0],
            retrievalReferenceNumber = Request.Form["retrievalReferenceNumber"][0],
            sha256OfPan = Request.Form["sha256OfPan"][0],
            systemTraceAuditNumber = Request.Form["systemTraceAuditNumber"][0]
        };

        var vr = new VerifyInquiryResult();

        try
        {
            var lr = _linkRequest.FirstOrDefault(w => w.ReqId.ToString() == br.RequestId);
            ViewBag.GUID = lr.Guid;

            // var lr = _linkRequest.Where(w => w.ReqId == Int32.Parse(Request.Form["RequestId"][0]) && w.Status == 1).FirstOrDefault();
            //var lr = _linkRequest.Where(w => w.ReqId == 229 && w.Status == 1).FirstOrDefault();

            //var br = new BackResult
            //{
            //    token = "123456789",
            //    acceptorId = "123456789",
            //    maskedPan = "123456789",
            //    paymentId = "123456789",
            //    RequestId = "123456789",
            //    responseCode = "00",
            //    retrievalReferenceNumber = "123456789",
            //    sha256OfPan = "123456789",
            //    systemTraceAuditNumber = "123456789"
            //};
            var bres = new BankResult();
            var bankStatus = _bankStatus.Where(w => w.bankId == 10002 && w.Code == int.Parse(br.responseCode)).FirstOrDefault();
            vr = new VerifyInquiryResult
            {
                description = bankStatus.Description,
                responseCode = bankStatus.Code.ToString(),
                status = br.responseCode == "00" ? true : false,
                result = new SubResult()
            };


            if (!string.IsNullOrEmpty(br.RequestId))
            {
                bres.ReqId = int.Parse(br.RequestId);
                bres.BackInfo = JsonConvert.SerializeObject(br);
                bres.BankId = 10002;//irankish
                bres.InsDate = DateTime.Now;

                _bankResult.Add(bres);
                _uow.SaveChanges();



                var gateway = _bank.Find(10002);
                if (br.responseCode == "00" && lr.CallBackType == 1)//Get
                {
                    Uri url = new Uri(lr.CallBackURL + string.Format("?OrderId={0}&PaymentId={1}&TransactionId={2}&TrasactionResultText={3}&TrasactionResult={4}&GatewayName={5}",
                        lr.OrderId, lr.Guid, br.RequestId, bankStatus.Description, br.responseCode, gateway.Name));
                    string jresponse = Helper.Get(url);

                    if (jresponse != null)
                    {
                        //VerifyInquiryResult jResult = JsonConvert.DeserializeObject<VerifyInquiryResult>(jresponse);
                        vr.result.systemTraceAuditNumber = lr.ReqId.ToString();

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
}
