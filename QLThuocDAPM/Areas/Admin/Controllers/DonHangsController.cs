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
    public class DonHangsController : Controller
    {
        private readonly QlthuocDapm4Context _context;

        public DonHangsController(QlthuocDapm4Context context)
        {
            _context = context;
        }

        // GET: Admin/DonHangs
        public async Task<IActionResult> Index()
        {
            var qlthuocDapm2Context = _context.DonHangs.Include(d => d.MaKhuyenMaiNavigation).Include(d => d.MaNguoiDungNavigation);
            return View(await qlthuocDapm2Context.ToListAsync());
        }

        // GET: Admin/DonHangs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhuyenMaiNavigation)
                .Include(d => d.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // GET: Admin/DonHangs/Create
        public IActionResult Create()
        {
            ViewData["MaKhuyenMai"] = new SelectList(_context.KhuyenMais, "MaKhuyenMai", "MaKhuyenMai");
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            return View();
        }

        // POST: Admin/DonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDh,Username,Diachi,MaKhuyenMai,TongTien,SoLuong,TrangThai,CreatedAt,UpdatedAt,MaNguoiDung,HoTen,Sdt")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhuyenMai"] = new SelectList(_context.KhuyenMais, "MaKhuyenMai", "MaKhuyenMai", donHang.MaKhuyenMai);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", donHang.MaNguoiDung);
            return View(donHang);
        }

        // GET: Admin/DonHangs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }
            ViewData["MaKhuyenMai"] = new SelectList(_context.KhuyenMais, "MaKhuyenMai", "MaKhuyenMai", donHang.MaKhuyenMai);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", donHang.MaNguoiDung);
            return View(donHang);
        }

        // POST: Admin/DonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDh,Username,Diachi,MaKhuyenMai,TongTien,SoLuong,TrangThai,CreatedAt,UpdatedAt,MaNguoiDung,HoTen,Sdt")] DonHang donHang)
        {
            if (id != donHang.MaDh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonHangExists(donHang.MaDh))
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
            ViewData["MaKhuyenMai"] = new SelectList(_context.KhuyenMais, "MaKhuyenMai", "MaKhuyenMai", donHang.MaKhuyenMai);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", donHang.MaNguoiDung);
            return View(donHang);
        }

        // GET: Admin/DonHangs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhuyenMaiNavigation)
                .Include(d => d.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // POST: Admin/DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                _context.DonHangs.Remove(donHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonHangExists(string id)
        {
            return _context.DonHangs.Any(e => e.MaDh == id);
        }
        [HttpPost]
        public IActionResult UpdateOrderStatus(string id)
        {
            var donHang = _context.DonHangs.FirstOrDefault(d => d.MaDh == id); // Tìm đơn hàng theo ID

            if (donHang != null)
            {
                donHang.TrangThai = "Đã giao"; // Cập nhật trạng thái đơn hàng
                _context.SaveChanges(); // Lưu thay đổi vào DB
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách đơn hàng (hoặc trang phù hợp)
            }

            return NotFound(); // Trả về lỗi nếu không tìm thấy đơn hàng
        }
        [HttpPost]
        public IActionResult XacNhanDon(string id)
        {
            var donHang = _context.DonHangs.FirstOrDefault(d => d.MaDh == id); // Tìm đơn hàng theo ID

            if (donHang != null)
            {
                donHang.TrangThai = "Đã xác nhận đơn hàng sẽ sớm được giao đến bạn"; // Cập nhật trạng thái đơn hàng
                _context.SaveChanges(); // Lưu thay đổi vào DB
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách đơn hàng
            }

            return NotFound(); // Trả về lỗi nếu không tìm thấy đơn hàng
        }

        [HttpPost]
        public IActionResult ChapNhanHuy(string id)
        {
            var donHang = _context.DonHangs.FirstOrDefault(d => d.MaDh == id); // Tìm đơn hàng theo ID

            if (donHang != null)
            {
                donHang.TrangThai = "Hủy đơn hàng thành công"; // Cập nhật trạng thái đơn hàng
                _context.SaveChanges(); // Lưu thay đổi vào DB
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách đơn hàng
            }

            return NotFound(); // Trả về lỗi nếu không tìm thấy đơn hàng
        }

    }
}
