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
        public IActionResult Create(int? userID)
        {
            ViewBag.userID = userID;

            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories.Where(s => s.Category.UserId == userID), "SubcategoryId", "SubcategoryName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionsId,UserId,SubcategoryId,Value")] Transactions transactions)
        {
            String errmssg = "";
            if (ModelState.IsValid)
            {
                var subcategory = await _context.Subcategories.SingleOrDefaultAsync(m => m.SubcategoryId == transactions.SubcategoryId);
                var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == subcategory.CategoryId);
                if (category.UserId == transactions.UserId)
                {
                    _context.Add(transactions);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Transaction", "Categories", new { userID = transactions.UserId });
                }

                errmssg = "Invalid subcategory ";
            }

            errmssg = "Invalid Value";
            ViewBag.errmssg = errmssg;
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories.Where(s => s.Category.UserId == transactions.UserId), "SubcategoryId", "SubcategoryName", transactions.SubcategoryId);
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
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories.Where(s => s.Category.UserId == transactions.UserId), "SubcategoryId", "SubcategoryName", transactions.SubcategoryId);
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
            String errmssg = "";
            if (ModelState.IsValid)
            {
                var subcategory = await _context.Subcategories.SingleOrDefaultAsync(m => m.SubcategoryId == transactions.SubcategoryId);
                var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == subcategory.CategoryId);
                if (category.UserId == transactions.UserId)
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
                    return RedirectToAction("Transaction", "Categories", new { userID = transactions.UserId });
                }
                errmssg = "Invalid subcategory ";
            }
            errmssg += "Invalid Value";
            ViewBag.errmssg = errmssg;
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories.Where(s => s.Category.UserId == transactions.UserId), "SubcategoryId", "SubcategoryName", transactions.SubcategoryId);
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
            return RedirectToAction("Transaction", "Categories", new { userID = transactions.UserId });
        }

        public async Task<IActionResult> Reset(int userID)
        {

            var transactions = _context.Transactions
                .Include(t => t.User);

            ViewBag.userID = userID;
            if (transactions == null)
            {
                return NotFound();
            }

            return View(await transactions.ToListAsync());
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Reset")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetConfirmed(int userID)
        {
            var transactions = _context.Transactions;
            if (transactions.Count() > 0)
            {
                foreach (var i in transactions)
                {
                    if (i.UserId == userID)
                    {
                        _context.Transactions.Remove(i);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Home", "Categories", new { userID = userID });
        }

        private bool TransactionsExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionsId == id);
        }

       
    }
}
