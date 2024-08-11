namespace G_IPG_API.Models;

public class PayInfo
{
    public long MID { get; set; }
    public string State { get; set; }
    public int Status { get; set; }
    public string RRN { get; set; }
    public string RefNum { get; set; }
    public long ResNum { get; set; }
    public long TerminalId { get; set; }
    public string TraceNo { get; set; }
    public long Amount { get; set; }
    public long Wage { get; set; }
    public string SecurePan { get; set; }
}