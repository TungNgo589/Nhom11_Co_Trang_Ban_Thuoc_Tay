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
    public class DanhGiumsController : Controller
    {
        private readonly QlthuocDapm4Context _context;

        public DanhGiumsController(QlthuocDapm4Context context)
        {
            _context = context;
        }

        // GET: Admin/DanhGiums
        public async Task<IActionResult> Index()
        {
            var qlthuocDapm2Context = _context.DanhGia.Include(d => d.MaNguoiDungNavigation).Include(d => d.MaSanPhamNavigation);
            return View(await qlthuocDapm2Context.ToListAsync());
        }

        // GET: Admin/DanhGiums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhGia == id);
            if (danhGium == null)
            {
                return NotFound();
            }

            return View(danhGium);
        }

        // GET: Admin/DanhGiums/Create
        public IActionResult Create()
        {
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSp", "MaSp");
            return View();
        }

        // POST: Admin/DanhGiums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDanhGia,MaSanPham,MaNguoiDung,SoSao,SoSaoTrungBinh,NgayBinhLuan")] DanhGium danhGium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhGium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhGium.MaNguoiDung);
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", danhGium.MaSanPham);
            return View(danhGium);
        }

        // GET: Admin/DanhGiums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium == null)
            {
                return NotFound();
            }
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhGium.MaNguoiDung);
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", danhGium.MaSanPham);
            return View(danhGium);
        }

        // POST: Admin/DanhGiums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDanhGia,MaSanPham,MaNguoiDung,SoSao,SoSaoTrungBinh,NgayBinhLuan")] DanhGium danhGium)
        {
            if (id != danhGium.MaDanhGia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhGium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhGiumExists(danhGium.MaDanhGia))
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
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhGium.MaNguoiDung);
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", danhGium.MaSanPham);
            return View(danhGium);
        }

        // GET: Admin/DanhGiums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhGia == id);
            if (danhGium == null)
            {
                return NotFound();
            }

            return View(danhGium);
        }

        // POST: Admin/DanhGiums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium != null)
            {
                _context.DanhGia.Remove(danhGium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhGiumExists(int id)
        {
            return _context.DanhGia.Any(e => e.MaDanhGia == id);
        }
    }
}
