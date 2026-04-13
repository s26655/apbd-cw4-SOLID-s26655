namespace LegacyRenewalApp
{
    public class RenewalInvoiceData
    {
        public int CustomerId { get; }
        public string CustomerName { get; }
        public string PlanCode { get; }
        public string PaymentMethod { get; }
        public int SeatCount { get; }
        public decimal BaseAmount { get; }
        public decimal DiscountAmount { get; }
        public decimal SupportFee { get; }
        public decimal PaymentFee { get; }
        public decimal TaxAmount { get; }
        public decimal FinalAmount { get; }
        public string Notes { get; }

        public RenewalInvoiceData(
            int customerId,
            string customerName,
            string planCode,
            string paymentMethod,
            int seatCount,
            decimal baseAmount,
            decimal discountAmount,
            decimal supportFee,
            decimal paymentFee,
            decimal taxAmount,
            decimal finalAmount,
            string notes)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            PlanCode = planCode;
            PaymentMethod = paymentMethod;
            SeatCount = seatCount;
            BaseAmount = baseAmount;
            DiscountAmount = discountAmount;
            SupportFee = supportFee;
            PaymentFee = paymentFee;
            TaxAmount = taxAmount;
            FinalAmount = finalAmount;
            Notes = notes;
        }
    }
}