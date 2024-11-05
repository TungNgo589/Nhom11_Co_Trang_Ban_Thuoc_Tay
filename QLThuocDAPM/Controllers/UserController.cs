
using Microsoft.AspNetCore.Mvc;

using QLThuocDAPM.Data;

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace QLThuocDAPM.Controllers
{
    public class UserController : Controller
    {
        private readonly QlthuocDapm4Context _context;

        public UserController(QlthuocDapm4Context context)

        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // View đăng ký
        public IActionResult Register()
        {
            return View();
        }

        // View đăng nhập
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear(); // Xóa tất cả session
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string matkhau)
        {


            if (ModelState.IsValid)
            {
                NguoiDung check = _context.NguoiDungs.FirstOrDefault(s => s.Username == username);
                if (check == null || check.Matkhau != matkhau)
                {
                    ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
                    return View();
                }

                // Lưu thông tin người dùng vào TempData hoặc session
                HttpContext.Session.SetString("hoTen", check.HoTen);
                HttpContext.Session.SetString("email", check.Email);
                HttpContext.Session.SetString("sdt", check.Sdt);
                HttpContext.Session.SetString("userLogin", check.Username);

                // Kiểm tra quyền Admin
                if (check.RoleId == 2) // RoleId = 2 nghĩa là Admin
                {
                    HttpContext.Session.SetString("adminLogin", check.Username);
                    return RedirectToAction("Index", "SanPhams", new { area = "Admin" });
                }

                return RedirectToAction("Index", "Home"); // Người dùng thông thường
            

           
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(NguoiDung user)
        {
            user.TrangThai = "Chưa mua hàng";
            //user.Role = 2;

            var existingUser = _context.NguoiDungs.FirstOrDefault(s => s.Username == user.Username);
                if (existingUser == null)
                {
                    _context.NguoiDungs.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Tài khoản đã tồn tại";
                    return View();
                }
            
            return View();
        }

        // Quên mật khẩu
        public IActionResult ForgotPassword()
        {
            return View();
        }

    
        public IActionResult VerifyRecoveryCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyRecoveryCode(string recoveryCode)
        {
            if (ModelState.IsValid)
            {
                var sessionRecoveryCode = HttpContext.Session.GetString("RecoveryCode");
                var email = HttpContext.Session.GetString("Email");

                if (sessionRecoveryCode == recoveryCode)
                {
                    // Mã khôi phục hợp lệ, chuyển sang trang đặt lại mật khẩu
                    return RedirectToAction("ResetPassword", new { email = email });
                }
                else
                {
                    ViewBag.Error = "Mã khôi phục không hợp lệ.";
                }
            }
            return View();
        }

        // Đặt lại mật khẩu
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.NguoiDungs.FirstOrDefault(u => u.Email == email);

            if (user != null && newPassword == confirmPassword)
            {
                // Cập nhật mật khẩu mới
                user.Matkhau = newPassword;
                _context.Update(user);
                _context.SaveChanges();

                // Xóa session RecoveryCode và Email
                HttpContext.Session.Remove("RecoveryCode");
                HttpContext.Session.Remove("Email");

                ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "Mật khẩu không khớp hoặc có lỗi xảy ra.";
            }

            return View();
        }
        // Quản lý thông tin cá nhân
        public IActionResult ThongTinNguoiDung()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (HttpContext.Session.GetString("userLogin") == null)
            {
                return RedirectToAction("Login", "User");
            }

            string username = HttpContext.Session.GetString("userLogin");
            NguoiDung user = _context.NguoiDungs.FirstOrDefault(u => u.Username == username); // Đảm bảo rằng bạn đã có thuộc tính "Username" trong mô hình NguoiDung

            if (user == null)
            {
                return NotFound(); // Trả về lỗi nếu không tìm thấy người dùng
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThongTinNguoiDung(NguoiDung model)
        {
          
                // Tìm người dùng theo username từ session
                string username = HttpContext.Session.GetString("userLogin");
                var user = _context.NguoiDungs.FirstOrDefault(u => u.Username == username);


            user.HoTen = model.HoTen;
                    user.Email = model.Email;
                    user.Sdt = model.Sdt;

                    _context.Entry(user).State = EntityState.Modified;
                    _context.SaveChanges();
                    ViewData["Message"] = "Cập nhật thông tin thành công!";
                    return View(model);

          
        }
       

    }
}
