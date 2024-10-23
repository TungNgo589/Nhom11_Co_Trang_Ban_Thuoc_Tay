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
    public class BenhsController : Controller
    {
        private readonly QlthuocDapm3Context _context;

        public BenhsController(QlthuocDapm3Context context)
        {
            _context = context;
        }

        // GET: Admin/Benhs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Benhs.ToListAsync());
        }

        // GET: Admin/Benhs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benh = await _context.Benhs
                .FirstOrDefaultAsync(m => m.MaBenh == id);
            if (benh == null)
            {
                return NotFound();
            }

            return View(benh);
        }

        // GET: Admin/Benhs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Benhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBenh,TenBenh,MoTa")] Benh benh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(benh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(benh);
        }

        // GET: Admin/Benhs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benh = await _context.Benhs.FindAsync(id);
            if (benh == null)
            {
                return NotFound();
            }
            return View(benh);
        }

        // POST: Admin/Benhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaBenh,TenBenh,MoTa")] Benh benh)
        {
            if (id != benh.MaBenh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(benh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenhExists(benh.MaBenh))
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
            return View(benh);
        }

        // GET: Admin/Benhs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benh = await _context.Benhs
                .FirstOrDefaultAsync(m => m.MaBenh == id);
            if (benh == null)
            {
                return NotFound();
            }

            return View(benh);
        }

        // POST: Admin/Benhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var benh = await _context.Benhs.FindAsync(id);
            if (benh != null)
            {
                _context.Benhs.Remove(benh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BenhExists(int id)
        {
            return _context.Benhs.Any(e => e.MaBenh == id);
        }
    }
}
