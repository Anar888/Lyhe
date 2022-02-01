using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LayiheBackEnd;
using LayiheBackEnd.ViewModels;
using LayiheBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace LayiheBackEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly JuanContext _context;
        public HomeController(JuanContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeViewModel homew = new HomeViewModel
            {
                Sliders = _context.Sliders.ToList(),
                Policies=_context.policies.ToList(),
                Banners=_context.Banners.ToList(),
                Blogs=_context.Blogs.ToList(),
                Settings = _context.Settings.ToDictionary(x => x.Key, x => x.Value),
                Products=_context.Products.Include(x => x.ProductImages).Take(15).ToList(),
                Topseller=_context.Products.Include(x => x.ProductImages).Where(x=>x.IsTopseller==true && x.IsAvailable==true).Take(4).ToList()
            };
            return View(homew);
        }

    
    }
}
