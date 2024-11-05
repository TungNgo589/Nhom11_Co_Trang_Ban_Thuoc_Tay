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
        private readonly QlthuocDapm4Context _context;

        public SanPhamsController(QlthuocDapm4Context context)
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
            ViewData["MaBenh"] = new SelectList(_context.Benhs, "MaBenh", "MaBenh");
            ViewData["MaDm"] = new SelectList(_context.DanhMucs, "MaDm", "MaDm");
            ViewData["MaGiamGia"] = new SelectList(_context.GiamGia, "MaGiamGia", "MaGiamGia");
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap");
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,TenSp,MaBenh,MaNhaCungCap,MaGiamGia,SoBinhLuan,ThanhPhan,Congdung,Cachdung,Doituongsudung,Tacdungphu,GiaTien,DonVi,Ngaysanxuat,Noisanxuat,HansuDung,ChitietSp,MaDm,SoLuong,SoLuongMua,HinhAnh1,HinhAnh2,HinhAnh3,HinhAnh4")]
        SanPham sanPham, IFormFile file1, IFormFile file2, IFormFile file3, IFormFile file4)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem file có được upload không
                if ((file1 != null && file1.Length > 0) && (file2 != null && file2.Length > 0) && (file3 != null && file4.Length > 0) && (file4 != null && file4.Length > 0))
                {
                    // Tạo tên file duy nhất để tránh xung đột
                    var fileName1 = Path.GetFileName(file1.FileName);
                    var fileName2 = Path.GetFileName(file2.FileName);
                    var fileName3 = Path.GetFileName(file3.FileName);
                    var fileName4 = Path.GetFileName(file4.FileName);
                    var filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName1);
                    var filePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName2);
                    var filePath3 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName3);
                    var filePath4 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName4);

                    // Lưu file vào thư mục wwwroot/images
                    using (var stream = new FileStream(filePath1, FileMode.Create))
                    {
                        await file1.CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(filePath2, FileMode.Create))
                    {
                        await file2.CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(filePath3, FileMode.Create))
                    {
                        await file3.CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(filePath4, FileMode.Create))
                    {
                        await file4.CopyToAsync(stream);
                    }

                    // Gán đường dẫn của ảnh cho thuộc tính AnhSp của sản phẩm
                    sanPham.HinhAnh1 = "/images/" + fileName1; // Đảm bảo đường dẫn hợp lệ
                    sanPham.HinhAnh2 = "/images/" + fileName2; // Đảm bảo đường dẫn hợp lệ
                    sanPham.HinhAnh3 = "/images/" + fileName3; // Đảm bảo đường dẫn hợp lệ
                    sanPham.HinhAnh4 = "/images/" + fileName4; // Đảm bảo đường dẫn hợp lệ
                    _context.Add(sanPham);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["MaBenh"] = new SelectList(_context.Benhs, "MaBenh", "MaBenh", sanPham.MaBenh);
                ViewData["MaDm"] = new SelectList(_context.DanhMucs, "MaDm", "MaDm", sanPham.MaDm);
                ViewData["MaGiamGia"] = new SelectList(_context.GiamGia, "MaGiamGia", "MaGiamGia", sanPham.MaGiamGia);
                ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);
                return View(sanPham);
            }
            return View();
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
