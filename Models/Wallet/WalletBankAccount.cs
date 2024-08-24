using System;
using System.Collections.Generic;
using NodaTime;

namespace G_IPG_API.Models.Wallet;

public partial class WalletBankAccount
{
    public long Id { get; set; }

    public long WalletId { get; set; }

    public int BankId { get; set; }

    public int? RegionId { get; set; }

    public short? Status { get; set; }

    public string? Shaba { get; set; }

    public short? OrderId { get; set; }

    public DateTime? RegDate { get; set; }

    public string? ValidationInfo { get; set; }

    public string? Name { get; set; }

    public string? BankAccountNumber { get; set; }
}
