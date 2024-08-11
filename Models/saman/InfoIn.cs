namespace G_IPG_API.Models;

public class InfoIn
{
    public string RefNum { get; set; }
    public int TerminalNumber { get; set; }
    public int TxnRandomSessionKey { get; set; }
    public string CellNumber { get; set; }
    public bool IgnoreNationalcode { get; set; }
}