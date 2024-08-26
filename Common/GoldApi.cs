using G_APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace G_IPG_API.Common;

public class GoldApi
{
    private string ApiPath { get; set; }
    public Method _Method { get; set; }
    public object Data { get; set; }

    public GoldApi(string path, object data, Method method = Method.Post)
    {

        ApiPath = path;
        Data = data;
        _Method = method;
    }

    public string Post()
    {
        try
        {
            RestClient client = new RestClient(ApiPath);
            RestRequest request = new RestRequest
            {
                Method = _Method,
                Timeout = TimeSpan.FromSeconds(20),
            };

            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");

            request.AddJsonBody(Data);

            RestResponse response = client.Execute(request);
            return response.Content!;

        }
        catch (Exception ex)
        {
            throw;
        }
    }
}