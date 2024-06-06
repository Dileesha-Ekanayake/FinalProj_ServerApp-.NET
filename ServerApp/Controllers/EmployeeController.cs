using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        private readonly DatabaseUtil _context;
        private readonly IMapper _mapper;

        public EmployeeController(IMapper mapper, DatabaseUtil context)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] Dictionary<string, string> qryParams)
        {
            var employees = await _context.Employees
                                          .Include(e => e.Designation)
                                          .Include(e => e.Empstatus)
                                          .Include(e => e.Emptype)
                                          .Include(e => e.Gender)
                                          .ToListAsync();
            
            if (qryParams == null || !qryParams.Any())
            {
                return Json(employees);
            }
            qryParams.TryGetValue("number", out var number);
            qryParams.TryGetValue("genderid", out var genderid);
            qryParams.TryGetValue("fullname", out var fullname);
            qryParams.TryGetValue("designationid", out var designationid);
            qryParams.TryGetValue("nic", out var nic);

            var filteredEmployees = employees.AsQueryable();

            if (!string.IsNullOrEmpty(number))
            {
                filteredEmployees = filteredEmployees.Where(e => e.Number == number);
            }

            if (!string.IsNullOrEmpty(genderid) && int.TryParse(genderid, out var genderId))
            {
                filteredEmployees = filteredEmployees.Where(e => e.GenderId == genderId);
            }

            if (!string.IsNullOrEmpty(fullname))
            {
                filteredEmployees = filteredEmployees.Where(e => e.Fullname.Contains(fullname, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(designationid) && int.TryParse(designationid, out var designationId))
            {
                filteredEmployees = filteredEmployees.Where(e => e.DesignationId == designationId);
            }

            if (!string.IsNullOrEmpty(nic))
            {
                filteredEmployees = filteredEmployees.Where(e => e.Nic == nic);
            }

            return Json(filteredEmployees.ToList());
        }


        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Designation)
                .Include(e => e.Empstatus)
                .Include(e => e.Emptype)
                .Include(e => e.Gender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return Json(employee);
        }

        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var employees = await _context.Employees
                .Select(e => new Employee { Id = e.Id, Callingname = e.Callingname })
                .ToListAsync();

            return Json(employees);
        }

        [HttpPost("Create")]
        //[PreAuthorize("employee-insert")]
        public async Task<Dictionary<string,string>> Create([FromBody] Employee employee)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            if (await _context.Employees.FirstOrDefaultAsync(e => e.Number == employee.Number) != null)
            {
                errors.Add("Existing Number");
            }

            if (await _context.Employees.FirstOrDefaultAsync(e => e.Nic == employee.Nic) != null)
            {
                errors.Add("Existing NIC");
            }

            if (ModelState.IsValid)
            {
               employee.Designation = await _context.Designations.FirstOrDefaultAsync(d => d.Name == employee.Designation.Name);
               employee.Gender = await _context.Genders.FirstOrDefaultAsync(g => g.Name == employee.Gender.Name);
               employee.Emptype = await _context.Emptypes.FirstOrDefaultAsync(e => e.Name == employee.Emptype.Name);
               employee.Empstatus = await _context.Empstatus.FirstOrDefaultAsync(e => e.Name == employee.Empstatus.Name);

                if(!errors.Any())
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    errors.Add("Server Validation Errors");
                }
            }
            response.Add("id", employee.Id.ToString());
            response.Add("url", "/employees/" + employee.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        [HttpPut("Edit")]
        public async Task<Dictionary<string,string>> Edit([FromBody] Employee employee)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            if (ModelState.IsValid)
            {
                var existingEmployee = await _context.Employees.FindAsync(employee.Id);
                if (existingEmployee == null)
                {
                     errors.Add("Not Found");
                }

                var emp1 = await _context.Employees.FirstOrDefaultAsync(e => e.Number == employee.Number && e.Id != employee.Id);
                var emp2 = await _context.Employees.FirstOrDefaultAsync(e => e.Nic == employee.Nic && e.Id != employee.Id);
                Console.Write(employee.Photo);

                if (emp1 != null)
                {
                    errors.Add("Existing Number");
                }
                if (emp2 != null)
                {
                    errors.Add("Existing NIC");
                }

                try
                {
                    _mapper.Map(employee, existingEmployee);

                    existingEmployee.Designation = await _context.Designations.FirstOrDefaultAsync(d => d.Name == employee.Designation.Name);
                    existingEmployee.Gender = await _context.Genders.FirstOrDefaultAsync(g => g.Name == employee.Gender.Name);
                    existingEmployee.Emptype = await _context.Emptypes.FirstOrDefaultAsync(e => e.Name == employee.Emptype.Name);
                    existingEmployee.Empstatus = await _context.Empstatus.FirstOrDefaultAsync(e => e.Name == employee.Empstatus.Name);
                    
                    if(!errors.Any()){
                        _context.Update(existingEmployee);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        errors.Add("Server Validation Errors");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        errors.Add("Not Found");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            response.Add("id", employee.Id.ToString());
            response.Add("url", "/employees/" + employee.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        [HttpDelete("Delete/{id}")]
        public async Task<Dictionary<string, string>> DeleteConfirmed(int id)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                errors.Add("Employee not found");
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Employee.Id == id);
                if (user != null)
                {
                    errors.Add("There is a User Account associated with this Employee");
                }
            }

            if (!errors.Any())
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

            }
            else
            {
                response.Add("errors", string.Join("<br>", errors));
            }

            response.Add("id", employee.Id.ToString());
            response.Add("url", "/employees/" + employee.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }


        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
