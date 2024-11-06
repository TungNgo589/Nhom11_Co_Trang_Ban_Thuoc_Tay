using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;
using QLThuocDAPM.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace QLThuocDAPM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangsController : Controller
    {
        private readonly QlthuocDapm4Context _context;
        private readonly Common.Common _common;

        public DonHangsController(QlthuocDapm4Context context, Common.Common common)
        {
            _context = context;
            _common = common;
        }

        // GET: Admin/DonHangs
        //public async Task<IActionResult> Index()
        //{
        //    var qlthuocDapm2Context = _context.DonHangs.Include(d => d.MaKhuyenMaiNavigation).Include(d => d.MaNguoiDungNavigation);
        //    return View(await qlthuocDapm2Context.ToListAsync());
        //}



        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5; // Number of products per page

            // Calculate the number of products to skip based on the current page
            var qlthuocDapm2Context = _context.DonHangs
                .Include(s => s.MaKhuyenMaiNavigation)
                .Include(s => s.MaNguoiDungNavigation)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // Get the total count of products to calculate the total pages
            int totalProducts = await _context.DonHangs.CountAsync();
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Pass pagination data to the view using ViewBag
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

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

                // Gửi email thông báo
                string subject = "Thông báo giao hàng thành công";
                string content = $"Xin chào {donHang.HoTen},<br><br>Đơn hàng mã {donHang.MaDh} của bạn đã được giao thành công.<br><br>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi!";

                // Gọi phương thức gửi email
                if (Common.Common.SendMail("Nhà Thuốc Long Châu", subject, content, "dongtrieudeptraizodich@gmail.com")) // Email người dùng nằm ở trường `Email`
                {
                    ViewBag.Message = "Thông báo giao hàng đã được gửi qua email của khách hàng.";
                }
                else
                {
                    ViewBag.Error = "Có lỗi xảy ra trong quá trình gửi email thông báo.";
                }

                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách đơn hàng
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











        public ActionResult XuatHoaDonPDF(string maDH)
        {

            var donhang = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSpNavigation)  // Ensure product navigation property is loaded
                .FirstOrDefault(dh => dh.MaDh == maDH /*&& dh.Username == userID*/);

            if (donhang == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy đơn hàng
            }

            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Add Title
                pdfDoc.Add(new Paragraph("Hoa don"));
                pdfDoc.Add(new Paragraph($"Ma Hoa Don: {donhang.MaDh}"));
                pdfDoc.Add(new Paragraph($"Ten Khach Hang: {donhang.HoTen}"));
                pdfDoc.Add(new Paragraph($"So Dien Thoai: {donhang.Sdt}"));
                pdfDoc.Add(new Paragraph($"Dia Chi: {donhang.Diachi}"));
                pdfDoc.Add(new Paragraph($"Trang Thai: {donhang.TrangThai}"));

                // Add table header for order details
                pdfDoc.Add(new Paragraph("Chi Tiet Don Hang"));
                PdfPTable table = new PdfPTable(4);
                table.AddCell("Ten San Pham");
                table.AddCell("So Luong");
                table.AddCell("Gia");
                table.AddCell("Tong");

                // Add each product's details to the table with null checks
                foreach (var item in donhang.ChiTietDonHangs)
                {
                    string tenSp = item.MaSpNavigation?.TenSp ?? "Sản phẩm không tồn tại";
                    string gia = item.MaSpNavigation?.GiaTien.ToString("C") ?? "0 vnđ";
                    string soLuong = item.SoLuong.ToString();
                    string tongTien = (item.MaSpNavigation?.GiaTien * item.SoLuong)?.ToString("C") ?? "0 vnđ";

                    table.AddCell(tenSp);     // Product Name
                    table.AddCell(soLuong);    // Quantity
                    table.AddCell(gia);        // Price
                    table.AddCell(tongTien);   // Total for this item
                }

                pdfDoc.Add(table);
                pdfDoc.Add(new Paragraph($"Tong Gia Tri Don Hang: {donhang.TongTien.ToString("C")}"));

                pdfDoc.Close();

                // Return PDF to user
                byte[] bytes = stream.ToArray();
                return File(bytes, "application/pdf", "HoaDon_" + donhang.MaDh + ".pdf");
            }
        }
        public IActionResult DoanhThu()
        {
            var doanhThu = _context.ChiTietDonHangs
                .Join(_context.DonHangs, cdh => cdh.MaDh, dh => dh.MaDh, (cdh, dh) => new { cdh, dh })
                .Join(_context.SanPhams, combined => combined.cdh.MaSp, sp => sp.MaSp, (combined, sp) => new { combined.cdh, combined.dh, sp })
                .Join(_context.DanhMucs, combined => combined.sp.MaDm, dm => dm.MaDm, (combined, dm) => new
                {
                    DanhMuc = dm.TenDm,
                    DoanhThu = combined.cdh.TongTien
                })
                .GroupBy(x => x.DanhMuc)
                .Select(g => new
                {
                    DanhMuc = g.Key,
                    DoanhThu = g.Sum(x => x.DoanhThu)
                })
                .ToList();

            return View(doanhThu);
        }


    }
}
