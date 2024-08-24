using System;
using System.Collections.Generic;
using NodaTime;

namespace G_IPG_API.Models.Wallet.VM;

public partial class WalletBankAccountVM
{
    public long Id { get; set; }

    public long WalletId { get; set; }

    public int BankId { get; set; }
    public string? BankName { get; set; }
    public string? LogoPath { get; set; }

    public string? BankAccountNumber { get; set; }

    public int RegionId { get; set; }

    public short Status { get; set; }

    public DateTime RegDate { get; set; }

    public string Shaba { get; set; } = null!;

    public short OrderId { get; set; }

    public string? ValidationInfo { get; set; }
}
