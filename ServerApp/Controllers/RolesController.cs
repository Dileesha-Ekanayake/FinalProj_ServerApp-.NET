using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : Controller
    {
        private readonly DatabaseUtil _context;

        public RolesController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Roles.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var roles = await _context.Roles
                .Select(d => new Role { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(roles);
        }
    }
}
