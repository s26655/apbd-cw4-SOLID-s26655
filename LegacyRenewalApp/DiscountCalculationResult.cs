namespace LegacyRenewalApp
{
    public class DiscountCalculationResult
    {
        public decimal DiscountAmount { get; }
        public decimal SubtotalAfterDiscount { get; }
        public string Notes { get; }

        public DiscountCalculationResult(
            decimal discountAmount,
            decimal subtotalAfterDiscount,
            string notes)
        {
            DiscountAmount = discountAmount;
            SubtotalAfterDiscount = subtotalAfterDiscount;
            Notes = notes;
        }
    }
}