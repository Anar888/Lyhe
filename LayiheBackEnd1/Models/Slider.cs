using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LayiheBackEnd.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [StringLength(maximumLength:150)]
        public string SubTitle { get; set; }
        [StringLength(maximumLength: 200)]
        public string Title { get; set; }
        [StringLength(maximumLength: 300)]
        public string Descrption { get; set; }
        public string BackImage { get; set; }
        [StringLength(maximumLength: 250)]
        public string BtnUrl { get; set; }
        [StringLength(maximumLength: 35)]
        public string BtnText { get; set; }
        public byte Order { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
