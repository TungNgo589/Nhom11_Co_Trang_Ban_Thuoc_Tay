using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using QLThuocDAPM.Data;
using QLThuocDAPM.Models;
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

            var diemTrungBinh = data.DanhGia.Any()
                ? data.DanhGia.Average(dg => dg.SoSao)
                : 0; // Trường hợp không có đánh giá thì trả về 0
           
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

            // Kiểm tra xem rating có hợp lệ không (nằm trong khoảng 1-5)
            if (rating < 1 || rating > 5)
            {
                TempData["ErrorMessage"] = "Đánh giá không hợp lệ. Vui lòng chọn số sao từ 1 đến 5.";
                return RedirectToAction("ChiTietSanPham", new { id = maSP });
            }

            // Kiểm tra xem người dùng đã mua sản phẩm chưa
            var daMuaHang = await db.ChiTietDonHangs
                .AnyAsync(ct => ct.MaSp == maSP && ct.MaDhNavigation.MaNguoiDung == nguoiDung.MaNguoiDung);

            if (!daMuaHang)
            {
                TempData["ErrorMessage"] = "Bạn chưa mua sản phẩm này, không thể đánh giá.";
                return RedirectToAction("ChiTietSanPham", new { id = maSP });
            }

            // Kiểm tra xem người dùng đã đánh giá chưa
            var daDanhGia = await db.DanhGia
                .AnyAsync(dg => dg.MaNguoiDung == nguoiDung.MaNguoiDung && dg.MaSanPham == maSP);

            if (daDanhGia)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("ChiTietSanPham", new { id = maSP });
            }

            // Tạo mới đối tượng đánh giá
            var danhGia = new DanhGium
            {
                MaSanPham = maSP,
                MaNguoiDung = nguoiDung.MaNguoiDung,
                SoSao = rating,
                NgayBinhLuan = DateTime.Now
            };

            // Thêm đánh giá vào cơ sở dữ liệu
            await db.DanhGia.AddAsync(danhGia);
            await db.SaveChangesAsync();

            // Cập nhật đánh giá trung bình sau khi thêm đánh giá mới
            await UpdateAverageRating(maSP);

            return RedirectToAction("ChiTietSanPham", new { id = maSP });
        }

        // Hàm cập nhật đánh giá trung bình của sản phẩm
        private async Task UpdateAverageRating(int maSP)
        {
            // Tính toán đánh giá trung bình
            var averageRating = await db.DanhGia
                .Where(dg => dg.MaSanPham == maSP)
                .AverageAsync(dg => dg.SoSao);

            // Cập nhật trường SoSaoTrungBinh của sản phẩm
            var danhGia = await db.DanhGia.FindAsync(maSP);
            if (danhGia != null)
            {
                danhGia.SoSaoTrungBinh = averageRating; // Cập nhật giá trị trung bình
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

