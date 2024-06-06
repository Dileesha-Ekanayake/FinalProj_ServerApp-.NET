using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GendersController : Controller
    {
        private readonly DatabaseUtil _context;

        public GendersController(DatabaseUtil context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Genders.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var genders = await _context.Genders
                .Select(d => new Gender { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(genders);
        }

    }
}
