using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LayiheBackEnd.Models
{
    public class Banner
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 200)]
        public string Title1 { get; set; }
        [StringLength(maximumLength: 200)]
        public string Title2 { get; set; }
        [StringLength(maximumLength: 200)]
        public string SubTitle { get; set; }
        [StringLength(maximumLength: 200)]
        public string BackImage { get; set; }
        [StringLength(maximumLength: 200)]
        public string RedirectUrl { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
