﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PFM.Models;

namespace PFM.Controllers
{
    public class SubcategoriesController : Controller
    {
        private readonly PersonalFinanceManagerDBContext _context;

        public SubcategoriesController(PersonalFinanceManagerDBContext context)
        {
            _context = context;
        }

        // GET: Subcategories
        public async Task<IActionResult> Index()
        {
            var personalFinanceManagerDBContext = _context.Subcategories.Include(s => s.Category);
            ViewBag.Categories = _context.Categories;
            return View(await personalFinanceManagerDBContext.ToListAsync());
        }

        // GET: Subcategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = await _context.Subcategories
                .Include(s => s.Category)
                .Include(i => i.Transactions)
                .SingleOrDefaultAsync(m => m.SubcategoryId == id);
            ViewData["ds"] = deductionsSum(id);
            if (subcategories == null)
            {
                return NotFound();
            }

            return View(subcategories);
        }

        // GET: Subcategories/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Subcategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubcategoryId,CategoryId,Value,SubcategoryName")] Subcategories subcategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subcategories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", subcategories.CategoryId);
            return View(subcategories);
        }

        // GET: Subcategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = await _context.Subcategories.SingleOrDefaultAsync(m => m.SubcategoryId == id);
            if (subcategories == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", subcategories.CategoryId);
            return View(subcategories);
        }

        // POST: Subcategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubcategoryId,CategoryId,Value,SubcategoryName")] Subcategories subcategories)
        {
            if (id != subcategories.SubcategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subcategories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubcategoriesExists(subcategories.SubcategoryId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", subcategories.CategoryId);
            return View(subcategories);
        }

        // GET: Subcategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = await _context.Subcategories
                .Include(s => s.Category)
                .SingleOrDefaultAsync(m => m.SubcategoryId == id);
            if (subcategories == null)
            {
                return NotFound();
            }

            return View(subcategories);
        }

        // POST: Subcategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcategories = await _context.Subcategories.SingleOrDefaultAsync(m => m.SubcategoryId == id);
            _context.Subcategories.Remove(subcategories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubcategoriesExists(int id)
        {
            return _context.Subcategories.Any(e => e.SubcategoryId == id);
        }


        public int deductionsSum(int? subcatID)//int catID
        {

            //var cats = _context.Categories.SingleOrDefault(e => e.CategoryId == catID);
            var sum = 0;

            var subcats = _context.Subcategories.SingleOrDefault(e => e.SubcategoryId == subcatID);
            
                foreach (var j in subcats.Transactions)
                {
                    sum += j.Value;
                }
            
            return sum;
        }

        public int totalDeductionsSum()//int catID
        {

            //var cats = _context.Categories.SingleOrDefault(e => e.CategoryId == catID);
            var sum = 0;

            var subcats = _context.Subcategories.Include(s => s.Category).Include(s => s.Transactions);
            
            foreach (var i in subcats)
            {
                    foreach (var j in i.Transactions)
                    {
                        sum += j.Value;
                    }
            }
            return sum;
        }

    }
}
