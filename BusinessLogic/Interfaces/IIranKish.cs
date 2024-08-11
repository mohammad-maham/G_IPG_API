using G_IPG_API.Models;

namespace G_IPG_API.Interfaces;

public interface IIranKish
{
    TokenResult GetToken(IPG_IrKish txn);
    VerifyInquiryResult ConfirmationPurchase(RequestVerify req);
    VerifyInquiryResult InquirySingle(Inquery inq);
}