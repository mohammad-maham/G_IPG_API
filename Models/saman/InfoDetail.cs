namespace G_IPG_API.Models;

public class InfoDetail
{
    public string RRN { get; set; }
    public string RefNum { get; set; }
    public string MaskedPan { get; set; }
    public string HashedPan { get; set; }
    public int TerminalNumber { get; set; }
    public int OrginalAmount { get; set; }
    public int AffectiveAmount { get; set; }
    public string StraceDate { get; set; }
    public string StraceNo { get; set; }
}