using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IvaETicaret.Data;
using IvaETicaret.Models;
using System.Security.Claims;

namespace IvaETicaret.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StoreAdressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreAdressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/StoreAdress
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StoreAdresses.Include(s => s.District).Include(s => s.Store);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/StoreAdress/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StoreAdresses == null)
            {
                return NotFound();
            }

            var storeAdress = await _context.StoreAdresses
                .Include(s => s.District)
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeAdress == null)
            {
                return NotFound();
            }

            return View(storeAdress);
        }

        // GET: Customer/StoreAdress/Create
        public IActionResult Create()
        {
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "name");
            if (User.IsInRole(Diger.Role_Admin))
            {
                ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "name");
            }
            else if (User.IsInRole(Diger.Role_Bayi))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
                ViewData["StoreId"] = new SelectList(_context.Stores.Where(c => c.Id == storeId), "Id", "name");
            }

            return View();
        }

        // POST: Customer/StoreAdress/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StoreId,CityId,CountyId,DistrictId")] StoreAdress storeAdress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeAdress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Id", storeAdress.DistrictId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id", storeAdress.StoreId);
            return View(storeAdress);
        }

        // GET: Customer/StoreAdress/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StoreAdresses == null)
            {
                return NotFound();
            }
            if (User.IsInRole(Diger.Role_Admin))
            {
                var storeAdress = await _context.StoreAdresses.FindAsync(id);
                if (storeAdress == null)
                {
                    return NotFound();
                }
                ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "name", storeAdress.DistrictId);
                ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "name", storeAdress.StoreId);
                return View(storeAdress);
            }
            else if (User.IsInRole(Diger.Role_Bayi))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
                var storeAdress = await _context.StoreAdresses.FindAsync(id);
                if (storeAdress == null)
                {
                    return NotFound();
                }
                ViewData["StoreId"] = new SelectList(_context.Stores.Where(c => c.Id == storeId), "Id", "name", storeAdress.StoreId);
                ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "name", storeAdress.DistrictId);
                return View(storeAdress);
            }
            return View();
        }

        // POST: Customer/StoreAdress/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StoreId,CityId,CountyId,DistrictId")] StoreAdress storeAdress)
        {
            if (id != storeAdress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeAdress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreAdressExists(storeAdress.Id))
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
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Id", storeAdress.DistrictId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id", storeAdress.StoreId);
            return View(storeAdress);
        }

        // GET: Customer/StoreAdress/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StoreAdresses == null)
            {
                return NotFound();
            }

            var storeAdress = await _context.StoreAdresses
                .Include(s => s.District)
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeAdress == null)
            {
                return NotFound();
            }

            return View(storeAdress);
        }

        // POST: Customer/StoreAdress/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StoreAdresses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StoreAdresses'  is null.");
            }
            var storeAdress = await _context.StoreAdresses.FindAsync(id);
            if (storeAdress != null)
            {
                _context.StoreAdresses.Remove(storeAdress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreAdressExists(int id)
        {
            return (_context.StoreAdresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
