using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModulesController : Controller
    {
        private readonly DatabaseUtil _context;

        public ModulesController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Modules.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var modules = await _context.Modules
                .Select(d => new Module { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(modules);
        }
    }
}
