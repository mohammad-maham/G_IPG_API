using System;
using System.Collections.Generic;
using NodaTime;

namespace G_IPG_API.Models.Wallet.VM;

public partial class WalletCurrencyVM
{
    public long Id { get; set; }
    public long UserId { get; set; }

    public long WalletId { get; set; }

    public short CurrencyId { get; set; }

    public short Status { get; set; }

    public DateTime RegDate { get; set; }

    public decimal Amount { get; set; }

    public string? WC_Address { get; set; }
    public string? Unit { get; set; }

    public string? CurrencyName { get; set; }

}
