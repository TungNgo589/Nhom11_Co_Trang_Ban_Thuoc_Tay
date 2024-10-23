using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using QLThuocDAPM.Data;
namespace QLThuocDAPM.Controllers
{
    public class SanPhamController : Controller
    {

        private readonly QlthuocDapm3Context db;

        public SanPhamController(QlthuocDapm3Context context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var qlthuocDapm2Context = db.SanPhams.Include(s => s.MaBenhNavigation).Include(s => s.MaDmNavigation).Include(s => s.MaGiamGiaNavigation).Include(s => s.MaNhaCungCapNavigation);
            return View(await qlthuocDapm2Context.ToListAsync());
        }

        public IActionResult ChiTietSanPham(int id)
        {
            var data = db.SanPhams
                .Include(p => p.MaDmNavigation) // Bao gồm thông tin danh mục nếu cần
                        .Include(p => p.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                                                .Include(p => p.MaBenhNavigation) // Bao gồm thông tin nhà cung cấp

                .SingleOrDefault(p => p.MaSp == id);

            var benhs = db.Benhs.ToList(); // Lấy danh sách các bệnh từ cơ sở dữ liệu
            ViewBag.Benhs = benhs;

            // Tính số sao trung bình
            var soSaoTrungBinh = db.DanhGia
                .Where(dg => dg.MaSanPham == id)
                .Average(dg => dg.SoSao) ?? 0; // Nếu không có đánh giá, trả về 0

            ViewBag.SoSaoTrungBinh = soSaoTrungBinh;
            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }
           

            ViewBag.BinhLuans = db.BinhLuans
                    // Lấy tên nhà cung cấp

       .Where(bl => bl.MaSp == id)
       .Include(bl => bl.MaNguoiDungNavigation) // Nếu bạn muốn hiển thị tên người dùng
       .ToList();
            return View("ChiTietSanPham", data); // Gọi rõ ràng View "ChiTietSanPham"
        }

        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var listGame = db.SanPhams.Include("HinhAnhs").AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                listGame = listGame.Where(b => b.TenSp.ToLower().Contains(keyword));
            }

            return View(listGame.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int maSP, string noiDung)
        {
            if (HttpContext.Session.GetString("userLogin") == null)
            {
                return RedirectToAction("Login", "User");
            }

            string username = HttpContext.Session.GetString("userLogin");
            var nguoiDung = await db.NguoiDungs.FirstOrDefaultAsync(nd => nd.Username == username);

            if (nguoiDung == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            var binhLuan = new BinhLuan
            {
                MaSp = maSP,
                MaNguoiDung = nguoiDung.MaNguoiDung,
                NoiDung = noiDung,
                NgayBinhLuan = DateTime.Now
            };

            try
            {
                await db.BinhLuans.AddAsync(binhLuan);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error message here (ex.Message)
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi thêm bình luận. Vui lòng thử lại.");
                return RedirectToAction("ChiTietSanPham", new { id = maSP });
            }

            return RedirectToAction("ChiTietSanPham", new { id = maSP });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(int maSP, decimal rating)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (HttpContext.Session.GetString("userLogin") == null)
            {
                return RedirectToAction("Login", "User");
            }

            string username = HttpContext.Session.GetString("userLogin");
            var nguoiDung = await db.NguoiDungs.FirstOrDefaultAsync(nd => nd.Username == username);

            if (nguoiDung == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            // Kiểm tra xem người dùng đã mua sản phẩm chưa
            var daMuaHang = await db.ChiTietDonHangs.AnyAsync(ct => ct.MaSp == maSP && ct.MaDhNavigation.MaNguoiDung == nguoiDung.MaNguoiDung);

            // Kiểm tra xem người dùng đã đánh giá chưa
            var daDanhGia = await db.DanhGia.AnyAsync(dg => dg.MaNguoiDung == nguoiDung.MaNguoiDung && dg.MaSanPham == maSP);

            // Kiểm tra nếu người dùng chưa mua hàng
            if (!daMuaHang)
            {
                TempData["ErrorMessage"] = "Bạn chưa mua hàng, không thể đánh giá.";
                return RedirectToAction("ChiTietSanPham", new { id = maSP });
            }

            // Kiểm tra nếu người dùng đã đánh giá
            if (daDanhGia)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("ChiTietSanPham", new { id = maSP });
            }

            // Tạo mới đối tượng đánh giá
            DanhGium danhGia = new DanhGium
            {
                MaSanPham = maSP,
                MaNguoiDung = nguoiDung.MaNguoiDung,
                SoSao = rating, // Lưu số sao vào thuộc tính SoSao
                NgayBinhLuan = DateTime.Now
            };

            // Thêm đánh giá vào cơ sở dữ liệu
            await db.DanhGia.AddAsync(danhGia);
            await db.SaveChangesAsync();

            // Cập nhật đánh giá trung bình
            await UpdateAverageRating(maSP);

            return RedirectToAction("ChiTietSanPham", new { id = maSP });
        }

        private async Task UpdateAverageRating(int maSP)
        {
            // Tính toán đánh giá trung bình
            var averageRating = await db.DanhGia
                .Where(dg => dg.MaSanPham == maSP)
                .AverageAsync(dg => dg.SoSao);

            // Cập nhật trường SoSaoTrungBinh của sản phẩm
            var sanPham = await db.DanhGia.FindAsync(maSP);
            if (sanPham != null)
            {
                sanPham.SoSaoTrungBinh = averageRating; // Cập nhật giá trị trung bình
                await db.SaveChangesAsync();
            }
        }


        public IActionResult MoTaBenh(int id)
            {
                var benh = db.Benhs.SingleOrDefault(b => b.MaBenh == id);

                if (benh == null)
                {
                    TempData["Message"] = $"Không tìm thấy bệnh với mã {id}";
                    return Redirect("/404");
                }

                return View(benh);
            }
        
    }



}

