namespace ShaTask.ViewModels
{
    public class InvoiceDto
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime Invoicedate { get; set; }
        public int? CashierId { get; set; }
        public int? BranchId { get; set; }
        public List<InvoiceDetailDto> InvoiceDetails { get; set; } = new List<InvoiceDetailDto>();
    }

}
