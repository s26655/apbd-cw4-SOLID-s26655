namespace LegacyRenewalApp
{
    public class PaypalPaymentFeePolicy : IPaymentFeePolicy
    {
        public bool Supports(string paymentMethod)
        {
            return paymentMethod == "PAYPAL";
        }

        public decimal Calculate(decimal feeBase)
        {
            return feeBase * 0.035m;
        }
    }
}