using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;
using QLThuocDAPM.Models; // Đảm bảo bạn đã nhập namespace của mô hình KhuyenMai
using System.Threading.Tasks;

namespace QLThuocDAPM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhuyenMaisController : Controller
    {
        private readonly QlthuocDapm4Context db;

        public KhuyenMaisController(QlthuocDapm4Context context)
        {
            db = context;
        }

        // GET: KhuyenMais
        public async Task<IActionResult> Index()
        {
            return View(await db.KhuyenMais.ToListAsync());
        }

        // GET: Admin/KhuyenMais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var khuyenMai = await db.KhuyenMais.FindAsync(id);
            if (khuyenMai == null)
            {
                return NotFound();
            }
            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMais/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhuyenMais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhuyenMai,GiaTri,ThoiGianBatDau,ThoiGianKetThuc,TrangThai,NgayTao,DieuKienApDung,SoLuong")] KhuyenMai khuyenMai)
        {
            if (ModelState.IsValid)
            {
            
                await db.KhuyenMais.AddAsync(khuyenMai);
                await db.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

                if (khuyenMai.ThoiGianKetThuc > DateTime.Now)
                {
                    khuyenMai.TrangThai = false;
                    await db.KhuyenMais.AddAsync(khuyenMai);
                    await db.SaveChangesAsync();
                }                    
                        
            }

            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var khuyenMai = await db.KhuyenMais.FindAsync(id);
            if (khuyenMai == null)
            {
                return NotFound();
            }
            return View(khuyenMai);
        }

        // POST: Admin/KhuyenMais/Edit/5
    }
}
