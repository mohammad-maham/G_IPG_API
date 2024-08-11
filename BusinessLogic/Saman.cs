using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;
using G_IPG_API.Models;
using G_IPG_API.Interfaces;

namespace G_IPG_API.BusinessLogic
{
    public class Saman : ISaman
    {
        private readonly IConfiguration _configuration;

        public Saman(IConfiguration iConfig)
        {
            _configuration = iConfig;
        }

        public JsonResult Verify_Reverse_Transcation(InfoIn param, int type)
        {
            string urlAddress = type == 1 ? _configuration.GetSection("Configuration").GetSection("VerifyTranscationAddress").Value
                : _configuration.GetSection("Configuration").GetSection("ReverseTranscationURL").Value;

            var restClient = new RestClient(urlAddress);
            var request = new RestRequest()
            {
                RequestFormat = DataFormat.Json,
                OnBeforeDeserialization = resp => resp.ContentType = "application/json",
                Method=Method.Post
            };

            request.AddJsonBody(param);

            var sepResult = restClient.Execute(request);

            var result = (InfoOut)JsonConvert.DeserializeObject(sepResult.Content);

            return new JsonResult(new { Data = result });
        }

        public TokenInfo SendViaToken(SepTxn txn)
        {
            string urlAddress = _configuration.GetSection("Configuration").GetSection("SepShaparakAddresses").Value;
            var restClient = new RestClient(urlAddress);
            var request = new RestRequest()
            {
                RequestFormat = DataFormat.Json,
                OnBeforeDeserialization = resp => resp.ContentType = "application/json",
                Method=Method.Post
            };
            //to request a token you should set Action property as token.
            txn.TerminalId = _configuration.GetSection("Configuration").GetSection("SepTerminalId").Value;
            txn.Action = "Token";

            request.AddJsonBody(txn);

            var sepResult = restClient.Execute(request);

            var sepPgToken = JsonConvert.DeserializeObject<TokenInfo>(sepResult.Content);

            return sepPgToken;
        }
    }
}