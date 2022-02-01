using LayiheBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LayiheBackEnd.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Policy> Policies { get; set; }
        public List<Banner> Banners { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> Topseller { get; set; }


    }
}
