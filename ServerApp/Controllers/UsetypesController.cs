using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsetypesController : Controller
    {
        private readonly DatabaseUtil _context;

        public UsetypesController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Usetypes.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var usrstypes = await _context.Usetypes
                .Select(d => new Usetype { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(usrstypes);
        }
    }
}
