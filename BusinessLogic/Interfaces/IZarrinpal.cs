using G_IPG_API.Common;
using G_IPG_API.Models;
using GoldHelpers.Helpers;
using GoldHelpers.Models;
using Newtonsoft.Json.Linq;

namespace G_IPG_API.Interfaces;

public interface IZarrinpal
{
    GoldAPIResult? Payment(LinkRequest model);
    GoldAPIResult? VerifyPayment(string authority, LinkRequest model);
}