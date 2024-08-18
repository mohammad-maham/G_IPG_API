using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace G_APIs.Services
{
    public class GoldApi
    {
        private string ApiPath { get; set; }
        public string Authorization { get; set; }
        public string Action { get; set; }
        public Method _Method { get; set; }
        public object Data { get; set; }

        public GoldApi( string apiPath, object data, Method method = Method.Post, string authorization = null)
        {
            ApiPath = apiPath;
            Authorization = authorization;
            Data = data;
            _Method = method;
        }

        public string Post()
        {
            try
            {
                RestClient client = new RestClient(ApiPath + Action);
                RestRequest request = new RestRequest
                {
                    Method = _Method,
                    Timeout = TimeSpan.FromSeconds(20),
                };

                if (Authorization != null)
                {
                    request.AddHeader("Authorization", "Bearer " + Authorization);
                }

                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-charset", "utf-8");

                request.AddJsonBody(Data);

                RestResponse response = client.Execute(request);
                return response.Content;

            }
            catch (Exception ex)
            {
                return ex.InnerException.ToString();
            }
        }
    }
}