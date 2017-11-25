using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PFM.Models;

namespace PFM.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly PersonalFinanceManagerDBContext _context;

        public TransactionsController(PersonalFinanceManagerDBContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var personalFinanceManagerDBContext = _context.Transactions.Include(t => t.Subcategory).Include(t => t.User);
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions
                .Include(t => t.Subcategory)
                .Include(t => t.User)
                .SingleOrDefaultAsync(m => m.TransactionsId == id);
            if (transactions == null)
            {
                return NotFound();
            }

            return View(transactions);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "SubcategoryId", "SubcategoryName");
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionsId,UserId,SubcategoryId,Value")] Transactions transactions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transactions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "SubcategoryId", "SubcategoryName", transactions.SubcategoryId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", transactions.UserId);
            return View(transactions);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions.SingleOrDefaultAsync(m => m.TransactionsId == id);
            if (transactions == null)
            {
                return NotFound();
            }
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "SubcategoryId", "SubcategoryName", transactions.SubcategoryId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", transactions.UserId);
            return View(transactions);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionsId,UserId,SubcategoryId,Value")] Transactions transactions)
        {
            if (id != transactions.TransactionsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transactions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionsExists(transactions.TransactionsId))
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
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "SubcategoryId", "SubcategoryName", transactions.SubcategoryId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", transactions.UserId);
            return View(transactions);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions
                .Include(t => t.Subcategory)
                .Include(t => t.User)
                .SingleOrDefaultAsync(m => m.TransactionsId == id);
            if (transactions == null)
            {
                return NotFound();
            }

            return View(transactions);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactions = await _context.Transactions.SingleOrDefaultAsync(m => m.TransactionsId == id);
            _context.Transactions.Remove(transactions);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reset()
        {

            var transactions = _context.Transactions
                .Include(t => t.User);
            if (transactions == null)
            {
                return NotFound();
            }

            return View(await transactions.ToListAsync());
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Reset")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetConfirmed()
        {
            var transactions = _context.Transactions;
            if (transactions.Count() > 0)
            {
                foreach (var i in transactions)
                {
                    _context.Transactions.Remove(i);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionsExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionsId == id);
        }
    }
}
