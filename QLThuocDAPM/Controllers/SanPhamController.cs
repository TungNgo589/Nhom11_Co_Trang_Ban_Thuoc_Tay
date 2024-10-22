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
     
            private readonly QlthuocDapm2Context db;

            public SanPhamController(QlthuocDapm2Context context)
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
                .SingleOrDefault(p => p.MaSp == id);

            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            return View("ChiTietSanPham", data); // Gọi rõ ràng View "ChiTietSanPham"
        }
      


    }



}

