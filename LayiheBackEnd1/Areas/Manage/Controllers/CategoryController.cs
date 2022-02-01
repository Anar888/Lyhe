using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LayiheBackEnd.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LayiheBackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    public class CategoryController : Controller
    {
        private readonly JuanContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(JuanContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {

            var categories = _context.Categories.Include(x => x.Products).AsQueryable();

            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeVal").Value;
            int pageSize = pageSizeStr == null ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Category>.Create(categories, page, pageSize));


        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View();

            Category existCategory = _context.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (existCategory == null) return NotFound();
            existCategory.Name = category.Name;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Category cat = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (cat == null) return NotFound();

            return View(cat);
        }
        [HttpPost]
        public IActionResult Delete(Category cat)
        {


            Category existcat = _context.Categories.FirstOrDefault(x => x.Id == cat.Id);

            if (existcat == null) return NotFound();

            _context.Categories.Remove(existcat);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
