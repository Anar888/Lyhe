using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LayiheBackEnd.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LayiheBackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BannerController : Controller
    {
        private readonly JuanContext _context;
        private readonly IWebHostEnvironment _env;

        public BannerController(JuanContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeVal").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Banner>.Create(_context.Banners.AsQueryable(), page, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Banner banner)
        {
            if (banner.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Image file is required!");

            if (!ModelState.IsValid)
                return View();

            if (banner.ImageFile.ContentType != "image/jpeg" && banner.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "incorrect file type");
                return View();
            }

            if (banner.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                return View();
            }
            string b = Guid.NewGuid().ToString() + banner.ImageFile.FileName;
            if (b.Length > 99)
            {
                b.Substring(64, banner.ImageFile.FileName.Length - 64);
            }

            banner.BackImage = b;

            string path = Path.Combine(_env.WebRootPath, "uploads/banner", banner.BackImage);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                banner.ImageFile.CopyTo(stream);
            }


            _context.Banners.Add(banner);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Banner banner = _context.Banners.FirstOrDefault(x => x.Id == id);
            if (banner == null) return NotFound();



            return View(banner);
        }
        [HttpPost]
        public IActionResult Delete(Banner banner)
        {



            Banner existbanner = _context.Banners.FirstOrDefault(x => x.Id == banner.Id);

            if (existbanner == null) return NotFound();

            if (existbanner.BackImage != null)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/banner", existbanner.BackImage);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }


            _context.Banners.Remove(existbanner);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Banner banner = _context.Banners.FirstOrDefault(x => x.Id == id);

            if (banner == null) return NotFound();

            return View(banner);
        }

        [HttpPost]
        public IActionResult Edit(Banner banner)
        {
            if (!ModelState.IsValid)
                return View();

            Banner existbanner = _context.Banners.FirstOrDefault(x => x.Id == banner.Id);
            if (existbanner == null) return NotFound();

            if (banner.ImageFile != null)
            {
                if (banner.ImageFile.ContentType != "image/jpeg" && banner.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (banner.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                    return View();
                }

                banner.BackImage = Guid.NewGuid().ToString() + banner.ImageFile.FileName;

                //string path = _env.WebRootPath + @"uploads\sliders\" + slider.Image;
                string path = Path.Combine(_env.WebRootPath, "uploads/banner", banner.BackImage);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    banner.ImageFile.CopyTo(stream);
                }

                if (existbanner.BackImage != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/slider", existbanner.BackImage);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);
                }

                existbanner.BackImage = banner.BackImage;
            }
            else
            {
                if (banner.BackImage == null && existbanner.BackImage != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/slider", existbanner.BackImage);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);

                    existbanner.BackImage = null;
                }
            }


            existbanner.RedirectUrl = banner.RedirectUrl;
            existbanner.SubTitle = banner.SubTitle;
            existbanner.Title1 = banner.Title1;
            existbanner.Title2 = banner.Title2;
        


            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
