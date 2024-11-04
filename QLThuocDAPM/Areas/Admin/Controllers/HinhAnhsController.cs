using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;

namespace QLThuocDAPM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HinhAnhsController : Controller
    {
        private readonly QlthuocDapm4Context _context;

        public HinhAnhsController(QlthuocDapm4Context context)
        {
            _context = context;
        }

        // GET: Admin/HinhAnhs
        public async Task<IActionResult> Index()
        {
            var qlthuocDapm3Context = _context.HinhAnhs.Include(h => h.MaSpNavigation);
            return View(await qlthuocDapm3Context.ToListAsync());
        }

        // GET: Admin/HinhAnhs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhAnh = await _context.HinhAnhs
                .Include(h => h.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hinhAnh == null)
            {
                return NotFound();
            }

            return View(hinhAnh);
        }

        // GET: Admin/HinhAnhs/Create
        public IActionResult Create()
        {
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp");
            return View();
        }

        // POST: Admin/HinhAnhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UrlHinh,MaSp")] HinhAnh hinhAnh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hinhAnh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", hinhAnh.MaSp);
            return View(hinhAnh);
        }

        // GET: Admin/HinhAnhs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhAnh = await _context.HinhAnhs.FindAsync(id);
            if (hinhAnh == null)
            {
                return NotFound();
            }
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", hinhAnh.MaSp);
            return View(hinhAnh);
        }

        // POST: Admin/HinhAnhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UrlHinh,MaSp")] HinhAnh hinhAnh)
        {
            if (id != hinhAnh.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hinhAnh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HinhAnhExists(hinhAnh.Id))
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
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", hinhAnh.MaSp);
            return View(hinhAnh);
        }

        // GET: Admin/HinhAnhs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhAnh = await _context.HinhAnhs
                .Include(h => h.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hinhAnh == null)
            {
                return NotFound();
            }

            return View(hinhAnh);
        }

        // POST: Admin/HinhAnhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hinhAnh = await _context.HinhAnhs.FindAsync(id);
            if (hinhAnh != null)
            {
                _context.HinhAnhs.Remove(hinhAnh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HinhAnhExists(int id)
        {
            return _context.HinhAnhs.Any(e => e.Id == id);
        }
    }
}
