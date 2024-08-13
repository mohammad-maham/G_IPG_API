using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;
using G_IPG_API.Models;
using G_IPG_API.Interfaces;
using G_IPG_API.BusinessLogic.Interfaces;
using G_IPG_API.Models.Mellat;
using BankPay.Services.Global;

namespace G_IPG_API.BusinessLogic
{
    public class Mellat : IMellat
    {
        private readonly IConfiguration _configuration;

        public Mellat(IConfiguration iConfig)
        {
            _configuration = iConfig;
        }

        public string bpPayRequest(MellatPayment model)
        {
            //return "1";

            model.TerminalId = Convert.ToInt64(_configuration.GetSection("Configuration:Mellat:TerminalId").Value);
            model.UserName =  _configuration.GetSection("Configuration:Mellat:UserName").Value ;
            model.UserPassword =  _configuration.GetSection("Configuration:Mellat:Password").Value ;
            model.CallBackUrl = _configuration.GetSection("Configuration:Mellat:Password").Value ;
            model.LocalDate=DateTime.Now.ToShortDateString();
            model.LocalTime=DateTime.Now.ToShortTimeString();

            //var request = CreateJsonRequest.CreateJasonRequest(model);
            var request = JsonConvert.SerializeObject(model);

            Uri url = new Uri(_configuration.GetSection("Configuration:Mellat:Url").Value!);
            var res = Helper.Post(url, request);

            return res;
        }

        public string bpVerifyRequest(MellatPayment model)
        {
            throw new NotImplementedException();
        }

        public string bpInquiryRequest(MellatPayment model)
        {
            throw new NotImplementedException();
        }
    }
}