using System;
using System.Collections.Generic;
using NodaTime;

namespace G_IPG_API.Models;

public partial class LinkRequest
{
    public string? Guid { get; set; }

    public int RequestId { get; set; }

    public long UserId { get; set; }

    public DateTime InsertDate { get; set; }

    public short Status { get; set; }

    public string Title { get; set; } = null!;

    public long? Price { get; set; }

    public short CallBackType { get; set; }

    public string? OrderId { get; set; }

    public DateTime ExpireDate { get; set; }

    public string? AccLinkReqConf { get; set; }

    public string? CallbackUrl { get; set; }

    public string? ClientMobile { get; set; }

    public string? FactorDetail { get; set; }
}
