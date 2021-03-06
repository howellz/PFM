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
    public class UsersController : Controller
    {
        private readonly PersonalFinanceManagerDBContext _context;

        public UsersController(PersonalFinanceManagerDBContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(String email, String password)
        {

            var user = await _context.User.SingleOrDefaultAsync(m => m.Email == email && m.Password == password);
            if (user == null)
            {
                ViewBag.errormsg = "User name or password is incorrect!";
                return View();
            }
            return RedirectToAction("Home", "Categories", new { userID = user.UserId });

        }



        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserFirstName,UserLastName,Email,ConfirmEmail,Password,ConfirmPassword")] User user)
        {
            ViewBag.emssg = "Input invalid!";
            if (ModelState.IsValid)
            {
                if (_context.User.SingleOrDefault(m => m.Email == user.Email) == null)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.emssg = "Account already exists!";
                }
            }

            return View(user);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserFirstName,UserLastName,Email,ConfirmEmail,Password,ConfirmPassword")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Home", "Categories", new { userID = user.UserId });
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            foreach (var i in _context.Transactions)
            {
                if (i.UserId == id)
                {
                    _context.Transactions.Remove(i);
                }
            }
            var cat = _context.Categories.Include(c => c.Subcategories);

            foreach (var i in cat)
            {
                if (i.UserId == id)
                {
                    foreach (var k in i.Subcategories)
                    {
                        _context.Subcategories.Remove(k);
                    }

                    _context.Categories.Remove(i);
                }
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.UserId == id);

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
