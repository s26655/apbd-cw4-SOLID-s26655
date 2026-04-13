namespace LegacyRenewalApp
{
    public class TaxCalculator : ITaxCalculator
    {
        public TaxCalculationResult Calculate(string country, decimal taxBase)
        {
            decimal taxRate = 0.20m;

            if (country == "Poland")
            {
                taxRate = 0.23m;
            }
            else if (country == "Germany")
            {
                taxRate = 0.19m;
            }
            else if (country == "Czech Republic")
            {
                taxRate = 0.21m;
            }
            else if (country == "Norway")
            {
                taxRate = 0.25m;
            }

            decimal taxAmount = taxBase * taxRate;

            return new TaxCalculationResult(taxRate, taxAmount);
        }
    }
}