using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LayiheBackEnd.Models
{
    public class Policy
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 300)]
        public string Desc { get; set; }
        [StringLength(maximumLength: 200)]
        public string Title { get; set; }
        [StringLength(maximumLength: 100)]
        public string Icon { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
