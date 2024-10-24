﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLThuocDAPM.Data;
using QLThuocDAPM.Models;
using System.Diagnostics;

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
       
        public IActionResult Index()
        {
            return View();
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
