namespace LegacyRenewalApp
{
    public interface IRenewalRequestValidator
    {
        RenewalRequest ValidateAndNormalize(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints);
    }
}