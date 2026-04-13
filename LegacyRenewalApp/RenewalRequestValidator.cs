using System;

namespace LegacyRenewalApp
{
    public class RenewalRequestValidator : IRenewalRequestValidator
    {
        public RenewalRequest ValidateAndNormalize(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer id must be positive");
            }

            if (string.IsNullOrWhiteSpace(planCode))
            {
                throw new ArgumentException("Plan code is required");
            }

            if (seatCount <= 0)
            {
                throw new ArgumentException("Seat count must be positive");
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException("Payment method is required");
            }

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            return new RenewalRequest(
                customerId,
                normalizedPlanCode,
                seatCount,
                normalizedPaymentMethod,
                includePremiumSupport,
                useLoyaltyPoints);
        }
    }
}