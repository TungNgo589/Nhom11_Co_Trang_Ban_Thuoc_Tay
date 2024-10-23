using QLThuocDAPM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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

        // GET: Benh
        public IActionResult Index()
        {
            // Lấy danh sách các bệnh từ database và chuyển vào view
            var listBenh = _context.Benhs.ToList(); // Sử dụng ToList() ngay lập tức
            return View(listBenh);
        }


        // GET: Benh/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            // Tìm bệnh dựa theo id
            var benh = _context.Benhs.Find(id);
            if (benh == null)
            {
                return NotFound();
            }

            return View(benh);  // Trả về view chi tiết của bệnh
        }
    }
}
