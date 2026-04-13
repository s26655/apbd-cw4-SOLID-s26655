using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IBillingGateway _billingGateway;
        private readonly IRenewalRequestValidator _renewalRequestValidator;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly ISupportFeeCalculator _supportFeeCalculator;
        private readonly IPaymentFeeCalculator _paymentFeeCalculator;

        public SubscriptionRenewalService()
            : this(
                new CustomerRepository(),
                new SubscriptionPlanRepository(),
                new LegacyBillingGatewayAdapter(),
                new RenewalRequestValidator(),
                new DiscountCalculator(),
                new SupportFeeCalculator(),
                new PaymentFeeCalculator())
        {
        }

        public SubscriptionRenewalService(
            ICustomerRepository customerRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IBillingGateway billingGateway,
            IRenewalRequestValidator renewalRequestValidator,
            IDiscountCalculator discountCalculator,
            ISupportFeeCalculator supportFeeCalculator,
            IPaymentFeeCalculator paymentFeeCalculator)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _subscriptionPlanRepository = subscriptionPlanRepository ?? throw new ArgumentNullException(nameof(subscriptionPlanRepository));
            _billingGateway = billingGateway ?? throw new ArgumentNullException(nameof(billingGateway));
            _renewalRequestValidator = renewalRequestValidator ?? throw new ArgumentNullException(nameof(renewalRequestValidator));
            _discountCalculator = discountCalculator ?? throw new ArgumentNullException(nameof(discountCalculator));
            _supportFeeCalculator = supportFeeCalculator ?? throw new ArgumentNullException(nameof(supportFeeCalculator));
            _paymentFeeCalculator = paymentFeeCalculator ?? throw new ArgumentNullException(nameof(paymentFeeCalculator));
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            var request = _renewalRequestValidator.ValidateAndNormalize(
                customerId,
                planCode,
                seatCount,
                paymentMethod,
                includePremiumSupport,
                useLoyaltyPoints);

            var customer = _customerRepository.GetById(request.CustomerId);
            var plan = _subscriptionPlanRepository.GetByCode(request.PlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            decimal baseAmount = (plan.MonthlyPricePerSeat * request.SeatCount * 12m) + plan.SetupFee;

            var discountResult = _discountCalculator.Calculate(customer, plan, request, baseAmount);
            decimal discountAmount = discountResult.DiscountAmount;
            decimal subtotalAfterDiscount = discountResult.SubtotalAfterDiscount;
            string notes = discountResult.Notes;

            decimal supportFee = _supportFeeCalculator.Calculate(request);
            if (request.IncludePremiumSupport)
            {
                notes += "premium support included; ";
            }

            decimal paymentFee = _paymentFeeCalculator.Calculate(
                request.PaymentMethod,
                subtotalAfterDiscount,
                supportFee);

            if (request.PaymentMethod == "CARD")
            {
                notes += "card payment fee; ";
            }
            else if (request.PaymentMethod == "BANK_TRANSFER")
            {
                notes += "bank transfer fee; ";
            }
            else if (request.PaymentMethod == "PAYPAL")
            {
                notes += "paypal fee; ";
            }
            else if (request.PaymentMethod == "INVOICE")
            {
                notes += "invoice payment; ";
            }

            decimal taxRate = 0.20m;
            if (customer.Country == "Poland")
            {
                taxRate = 0.23m;
            }
            else if (customer.Country == "Germany")
            {
                taxRate = 0.19m;
            }
            else if (customer.Country == "Czech Republic")
            {
                taxRate = 0.21m;
            }
            else if (customer.Country == "Norway")
            {
                taxRate = 0.25m;
            }

            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{request.CustomerId}-{request.PlanCode}",
                CustomerName = customer.FullName,
                PlanCode = request.PlanCode,
                PaymentMethod = request.PaymentMethod,
                SeatCount = request.SeatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            _billingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {request.PlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                _billingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}