namespace LegacyRenewalApp
{
    public class CardPaymentFeePolicy : IPaymentFeePolicy
    {
        public bool Supports(string paymentMethod)
        {
            return paymentMethod == "CARD";
        }

        public decimal Calculate(decimal feeBase)
        {
            return feeBase * 0.02m;
        }
    }
}