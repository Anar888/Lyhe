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
    public class PolicyController : Controller
    {
        private readonly JuanContext _context;
        private readonly IWebHostEnvironment _env;

        public PolicyController(JuanContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeVal").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Policy>.Create(_context.policies.AsQueryable(), page, pageSize));


        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create( Policy policy)
        {
            if (policy.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Image file is required!");

            if (!ModelState.IsValid)
                return View();

            if (policy.ImageFile.ContentType != "image/jpeg" && policy.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "incorrect file type");
                return View();
            }

            if (policy.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                return View();
            }
            string b = Guid.NewGuid().ToString() + policy.ImageFile.FileName;
            if (b.Length > 99)
            {
                b.Substring(64, policy.ImageFile.FileName.Length - 64);
            }

            policy.Icon = b;

            string path = Path.Combine(_env.WebRootPath, "uploads/policy", policy.Icon);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                policy.ImageFile.CopyTo(stream);
            }


            _context.policies.Add(policy);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Policy policy = _context.policies.FirstOrDefault(x => x.Id == id);
            if (policy == null) return NotFound();



            return View(policy);
        }
        [HttpPost]
        public IActionResult Delete(Policy policy)
        {


            Policy existpolicy = _context.policies.FirstOrDefault(x => x.Id == policy.Id);

            if (existpolicy == null) return NotFound();

            if (existpolicy.Icon != null)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/policy", existpolicy.Icon);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }


            _context.policies.Remove(existpolicy);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Policy policy = _context.policies.FirstOrDefault(x => x.Id == id);

            if (policy == null) return NotFound();

            return View(policy);
        }

        [HttpPost]
        public IActionResult Edit(Policy policy)
        {
            if (!ModelState.IsValid)
                return View();

            Policy existpolicy = _context.policies.FirstOrDefault(x => x.Id == policy.Id);
            if (existpolicy == null) return NotFound();

            if (policy.ImageFile != null)
            {
                if (policy.ImageFile.ContentType != "image/jpeg" && policy.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (policy.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                    return View();
                }

                policy.Icon = Guid.NewGuid().ToString() + policy.ImageFile.FileName;

                //string path = _env.WebRootPath + @"uploads\sliders\" + slider.Image;
                string path = Path.Combine(_env.WebRootPath, "uploads/policy", policy.Icon);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    policy.ImageFile.CopyTo(stream);
                }

                if (existpolicy.Icon != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/policy", existpolicy.Icon);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);
                }

                existpolicy.Icon = policy.Icon;
            }
            else
            {
                if (policy.Icon == null && existpolicy.Icon != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/policy", existpolicy.Icon);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);

                    existpolicy.Icon = null;
                }
            }


            existpolicy.Title = policy.Title;
            existpolicy.Desc = policy.Desc;
            


            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
