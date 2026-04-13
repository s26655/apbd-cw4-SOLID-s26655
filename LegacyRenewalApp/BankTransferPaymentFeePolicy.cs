namespace LegacyRenewalApp
{
    public class BankTransferPaymentFeePolicy : IPaymentFeePolicy
    {
        public bool Supports(string paymentMethod)
        {
            return paymentMethod == "BANK_TRANSFER";
        }

        public decimal Calculate(decimal feeBase)
        {
            return feeBase * 0.01m;
        }
    }
}