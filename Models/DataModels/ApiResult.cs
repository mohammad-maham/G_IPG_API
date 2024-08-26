namespace G_APIs.Models
{
    public class ApiResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }

        public ApiResult() { }

        public ApiResult(int resultCode, string message = "", dynamic data = null)
        {
            StatusCode = resultCode;
            Message = message;
            Data = data;
        }
    }
}