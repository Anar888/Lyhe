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
    public class SliderController : Controller
    {
        private readonly JuanContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(JuanContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
                string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeVal").Value;
                int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
                return View(PagenatedList<Slider>.Create(_context.Sliders.AsQueryable(), page, pageSize));
            

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (slider.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Image file is required!");

            if (!ModelState.IsValid)
                return View();

            if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "incorrect file type");
                return View();
            }

            if (slider.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                return View();
            }
            string b = Guid.NewGuid().ToString() + slider.ImageFile.FileName;
            if (b.Length > 99)
            {
                b.Substring(64, slider.ImageFile.FileName.Length - 64);
            }

            slider.BackImage = b;

            string path = Path.Combine(_env.WebRootPath, "uploads/slider", slider.BackImage);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                slider.ImageFile.CopyTo(stream);
            }


            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Slider slider  = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) return NotFound();



            return View(slider);
        }
        [HttpPost]
        public IActionResult Delete(Slider slider)
        {


            Slider existslider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
         
            if (existslider == null) return NotFound();

            if (existslider.BackImage!=null)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/slider", existslider.BackImage);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
           
           
            _context.Sliders.Remove(existslider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid)
                return View();

            Slider existslider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
            if (existslider == null) return NotFound();

            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                    return View();
                }

                slider.BackImage = Guid.NewGuid().ToString() + slider.ImageFile.FileName;

                //string path = _env.WebRootPath + @"uploads\sliders\" + slider.Image;
                string path = Path.Combine(_env.WebRootPath, "uploads/slider", slider.BackImage);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    slider.ImageFile.CopyTo(stream);
                }

                if (existslider.BackImage != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/slider", existslider.BackImage);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);
                }

                existslider.BackImage = slider.BackImage;
            }
            else
            {
                if (slider.BackImage == null && existslider.BackImage != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/slider", existslider.BackImage);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);

                    existslider.BackImage = null;
                }
            }


            existslider.BtnText = slider.BtnText;
            existslider.BtnUrl = slider.BtnUrl;
            existslider.Descrption = slider.Descrption;
            existslider.Title = slider.Title;
            existslider.SubTitle = slider.SubTitle;
            existslider.Order = slider.Order;
            

            _context.SaveChanges();
            return RedirectToAction("index");
        }


    }
}
