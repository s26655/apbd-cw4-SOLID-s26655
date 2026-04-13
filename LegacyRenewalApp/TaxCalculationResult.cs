namespace LegacyRenewalApp
{
    public class TaxCalculationResult
    {
        public decimal TaxRate { get; }
        public decimal TaxAmount { get; }

        public TaxCalculationResult(decimal taxRate, decimal taxAmount)
        {
            TaxRate = taxRate;
            TaxAmount = taxAmount;
        }
    }
}