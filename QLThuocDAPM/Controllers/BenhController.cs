using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;

namespace QLThuocDAPM.Controllers
{
    public class BenhController : Controller
    {
        private readonly QlthuocDapm3Context _context;

        public BenhController(QlthuocDapm3Context context)
        {
            _context = context;
        }
        // GET: BenhController
        public IActionResult ThuocLienQuan(string tenBenh)
        {
            if (string.IsNullOrEmpty(tenBenh))
            {
                return BadRequest(); // Trả về mã trạng thái 400 nếu tên bệnh rỗng
            }

            // Tìm bệnh dựa theo tên bệnh
            var benh = _context.Benhs
                .Include(b => b.SanPhams)
                .FirstOrDefault(b => b.TenBenh.ToLower() == tenBenh.ToLower()); // So sánh không phân biệt chữ hoa chữ thường

            if (benh == null)
            {
                return NotFound(); // Trả về mã trạng thái 404 nếu không tìm thấy bệnh
            }

            // Lấy danh sách sản phẩm khác cùng loại bệnh
            var danhSachSanPhamKhac = _context.SanPhams
                .Where(sp => sp.MaBenh == benh.MaBenh && sp.MaSp != null)
                .ToList();

            ViewBag.DanhSachSanPhamKhac = danhSachSanPhamKhac;

            return View(benh); // Trả về view với đối tượng bệnh
        }

    }
}
