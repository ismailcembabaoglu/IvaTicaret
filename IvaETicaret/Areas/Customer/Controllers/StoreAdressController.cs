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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using iTextSharp.text;

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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
            if (User.IsInRole(Diger.Role_Admin))
            {
                var applicationDbContext = _context.StoreAdresses.Include(s => s.District).Include(s => s.Store);
                return View(await applicationDbContext.ToListAsync());
            }
            else if (User.IsInRole(Diger.Role_Bayi))
            {
                var applicationDbContext = _context.StoreAdresses.Include(s => s.District).Include(s => s.Store).Where(c => c.StoreId == storeId);
                return View(await applicationDbContext.ToListAsync());
            }
       
  
            return View();
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
            //ViewData["DistrictId"] = _context.Districts.Take(10).Select(c=>new SelectListItem() { Value=c.Id.ToString(),Text=c.Name});
            ViewData["CityId"] = _context.Cities.Select(c=>new SelectListItem() { Value=c.Id.ToString(),Text=c.Name});
            //ViewData["CountyId"] = new SelectList(_context.Counties,"Id","Name");
                //_context.Counties.Select(c=>new SelectListItem() { Value=c.Id.ToString(),Text=c.Name}).ToList();

            if (User.IsInRole(Diger.Role_Admin))
            {
                ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name");
            }
            else if (User.IsInRole(Diger.Role_Bayi))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var storeId = _context.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
                ViewBag.StoreId =_context.Stores.Where(c => c.Id == storeId).Select(c=>new SelectListItem()
                {
                    Value=c.Id.ToString(),
                    Text=c.CompanyName
                }).ToList();
            }

            return View();
        }
   
        public JsonResult ilcegetir(int p)
        {
            if (p == 79)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Tümü", Value = "0" });
                items.Add(new SelectListItem { Text = "Tümü", Value = "0" });

                return Json(items);
            }
            else
            {
                var ilceler = _context.Counties.Where(c => c.CityId == p).Select(c => new
                {
                    Text = c.Name,
                    Value = c.Id
                }).ToList();
                return Json(ilceler);
            }
      
        }
        public JsonResult mahallegetir(int p)
        {
            if (p==0 || p==-1)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Tümü", Value = "0" });
                return Json(items);
            }
            else
            {
                var mahalleler = _context.Districts.Where(c => c.CountyId == p).Select(c => new
                {
                    Text = c.Name,
                    Value = c.Id
                }).ToList();
                return Json(mahalleler);
            }

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
                if (storeAdress.CityId==79)
                {
                    var lst = _context.Districts.Include(c => c.County).ThenInclude(c => c.City).Where(c => c.County.CityId == 79).ToList();
                    foreach (var item in lst)
                    {
                        StoreAdress adressStore=new StoreAdress();
                        adressStore.CityId = item.County.CityId;
                        adressStore.CountyId = item.CountyId;
                        adressStore.DistrictId = item.Id;
                        adressStore.StoreId=storeAdress.StoreId;
                        _context.Add(adressStore);
                        await _context.SaveChangesAsync();
                       
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.Add(storeAdress);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
         
            }
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
                ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "CompanyName", storeAdress.StoreId);
                ViewData["CityId"] = _context.Cities.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name });
                return View(storeAdress);
            }
            else if (User.IsInRole(Diger.Role_Bayi))
            {
                var storeAdress = await _context.StoreAdresses.FindAsync(id);
                if (storeAdress == null)
                {
                    return NotFound();
                }
                ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "CompanyName", storeAdress.StoreId);
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", storeAdress.StoreId);
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
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", storeAdress.DistrictId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "CompanyName", storeAdress.StoreId);
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
