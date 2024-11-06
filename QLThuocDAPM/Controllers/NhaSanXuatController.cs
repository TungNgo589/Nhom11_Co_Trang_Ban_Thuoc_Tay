using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;

namespace QLThuocDAPM.Controllers
{
    public class NhaSanXuatController : Controller
    {
        private readonly QlthuocDapm4Context db;

        public NhaSanXuatController(QlthuocDapm4Context context)
        {
            db = context;
        }
        public IActionResult Index(int id)
        {
            // Lấy thông tin nhà cung cấp theo mã
            var nhaCungCap = db.NhaCungCaps.FirstOrDefault(ncc => ncc.MaNhaCungCap == id);

            // Kiểm tra xem nhà cung cấp có tồn tại hay không
            if (nhaCungCap == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy
            }

            return View(nhaCungCap); // Trả về view với thông tin nhà cung cấp
        }
    }
}
