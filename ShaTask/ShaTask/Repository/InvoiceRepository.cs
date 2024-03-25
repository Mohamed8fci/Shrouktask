using Microsoft.EntityFrameworkCore;
using ShaTask.IRepository;
using ShaTask.Models;

namespace ShaTask.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ShaTaskContext _dbContext;

        public InvoiceRepository(ShaTaskContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<InvoiceHeader>> GetInvoicesAsync()
        {
            return await _dbContext.InvoiceHeaders
                .Include(header => header.InvoiceDetails)
                .Include(header => header.Cashier)
                .Include(header => header.Branch)
                .ToListAsync();
        }

        public async Task<InvoiceHeader> GetInvoiceByIdAsync(long id)
        {
            return await _dbContext.InvoiceHeaders
                .Include(header => header.InvoiceDetails)
                .Include(header => header.Cashier)
                .Include(header => header.Branch)
                .FirstOrDefaultAsync(header => header.Id == id);
        }

        public async Task AddInvoiceAsync(InvoiceHeader invoiceHeader)
        {
            _dbContext.InvoiceHeaders.Add(invoiceHeader);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInvoiceAsync(InvoiceHeader invoiceHeader)
        {
            _dbContext.InvoiceHeaders.Update(invoiceHeader);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(long id)
        {
            var invoice = await _dbContext.InvoiceHeaders.FindAsync(id);
            if (invoice != null)
            {
                // Manually delete related InvoiceDetail records
                var detailsToDelete = _dbContext.InvoiceDetails.Where(d => d.InvoiceHeaderId == id);
                _dbContext.InvoiceDetails.RemoveRange(detailsToDelete);

                // Then delete the InvoiceHeader
                _dbContext.InvoiceHeaders.Remove(invoice);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddInvoiceDetailAsync(InvoiceDetail invoiceDetail)
        {
            _dbContext.InvoiceDetails.Add(invoiceDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInvoiceDetailAsync(InvoiceDetail invoiceDetail)
        {
            _dbContext.InvoiceDetails.Update(invoiceDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInvoiceDetailAsync(long id)
        {
            var detail = await _dbContext.InvoiceDetails.FindAsync(id);
            if (detail != null)
            {
                _dbContext.InvoiceDetails.Remove(detail);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

}
