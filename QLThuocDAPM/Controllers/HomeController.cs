using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLThuocDAPM.Data;
using QLThuocDAPM.Models;
using QLThuocDAPM.ViewModels;
using System.Diagnostics;
using System.Drawing.Printing;

namespace QLThuocDAPM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QlthuocDapm3Context _context;

        public HomeController(ILogger<HomeController> logger, QlthuocDapm3Context context)
        {
            _logger = logger;
            _context = context;

        }

        //public IActionResult Index()
        //{
        //    var danhMucList = _context.DanhMucs.ToList(); // Lấy danh sách danh mục

        //    var viewModel = new HomeViewModel
        //    {
        //        DanhMucs = danhMucList,
        //        SanPhams = new Dictionary<int, List<SanPham>>() // Khởi tạo từ điển để lưu sản phẩm theo danh mục
        //    };

        //    // Lặp qua từng danh mục và lấy sản phẩm theo mã danh mục
        //    foreach (var danhMuc in danhMucList)
        //    {
        //        var sanPhamTheoDanhMuc = _context.SanPhams
        //            .Where(sp => sp.MaDm == danhMuc.MaDm) // Lọc sản phẩm theo mã danh mục
        //            .ToList();

        //        viewModel.SanPhams[danhMuc.MaDm] = sanPhamTheoDanhMuc; // Thêm sản phẩm vào từ điển
        //    }

        //    return View(viewModel);
        //}


        public IActionResult Index(int page = 1, int pageSize = 3)
        {
            var danhMucList = _context.DanhMucs.ToList(); // Lấy danh sách danh mục

            var viewModel = new HomeViewModel
            {
                DanhMucs = danhMucList,
                SanPhams = new Dictionary<int, List<SanPham>>() // Khởi tạo từ điển để lưu sản phẩm theo danh mục
            };

            // Lặp qua từng danh mục và lấy sản phẩm theo mã danh mục
            foreach (var danhMuc in danhMucList)
            {
                var totalProducts = _context.SanPhams.Count(sp => sp.MaDm == danhMuc.MaDm);
                var sanPhamTheoDanhMuc = _context.SanPhams
                    .Where(sp => sp.MaDm == danhMuc.MaDm) // Lọc sản phẩm theo mã danh mục
                    .Skip((page - 1) * pageSize) // Bỏ qua các sản phẩm trước đó
                    .Take(pageSize) // Lấy số lượng sản phẩm cho trang hiện tại
                    .ToList();

                viewModel.SanPhams[danhMuc.MaDm] = sanPhamTheoDanhMuc; // Thêm sản phẩm vào từ điển

                // You can also calculate total pages for each category if needed
                int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

                // Optional: Store totalPages in viewModel or just use it for displaying in the view
            }

            return View(viewModel);
        }







        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public JsonResult GetData()
        {
            int cartCount = 0;

            var shoppingCart = HttpContext.Session.GetString("ShoppingCart");
            if (shoppingCart != null)
            {
                var ShoppingCart = JsonConvert.DeserializeObject<List<CartItem>>(shoppingCart);
                cartCount = ShoppingCart.Count();
            }

            return Json(new { cartCount = cartCount });
        }


        public IActionResult DonHang()
        {
            if (HttpContext.Session.GetString("userLogin") == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                string userID = HttpContext.Session.GetString("userLogin");

                var donhang = _context.DonHangs
                                     .Where(dh => dh.Username == userID)
                                     .Include(dh => dh.ChiTietDonHangs)
                                     .ThenInclude(ct => ct.MaSpNavigation) // Load sản phẩm liên quan
                                     .OrderByDescending(dh => dh.TrangThai)
                                     .ToList();

                return View(donhang);
            }
        }



        public IActionResult HuyDonHang(string maDH)
        {
            var userLogin = HttpContext.Session.GetString("userLogin");
            if (userLogin == null)
            {
                return RedirectToAction("Login", "User");
            }

            var donhang = _context.DonHangs
                            .Where(dh => dh.MaDh == maDH && dh.Username == userLogin)
                            .FirstOrDefault();

            if (donhang != null)
            {
                donhang.UpdatedAt = DateTime.Now;
                donhang.TrangThai = "Đã hủy";
                _context.SaveChanges();
            }

            return RedirectToAction("DonHang", "Home");
        }

    }
   


}
