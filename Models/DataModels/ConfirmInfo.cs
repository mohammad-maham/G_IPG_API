using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace G_IPG_API.Models.DataModels;

public class ConfirmInfo
{
    public ConfirmInfo()
    {
        result = new ConfirmResult();
    }
    public string? responseCode { get; set; }
    public string? description { get; set; }
    public bool? status { get; set; }
    public ConfirmResult? result { get; set; }

}

public class ConfirmResult
{
    public string responseCode { get; set; }
    public int? systemTraceAuditNumber { get; set; }
    public long? retrievalReferenceNumber { get; set; }
    public DateTime transactionDateTime { get; set; }
    public string? transactionDate { get; set; }
    public string? transactionTime { get; set; }
    public string? processCode { get; set; }
    public int? requestId { get; set; }
    public string? additional { get; set; }
    public string? billType { get; set; }
    public long? billId { get; set; }
    public long? amount { get; set; }
    public string revertUri { get; set; }
    public long? acceptorId { get; set; }
    public int? terminalId { get; set; }
    public string tokenIdentity { get; set; }
    public bool? isMultiplex { get; set; }
    public bool? isVerified { get; set; }
    public bool? isReversed { get; set; }
    public string maskedPan { get; set; }
    public string description { get; set; }
    public bool? status { get; set; }
    public string result { get; set; }

}

