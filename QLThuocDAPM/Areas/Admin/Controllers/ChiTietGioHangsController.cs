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
    public class ChiTietGioHangsController : Controller
    {
        private readonly QlthuocDapm3Context _context;

        public ChiTietGioHangsController(QlthuocDapm3Context context)
        {
            _context = context;
        }

        // GET: Admin/ChiTietGioHangs
        public async Task<IActionResult> Index()
        {
            var qlthuocDapm3Context = _context.ChiTietGioHangs.Include(c => c.MaGhNavigation).Include(c => c.MaSpNavigation);
            return View(await qlthuocDapm3Context.ToListAsync());
        }

        // GET: Admin/ChiTietGioHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietGioHang = await _context.ChiTietGioHangs
                .Include(c => c.MaGhNavigation)
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTietGioHang == null)
            {
                return NotFound();
            }

            return View(chiTietGioHang);
        }

        // GET: Admin/ChiTietGioHangs/Create
        public IActionResult Create()
        {
            ViewData["MaGh"] = new SelectList(_context.GioHangs, "MaGh", "MaGh");
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp");
            return View();
        }

        // POST: Admin/ChiTietGioHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaGh,SoLuongSp,MaSp,TongTien")] ChiTietGioHang chiTietGioHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chiTietGioHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaGh"] = new SelectList(_context.GioHangs, "MaGh", "MaGh", chiTietGioHang.MaGh);
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", chiTietGioHang.MaSp);
            return View(chiTietGioHang);
        }

        // GET: Admin/ChiTietGioHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietGioHang = await _context.ChiTietGioHangs.FindAsync(id);
            if (chiTietGioHang == null)
            {
                return NotFound();
            }
            ViewData["MaGh"] = new SelectList(_context.GioHangs, "MaGh", "MaGh", chiTietGioHang.MaGh);
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", chiTietGioHang.MaSp);
            return View(chiTietGioHang);
        }

        // POST: Admin/ChiTietGioHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaGh,SoLuongSp,MaSp,TongTien")] ChiTietGioHang chiTietGioHang)
        {
            if (id != chiTietGioHang.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietGioHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietGioHangExists(chiTietGioHang.Id))
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
            ViewData["MaGh"] = new SelectList(_context.GioHangs, "MaGh", "MaGh", chiTietGioHang.MaGh);
            ViewData["MaSp"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", chiTietGioHang.MaSp);
            return View(chiTietGioHang);
        }

        // GET: Admin/ChiTietGioHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietGioHang = await _context.ChiTietGioHangs
                .Include(c => c.MaGhNavigation)
                .Include(c => c.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTietGioHang == null)
            {
                return NotFound();
            }

            return View(chiTietGioHang);
        }

        // POST: Admin/ChiTietGioHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiTietGioHang = await _context.ChiTietGioHangs.FindAsync(id);
            if (chiTietGioHang != null)
            {
                _context.ChiTietGioHangs.Remove(chiTietGioHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietGioHangExists(int id)
        {
            return _context.ChiTietGioHangs.Any(e => e.Id == id);
        }
    }
}
