using LayiheBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LayiheBackEnd.Services
{
    public class LayoutService
    {
        private readonly JuanContext _context;
        public LayoutService(JuanContext context)
        {
            _context = context;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }
    }

}
