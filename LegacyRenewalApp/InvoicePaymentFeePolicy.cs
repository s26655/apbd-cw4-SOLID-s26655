namespace LegacyRenewalApp
{
    public class InvoicePaymentFeePolicy : IPaymentFeePolicy
    {
        public bool Supports(string paymentMethod)
        {
            return paymentMethod == "INVOICE";
        }

        public decimal Calculate(decimal feeBase)
        {
            return 0m;
        }
    }
}