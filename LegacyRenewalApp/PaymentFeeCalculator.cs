using System;

namespace LegacyRenewalApp
{
    public class PaymentFeeCalculator : IPaymentFeeCalculator
    {
        public decimal Calculate(string paymentMethod, decimal subtotalAfterDiscount, decimal supportFee)
        {
            decimal feeBase = subtotalAfterDiscount + supportFee;

            if (paymentMethod == "CARD")
            {
                return feeBase * 0.02m;
            }

            if (paymentMethod == "BANK_TRANSFER")
            {
                return feeBase * 0.01m;
            }

            if (paymentMethod == "PAYPAL")
            {
                return feeBase * 0.035m;
            }

            if (paymentMethod == "INVOICE")
            {
                return 0m;
            }

            throw new ArgumentException("Unsupported payment method");
        }
    }
}