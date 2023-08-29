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
using IvaETicaret.Email;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Iyzipay.Model.V2.Subscription;

namespace IvaETicaret.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public StoreController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(c=>c.Id!=1), "Id", "Name");
            return View();
        }

        // POST: Customer/Store/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Store store)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_he.WebRootPath, @"images\store");
                    var ext = Path.GetExtension(files[0].FileName);
                    if (store.Image != null)
                    {
                        var imagePath = Path.Combine(_he.WebRootPath, store.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    store.Image = @"\images\store\" + fileName + ext;
                }
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
                ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(c=>c.Id!=1), "Id", "Name", store.DepartmentId);
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
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_he.WebRootPath, @"images\store");
                    var ext = Path.GetExtension(files[0].FileName);
                    if (store.Image != null)
                    {
                        var imagePath = Path.Combine(_he.WebRootPath, store.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    store.Image = @"\images\store\" + fileName + ext;
                }
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments,"Id", "Name", store.DepartmentId);
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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
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
