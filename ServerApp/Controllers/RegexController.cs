using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Util;

namespace ServerApp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class RegexController : Controller
    {
        [HttpGet("employee")]
        public async Task<ActionResult<Dictionary<string, Dictionary<string, string>>>> EmployeeReg()
        {
            return RegexProvider.Get(new Employee());
        }

        [HttpGet("users")]
        public async Task<ActionResult<Dictionary<string, Dictionary<string, string>>>> UserReg()
        {
            return RegexProvider.Get(new User());
        }
    }
}
