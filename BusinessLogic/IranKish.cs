using System;
using Newtonsoft.Json;
using BankPay.Services.Global;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
 
using G_IPG_API.Models;
using G_IPG_API.Interfaces;

namespace G_IPG_API.BusinessLogic;

public class IranKish : IIranKish
{
    public IConfiguration _configuration;

    public IranKish(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResult GetToken(IPG_IrKish iPGData)
    {

        return new TokenResult
        {
            description = "description",
            responseCode = "00",
            status = true

        };

        iPGData.TransactionType = TransactionType.Purchase;
        iPGData.BillInfo = null;
        iPGData.RsaPublicKey = _configuration.GetSection("Configuration").GetSection("ikcRSAPublicKey").Value;
        iPGData.TreminalId = _configuration.GetSection("Configuration").GetSection("ikcTreminalId").Value;
        iPGData.AcceptorId = _configuration.GetSection("Configuration").GetSection("ikcAcceptorId").Value;
        iPGData.RevertURL = _configuration.GetSection("Configuration").GetSection("url").Value;
        iPGData.PassPhrase = _configuration.GetSection("Configuration").GetSection("ikcPassPhrase").Value;

        var request = CreateJsonRequest.CreateJasonRequest(iPGData);

        Uri url = new Uri(_configuration.GetSection("Configuration").GetSection("ikcToken").Value);
        var jresponse = Helper.Post(url, request);

        if (jresponse != null)
        {
            return JsonConvert.DeserializeObject<TokenResult>(jresponse);
        }

        return null;
    }

    public VerifyInquiryResult ConfirmationPurchase(RequestVerify req)
    {
        try
        {
            req.terminalId = _configuration.GetSection("Configuration").GetSection("ikcTreminalId").Value;

            string request = JsonConvert.SerializeObject(req);

            Uri url = new Uri(_configuration.GetSection("Configuration").GetSection("ikcVerify").Value);
            string jresponse = Helper.Post(url, request);


            if (jresponse != null)
            {
                VerifyInquiryResult jResult = JsonConvert.DeserializeObject<VerifyInquiryResult>(jresponse);
                //Handle your reponse here


                return jResult;
                // if (jResult.status)
                // {

                //     return "عملیات تایید تراکنش با موفقیت انجام شد" + " result=" + jResult.description;

                //     //  Verification succed , your statements Goes here

                // }
                // else
                // {
                //     return "عملیات تایید تراکنش با موفقیت انجام نشد" + " result=" + jResult.description;
                //     //  Verification Failed , your statements Goes here

                // }
            }

            return new VerifyInquiryResult
            {
                description = "خطایی در استعلام به وجود امد"
            };
        }
        catch (Exception exe)
        {
            return new VerifyInquiryResult
            {
                description = "خطا" + exe.Message
            };
        }
    }

    public VerifyInquiryResult InquirySingle(Inquery inq)
    {
        try
        {
            inq.terminalId = _configuration.GetSection("Configuration").GetSection("ikcTreminalId").Value;
            inq.passPhrase = _configuration.GetSection("Configuration").GetSection("ikcPassPhrase").Value;
            inq.findOption = 2;

            string request = JsonConvert.SerializeObject(inq);

            Uri url = new Uri(_configuration.GetSection("Configuration").GetSection("ikcInquery").Value);

            string jresponse = Helper.Post(url, request);

            if (jresponse != null)
            {
                VerifyInquiryResult jResult = JsonConvert.DeserializeObject<VerifyInquiryResult>(jresponse);

                return jResult;
            }

            return new VerifyInquiryResult
            {
                description = "خطایی در استعلام به وجود امد"
            };


        }
        catch (Exception exe)
        {
            return new VerifyInquiryResult
            {
                description = "خطا" + exe.Message
            };
        }
    }


}