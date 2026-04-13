namespace LegacyRenewalApp
{
    public class RenewalNotesBuilder : IRenewalNotesBuilder
    {
        public string Build(
            string discountNotes,
            RenewalRequest request,
            bool minimumInvoiceAmountApplied)
        {
            string notes = discountNotes;

            if (request.IncludePremiumSupport)
            {
                notes += "premium support included; ";
            }

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

            if (minimumInvoiceAmountApplied)
            {
                notes += "minimum invoice amount applied; ";
            }

            return notes;
        }
    }
}