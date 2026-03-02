using Stripe;
using Stripe.Checkout;

namespace WebHotelvesion1._0.Services
{
    public class StripeService
    {
        private readonly string _secretKey;
        private readonly string _publicKey;

        public StripeService(IConfiguration configuration)
        {
            _secretKey = configuration["Stripe:Secretkey"];
            _publicKey = configuration["Stripe:PublicKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }

        public Session CreateCheckoutSession(string successUrl, string cancelUrl, List<SessionLineItemOptions> lineItems)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            return service.Create(options);
        }

        public string GetPublicKey()
        {
            return _publicKey;
        }
    }
}