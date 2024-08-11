using System;
using System.Collections.Generic;

namespace G_IPG_API.Models;

public partial class BankResult
{
    public long Id { get; set; }

    public DateTime InsertDate { get; set; }

    public short Status { get; set; }

    public int? RequestId { get; set; }

    public string? ReferenceNumber { get; set; }

    public long BankId { get; set; }

    public string? BankInfo { get; set; }

    public string? ConfirmInfo { get; set; }
}
