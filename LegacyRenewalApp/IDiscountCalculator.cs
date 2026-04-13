namespace LegacyRenewalApp
{
    public interface IDiscountCalculator
    {
        DiscountCalculationResult Calculate(
            Customer customer,
            SubscriptionPlan plan,
            RenewalRequest request,
            decimal baseAmount);
    }
}