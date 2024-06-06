using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmptypesController : Controller
    {
        private readonly DatabaseUtil _context;

        public EmptypesController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Emptypes.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var empstypes = await _context.Emptypes
                .Select(d => new Emptype { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(empstypes);
        }
    }
}
