using G_IPG_API.Common;
using G_IPG_API.Models;
using Newtonsoft.Json.Linq;

namespace G_IPG_API.Interfaces;

public interface IZarrinpal
{
    string Payment(LinkRequest model);
    string VerifyPayment(string authority, LinkRequest model);
}