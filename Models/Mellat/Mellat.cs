namespace G_IPG_API.Models.Mellat;

public class MellatPayment
{
    public long TerminalId { get; set; }
    public string? UserName { get; set; }
    public string? UserPassword { get; set; }
    public long? OrderId { get; set; }
    public long? Amount { get; set; }
    public string? LocalDate { get; set; }
    public string? LocalTime { get; set; }
    public string? AdditionalData { get; set; }
    public string? CallBackUrl { get; set; }
    public long? PayerId { get; set; }
    public long? saleReferenceId { get; set; }
    public long? saleOrderId { get; set; }

    public long? SaleOrderId { get; set; }
    public long? SaleReferenceId { get; set; }
}
