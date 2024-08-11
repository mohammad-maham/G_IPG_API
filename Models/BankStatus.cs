using System;
using System.Collections.Generic;

namespace G_IPG_API.Models;

public partial class BankStatus
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public long BankId { get; set; }

    public int? Code { get; set; }
}
