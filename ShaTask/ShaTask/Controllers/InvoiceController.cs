using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShaTask.IRepository;
using ShaTask.Models;
using ShaTask.ViewModels;

namespace ShaTask.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ShaTaskContext _dbContext;

        public InvoiceController(IInvoiceRepository invoiceRepository, ShaTaskContext dbContext)
        {
            _invoiceRepository = invoiceRepository;
            _dbContext = dbContext; 
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetInvoicesAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Display(long id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // Assuming ViewBag is used for passing data to the view
            // This part might need adjustment based on your actual implementation
            ViewBag.Cashiers = new SelectList(_dbContext.Cashiers, "Id", "CashierName");
            ViewBag.Branches = new SelectList(_dbContext.Branches, "Id", "BranchName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(InvoiceDto invoiceDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var invoiceHeader = new InvoiceHeader
                    {
                        CustomerName = invoiceDto.CustomerName,
                        Invoicedate = invoiceDto.Invoicedate,
                        CashierId = invoiceDto.CashierId,
                        BranchId = invoiceDto.BranchId
                    };

                    await _invoiceRepository.AddInvoiceAsync(invoiceHeader);

                    foreach (var detailDto in invoiceDto.InvoiceDetails)
                    {
                        var invoiceDetail = new InvoiceDetail
                        {
                            InvoiceHeaderId = invoiceHeader.Id,
                            ItemName = detailDto.ItemName,
                            ItemCount = detailDto.ItemCount,
                            ItemPrice = detailDto.ItemPrice
                        };

                        // Assuming you have a method in your repository to add details
                        await _invoiceRepository.AddInvoiceDetailAsync(invoiceDetail);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. ");
                }
            }

            return View(invoiceDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var invoiceHeader = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoiceHeader == null)
            {
                return NotFound();
            }

            var invoiceDto = new InvoiceDto
            {
                Id = invoiceHeader.Id,
                CustomerName = invoiceHeader.CustomerName,
                Invoicedate = invoiceHeader.Invoicedate,
                CashierId = invoiceHeader.CashierId,
                BranchId = (int)invoiceHeader.BranchId,
                InvoiceDetails = invoiceHeader.InvoiceDetails.Select(detail => new InvoiceDetailDto
                {
                    Id = detail.Id,
                    ItemName = detail.ItemName,
                    ItemCount = detail.ItemCount,
                    ItemPrice = detail.ItemPrice
                }).ToList()
            };

            // Assuming ViewBag is used for passing data to the view
            // This part might need adjustment based on your actual implementation
            ViewBag.Cashiers = new SelectList(_dbContext.Cashiers, "Id", "CashierName");
            ViewBag.Branches = new SelectList(_dbContext.Branches, "Id", "BranchName");

            return View(invoiceDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, InvoiceDto invoiceDto)
        {
            if (id != invoiceDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingInvoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
                    if (existingInvoice == null)
                    {
                        return NotFound();
                    }

                    // Update the existing invoice with the new values from invoiceDto
                    existingInvoice.CustomerName = invoiceDto.CustomerName;
                    existingInvoice.Invoicedate = invoiceDto.Invoicedate;
                    existingInvoice.CashierId = invoiceDto.CashierId;
                    existingInvoice.BranchId = invoiceDto.BranchId;

                    // Update related invoice details
                    foreach (var detailDto in invoiceDto.InvoiceDetails)
                    {
                        var existingDetail = existingInvoice.InvoiceDetails.FirstOrDefault(d => d.Id == detailDto.Id);
                        if (existingDetail != null)
                        {
                            existingDetail.ItemName = detailDto.ItemName;
                            existingDetail.ItemCount = detailDto.ItemCount;
                            existingDetail.ItemPrice = detailDto.ItemPrice;
                        }
                        else
                        {
                            var newDetail = new InvoiceDetail
                            {
                                ItemName = detailDto.ItemName,
                                ItemCount = detailDto.ItemCount,
                                ItemPrice = detailDto.ItemPrice
                            };
                            existingInvoice.InvoiceDetails.Add(newDetail);
                        }
                    }

                    // Remove any details not in the updated list
                    foreach (var existingDetail in existingInvoice.InvoiceDetails.ToList())
                    {
                        if (!invoiceDto.InvoiceDetails.Any(d => d.Id == existingDetail.Id))
                        {
                            existingInvoice.InvoiceDetails.Remove(existingDetail);
                        }
                    }

                    await _invoiceRepository.UpdateInvoiceAsync(existingInvoice);

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }

            return View(invoiceDto);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _invoiceRepository.DeleteInvoiceAsync(id);
            return RedirectToAction("Index");
        }
    }
}
