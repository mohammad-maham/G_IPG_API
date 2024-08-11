using System;
using System.Collections.Generic;

namespace G_IPG_API.Models;

public partial class LinkCall
{
    public long Id { get; set; }

    public DateTime InsertDate { get; set; }

    public int RequestId { get; set; }

    public string? RequestAddress { get; set; }

    public short? Status { get; set; }

    public string TokenInfo { get; set; } = null!;
}
