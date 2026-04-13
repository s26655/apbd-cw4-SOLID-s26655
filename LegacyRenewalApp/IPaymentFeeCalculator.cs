namespace LegacyRenewalApp
{
    public interface IPaymentFeeCalculator
    {
        decimal Calculate(string paymentMethod, decimal subtotalAfterDiscount, decimal supportFee);
    }
}