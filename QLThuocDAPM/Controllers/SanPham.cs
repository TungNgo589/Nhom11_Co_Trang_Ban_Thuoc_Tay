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

        private readonly QlthuocDapm4Context db;

        public SanPhamController(QlthuocDapm4Context context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {

            var sanPhams = db.SanPhams
                
       .Include(sp => sp.MaGiamGiaNavigation) // Lấy thông tin giảm giá
       .ToList();

            var qlthuocDapm2Context = db.SanPhams.Include(s => s.MaBenhNavigation).Include(s => s.MaDmNavigation).Include(s => s.MaGiamGiaNavigation).Include(s => s.MaNhaCungCapNavigation);
         
            return View(await qlthuocDapm2Context.ToListAsync());

        }

        public IActionResult ChiTietSanPham(int id)
        {
            var data = db.SanPhams
                .Include(p => p.MaDmNavigation) // Bao gồm thông tin danh mục nếu cần
                        .Include(p => p.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                                                .Include(p => p.MaBenhNavigation) // Bao gồm thông tin nhà cung cấp
                                                   .Include(p => p.MaGiamGiaNavigation)
    // Bao gồm thông tin đánh giá
    .Include(p=>p.DanhGia)

                .SingleOrDefault(p => p.MaSp == id);

            var benhs = db.Benhs.ToList(); // Lấy danh sách các bệnh từ cơ sở dữ liệu
            ViewBag.Benhs = benhs;


            var diemTrungBinh = data.DanhGia.Any()
        ? data.DanhGia.Average(dg => dg.SoSao)
        : 0;
           
            ViewBag.SoSaoTrungBinh = diemTrungBinh;

            // Lưu danh sách đánh giá vào ViewBag
            ViewBag.DanhGia = data.DanhGia.ToList(); // Chuyển thành danh sách
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
public async Task<IActionResult> AddAndUpdateRating(int maSP, decimal rating)
{
    try
    {
        // Kiểm tra xem người dùng đã đăng nhập chưa
        if (HttpContext.Session.GetString("userLogin") == null)
        {
            return RedirectToAction("Login", "User");
        }

        // Lấy thông tin người dùng từ session
        string username = HttpContext.Session.GetString("userLogin");
        var nguoiDung = await db.NguoiDungs.FirstOrDefaultAsync(nd => nd.Username == username);

                if (nguoiDung == null)
        {
            return RedirectToAction("Index", "LoginUser");
        }

        // Kiểm tra giá trị đánh giá hợp lệ
        if (rating < 1 || rating > 5)
        {
            TempData["ErrorMessage"] = "Đánh giá không hợp lệ. Vui lòng chọn số sao từ 1 đến 5.";
            return RedirectToAction("ChiTietSanPham", new { id = maSP });
        }

                // Kiểm tra người dùng đã mua sản phẩm chưa
                var daMuaHang = await db.ChiTietDonHangs
            .AnyAsync(ct => ct.MaSp == maSP );

                if (!daMuaHang)
                {
                    TempData["ErrorMessage"] = "Bạn chưa mua sản phẩm này, không thể đánh giá.";
                    return RedirectToAction("ChiTietSanPham", new { id = maSP });
                }
                // Kiểm tra xem người dùng đã đánh giá sản phẩm này chưa
                var daDanhGia = await db.DanhGia
            .AnyAsync(dg => dg.MaNguoiDung == nguoiDung.MaNguoiDung && dg.MaSanPham == maSP);

        if (daDanhGia)
        {
            TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này rồi.";
            return RedirectToAction("ChiTietSanPham", new { id = maSP });
        }

        // Thêm đánh giá mới vào cơ sở dữ liệu
        var danhGia = new DanhGium
        {
            MaSanPham = maSP,
            MaNguoiDung = nguoiDung.MaNguoiDung,
            SoSao = rating,
            NgayBinhLuan = DateTime.Now
        };

        await db.DanhGia.AddAsync(danhGia);
        await db.SaveChangesAsync();

        // Tính và cập nhật điểm trung bình
        var averageRating = await db.DanhGia
            .Where(dg => dg.MaSanPham == maSP)
            .AverageAsync(dg => dg.SoSao);

        // Cập nhật lại SoSaoTrungBinh của sản phẩm trong bảng DanhGia
        //danhGia.SoSaoTrungBinh = averageRating;
        await db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đánh giá của bạn đã được thêm thành công!";
    }
    catch (Exception ex)
    {
        // Thêm lỗi vào ModelState để hiển thị lỗi nếu có
        ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi thêm đánh giá: " + ex.Message);
    }

    return RedirectToAction("ChiTietSanPham", new { id = maSP });
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


        public IActionResult DanhSachSanPham(int page = 1, int pageSize = 3)
        {
            var products = db.SanPhams.ToList(); // Get all products
            var totalProducts = products.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var paginatedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Find the bestselling product based on highest purchase count
            var bestsellingProduct = products
                .OrderByDescending(p => p.SoLuongMua) // Assuming SoLuongMua is the purchase count
                .FirstOrDefault(); // Get the product with the highest purchase count

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.BestsellingProduct = bestsellingProduct; // Pass the single bestselling product to the view

            return View(paginatedProducts);
        }


        public IActionResult ChiTietSP(int id)
        {
            var data = db.SanPhams
                .Include(p => p.MaDmNavigation) // Bao gồm thông tin danh mục nếu cần
                        .Include(p => p.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                                                .Include(p => p.MaBenhNavigation) // Bao gồm thông tin nhà cung cấp
                                                                                  // Bao gồm thông tin đánh giá
    .Include(p => p.DanhGia)

                .SingleOrDefault(p => p.MaSp == id);

            var benhs = db.Benhs.ToList(); // Lấy danh sách các bệnh từ cơ sở dữ liệu
            ViewBag.Benhs = benhs;


            var diemTrungBinh = data.DanhGia.Any()
        ? data.DanhGia.Average(dg => dg.SoSao)
        : 0;

            ViewBag.SoSaoTrungBinh = diemTrungBinh;

            // Lưu danh sách đánh giá vào ViewBag
            ViewBag.DanhGia = data.DanhGia.ToList(); // Chuyển thành danh sách
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

    }



}

