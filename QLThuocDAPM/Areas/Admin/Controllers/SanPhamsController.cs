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
    public class SanPhamsController : Controller
    {
        private readonly QlthuocDapm3Context _context;

        public SanPhamsController(QlthuocDapm3Context context)
        {
            _context = context;
        }

        // GET: Admin/SanPhams
        public async Task<IActionResult> Index()
        {
            var qlthuocDapm2Context = _context.SanPhams.Include(s => s.MaBenhNavigation).Include(s => s.MaDmNavigation).Include(s => s.MaGiamGiaNavigation).Include(s => s.MaNhaCungCapNavigation);
            return View(await qlthuocDapm2Context.ToListAsync());
        }

        // GET: Admin/SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaBenhNavigation)
                .Include(s => s.MaDmNavigation)
                .Include(s => s.MaGiamGiaNavigation)
                .Include(s => s.MaNhaCungCapNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaBenh"] = new SelectList(_context.Benhs, "MaBenh", "TenBenh"); // Thay đổi "MaBenh" thành "TenBenh" nếu cần
            ViewData["MaDm"] = new SelectList(_context.DanhMucs, "MaDm", "TenDm"); // Thay đổi "MaDm" thành "TenDm" nếu cần
            ViewData["MaGiamGia"] = new SelectList(_context.GiamGia, "MaGiamGia", "TenGiamGia"); // Thay đổi nếu cần
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap"); // Thay đổi nếu cần
            return View();
        }

        // POST: Admin/SanPhams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,TenSp,MaBenh,MaNhaCungCap,MaGiamGia,ThanhPhan,GiaTien,DonVi,HansuDung,ChitietSp,MaDm,SoLuong,SoLuongMua,HinhAnh1,HinhAnh2,HinhAnh3,HinhAnh4")] SanPham sanPham)
        {
         
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            
           
            return View(sanPham);
        }
        // GET: Admin/SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["MaBenh"] = new SelectList(_context.Benhs, "MaBenh", "MaBenh", sanPham.MaBenh);
            ViewData["MaDm"] = new SelectList(_context.DanhMucs, "MaDm", "MaDm", sanPham.MaDm);
            ViewData["MaGiamGia"] = new SelectList(_context.GiamGia, "MaGiamGia", "MaGiamGia", sanPham.MaGiamGia);
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSp,TenSp,MaBenh,MaNhaCungCap,MaGiamGia,ThanhPhan,GiaTien,DonVi,HansuDung,ChitietSp,MaDm,SoLuong,SoLuongMua,HinhAnh1,HinhAnh2,HinhAnh3,HinhAnh4")] SanPham sanPham)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
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
            ViewData["MaBenh"] = new SelectList(_context.Benhs, "MaBenh", "MaBenh", sanPham.MaBenh);
            ViewData["MaDm"] = new SelectList(_context.DanhMucs, "MaDm", "MaDm", sanPham.MaDm);
            ViewData["MaGiamGia"] = new SelectList(_context.GiamGia, "MaGiamGia", "MaGiamGia", sanPham.MaGiamGia);
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaBenhNavigation)
                .Include(s => s.MaDmNavigation)
                .Include(s => s.MaGiamGiaNavigation)
                .Include(s => s.MaNhaCungCapNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}
