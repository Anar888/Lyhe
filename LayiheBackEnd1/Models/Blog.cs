using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LayiheBackEnd.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 200)]
        public string RedirectUrl { get; set; }
        [StringLength(maximumLength: 200)]
        public string Image { get; set; }
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        [StringLength(maximumLength: 50)]
        public string ByWho { get; set; }
       
        public DateTime Date { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }



    }
}
