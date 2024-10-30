using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;
using System.Linq;
using System.Collections.Generic;
using QLThuocDAPM.Helpers;
using QLThuocDAPM.Models;
using Newtonsoft.Json;

namespace QLThuocDAPM.Controllers
{
    public class GioHangController : Controller
    {
        private readonly QlthuocDapm3Context _context;

        public GioHangController(QlthuocDapm3Context context)
        {
            _context = context;
        }


        private void UpdateCartItemCount(GioHang gioHang)
        {
            ViewBag.CartItemCount = gioHang.ChiTietGioHangs.Sum(ct => ct.SoLuongSp);

        }




        const string CART_KEY = "MYCART";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = _context.SanPhams.SingleOrDefault(p => p.MaSp == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaSp,
                    TenHH = hangHoa.TenSp,
                    DonGia = hangHoa.GiaTien,
                    Hinh = hangHoa.HinhAnh1 ?? string.Empty,
                    SoLuong = quantity,
                    TongTien = quantity * hangHoa.GiaTien
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(CART_KEY, gioHang);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var gioHang = Cart; // Lấy giỏ hàng từ session
            var item = gioHang.SingleOrDefault(p => p.MaHh == id); // Tìm sản phẩm trong giỏ hàng

            if (item != null)
            {
                if (quantity <= 0) // Nếu số lượng <= 0, xóa sản phẩm
                {
                    gioHang.Remove(item);
                }
                else // Cập nhật số lượng
                {
                    item.SoLuong = quantity;
                    item.TongTien = item.DonGia * quantity; // Cập nhật tổng tiền
                }
            }

            HttpContext.Session.Set(CART_KEY, gioHang); // Lưu lại giỏ hàng vào session

            return RedirectToAction("Index"); // Quay lại trang giỏ hàng
        }
        public IActionResult ThanhToan()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            string username = HttpContext.Session.GetString("userLogin");

            if (username == null)
            {
                return RedirectToAction("Login", "User");
            }

            NguoiDung user = _context.NguoiDungs.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["HoTen"] = user.HoTen;
            ViewData["Email"] = user.Email;
            ViewData["Sdt"] = user.Sdt;

       
            return View(Cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LuuDonHang(string address)
        {
            var shoppingCart = HttpContext.Session.Get<List<CartItem>>(CART_KEY);

            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống!" });
            }

            string username = HttpContext.Session.GetString("userLogin") ?? string.Empty;

            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thanh toán!" });
            }

            double tongTien = shoppingCart.Sum(item => item.TongTien);
            int soLuong = shoppingCart.Sum(item => item.SoLuong);
            string maDonHang = Guid.NewGuid().ToString();

            var donhang = new DonHang
            {
                TrangThai = "Đang chờ",
                TongTien = tongTien,
                Username = username,
                HoTen = "t",
                Sdt = 0327476018,
                SoLuong = soLuong,
                Diachi = address,
                MaDh = maDonHang,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                MaKhuyenMai = 1
            };

            _context.DonHangs.Add(donhang);
            _context.SaveChanges();

            foreach (var item in shoppingCart)
            {
                var chitiet = new ChiTietDonHang
                {
                    MaDh = maDonHang,
                    MaSp = item.MaHh,
                    SoLuong = item.SoLuong,
                    TongTien = item.TongTien2
                };

                _context.ChiTietDonHangs.Add(chitiet);
                var sanPham = _context.SanPhams.FirstOrDefault(sp => sp.MaSp == item.MaHh);
                if (sanPham != null)
                {
                    sanPham.SoLuongMua += item.SoLuong; // Cộng số lượng mua
                    sanPham.SoLuong -= item.SoLuong; // Giảm số lượng tồn kho
                                                     // Nếu bạn cần thêm kiểm tra để không giảm số lượng tồn kho âm
                    if (sanPham.SoLuong < 0)
                    {
                        sanPham.SoLuong = 0; // Đảm bảo không âm
                    }
                    _context.SanPhams.Update(sanPham); // Cập nhật sản phẩm
                }
            }

            _context.SaveChanges();

            // Clear the shopping cart
            HttpContext.Session.Remove(CART_KEY);

            return View("ThanhCong","GioHang");
        }
        public IActionResult ThanhCong()
        {
            return View();
        }

    }
}
