using G_IPG_API.Models;
using Newtonsoft.Json.Linq;

namespace G_IPG_API.Interfaces;

public interface IZarrinpal
{
    string AddPaymentData(PaymentLinkRequest request);
    string Payment(LinkRequest model);
    string VerifyPayment(string authority, string amount);
}