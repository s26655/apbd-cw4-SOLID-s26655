namespace LegacyRenewalApp
{
    public interface IPaymentFeePolicy
    {
        bool Supports(string paymentMethod);
        decimal Calculate(decimal feeBase);
    }
}