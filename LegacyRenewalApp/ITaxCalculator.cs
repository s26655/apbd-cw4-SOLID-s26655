namespace LegacyRenewalApp
{
    public interface ITaxCalculator
    {
        TaxCalculationResult Calculate(string country, decimal taxBase);
    }
}