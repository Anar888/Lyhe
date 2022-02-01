using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LayiheBackEnd.Models;
using LayiheBackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LayiheBackEnd.Controllers
{
    public class ProductController : Controller
    {
        private readonly JuanContext _context;
        public ProductController(JuanContext context)
        {
            _context = context;
        }
        public IActionResult GetProduct(int id)
        {
            Product product = _context.Products.Include(x => x.Brand).Include(x => x.ProductImages).Include(x => x.ProductColors).ThenInclude(x=>x.Color).FirstOrDefault(x => x.Id == id);
          
            return PartialView("_ProductModalPartial", product);
        }
        public IActionResult Index(int? colorid, int? brandid,  int? categoryid,int page = 1)
        {
            var products = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductColors).ThenInclude(x=>x.Color).Where(x => x.IsAvailable );
            ViewBag.BrandId = brandid;
            ViewBag.ColorId = colorid;
            ViewBag.CategoryId = categoryid;
            ViewBag.PageIndex = page;
            if (brandid != null)
                products = products.Where(x =>x.BrandId==brandid);
            if (categoryid != null)
                products = products.Where(x => x.CategoryId == categoryid);
            if (colorid != null)
                products = products.Where(x => x.ProductColors.Any(x => x.Color.Id == colorid));
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 2d);

            ProductViewModel prviewm = new ProductViewModel
            {
             Products=products.Skip((page - 1) * 2).Take(2).ToList(),
             Brands=_context.Brands.Include(x => x.Products).ToList(),
             Colors=_context.Colors.Include(x => x.ProductColors).ThenInclude(x=>x.Product).ToList(),
             Categories=_context.Categories.Include(x=>x.Products).ToList()
            };

            return View(prviewm);
        }
    }
}
