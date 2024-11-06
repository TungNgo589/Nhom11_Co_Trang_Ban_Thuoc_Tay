
using Microsoft.AspNetCore.Mvc;

using QLThuocDAPM.Data;
using QLThuocDAPM.Common;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace QLThuocDAPM.Controllers
{
    public class UserController : Controller
    {
        private readonly QlthuocDapm4Context _context;
        private readonly Common.Common _common;

        // Lưu thời gian mã khôi phục được gửi lần cuối
        private const int RecoveryCodeExpiryTimeInMinutes = 1;

        public UserController(QlthuocDapm4Context context, Common.Common common)
        {
            _context = context;
            _common = common;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email có tồn tại trong hệ thống không
                var account = _context.NguoiDungs.FirstOrDefault(u => u.Email == email);
                var user = _context.NguoiDungs.FirstOrDefault(u => u.Username == account.Username);

                if (user != null)
                {
                    // Tạo mã khôi phục gồm 6 chữ số
                    Random random = new Random();
                    string recoveryCode = random.Next(100000, 999999).ToString(); // Mã gồm 6 chữ số

                    // Gửi mã khôi phục qua email
                    string subject = "Khôi phục mật khẩu";
                    string content = $"Mã khôi phục mật khẩu của bạn là: {recoveryCode}. <br>Lưu ý: mã khôi phục sẽ hết hạn trong 1 phút !";


                    // Mã tồn tại trong 1 phút
                    HttpContext.Session.SetString("RecoveryCodeCreationTime", DateTime.Now.ToString());



                    if (Common.Common.SendMail(user.Username, subject, content, account.Email))
                    {
                        HttpContext.Session.SetString("RecoveryCode", recoveryCode);
                        HttpContext.Session.SetString("email", account.Email); // Lưu email để khôi phục sau
                        ViewBag.Message = "Mã khôi phục đã được gửi tới email của bạn.";
                        return RedirectToAction("VerifyRecoveryCode");
                    }
                    else
                    {
                        ViewBag.Error = "Có lỗi xảy ra trong việc gửi email.";
                    }
                }
                else
                {
                    ViewBag.Error = "Email không tồn tại.";
                }
            }
            return View();
        }

        // Xác nhận mã khôi phục
        [HttpGet]
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
                string sessionRecoveryCode = HttpContext.Session.GetString("RecoveryCode");
                string recoveryCodeCreationTime = HttpContext.Session.GetString("RecoveryCodeCreationTime");

                // Kiểm tra nếu mã tồn tại và thời gian không quá 1 phút
                if (sessionRecoveryCode != null && recoveryCodeCreationTime != null)
                {
                    DateTime creationTime = DateTime.Parse(recoveryCodeCreationTime);
                    if ((DateTime.Now - creationTime).TotalMinutes <= 1)
                    {
                        if (sessionRecoveryCode == recoveryCode)
                        {
                            // Mã khôi phục hợp lệ, chuyển sang trang đặt lại mật khẩu
                            return RedirectToAction("ResetPassword");
                        }
                        else
                        {
                            ViewBag.Error = "Mã khôi phục không hợp lệ.";
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Mã khôi phục đã hết hạn. Vui lòng thử lại.";
                    }
                }
                else
                {
                    ViewBag.Error = "Mã khôi phục không hợp lệ hoặc đã hết hạn.";
                }
            }
            return View();
        }



        // Đặt lại mật khẩu
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            string email = HttpContext.Session.GetString("email");
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu không khớp.";
                return View();
            }
            var account = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == email);
            if (account != null)
            {
                // Mã hóa mật khẩu mới trước khi lưu
                // Mã hóa mật khẩu
                string hashedPassword = newPassword;

                account.Matkhau = hashedPassword;
                // Cập nhật tài khoản
                _context.NguoiDungs.Update(account);
                await _context.SaveChangesAsync();


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
