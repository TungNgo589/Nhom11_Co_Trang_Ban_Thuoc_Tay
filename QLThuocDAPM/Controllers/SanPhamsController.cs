using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;

namespace QLThuocDAPM.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly QlthuocDapmContext _context;

        public SanPhamsController(QlthuocDapmContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> CusSanPham()
        {
            // Giả sử bạn có một phương thức để lấy danh sách sản phẩm
            var products = _context.SanPhams.Include(s => s.MaBenhNavigation).Include(s => s.MaDmNavigation).Include(s => s.MaGiamGiaNavigation).Include(s => s.MaNhaCungCapNavigation);
            return View(products);
        }

        public async Task<IActionResult> CusChiTietSanPham(int id)
        {
            // Tìm sản phẩm dựa trên ID (MaSp)
            var product = await _context.SanPhams
                                        .Include(s => s.MaBenhNavigation)
                                        .Include(s => s.MaDmNavigation)
                                        .Include(s => s.MaGiamGiaNavigation)
                                        .Include(s => s.MaNhaCungCapNavigation)
                                        .FirstOrDefaultAsync(s => s.MaSp == id);

            // Kiểm tra nếu không tìm thấy sản phẩm
            if (product == null)
            {
                return NotFound();
            }

            // Trả về View cùng với chi tiết sản phẩm
            return View(product);
        }

        // GET: SanPhams
        public async Task<IActionResult> Index()
        {
            var qlthuocDapmContext = _context.SanPhams.Include(s => s.MaBenhNavigation).Include(s => s.MaDmNavigation).Include(s => s.MaGiamGiaNavigation).Include(s => s.MaNhaCungCapNavigation);
            return View(await qlthuocDapmContext.ToListAsync());
        }

        // GET: SanPhams/Details/5
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

        // GET: SanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaBenh"] = new SelectList(_context.Benhs, "MaBenh", "MaBenh");
            ViewData["MaDm"] = new SelectList(_context.DanhMucs, "MaDm", "MaDm");
            ViewData["MaGiamGia"] = new SelectList(_context.GiamGia, "MaGiamGia", "MaGiamGia");
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,TenSp,MaBenh,MaNhaCungCap,MaGiamGia,ThanhPhan,GiaTien,DonVi,HansuDung,ChitietSp,MaDm,SoLuong,HinhAnh1,HinhAnh2,HinhAnh3,HinhAnh4")] SanPham sanPham,  IFormFile HinhAnh1)
        {
            //if (ModelState.IsValid)
            //{

            if (HinhAnh1 != null && HinhAnh1.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadFile", HinhAnh1.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await HinhAnh1.CopyToAsync(stream);
                }
                sanPham.HinhAnh1 = HinhAnh1.FileName; // Lưu tên file vào cơ sở dữ liệu
            }
            _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            //ViewData["MaBenh"] = new SelectList(_context.Benhs, "MaBenh", "MaBenh", sanPham.MaBenh);
            //ViewData["MaDm"] = new SelectList(_context.DanhMucs, "MaDm", "MaDm", sanPham.MaDm);
            //ViewData["MaGiamGia"] = new SelectList(_context.GiamGia, "MaGiamGia", "MaGiamGia", sanPham.MaGiamGia);
            //ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);

            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
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

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSp,TenSp,MaBenh,MaNhaCungCap,MaGiamGia,ThanhPhan,GiaTien,DonVi,HansuDung,ChitietSp,MaDm,SoLuong,HinhAnh1,HinhAnh2,HinhAnh3,HinhAnh4")] SanPham sanPham)
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

        // GET: SanPhams/Delete/5
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

        // POST: SanPhams/Delete/5
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
