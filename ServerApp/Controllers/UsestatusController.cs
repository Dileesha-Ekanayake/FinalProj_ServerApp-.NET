using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsestatusController : Controller
    {
        private readonly DatabaseUtil _context;

        public UsestatusController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Usestatus.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var usrstatuses = await _context.Usestatus
                .Select(d => new Usestatus { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(usrstatuses);
        }
    }
}
