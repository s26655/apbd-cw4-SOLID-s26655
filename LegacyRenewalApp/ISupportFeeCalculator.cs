namespace LegacyRenewalApp
{
    public interface ISupportFeeCalculator
    {
        decimal Calculate(RenewalRequest request);
    }
}