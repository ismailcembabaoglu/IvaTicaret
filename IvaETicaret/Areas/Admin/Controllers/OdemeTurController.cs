using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IvaETicaret.Data;
using IvaETicaret.Models;

namespace IvaETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OdemeTurController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OdemeTurController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/OdemeTur
        public async Task<IActionResult> Index()
        {
              return _context.OdemeTurleri != null ? 
                          View(await _context.OdemeTurleri.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.OdemeTurleri'  is null.");
        }

        // GET: Admin/OdemeTur/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OdemeTurleri == null)
            {
                return NotFound();
            }

            var odemeTur = await _context.OdemeTurleri
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odemeTur == null)
            {
                return NotFound();
            }

            return View(odemeTur);
        }

        // GET: Admin/OdemeTur/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/OdemeTur/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( OdemeTur odemeTur)
        {
            if (ModelState.IsValid)
            {
                _context.Add(odemeTur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(odemeTur);
        }

        // GET: Admin/OdemeTur/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OdemeTurleri == null)
            {
                return NotFound();
            }

            var odemeTur = await _context.OdemeTurleri.FindAsync(id);
            if (odemeTur == null)
            {
                return NotFound();
            }
            return View(odemeTur);
        }

        // POST: Admin/OdemeTur/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] OdemeTur odemeTur)
        {
            if (id != odemeTur.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(odemeTur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OdemeTurExists(odemeTur.Id))
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
            return View(odemeTur);
        }

        // GET: Admin/OdemeTur/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OdemeTurleri == null)
            {
                return NotFound();
            }

            var odemeTur = await _context.OdemeTurleri
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odemeTur == null)
            {
                return NotFound();
            }

            return View(odemeTur);
        }

        // POST: Admin/OdemeTur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OdemeTurleri == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OdemeTurleri'  is null.");
            }
            var odemeTur = await _context.OdemeTurleri.FindAsync(id);
            if (odemeTur != null)
            {
                _context.OdemeTurleri.Remove(odemeTur);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OdemeTurExists(int id)
        {
          return (_context.OdemeTurleri?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
