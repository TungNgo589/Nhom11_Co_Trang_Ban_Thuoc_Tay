using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuocDAPM.Data;

namespace QLThuocDAPM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhaCungCapsController : Controller
    {
        private readonly QlthuocDapm4Context _context;

        public NhaCungCapsController(QlthuocDapm4Context context)
        {
            _context = context;
        }

        // GET: Admin/NhaCungCaps
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhaCungCaps.ToListAsync());
        }

        // GET: Admin/NhaCungCaps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps
                .FirstOrDefaultAsync(m => m.MaNhaCungCap == id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }

        // GET: Admin/NhaCungCaps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaCungCaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhaCungCap,TenNhaCungCap,MoTaNhaCungCap,Sdt,DiaChi,Email,HinhAnhNhaCungCap")] NhaCungCap nhaCungCap, IFormFile file1)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem file có được upload không
                if ((file1 != null && file1.Length > 0))
                {
                    // Tạo tên file duy nhất để tránh xung đột
                    var fileName1 = Path.GetFileName(file1.FileName);
                    var filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName1);


                    // Lưu file vào thư mục wwwroot/images
                    using (var stream = new FileStream(filePath1, FileMode.Create))
                    {
                        await file1.CopyToAsync(stream);
                    }


                    // Gán đường dẫn của ảnh cho thuộc tính AnhSp của sản phẩm
                    nhaCungCap.HinhAnhNhaCungCap = "/images/" + fileName1; // Đảm bảo đường dẫn hợp lệ

                    _context.Add(nhaCungCap);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(nhaCungCap);
            }
            return View();
        }

        // GET: Admin/NhaCungCaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }
            return View(nhaCungCap);
        }

        // POST: Admin/NhaCungCaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNhaCungCap,TenNhaCungCap,HinhAnhNhaCungCap,MoTaNhaCungCap,Sdt,DiaChi,Email")] NhaCungCap nhaCungCap)
        {
            if (id != nhaCungCap.MaNhaCungCap)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhaCungCap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaCungCapExists(nhaCungCap.MaNhaCungCap))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nhaCungCap);
        }

        // GET: Admin/NhaCungCaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps
                .FirstOrDefaultAsync(m => m.MaNhaCungCap == id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }

        // POST: Admin/NhaCungCaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap != null)
            {
                _context.NhaCungCaps.Remove(nhaCungCap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhaCungCapExists(int id)
        {
            return _context.NhaCungCaps.Any(e => e.MaNhaCungCap == id);
        }
    }
}
