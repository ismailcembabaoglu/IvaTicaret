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
using Iyzipay.Model.V2.Subscription;

namespace IvaETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Diger.Role_Admin)]
    public class SiteSettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
  
        public SiteSettingController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
         
        }

        // GET: Admin/SiteSetting
        public async Task<IActionResult> Index()
        {
            return _context.SiteSettings != null ?
                        View(await _context.SiteSettings.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.SiteSettings'  is null.");
        }

        // GET: Admin/SiteSetting/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SiteSettings == null)
            {
                return NotFound();
            }

            var siteSetting = await _context.SiteSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteSetting == null)
            {
                return NotFound();
            }

            return View(siteSetting);
        }

        // GET: Admin/SiteSetting/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SiteSetting/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteSetting siteSetting)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string fileName2 = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_he.WebRootPath, @"images\image");
                    var ext = Path.GetExtension(files[0].FileName);
                    if (siteSetting.UserEntryLogo != null)
                    {
                        var imagePath = Path.Combine(_he.WebRootPath, siteSetting.UserEntryLogo.TrimStart('\\'));
                   
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                 
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    siteSetting.UserEntryLogo = @"\images\image\" + fileName + ext;
                    using (var fileStreams2 = new FileStream(Path.Combine(uploads, fileName2 + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams2);
                    }
                    siteSetting.MainLogo = @"\images\image\" + fileName2 + ext;
                }

                _context.Add(siteSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteSetting);
        }

        // GET: Admin/SiteSetting/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SiteSettings == null)
            {
                return NotFound();
            }

            var siteSetting = await _context.SiteSettings.FindAsync(id);
            if (siteSetting == null)
            {
                return NotFound();
            }
            return View(siteSetting);
        }

        // POST: Admin/SiteSetting/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SiteName,UserEntryLogo,MainLogo")] SiteSetting siteSetting)
        {
            if (id != siteSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string fileName2 = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(_he.WebRootPath, @"images\image");
                    var ext = Path.GetExtension(files[0].FileName);
                    if (siteSetting.UserEntryLogo != null)
                    {
                        var imagePath = Path.Combine(_he.WebRootPath, siteSetting.UserEntryLogo.TrimStart('\\'));
                        var imagePath2 = Path.Combine(_he.WebRootPath, siteSetting.MainLogo.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                        if (System.IO.File.Exists(imagePath2))
                        {
                            System.IO.File.Delete(imagePath2);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    siteSetting.UserEntryLogo = @"\images\image\" + fileName + ext;
                    using (var fileStreams2 = new FileStream(Path.Combine(uploads, fileName2 + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams2);
                    }
                    siteSetting.MainLogo = @"\images\image\" + fileName2 + ext;
                }
                try
                {
                    _context.Update(siteSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteSettingExists(siteSetting.Id))
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
            return View(siteSetting);
        }

        // GET: Admin/SiteSetting/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SiteSettings == null)
            {
                return NotFound();
            }

            var siteSetting = await _context.SiteSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteSetting == null)
            {
                return NotFound();
            }

            return View(siteSetting);
        }

        // POST: Admin/SiteSetting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SiteSettings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SiteSettings'  is null.");
            }
            var siteSetting = await _context.SiteSettings.FindAsync(id);
            if (siteSetting != null)
            {
                _context.SiteSettings.Remove(siteSetting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteSettingExists(int id)
        {
            return (_context.SiteSettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
