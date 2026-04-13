using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class PaymentFeeCalculator : IPaymentFeeCalculator
    {
        private readonly IEnumerable<IPaymentFeePolicy> _paymentFeePolicies;

        public PaymentFeeCalculator()
            : this(new IPaymentFeePolicy[]
            {
                new CardPaymentFeePolicy(),
                new BankTransferPaymentFeePolicy(),
                new PaypalPaymentFeePolicy(),
                new InvoicePaymentFeePolicy()
            })
        {
        }

        public PaymentFeeCalculator(IEnumerable<IPaymentFeePolicy> paymentFeePolicies)
        {
            _paymentFeePolicies = paymentFeePolicies ?? throw new ArgumentNullException(nameof(paymentFeePolicies));
        }

        public decimal Calculate(string paymentMethod, decimal subtotalAfterDiscount, decimal supportFee)
        {
            decimal feeBase = subtotalAfterDiscount + supportFee;

            foreach (var policy in _paymentFeePolicies)
            {
                if (policy.Supports(paymentMethod))
                {
                    return policy.Calculate(feeBase);
                }
            }

            throw new ArgumentException("Unsupported payment method");
        }
    }
}