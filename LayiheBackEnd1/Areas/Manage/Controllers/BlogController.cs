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
    public class BlogController : Controller
    {
        private readonly JuanContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(JuanContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeVal").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Blog>.Create(_context.Blogs.AsQueryable(), page, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Blog blog)
        {
            
            if (blog.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Image file is required!");

            if (!ModelState.IsValid)
                return View();

            if (blog.ImageFile.ContentType != "image/jpeg" && blog.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "incorrect file type");
                return View();
            }

            if (blog.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                return View();
            }
            if (blog.Date!=null)
            {
                
            }
            string b = Guid.NewGuid().ToString() + blog.ImageFile.FileName;
            if (b.Length > 99)
            {
                b.Substring(64, blog.ImageFile.FileName.Length - 64);
            }

            blog.Image = b;
           

            string path = Path.Combine(_env.WebRootPath, "uploads/blog", blog.Image);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                blog.ImageFile.CopyTo(stream);
            }
            
            _context.Blogs.Add(blog);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Blog blog = _context.Blogs.FirstOrDefault(x => x.Id == id);

            if (blog == null) return NotFound();

            return View(blog);
        }

        [HttpPost]
        public IActionResult Edit(Blog blog)
        {
            if (!ModelState.IsValid)
                return View();

            Blog existblog = _context.Blogs.FirstOrDefault(x => x.Id == blog.Id);
            if (existblog == null) return NotFound();

            if (blog.ImageFile != null)
            {
                if (blog.ImageFile.ContentType != "image/jpeg" && blog.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (blog.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                    return View();
                }

                blog.Image = Guid.NewGuid().ToString() + blog.ImageFile.FileName;

                //string path = _env.WebRootPath + @"uploads\sliders\" + slider.Image;
                string path = Path.Combine(_env.WebRootPath, "uploads/blog", blog.Image);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    blog.ImageFile.CopyTo(stream);
                }

                if (existblog.Image != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/blog", existblog.Image);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);
                }

                existblog.Image = blog.Image;
            }
            else
            {
                if (blog.Image == null && existblog.Image != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/blog", existblog.Image);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);

                    existblog.Image = null;
                }
            }


            existblog.Title = blog.Title;
            existblog.RedirectUrl = blog.RedirectUrl;
            existblog.ByWho = blog.ByWho;
            existblog.Date = blog.Date;
         




            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Blog blog = _context.Blogs.FirstOrDefault(x => x.Id == id);
            if (blog == null) return NotFound();



            return View(blog);
        }
        [HttpPost]
        public IActionResult Delete(Blog blog)
        {


            Blog existblog = _context.Blogs.FirstOrDefault(x => x.Id == blog.Id);

            if (existblog == null) return NotFound();

            if (existblog.Image != null)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/slider", existblog.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }


            _context.Blogs.Remove(existblog);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
