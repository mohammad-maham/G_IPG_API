namespace G_IPG_API.Models;

public class TokenInfo
{
    public long TerminalId { get; set; }
    public long ResNum { get; set; }
    public int Status { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorDesc { get; set; }
    public string Token { get; set; }
    public string Type { get; set; }
}