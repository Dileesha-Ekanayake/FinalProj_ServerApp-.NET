using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmpstatusController : Controller
    {
        private readonly DatabaseUtil _context;

        public EmpstatusController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Json(await _context.Empstatus.ToListAsync());
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var empstatuses = await _context.Empstatus
                .Select(d => new Empstatus { Id = d.Id, Name = d.Name })
                .ToListAsync();

            return Json(empstatuses);
        }
    }

}