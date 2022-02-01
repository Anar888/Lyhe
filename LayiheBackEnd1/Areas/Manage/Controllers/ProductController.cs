using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LayiheBackEnd.Helpers;
using LayiheBackEnd.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LayiheBackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly JuanContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(JuanContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeVal").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Product>.Create(_context.Products.Include(x=>x.ProductImages).AsQueryable(), page, pageSize));


        }
        public IActionResult Create()
        {
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.Include(x=>x.ProductColors).ThenInclude(x=>x.Product).ToList();


            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.Include(x => x.ProductColors).ThenInclude(x => x.Product).ToList();

            if (!ModelState.IsValid)
                return View();

            if (product.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "PosterFile is required");
                return View();
            }
            else
            {
                if (product.PosterFile.ContentType != "image/jpeg" && product.PosterFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (product.PosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterFile", "file size must be less than 2mb");
                    return View();
                }
            }

            

            if (product.Files != null)
            {
                foreach (var file in product.Files)
                {
                    if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Files", "file type must be image/jpeg or image/png");
                        return View();
                    }

                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("Files", "file size must be less than 2mb");
                        return View();
                    }


                }
            }


            if (!_context.Brands.Any(x => x.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Brand not found");
                return View();
            }
            if (!_context.Categories.Any(x => x.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "CategoryId not found");
                return View();
            }

            product.ProductImages = new List<ProductImage>();
            product.ProductColors = new List<ProductColor>();

            if (product.ColorIds != null)
            {
                foreach (var colorid in product.ColorIds)
                {
                    if (_context.Colors.Any(x => x.Id == colorid))
                    {
                        ProductColor productcolor = new ProductColor
                        {
                            ColorId = colorid,
                        };
                        product.ProductColors.Add(productcolor);
                    }
                    else
                    {
                        ModelState.AddModelError("ColorIds", "Color not found");
                        return View();
                    }
                }
            }

            ProductImage posterImage = new ProductImage
            {
                PosterStatus = true,
                Image = FileManager.Save(_env.WebRootPath, "uploads/product", product.PosterFile)
            };
            product.ProductImages.Add(posterImage);

            
            if (product.Files != null)
            {
                foreach (var file in product.Files)
                {
                    ProductImage productImage = new ProductImage
                    {
                        PosterStatus = null,
                        Image = FileManager.Save(_env.WebRootPath, "uploads/product", file)
                    };
                    product.ProductImages.Add(productImage);
                }
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Product product = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductColors).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.Include(x => x.ProductColors).ThenInclude(x => x.Product).ToList();

            product.ColorIds = product.ProductColors.Select(x => x.ColorId).ToList();

            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            Product existproduct = _context.Products.Include(c => c.ProductImages).FirstOrDefault(x => x.Id == product.Id);

            if (existproduct == null) return NotFound();

            if (!_context.Brands.Any(x => x.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Brand not found");
                return View();
            }
            if (!_context.Categories.Any(x => x.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View();
            }


            if (product.PosterFile != null && product.PosterFile.ContentType != "image/jpeg" && product.PosterFile.ContentType != "image/png")
            {
                ModelState.AddModelError("PosterFile", "file type must be image/jpeg or image/png");
                return View();
            }

            if (product.PosterFile != null && product.PosterFile.Length > 2097152)
            {
                ModelState.AddModelError("PosterFile", "file size must be less than 2mb");
                return View();
            }

            if (product.Files != null)
            {
                foreach (var file in product.Files)
                {
                    if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Files", "file type must be image/jpeg or image/png");
                        return View();
                    }

                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("Files", "file size must be less than 2mb");
                        return View();
                    }
                }
            }

            ProductImage poster = existproduct.ProductImages.FirstOrDefault(x => x.PosterStatus == true);
           


            if (product.PosterFile != null)
            {
                string newPosterImg = FileManager.Save(_env.WebRootPath, "uploads/product", product.PosterFile);
                if (poster != null)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/product", poster.Image);
                    poster.Image = newPosterImg;
                }
                else
                {
                    poster = new ProductImage { Image = newPosterImg, PosterStatus = true };
                    existproduct.ProductImages.Add(poster);
                }
            }
            //product.ProductColors = new List<ProductColor>();
            //existproduct.ProductColors.RemoveAll(x => x.ColorId != null && !product.ColorIds.Contains(x.Id));
            //if (product.ColorIds != null)
            //{
            //    foreach (var colorid in product.ColorIds)
            //    {
            //        if (_context.Colors.Any(x => x.Id == colorid))
            //        {
            //            ProductColor productcolor = new ProductColor
            //            {
            //                ColorId = colorid,
            //            };
            //            existproduct.ProductColors.Add(productcolor);
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("ColorIds", "Color not found");
            //            return View();
            //        }
            //    }
            //}
            
            existproduct.ProductImages.RemoveAll(x => x.PosterStatus == null && !product.FileIds.Contains(x.Id));

            if (product.Files != null)
            {
                foreach (var file in product.Files)
                {
                    ProductImage productimage = new ProductImage
                    {
                        PosterStatus = null,
                        Image = FileManager.Save(_env.WebRootPath, "uploads/product", file)
                    };
                    existproduct.ProductImages.Add(productimage);
                }
            }


            existproduct.CategoryId = product.CategoryId;
            existproduct.BrandId = product.BrandId;
            existproduct.CostPrice = product.CostPrice;
            existproduct.SalePrice = product.SalePrice;
            existproduct.IsAvailable = product.IsAvailable;
            existproduct.IsMan = product.IsMan;
            existproduct.IsTopseller = product.IsTopseller;
            existproduct.Name = product.Name;
            existproduct.Desc = product.Desc;
            existproduct.DiscountPercent = product.DiscountPercent;


            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Include(x=>x.ProductImages).Include(x=>x.ProductColors).ThenInclude(x=>x.Color).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();



            return View(product);
        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {


            Product existproduct = _context.Products.FirstOrDefault(x => x.Id == product.Id);

            if (existproduct == null) return NotFound();

            //if (existproduct.BackImage != null)
            //{
            //    string path = Path.Combine(_env.WebRootPath, "uploads/slider", existproduct.BackImage);
            //    if (System.IO.File.Exists(path))
            //    {
            //        System.IO.File.Delete(path);
            //    }
            //}


            _context.Products.Remove(existproduct);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
