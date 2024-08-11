using System;

namespace G_IPG_API.Models;

public class SepTxn
{
    public string Action { get; set; }
    public string TerminalId { get; set; }
    public string RedirectUrl { get; set; }
    public string ResNum { get; set; }
    public long Amount { get; set; }
    public long CellNumber { get; set; }
}
