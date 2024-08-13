using G_IPG_API.Models;
using G_IPG_API.Models.Mellat;

namespace G_IPG_API.BusinessLogic.Interfaces;

public interface IMellat
{
    string bpPayRequest(MellatPayment model);
    string bpVerifyRequest(MellatPayment model);
    string bpInquiryRequest(MellatPayment model);
}
