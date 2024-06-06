using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationsController : Controller
    {
        private readonly DatabaseUtil _context;

        public OperationsController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] Dictionary<string, string> qryParams)
        {
            var operations = await _context.Operations.Include(op => op.Module).Include(op => op.Opetype).ToListAsync();

            if (qryParams == null || !qryParams.Any())
            {
                return Json(operations);
            }

            qryParams.TryGetValue("moduleid", out var moduleid);
            qryParams.TryGetValue("operation", out var operation);

            var filteredOperations = operations.AsQueryable();

            if (!string.IsNullOrEmpty(moduleid) && int.TryParse(moduleid, out int moduleIdInt))
            {
                filteredOperations = filteredOperations.Where(e => e.ModuleId == moduleIdInt);
            }
            if (!string.IsNullOrEmpty(operation))
            {
                filteredOperations = filteredOperations.Where(e => e.Opetype.Name == operation);
            }

            return Json(filteredOperations.ToList());
        }

        [HttpGet("list/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operations = await _context.Operations
                 .Select(op => new Operation { Id = op.Id, Name = op.Name, Opetype = op.Opetype, ModuleId = (int?)op.ModuleId })
                 .Where(op => op.ModuleId == id)
                 .ToListAsync();

            return Json(operations);
        }


        [HttpPost("Create")]
        public async Task<Dictionary<string, string>> Create([FromBody] Operation operation)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            if (ModelState.IsValid && !errors.Any())
            {
                var module = await _context.Modules.FirstOrDefaultAsync(m => m.Name == operation.Module.Name);
                var optype = await _context.Opetypes.FirstOrDefaultAsync(opt => opt.Name == operation.Opetype.Name);

                var opration = new Operation()
                {
                    Name = operation.Name,
                    Module = module,
                    Opetype = optype
                };

                _context.Add(opration);
                await _context.SaveChangesAsync();
                
            }
            else
            {
                errors.Add("Server Validation Errors");
            }
            response.Add("id", operation.Id.ToString());
            response.Add("url", "/operations/" + operation.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        [HttpPut("Edit")]
        public async Task<Dictionary<string, string>> Edit([FromBody] Operation operation)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            var extOperation = await _context.Operations.FirstOrDefaultAsync(op => op.Id == operation.Id);

            if (extOperation == null)
            {
                errors.Add("Not Found");
                response.Add("errors", string.Join("<br>", errors));
                return response;
            }

            if (ModelState.IsValid && !errors.Any())
            {
                var module = await _context.Modules.FirstOrDefaultAsync(m => m.Name == operation.Module.Name);
                var optype = await _context.Opetypes.FirstOrDefaultAsync(opt => opt.Name == operation.Opetype.Name);

                extOperation.Name = operation.Name;
                extOperation.Module = module;
                extOperation.Opetype = optype;
               
                _context.Update(extOperation);
                await _context.SaveChangesAsync();                              
            }
            else
            {
                errors.Add("Server Validation Errors");
            }

            response.Add("id", operation.Id.ToString());
            response.Add("url", "/operations/" + operation.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }


        [HttpDelete("Delete/{id}")]
        public async Task<Dictionary<string, string>> DeleteConfirmed(int id)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            var operation = await _context.Operations.FindAsync(id);
            if (operation != null && !errors.Any())
            {
                _context.Operations.Remove(operation);
                await _context.SaveChangesAsync();
            }
            else
            {
                errors.Add("Server Validation Errors");
            }

            response.Add("id", operation.Id.ToString());
            response.Add("url", "/operations/" + operation.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        private bool OperationExists(int id)
        {
            return _context.Operations.Any(e => e.Id == id);
        }
    }
}
