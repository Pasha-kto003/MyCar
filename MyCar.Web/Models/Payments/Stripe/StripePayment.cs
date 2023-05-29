namespace MyCar.Web.Models.Payments.Stripe
{
    public record StripePayment(
        string CustomerId,
        string ReceiptEmail,
        string Description,
        string Currency,
        long Amount,
        string PaymentId);
}
