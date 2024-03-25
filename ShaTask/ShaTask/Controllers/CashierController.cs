using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShaTask.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ShaTask.Controllers
{
    public class CashierController : Controller
    {
        private readonly ShaTaskContext _dbContext;

        public CashierController(ShaTaskContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var cashiers = await _dbContext.Cashiers.Include(e=>e.Branch).ToListAsync();
            return View(cashiers);
        }

        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_dbContext.Branches, "Id", "BranchName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CashierName,BranchId")] Cashier cashier)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(cashier);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_dbContext.Branches, "Id", "BranchName", cashier.BranchId);
            return View(cashier);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashier = await _dbContext.Cashiers.FindAsync(id);
            if (cashier == null)
            {
                return NotFound();
            }
            ViewData["BranchId"] = new SelectList(_dbContext.Branches, "Id", "BranchName", cashier.BranchId);
            return View(cashier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CashierName,BranchId")] Cashier cashier)
        {
            if (id != cashier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(cashier);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashierExists(cashier.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_dbContext.Branches, "Id", "BranchName", cashier.BranchId);
            return View(cashier);
        }

        private bool CashierExists(int id)
        {
            return _dbContext.Cashiers.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashier = await _dbContext.Cashiers.FirstOrDefaultAsync(m => m.Id == id);
            if (cashier == null)
            {
                return NotFound();
            }

            return View(cashier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashier = await _dbContext.Cashiers.FindAsync(id);
            _dbContext.Cashiers.Remove(cashier);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
