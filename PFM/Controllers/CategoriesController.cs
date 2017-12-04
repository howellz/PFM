using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Web;
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
        public async Task<IActionResult> Index(int? userID)
        {
            var personalFinanceManagerDBContext = _context.Categories.Include(c => c.User).Include(i => i.Subcategories).Where(s => s.UserId == userID);


            //var userID = _context.User.FirstOrDefault().UserId;


            ViewBag.subDeductionsSum = new Func<int, int>(subDeductionsSum);
            ViewBag.subRemaining = new Func<int, int>(subRemaining);
            var name = _context.User.SingleOrDefault(s => s.UserId == userID).FullName;
            ViewBag.userID = userID;
            ViewBag.name = name;
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }


        public int subDeductionsSum(int id)
        {

            var total = 0;

            var subcategory = _context.Subcategories.Include(s => s.Category).Include(s => s.Transactions).SingleOrDefault(s => s.SubcategoryId == id);

            foreach (var i in subcategory.Transactions)
            {
                total += i.Value;
            }

            return total;
        }

        public int subRemaining(int id)
        {

            var remaining = 0;

            var subcategory = _context.Subcategories.Include(s => s.Category).Include(s => s.Transactions).SingleOrDefault(s => s.SubcategoryId == id);

            remaining = subcategory.Value - subDeductionsSum(id);

            return remaining;
        }


        // GET: Categories
        public async Task<IActionResult> Home(int? userID)
        {
            var personalFinanceManagerDBContext = _context.Categories.Include(c => c.User).Include(c => c.Subcategories).Where(s => s.UserId == userID);
            int? i = userID;
            ViewBag.userID = i;
            var user = _context.User.SingleOrDefault(s => s.UserId == userID);

            ViewBag.name = user.FullName;

            ViewBag.remaining = totalRemaining(i);

            return View(await personalFinanceManagerDBContext.ToListAsync());
        }

        public int totalRemaining(int? userID)
        {
            return totalBudget(userID) - totalDeductionsSum(userID);
        }

        public async Task<IActionResult> Budgets(int? userID)
        {
            var personalFinanceManagerDBContext = _context.Categories.Include(c => c.User).Include(i => i.Subcategories).Where(s => s.UserId == userID);

            ViewBag.deductionsSum = new Func<int, int>(remaining);
            ViewBag.totalBuget = new Func<int, int>(catSum);

            ViewBag.totalBuget = new Func<int, int>(catSum);
            ViewBag.userID = userID;
            ViewBag.max = mostExpensiveCategory(userID);
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }

        public Categories mostExpensiveCategory(int? userID)
        {
            var user = _context.User.Include(c => c.Categories).SingleOrDefault(c => c.UserId == userID);
            var max = user.Categories.FirstOrDefault();
            foreach (var i in user.Categories)
            {
                if (catSum(max.CategoryId) < catSum(i.CategoryId))
                {
                    max = i;
                }
            }
            return max;
        }

        public async Task<IActionResult> Transaction(int? userID)
        {
            var personalFinanceManagerDBContext = _context.Subcategories.Include(c => c.Category).Include(i => i.Transactions).Where(s => s.Category.UserId == userID);

            ViewBag.userID = userID;

            ViewBag.Categories = _context.Categories;
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }


        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id, int? userID)
        {
            if (id == null)
            {
                return NotFound();
            }


            ViewBag.userID = userID;

            var categories = await _context.Categories
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.CategoryId == id);

            if (categories == null)
            {
                return NotFound();
            }

            List<Subcategories> subcategories = categories.Subcategories.ToList<Subcategories>();
            ViewBag.subcategories = subcategories;
            ViewBag.length = subcategories.Count();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create(int? id)
        {
            ViewBag.id = id;
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
                return RedirectToAction("Index", "Categories", new { userID = categories.UserId });
            }

            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", categories.UserId);

            ViewBag.emssg = "Input invalid!";
            return View(categories);
            // return RedirectToAction("Index", "Categories", new { userID = categories.UserId });

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
                return RedirectToAction("Index", "Categories", new { userID = categories.UserId });
            }
            ViewBag.emssg = "Input invalid!";
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
            foreach (var i in _context.Subcategories)
            {
                if (i.CategoryId == id)
                {
                    _context.Subcategories.Remove(i);
                }
            }

            var categories = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == id);

            _context.Categories.Remove(categories);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Categories", new { userID = categories.UserId });
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

        private int totalBudget(int? userID)//
        {
            var sum = 0;
            foreach (var subcat in _context.Subcategories.Include(e => e.Category).Where(s => s.Category.UserId == userID))
            {
                if (subcat.Category.UserId == userID)
                {
                    // var cats = user.Categories;
                    sum += subcat.Value;
                }
            }
            return sum;
        }


        public int totalDeductionsSum(int? userID)
        {

            //var cats = _context.Categories.SingleOrDefault(e => e.CategoryId == catID);
            var sum = 0;
            var trans = _context.Transactions.Where(s => s.UserId == userID);

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