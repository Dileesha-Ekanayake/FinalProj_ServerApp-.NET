using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp;

namespace ServerApp.Controllers
{
    public class PrivilegesController : Controller
    {
        private readonly DatabaseUtil _context;

        public PrivilegesController(DatabaseUtil context)
        {
            _context = context;
        }

        [HttpGet("Privileges")]
        public async Task<IActionResult> Index([FromQuery] Dictionary<string, string> qryParams)
        {
            var privileges = await _context.Privileges
                .Include(p => p.Module)
                .Include(p => p.Operation)
                .Include(p => p.Role)
                .ToListAsync();

            if (qryParams == null || !qryParams.Any())
            {
                return Json(privileges);
            }

            qryParams.TryGetValue("roleid", out var roleid);
            qryParams.TryGetValue("moduleid", out var moduleid);
            qryParams.TryGetValue("operationid", out var operationid);

            var filteredPrivileges = privileges.AsQueryable();

            if (!string.IsNullOrEmpty(roleid) && int.TryParse(roleid, out var roleId))
            {
                filteredPrivileges = filteredPrivileges.Where(p => p.RoleId == roleId);
            }
            if (!string.IsNullOrEmpty(moduleid) && int.TryParse(moduleid, out var moduleId))
            {
                filteredPrivileges = filteredPrivileges.Where(p => p.ModuleId == moduleId);
            }
            if (!string.IsNullOrEmpty(operationid) && int.TryParse(operationid, out var operationId))
            {
                filteredPrivileges = filteredPrivileges.Where(p => p.OperationId == operationId);
            }

            return Json(filteredPrivileges.ToList());
        }

        [HttpPost("Create")]
        public async Task<Dictionary<string, string>> Create([FromBody] Privilege privilege)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            if (ModelState.IsValid && !errors.Any())
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == privilege.Role.Name);
                var module = await _context.Modules.FirstOrDefaultAsync(m => m.Name == privilege.Module.Name);
                var operation = await _context.Operations.FirstOrDefaultAsync(o => o.Name == privilege.Operation.Name);

                var prvlge = new Privilege()
                {
                    Authority = privilege.Authority,
                    Role = role,
                    Module = module,
                    Operation = operation
                };

                _context.Add(prvlge);
                await _context.SaveChangesAsync();
            }
            else
            {
                errors.Add("Server Validation Errors");
            }

            response.Add("id", privilege.Id.ToString());
            response.Add("url", "/privileges/" + privilege.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        [HttpPut("Edit")]
        public async Task<Dictionary<string, string>> Edit([FromBody] Privilege privilege)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            var extPrivilege = await _context.Privileges.FirstOrDefaultAsync(p => p.Id == privilege.Id);

            if (extPrivilege == null)
            {
                errors.Add("Not Found");
                response.Add("errors", string.Join("<br>", errors));
                return response;
            }

            if (ModelState.IsValid && !errors.Any())
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == privilege.Role.Name);
                var module = await _context.Modules.FirstOrDefaultAsync(m => m.Name == privilege.Module.Name);
                var operation = await _context.Operations.FirstOrDefaultAsync(o => o.Name == privilege.Operation.Name);

                extPrivilege.Authority = privilege.Authority;
                extPrivilege.Role = role;
                extPrivilege.Module = module;
                extPrivilege.Operation = operation;

                _context.Update(privilege);
                await _context.SaveChangesAsync();
            }
            else
            {
                errors.Add("Server Validation Errors");
            }
            response.Add("id", privilege.Id.ToString());
            response.Add("url", "/privileges/" + privilege.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        [HttpDelete("Delete/{id}")]
        public async Task<Dictionary<string, string>> DeleteConfirmed(int id)
        {

            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege != null)
            {
                _context.Privileges.Remove(privilege);
                await _context.SaveChangesAsync();
            }
            else
            {
                errors.Add("Server Validation Errors");
            }

            response.Add("id", privilege.Id.ToString());
            response.Add("url", "/privileges/" + privilege.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        private bool PrivilegeExists(int id)
        {
            return _context.Privileges.Any(e => e.Id == id);
        }
    }
}
