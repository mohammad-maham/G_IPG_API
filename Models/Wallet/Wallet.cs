using System;
using System.Collections.Generic;
using NodaTime;

namespace G_IPG_API.Models.Wallet;

public partial class Wallet
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public DateTime CreateDate { get; set; }

    public short Status { get; set; }
}
