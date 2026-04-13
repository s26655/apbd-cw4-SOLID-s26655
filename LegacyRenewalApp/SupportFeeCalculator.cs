namespace LegacyRenewalApp
{
    public class SupportFeeCalculator : ISupportFeeCalculator
    {
        public decimal Calculate(RenewalRequest request)
        {
            if (!request.IncludePremiumSupport)
            {
                return 0m;
            }

            if (request.PlanCode == "START")
            {
                return 250m;
            }

            if (request.PlanCode == "PRO")
            {
                return 400m;
            }

            if (request.PlanCode == "ENTERPRISE")
            {
                return 700m;
            }

            return 0m;
        }
    }
}