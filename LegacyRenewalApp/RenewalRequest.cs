namespace LegacyRenewalApp
{
    public class RenewalRequest
    {
        public int CustomerId { get; }
        public string PlanCode { get; }
        public int SeatCount { get; }
        public string PaymentMethod { get; }
        public bool IncludePremiumSupport { get; }
        public bool UseLoyaltyPoints { get; }

        public RenewalRequest(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            CustomerId = customerId;
            PlanCode = planCode;
            SeatCount = seatCount;
            PaymentMethod = paymentMethod;
            IncludePremiumSupport = includePremiumSupport;
            UseLoyaltyPoints = useLoyaltyPoints;
        }
    }
}