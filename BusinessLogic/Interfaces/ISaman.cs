using Microsoft.AspNetCore.Mvc;
using G_IPG_API.Models;

namespace G_IPG_API.Interfaces;

public interface ISaman
{
    JsonResult Verify_Reverse_Transcation(InfoIn param, int type);
    TokenInfo SendViaToken(SepTxn txn);
}