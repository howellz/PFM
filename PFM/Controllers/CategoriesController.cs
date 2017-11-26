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
    public class CategoriesController : Controller
    {
        private readonly PersonalFinanceManagerDBContext _context;

        public CategoriesController(PersonalFinanceManagerDBContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var personalFinanceManagerDBContext = _context.Categories.Include(c => c.User).Include(i => i.Subcategories);
          
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }

        // GET: Categories
        public async Task<IActionResult> Home()
        {
            var personalFinanceManagerDBContext = _context.Categories.Include(c => c.User).Include(i => i.Subcategories);

            var userID = _context.User.FirstOrDefault().UserId;
            ViewData["ds"] = totalBudget(userID) - totalDeductionsSum(userID) ;
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }
        public async Task<IActionResult> Budgets()
        {
            var personalFinanceManagerDBContext = _context.Categories.Include(c => c.User).Include(i => i.Subcategories);

            var userID = _context.User.FirstOrDefault().UserId;
            ViewBag.deductionsSum = new Func<int, int>(remaining);
            ViewBag.totalBuget = new Func<int, int>(catSum);
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }
        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.CategoryId == id);
            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,UserId,CategoryName")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", categories.UserId);
            return View(categories);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == id);
            if (categories == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", categories.UserId);
            return View(categories);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,UserId,CategoryName")] Categories categories)
        {
            if (id != categories.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriesExists(categories.CategoryId))
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
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", categories.UserId);
            return View(categories);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.CategoryId == id);
            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categories = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == id);
            _context.Categories.Remove(categories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriesExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
        
        private int catSum(int catID)
        {
            var sum = 0;

            foreach (var subcat in _context.Subcategories.Include(e => e.Category))
            {
                if (subcat.Category.CategoryId == catID)
                {
                    sum += subcat.Value;
                }
            }
            return sum;
        }

        private int totalBudget(int userID)//
        {
            var sum = 0;
            foreach (var subcat in _context.Subcategories.Include(e => e.Category))
            {
                if (subcat.Category.UserId == userID)
                {
                    // var cats = user.Categories;
                        sum += subcat.Value;
                }
            }
            return sum;
        }


        public int totalDeductionsSum(int userID)
        {

            //var cats = _context.Categories.SingleOrDefault(e => e.CategoryId == catID);
            var sum = 0;
            var trans = _context.Transactions;

            foreach (var i in trans)
            {
                if (i.UserId == userID)
                {
                    sum += i.Value;
                }
            }
            return sum;
        }

        public int deductionsSum(int catID)
        {
            var sum = 0;
            var trans = _context.Transactions.Include(e => e.Subcategory);

            foreach (var i in trans)
            {
                if (i.Subcategory.CategoryId == catID)
                {
                    sum += i.Value;
                }
            }
            return sum;
        }

        public int remaining(int catID)
        {
            var sum = 0;
            sum = catSum(catID) - deductionsSum(catID);
            return sum;
        }

    }
}
