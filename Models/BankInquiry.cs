using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G_IPG_API.Models;

[Table("BANK_INQUIRY")]
public class BankInquiry
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("REQ_ID")]
    public int ReqId { get; set; }

    [Column("INS_DATE")]
    public DateTime InsDate { get; set; }

    [Column("STATUS")]
    public int Status { get; set; }

    [Column("BANK_TERMINAL_ID")]
    public int BankTerminalId { get; set; }

    [Column("BANK_STATUS")]
    public int BankStatus { get; set; }

    [Column("BANK_STATUS_MESSAGE")]
    public string BankStatusMessage { get; set; }

    [Column("BANK_REF_NUMBER")]
    public string BankRefNumber { get; set; }

    [Column("BANK_DIGITAL_REF_NUMBER")]
    public string BankDigitalRefNumber { get; set; }

    [Column("BANK_MID")]
    public string BankMid { get; set; }

    [Column("BANK_TRACKING_CODE")]
    public string BankTrackingCode { get; set; }

    [Column("BANK_AMOUNT")]
    public int BankAmount { get; set; }

    [Column("BANK_WAGE")]
    public string BankWage { get; set; }

    [Column("BANK_SOURCE_CARD_NUM")]
    public string BankSourceCardNum { get; set; }

    [Column("BANK_ID")]
    public int BankId { get; set; }

    [Column("VERIFY_INFO")]
    public string VerifyInfo { get; set; }
}