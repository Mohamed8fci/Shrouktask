using ShaTask.Models;

namespace ShaTask.IRepository
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<InvoiceHeader>> GetInvoicesAsync();
        Task<InvoiceHeader> GetInvoiceByIdAsync(long id);
        Task AddInvoiceAsync(InvoiceHeader invoiceHeader);
        Task UpdateInvoiceAsync(InvoiceHeader invoiceHeader);
        Task DeleteInvoiceAsync(long id);
        Task AddInvoiceDetailAsync(InvoiceDetail invoiceDetail);
        Task UpdateInvoiceDetailAsync(InvoiceDetail invoiceDetail);
        Task DeleteInvoiceDetailAsync(long id);
    }


}
