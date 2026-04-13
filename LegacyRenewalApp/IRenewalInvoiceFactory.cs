namespace LegacyRenewalApp
{
    public interface IRenewalInvoiceFactory
    {
        RenewalInvoice Create(RenewalInvoiceData data);
    }
}