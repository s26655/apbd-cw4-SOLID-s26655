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
        private readonly ITaxCalculator _taxCalculator;
        private readonly IRenewalInvoiceFactory _renewalInvoiceFactory;
        private readonly IRenewalNotesBuilder _renewalNotesBuilder;

        public SubscriptionRenewalService()
            : this(
                new CustomerRepository(),
                new SubscriptionPlanRepository(),
                new LegacyBillingGatewayAdapter(),
                new RenewalRequestValidator(),
                new DiscountCalculator(),
                new SupportFeeCalculator(),
                new PaymentFeeCalculator(),
                new TaxCalculator(),
                new RenewalInvoiceFactory(),
                new RenewalNotesBuilder())
        {
        }

        public SubscriptionRenewalService(
            ICustomerRepository customerRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IBillingGateway billingGateway,
            IRenewalRequestValidator renewalRequestValidator,
            IDiscountCalculator discountCalculator,
            ISupportFeeCalculator supportFeeCalculator,
            IPaymentFeeCalculator paymentFeeCalculator,
            ITaxCalculator taxCalculator,
            IRenewalInvoiceFactory renewalInvoiceFactory,
            IRenewalNotesBuilder renewalNotesBuilder)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _subscriptionPlanRepository = subscriptionPlanRepository ?? throw new ArgumentNullException(nameof(subscriptionPlanRepository));
            _billingGateway = billingGateway ?? throw new ArgumentNullException(nameof(billingGateway));
            _renewalRequestValidator = renewalRequestValidator ?? throw new ArgumentNullException(nameof(renewalRequestValidator));
            _discountCalculator = discountCalculator ?? throw new ArgumentNullException(nameof(discountCalculator));
            _supportFeeCalculator = supportFeeCalculator ?? throw new ArgumentNullException(nameof(supportFeeCalculator));
            _paymentFeeCalculator = paymentFeeCalculator ?? throw new ArgumentNullException(nameof(paymentFeeCalculator));
            _taxCalculator = taxCalculator ?? throw new ArgumentNullException(nameof(taxCalculator));
            _renewalInvoiceFactory = renewalInvoiceFactory ?? throw new ArgumentNullException(nameof(renewalInvoiceFactory));
            _renewalNotesBuilder = renewalNotesBuilder ?? throw new ArgumentNullException(nameof(renewalNotesBuilder));
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

            decimal supportFee = _supportFeeCalculator.Calculate(request);

            decimal paymentFee = _paymentFeeCalculator.Calculate(
                request.PaymentMethod,
                subtotalAfterDiscount,
                supportFee);

            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            var taxResult = _taxCalculator.Calculate(customer.Country, taxBase);
            decimal taxAmount = taxResult.TaxAmount;
            decimal finalAmount = taxBase + taxAmount;

            bool minimumInvoiceAmountApplied = false;
            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                minimumInvoiceAmountApplied = true;
            }

            string notes = _renewalNotesBuilder.Build(
                discountResult.Notes,
                request,
                minimumInvoiceAmountApplied);

            var invoiceData = new RenewalInvoiceData(
                request.CustomerId,
                customer.FullName,
                request.PlanCode,
                request.PaymentMethod,
                request.SeatCount,
                baseAmount,
                discountAmount,
                supportFee,
                paymentFee,
                taxAmount,
                finalAmount,
                notes);

            var invoice = _renewalInvoiceFactory.Create(invoiceData);

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