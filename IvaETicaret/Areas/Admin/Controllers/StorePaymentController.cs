using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IvaETicaret.Data;
using IvaETicaret.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IvaETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StorePaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StorePaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/StorePayment
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole(Diger.Role_Admin))
            {
                var applicationDbContext = _context.StorePayments.Include(s => s.Store);
                return View(await applicationDbContext.ToListAsync());
            }
            else if(User.IsInRole(Diger.Role_Bayi))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
                var applicationDbContext = _context.StorePayments.Include(s => s.Store).Where(c=>c.StoreId==storeId);
                return View(await applicationDbContext.ToListAsync());
            }
        
            return NotFound();
        }

        // GET: Admin/StorePayment/Details/5
        [Authorize(Roles = Diger.Role_Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StorePayments == null)
            {
                return NotFound();
            }

            var storePayment = await _context.StorePayments
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storePayment == null)
            {
                return NotFound();
            }

            return View(storePayment);
        }

        // GET: Admin/StorePayment/Create
        [Authorize(Roles =Diger.Role_Admin)]
        public IActionResult Create()
        {
            ViewData["StoreId"] = _context.Stores.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            return View();
        }

        // POST: Admin/StorePayment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Diger.Role_Admin)]
        public async Task<IActionResult> Create([Bind("Id,StoreId,Month,PricePayable")] StorePayment storePayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storePayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "CompanyName", storePayment.StoreId);
            return View(storePayment);
        }

        // GET: Admin/StorePayment/Edit/5
        [Authorize(Roles = Diger.Role_Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StorePayments == null)
            {
                return NotFound();
            }

            var storePayment = await _context.StorePayments.FindAsync(id);
            if (storePayment == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "CompanyName", storePayment.StoreId);
            return View(storePayment);
        }

        // POST: Admin/StorePayment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Diger.Role_Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StoreId,Month,PricePayable")] StorePayment storePayment)
        {
            if (id != storePayment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storePayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorePaymentExists(storePayment.Id))
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
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "CompanyName", storePayment.StoreId);
            return View(storePayment);
        }

        // GET: Admin/StorePayment/Delete/5
        [Authorize(Roles = Diger.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StorePayments == null)
            {
                return NotFound();
            }

            var storePayment = await _context.StorePayments
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storePayment == null)
            {
                return NotFound();
            }

            return View(storePayment);
        }

        // POST: Admin/StorePayment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Diger.Role_Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StorePayments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StorePayments'  is null.");
            }
            var storePayment = await _context.StorePayments.FindAsync(id);
            if (storePayment != null)
            {
                _context.StorePayments.Remove(storePayment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StorePaymentExists(int id)
        {
          return (_context.StorePayments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
