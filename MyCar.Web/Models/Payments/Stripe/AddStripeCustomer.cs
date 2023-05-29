namespace MyCar.Web.Models.Payments.Stripe
{
    public record AddStripeCustomer(
        string Email,
        string Name,
        AddStripeCard CreditCard);
}
