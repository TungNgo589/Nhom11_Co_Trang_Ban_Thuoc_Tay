﻿using Microsoft.AspNetCore.Mvc;
using QLThuocDAPM.Data;

namespace QLThuocDAPM.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class HomeController : Controller
    {
        private readonly QlthuocDapm2Context _context;

        public HomeController(QlthuocDapm2Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Benhs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBenh,TenBenh,MoTa")] Benh benh)
        {
            // Add the new Benh record to the context
            _context.Add(benh);
            await _context.SaveChangesAsync();
            _context.SaveChanges();



            // If we reach this point, something went wrong, so return the same view with the current model
            return View(benh);
        }
    }
}
