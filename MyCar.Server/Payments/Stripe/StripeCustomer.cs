namespace MyCar.Web.Models.Payments.Stripe
{
    public record StripeCustomer(
        string Name,
        string Email,
        string CustomerId);
}
