using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;
using QLThuocDAPM.Common;
using QLThuocDAPM.Helpers;
using QLThuocDAPM.Models;
using Microsoft.CodeAnalysis;

namespace QLThuocDAPM.Controllers
{
    public class GioHangController : Controller
    {
        private readonly QlthuocDapm4Context _context;
        private readonly Common.Common _common;
        public GioHangController(QlthuocDapm4Context context, Common.Common common)
        {
            _context = context; 
            _common = common;
        }

        const string CART_KEY = "MYCART";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            var cart = Cart; // Giả sử Cart là danh sách các sản phẩm trong giỏ hàng

            // Tính tổng tiền cho từng sản phẩm trong giỏ hàng
            foreach (var item in cart)
            {
                var product = _context.SanPhams
                    .Include(s => s.MaGiamGiaNavigation)
                    .FirstOrDefault(s => s.MaSp == item.MaHh);

                if (product != null)
                {
                    item.DonGia = product.GiaSauGiam ?? product.GiaTien; // Lấy giá sau giảm
                    item.TongTien = item.GiaSauGiam * item.SoLuong; // Tính tổng tiền cho sản phẩm
                }
            }

            // Tính tổng tiền của giỏ hàng
            decimal totalAmount = (decimal)cart.Sum(item => item.TongTien);

            ViewData["TotalAmount"] = totalAmount; // Lưu tổng tiền vào ViewData để sử dụng trong view

            return View(cart); // Trả về view với danh sách sản phẩm trong giỏ hàng
        }

        public IActionResult AddToCart(int id, int quantity = 1, string type = "Normal")
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
                    DonVi=hangHoa.DonVi,
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


            //TempData["SuccessMessage"] = $"Sản phẩm đã ở trong giỏ hàng của bạn";

            // Tính lại tổng số lượng trong giỏ
    
            HttpContext.Session.Set(CART_KEY, gioHang);

            var cartItemCount = gioHang.Sum(item => item.SoLuong);

            return RedirectToAction("Index");
        }
        // Lấy số lượng sản phẩm trong giỏ
      
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
            foreach (var item in cart)
            {
                var product = _context.SanPhams
                    .Include(s => s.MaGiamGiaNavigation)
                    .FirstOrDefault(s => s.MaSp == item.MaHh);

                if (product != null)
                {
                    item.DonGia = product.GiaSauGiam ?? product.GiaTien; // Lấy giá sau giảm
                    item.TongTien = item.GiaSauGiam * item.SoLuong; // Tính tổng tiền cho sản phẩm
                }
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
        public IActionResult LuuDonHang(string address,string tennguoinhan,string sdtnguoinhan)
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
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY);

            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thanh toán!" });
            }
            foreach (var item in cart)
            {
                var product = _context.SanPhams
                    .Include(s => s.MaGiamGiaNavigation)
                    .FirstOrDefault(s => s.MaSp == item.MaHh);

                if (product != null)
                {
                    item.DonGia = product.GiaSauGiam ?? product.GiaTien; // Lấy giá sau giảm
                    item.TongTien = item.DonGia * item.SoLuong; // Tính tổng tiền cho sản phẩm
                }
            }
            // Tính tổng tiền và số lượng sản phẩm trong giỏ hàng
            decimal totalAmount = (decimal)cart.Sum(item => item.TongTien);
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
                totalAmount -= khuyenMai.GiaTri;
            }
            // Đảm bảo tổng tiền không âm
            if (totalAmount < 0)
            {
                totalAmount = 0;
            }

            // Tạo đối tượng đơn hàng
            var donhang = new DonHang
            {
                MaKhuyenMai = string.IsNullOrEmpty(discountCode) ? "abc" : discountCode,
                TrangThai = "Đang chờ",
                TongTien = (double)totalAmount,
                Username = username,
                HoTen = tennguoinhan, // Gán tên người dùng
                Sdt = sdtnguoinhan,
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
            var nguoidung = _context.NguoiDungs
    .FirstOrDefault(u => u.Username == username);
            // Gửi email thông báo
            string subject = "Thông báo đặt hàng thành công";
            string content = $"Xin chào {nguoidung.HoTen},<br><br>Đơn hàng mã {donhang.MaDh} của bạn đã đặt thành công.<br><br> Đơn hàng sẽ sớm được giao đến bạn.<br><br>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi!";
            content = $"Thông tin đơn hàng của bạn: <br> Tên người nhận: {donhang.HoTen}<br><br>Số điện thoại: {donhang.Sdt}<br><br>Địa chỉ: {donhang.Diachi}<br><br>Số lượng: {donhang.SoLuong}<br><br>Tổng tiền: {donhang.TongTien}";

            // Gọi phương thức gửi email
            if (Common.Common.SendMail("Nhà Thuốc Long Châu", subject, content, nguoidung.Email)) // Email người dùng nằm ở trường `Email`
            {
                ViewBag.Message = "Thông báo giao hàng đã được gửi qua email của khách hàng.";
            }
            else
            {
                ViewBag.Error = "Có lỗi xảy ra trong quá trình gửi email thông báo.";
            }

            return View("ThanhCong", "GioHang"); // Chuyển hướng đến trang thành công
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApDungMaGiamGia(string maKhuyenMai)
        {
            var cart = Cart;
            foreach (var item in cart)
            {
                var product = _context.SanPhams
                    .Include(s => s.MaGiamGiaNavigation)
                    .FirstOrDefault(s => s.MaSp == item.MaHh);

                if (product != null)
                {
                    item.DonGia = product.GiaSauGiam ?? product.GiaTien; // Lấy giá sau giảm
                    item.TongTien = item.DonGia * item.SoLuong; // Tính tổng tiền cho sản phẩm
                }
            }
            // Tính tổng tiền và số lượng sản phẩm trong giỏ hàng
            decimal totalAmount = (decimal)cart.Sum(item => item.TongTien);

            var khuyenMai = _context.KhuyenMais.FirstOrDefault(km => km.MaKhuyenMai == maKhuyenMai);

            if (khuyenMai == null)
            {
                ViewData["QuaHan"] = "Mã giảm giá không hợp lệ";
                return View("ThanhToan", cart);
            }
            if (khuyenMai.TrangThai == false)
            {
                // Gán thông báo lỗi vào ViewData
                ViewData["QuaHan"] = "Mã giảm giá đã quá hạn";
                return View("ThanhToan", cart); // Trả về view cùng với thông báo lỗi trong ViewData
            }
            // Lưu mã giảm giá vào session
            HttpContext.Session.SetString("DiscountCode", maKhuyenMai);

 
             totalAmount = (decimal)(cart.Sum(item => item.TongTien) - khuyenMai.GiaTri); // Giảm giá từ tổng tiền

            // Trả về view Index với tổng tiền đã được giảm
            ViewData["TotalAmount"] = totalAmount;
            ViewData["DiscountMessage"] = $"Mã giảm giá {maKhuyenMai} đã được áp dụng!";

            return View("ThanhToan", cart); // Trả về view ThanhToan với giỏ hàng và tổng tiền đã giảm
        }



    }
}
