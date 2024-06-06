using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpetypesController : Controller
    {
        private readonly DatabaseUtil _context;

        public OpetypesController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Opetypes.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var optypes = await _context.Opetypes
                .Select(d => new Opetype { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(optypes);
        }
    }
}
