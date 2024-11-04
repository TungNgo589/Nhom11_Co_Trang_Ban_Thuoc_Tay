﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly QlthuocDapm4Context _context;

        public GioHangController(QlthuocDapm4Context context)
        {
            _context = context;
        }

        const string CART_KEY = "MYCART";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            var cart = Cart;

            // Tính tổng tiền trước khi áp dụng mã giảm giá
            decimal totalAmount = (decimal)cart.Sum(item => item.TongTien);

            // Kiểm tra mã giảm giá
            string discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = _context.KhuyenMais.FirstOrDefault(km => km.MaKhuyenMai == discountCode && km.TrangThai && km.ThoiGianBatDau <= DateTime.Now && km.ThoiGianKetThuc >= DateTime.Now);
                if (discount != null)
                {
                    totalAmount -= discount.GiaTri; // Áp dụng giảm giá
                    ViewData["DiscountMessage"] = $"Mã giảm giá {discountCode} đã được áp dụng!";
                }
                else
                {
                    ViewData["DiscountMessage"] = "Mã giảm giá không hợp lệ!";
                }
            }

            ViewData["TotalAmount"] = totalAmount; // Lưu tổng tiền vào ViewData để sử dụng trong view
            return View(cart);
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

            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY);

            // Nếu giỏ hàng rỗng, có thể hiển thị thông báo hoặc chuyển hướng
            if (cart == null || !cart.Any())
            {
                ViewData["ThongBao"] = "Giỏ hàng của bạn đang trống!";
                return View();
            }

            // Tính toán tổng tiền
            decimal totalAmount = (decimal)cart.Sum(item => item.TongTien);

            // Kiểm tra mã giảm giá (nếu có)
            var discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var khuyenMai = _context.KhuyenMais.FirstOrDefault(km => km.MaKhuyenMai == discountCode &&
                    km.TrangThai &&
                    km.ThoiGianBatDau <= DateTime.Now &&
                    km.ThoiGianKetThuc >= DateTime.Now);

                if (khuyenMai != null)
                {
                    totalAmount -= khuyenMai.GiaTri; // Giảm giá từ tổng tiền
                }
            }

            // Đưa tổng tiền vào ViewData để sử dụng trong view
            ViewData["TotalAmount"] = totalAmount;

            return View(cart); // Trả về giỏ hàng để hiển thị
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LuuDonHang(string address)
        {
            // Lấy giỏ hàng từ session
            var shoppingCart = HttpContext.Session.Get<List<CartItem>>(CART_KEY);

            // Kiểm tra xem giỏ hàng có rỗng không
            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống!" });
            }

            // Kiểm tra xem người dùng đã đăng nhập chưa
            string username = HttpContext.Session.GetString("userLogin") ?? string.Empty;

            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thanh toán!" });
            }

            // Tính tổng tiền và số lượng sản phẩm trong giỏ hàng
            double tongTien = shoppingCart.Sum(item => item.TongTien);
            int soLuong = shoppingCart.Sum(item => item.SoLuong);
            string maDonHang = Guid.NewGuid().ToString(); // Tạo mã đơn hàng duy nhất


     
            // Kiểm tra mã khuyến mãi từ session (không bắt buộc)
            var discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var khuyenMai = _context.KhuyenMais.FirstOrDefault(km => km.MaKhuyenMai == discountCode);
                if (discountCode == "abc" || khuyenMai == null)
                {
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ!" });
                }

                // Nếu mã hợp lệ, áp dụng giảm giá vào tổng tiền
                tongTien -= khuyenMai.GiaTri;
            }
            // Đảm bảo tổng tiền không âm
            if (tongTien < 0)
            {
                tongTien = 0;
            }

            // Tạo đối tượng đơn hàng
            var donhang = new DonHang
            {
                MaKhuyenMai = string.IsNullOrEmpty(discountCode) ? "abc" : discountCode,
                TrangThai = "Đang chờ",
                TongTien = tongTien,
                Username = username,
                HoTen = "Nguyễn Huỳnh Đông Triều", // Gán tên người dùng
                Sdt = 0327476028, // Gán số điện thoại người dùng
                SoLuong = soLuong,
                Diachi = address,
                MaDh = maDonHang,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            // Lưu đơn hàng vào cơ sở dữ liệu
            _context.DonHangs.Add(donhang);
            _context.SaveChanges();

            // Lưu chi tiết đơn hàng
            foreach (var item in shoppingCart)
            {
                var chitiet = new ChiTietDonHang
                {
                    MaDh = maDonHang,
                    MaSp = item.MaHh,
                    SoLuong = item.SoLuong,
                    TongTien = item.TongTien2 // Đảm bảo rằng giá trị này đã được tính toán đúng
                };

                _context.ChiTietDonHangs.Add(chitiet);

                // Cập nhật sản phẩm trong kho
                var sanPham = _context.SanPhams.FirstOrDefault(sp => sp.MaSp == item.MaHh);
                if (sanPham != null)
                {
                    sanPham.SoLuongMua += item.SoLuong; // Cộng số lượng mua
                    sanPham.SoLuong -= item.SoLuong; // Giảm số lượng tồn kho
                    if (sanPham.SoLuong < 0)
                    {
                        sanPham.SoLuong = 0; // Đảm bảo không âm
                    }
                    _context.SanPhams.Update(sanPham); // Cập nhật sản phẩm
                }
            }

            _context.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

            // Xóa giỏ hàng sau khi thanh toán thành công
            HttpContext.Session.Remove(CART_KEY);

            return View("ThanhCong", "GioHang"); // Chuyển hướng đến trang thành công
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApDungMaGiamGia(string maKhuyenMai)
        {
            // Kiểm tra mã giảm giá
            if (maKhuyenMai == "abc")
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ!" });
            }

            var khuyenMai = _context.KhuyenMais.FirstOrDefault(km => km.MaKhuyenMai == maKhuyenMai);

            if (khuyenMai == null)
            {
                // Trả về JSON khi mã không hợp lệ
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ!" });
            }

            // Lưu mã giảm giá vào session
            HttpContext.Session.SetString("DiscountCode", maKhuyenMai);

            // Tính toán tổng tiền sau khi áp dụng giảm giá
            var cart = Cart; // Lấy giỏ hàng hiện tại
            decimal totalAmount = (decimal)(cart.Sum(item => item.TongTien) - khuyenMai.GiaTri); // Giảm giá từ tổng tiền

            // Trả về view Index với tổng tiền đã được giảm
            ViewData["TotalAmount"] = totalAmount;
            ViewData["DiscountMessage"] = $"Mã giảm giá {maKhuyenMai} đã được áp dụng!";

            return View("ThanhToan", cart); // Trả về view ThanhToan với giỏ hàng và tổng tiền đã giảm
        }



    }
}
