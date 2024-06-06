using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DesignationsController : Controller
    {
        private readonly DatabaseUtil _context;

        public DesignationsController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Designations.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var designations = await _context.Designations
                .Select(d => new Designation { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(designations);
        }
    }
}
