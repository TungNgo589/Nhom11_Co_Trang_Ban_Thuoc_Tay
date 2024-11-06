using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;

namespace QLThuocDAPM.Controllers
{
    public class BenhController : Controller
    {
        private readonly QlthuocDapm4Context _context;

        public BenhController(QlthuocDapm4Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách các bệnh từ database và chuyển vào view
            var listBenh = _context.Benhs.ToList(); // Sử dụng bảng mới `Benh`
            return View(listBenh);
        }

        // GET: Benh/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            // Tìm bệnh dựa theo id trong bảng `Benh`
            var benh = _context.Benhs.Find(id);
            if (benh == null)
            {
                return NotFound();
            }

            return View(benh);  // Trả về view chi tiết của bệnh
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
