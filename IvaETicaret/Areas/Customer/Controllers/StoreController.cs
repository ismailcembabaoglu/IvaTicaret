﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IvaETicaret.Data;
using IvaETicaret.Models;
using System.Security.Claims;
using IvaETicaret.Email;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;

namespace IvaETicaret.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Store
        [Authorize(Roles = Diger.Role_Admin)]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Stores.Include(s => s.Department);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/Store/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Stores == null)
            {
                return NotFound();
            }

            var store = await _context.Stores
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Customer/Store/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Customer/Store/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RootName,CompanyName,TaxNumber,TaxOffice,ContractConfirmation,IsActive,TaxPlate,PhoneNumber,Email,DepartmentId")] Store store)
        {
            if (ModelState.IsValid)
            {

                _context.Add(store);
                SenderEmail.Gonder(
                    "İva Keyiniz",
                    $"İva mağaza Keyiniz='{store.Id}' budur.\n Bu keyiniz ile '{store.CompanyName}' \n Şirketinize Şirket aktivasyon maili geldikten sonra  kullanıcı ekleyebilirsiniz.",
                    store.Email
                       );
                await _context.SaveChangesAsync();
                if (User.IsInRole(Diger.Role_Admin))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index","Home");
                }
               
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", store.DepartmentId);
                return View(store);
        }

        // GET: Customer/Store/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {


            if (User.IsInRole(Diger.Role_Bayi) && !id.HasValue)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
                var store = await _context.Stores.FindAsync(storeId);
                if (store == null)
                {
                    return NotFound();
                }
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", store.DepartmentId);
                return View(store);
            }
            else if (User.IsInRole(Diger.Role_Admin))
            {
                var store = await _context.Stores.FindAsync(id);
                if (store == null)
                {
                    return NotFound();
                }
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", store.DepartmentId);
                return View(store);
            }
            return NotFound();
        }

        // POST: Customer/Store/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RootName,CompanyName,TaxNumber,TaxOffice,ContractConfirmation,IsActive,TaxPlate,PhoneNumber,Email,DepartmentId,StoreIsActive")] Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Home");
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", store.DepartmentId);
            return View(store);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id)
        {
          var store=  _context.Stores.Where(c => c.Id == id).FirstOrDefault();
            if (store.IsActive)
            {
                store.IsActive = false;
                SenderEmail.Gonder(
                   "İva Mağaza İptali",
                   $"İva mağazanız olan '{store.CompanyName}' \n mağzanız iptal edilmiştir.",
                   store.Email
                      );
            }
            else
            {
                store.IsActive = true;
                SenderEmail.Gonder(
                "İva Mağza Onayı",
                $"İva mağazanız olan '{store.CompanyName}' \n mağzanız Onaylanmıştır Göndermiş Olduğumuz '{store.Id}' keyiniz ile mağzanıza \n kullanıcı ekleyebilirsiniz \n iyi günler dileriz.",
                store.Email
                   );
            }
         
            _context.Update(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof( Index));
        }
        // GET: Customer/Store/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Stores == null)
            {
                return NotFound();
            }

            var store = await _context.Stores
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Customer/Store/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Stores == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Stores'  is null.");
            }
            var store = await _context.Stores.FindAsync(id);
            if (store != null)
            {
                _context.Stores.Remove(store);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(Guid id)
        {
            return (_context.Stores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
