namespace LegacyRenewalApp
{
    public interface IRenewalNotesBuilder
    {
        string Build(
            string discountNotes,
            RenewalRequest request,
            bool minimumInvoiceAmountApplied);
    }
}