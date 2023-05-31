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
using Iyzipay.Request.V2.Subscription;

namespace IvaETicaret.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AdressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Adress
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole(Diger.Role_Admin))
            {
                var applicationDbContext = _context.Adress.Include(a => a.ApplicationUser);
                return View(await applicationDbContext.ToListAsync());
            }
            else if (User.IsInRole(Diger.Role_Birey)|| User.IsInRole(Diger.Role_User))
            {
                var applicationDbContext = _context.Adress.Include(a => a.ApplicationUser).Where(c=>c.ApplicationUserId==claim.Value);
                return View(await applicationDbContext.ToListAsync());
            }
            return NotFound();
        }

        // GET: Customer/Adress/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Adress == null)
            {
                return NotFound();
            }

            var adress = await _context.Adress
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adress == null)
            {
                return NotFound();
            }

            return View(adress);
        }

        // GET: Customer/Adress/Create
        public IActionResult Create()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ViewData["CityId"] = _context.Cities.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name });
            if (User.IsInRole(Diger.Role_Admin))
            {
                ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "name");
                return View();
            }
            else if (User.IsInRole(Diger.Role_Birey) || User.IsInRole(Diger.Role_User))
            {
                ViewData["UserId"] = claim.Value;
                return View();
            }
            return NotFound();
        }
        public JsonResult ilcegetir(int p)
        {
            var ilceler = _context.Counties.Where(c => c.CityId == p).Select(c => new
            {
                Text = c.Name,
                Value = c.Id
            }).ToList();
            return Json(ilceler);
        }
        public JsonResult mahallegetir(int p)
        {
            var mahalleler = _context.Districts.Where(c => c.CountyId == p).Select(c => new
            {
                Text = c.Name,
                Value = c.Id
            }).ToList();
            return Json(mahalleler);
        }
        // POST: Customer/Adress/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,AdressTitle,TamAdres,Il,Ilce,Semt,IsActive")] Adress adress)
        {
       
            if (ModelState.IsValid)
            {
                _context.Add(adress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (User.IsInRole(Diger.Role_Admin))
            {
                ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", adress.ApplicationUserId);
            }
            else if (User.IsInRole(Diger.Role_Birey)|| User.IsInRole(Diger.Role_User))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ViewBag.UserId = claim.Value;
            }
         
            return View(adress);
        }

        // GET: Customer/Adress/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Adress == null)
            {
                return NotFound();
            }

        
      
            if (User.IsInRole(Diger.Role_Admin))
            {
                var adress = await _context.Adress.FindAsync(id);
                if (adress == null)
                {
                    return NotFound();
                }
                ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
                return View(adress);
            }
            else if (User.IsInRole(Diger.Role_Birey) || User.IsInRole(Diger.Role_User))
            {
                var adress = await _context.Adress.FindAsync(id);
                if (adress == null)
                {
                    return NotFound();
                }
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ViewBag.UserId = claim.Value;
                return View(adress);
            }
            return NotFound();
        }

        // POST: Customer/Adress/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,AdressTitle,TamAdres,Il,Ilce,Semt,IsActive")] Adress adress)
        {
            if (id != adress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdressExists(adress.Id))
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
            if (User.IsInRole(Diger.Role_Admin))
            {
                ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", adress.ApplicationUserId);
            }
            else if (User.IsInRole(Diger.Role_Birey) || User.IsInRole(Diger.Role_User))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ViewBag.UserId = claim.Value;
            }
            return View(adress);
        }

        // GET: Customer/Adress/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Adress == null)
            {
                return NotFound();
            }

            var adress = await _context.Adress
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adress == null)
            {
                return NotFound();
            }

            return View(adress);
        }

        // POST: Customer/Adress/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Adress == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Adress'  is null.");
            }
            var adress = await _context.Adress.FindAsync(id);
            if (adress != null)
            {
                _context.Adress.Remove(adress);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdressExists(int id)
        {
          return (_context.Adress?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
