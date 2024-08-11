namespace G_IPG_API.Models;

public class InfoOut
{
    public InfoDetail VerifyInfo { get; set; }
    public int ResultCode { get; set; }
    public string ResultDescription { get; set; }
    public bool Success { get; set; }
}