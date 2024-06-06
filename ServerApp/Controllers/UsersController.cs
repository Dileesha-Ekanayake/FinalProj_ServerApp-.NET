using System;
using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp;
using ServerApp.Security;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly DatabaseUtil _context;
        private readonly IMapper _mapper;

        public UsersController(DatabaseUtil context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _context.Users
                                .Include(u => u.Userroles)
                                    .ThenInclude(ur => ur.Role)
                                        .ThenInclude(r => r.Privileges)
                                .Include(u => u.Usestatus)
                                .Include(u => u.Usetype)
                                .Include(u => u.Employee)
                                    .ThenInclude(e => e.Gender)
                                .Include(e => e.Employee)
                                    .ThenInclude(e => e.Designation)
                                .Include(e => e.Employee)
                                    .ThenInclude(e => e.Empstatus)
                                .Include(e => e.Employee)
                                    .ThenInclude(e => e.Emptype)
                                .Select(u => new User
                                {
                                    Employee = u.Employee,
                                    SysUserName = u.SysUserName,
                                    Password = u.Password,
                                    Salt = u.Salt,
                                    Docreated = u.Docreated,
                                    Tocreated = u.Tocreated,
                                    Description = u.Description,
                                    Usestatus = u.Usestatus,
                                    Usetype = u.Usetype,
                                    Userroles = u.Userroles
                                    
                                    
                                });
            return Json(await users.ToListAsync());
        }

        [HttpGet("/Empbyuser/{username}")]
        public async Task<IActionResult> Empbyuser(string username)
        {
            var user = await _context.Users
                                .Where(u => u.SysUserName == username)
                                //.Select(u => new User
                               // {
                               //     EmployeeId = u.EmployeeId,
                              //      SysUserName = u.SysUserName,
                              //      Password = u.Password,
                               //     Salt = u.Salt,
                               //     Docreated = u.Docreated,
                               //     Tocreated = u.Tocreated,
                               //     Description = u.Description,
                               //     UsestatusId = u.UsestatusId,
                               //     UsetypeId = u.UsetypeId
                               // })
                                .FirstOrDefaultAsync();

            var employee = await _context.Employees.Where(e => e.Id == user.EmployeeId)
                                    .Include(e => e.Gender)
                                    .Include(e => e.Designation)
                                    .Include(e => e.Emptype)
                                    .Include(e => e.Empstatus)
                                    .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            if (employee == null)
            {               
                return NotFound(); 
            }

            return Json(employee);
        }

        [HttpPost("Create")]
        public async Task<Dictionary<string, string>> Create([FromBody] User usr)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.SysUserName == usr.SysUserName);

                if (existingUser != null)
                {
                    errors.Add("Existing Username");
                }

                if (!errors.Any())
                {
                    var userType = await _context.Usetypes.FirstOrDefaultAsync(ut => ut.Name == usr.Usetype.Name);
                    var userStatus = await _context.Usestatus.FirstOrDefaultAsync(us => us.Name == usr.Usestatus.Name);
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == usr.Employee.Id);

                    if (!errors.Any())
                    {
                        string salt = BCrypt.Net.BCrypt.GenerateSalt(10, 'x');
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(salt + usr.Password);

                        var user = new User()
                        {
                            SysUserName = usr.SysUserName,
                            Password = hashedPassword,
                            Salt = salt,
                            Docreated = usr.Docreated,
                            Tocreated = usr.Tocreated,
                            Description = usr.Description,
                            Usestatus = userStatus,
                            Usetype = userType,
                            Employee = employee
                        };

                        foreach (var userRole in usr.Userroles)
                        {
                            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == userRole.Role.Name);
                            if (role != null)
                            {
                                user.Userroles.Add(new Userrole { RoleId = role.Id });
                            }
                            else
                            {
                                errors.Add("Role " + userRole.Role.Name + " not found");
                            }
                        }

                        if (!errors.Any())
                        {
                            _context.Add(user);
                            await _context.SaveChangesAsync();

                            response.Add("id", user.Id.ToString());
                            response.Add("url", "/users/" + user.Id);
                            response.Add("errors", string.Join("<br>", errors));

                            return response;
                        }
                    }
                }

                response.Add("errors", "Server Validation Errors: <br>" + string.Join("<br>", errors));
                return response;
            }

            return response;
        }


        [HttpPut("Edit")]
        public async Task<Dictionary<string, string>> Edit(User usr)
        {
            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            if (usr == null)
            {
                errors.Add("User Not Found");
            }

            if (ModelState.IsValid)
            {
                var usertype = await _context.Usetypes.FirstOrDefaultAsync(ut => ut.Name == usr.Usetype.Name);
                var usestatus = await _context.Usestatus.FirstOrDefaultAsync(us => us.Name == usr.Usestatus.Name);
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == usr.Employee.Id);

                if (usertype == null || usestatus == null || employee == null)
                {
                    errors.Add("Not Found User Details");
                }

                if (!errors.Any()) {

                    try
                    {
                        var user = await _context.Users
                                                 .Include(u => u.Userroles)
                                                 .ThenInclude(ur => ur.Role)
                                                 .FirstOrDefaultAsync(u => u.SysUserName == usr.SysUserName);

                        if (user == null)
                        {
                            errors.Add("User Not Found");
                        }
                        string salt = BCrypt.Net.BCrypt.GenerateSalt(10, 'x');
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(salt + usr.Password);

                        user.SysUserName = usr.SysUserName;
                        user.Docreated = usr.Docreated;
                        user.Tocreated = usr.Tocreated;
                        user.Description = usr.Description;
                        user.Usestatus = usestatus;
                        user.Usetype = usertype;
                        user.Employee = employee;
                        user.Salt = salt;
                        user.Password = hashedPassword;


                        var newRoles = usr.Userroles.Select(ur => ur.Role.Name).ToList();
                        var existingUserRoles = user.Userroles.ToList();

                        foreach (var existingUserRole in existingUserRoles)
                        {
                            if (!newRoles.Contains(existingUserRole.Role.Name))
                            {
                                _context.Userroles.Remove(existingUserRole);
                            }
                        }

                        foreach (var newRoleName in newRoles)
                        {
                            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == newRoleName);
                            if (role != null)
                            {
                                var existingUserRole = existingUserRoles.FirstOrDefault(ur => ur.Role.Name == newRoleName);
                                if (existingUserRole == null)
                                {
                                    user.Userroles.Add(new Userrole { UserId = user.Id, RoleId = role.Id });
                                }
                            }
                            else
                            {
                                errors.Add("No User roles Founded");
                            }
                        }

                        _context.Update(user);
                        await _context.SaveChangesAsync();

                        response.Add("id", user.Id.ToString());
                        response.Add("url", "/users/" + user.Id);
                        response.Add("errors", string.Join("<br>", errors));

                        return response;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        response.Add("errors", "Server Validation Errors: <br>" + string.Join("<br>", errors));
                        return response;
                    }
                }
            }
           
            return response;
        }

        [HttpDelete("Delete/{username}")]
        public async Task<Dictionary<string, string>> Delete(string? username)
        {

            var response = new Dictionary<string, string>();
            var errors = new List<string>();

            if (username == null)
            {
                errors.Add("Not Found");
            }

            var user = await _context.Users
                                .Include(u => u.Userroles)
                                    .ThenInclude(ur => ur.Role)
                                        .ThenInclude(r => r.Privileges)
                                .Where(u => u.SysUserName == username)
                                .Select(u => new User
                                {
                                    Id = u.Id,
                                    EmployeeId = u.EmployeeId,
                                    SysUserName = u.SysUserName,
                                    Password = u.Password,
                                    Salt = u.Salt,
                                    Docreated = u.Docreated,
                                    Tocreated = u.Tocreated,
                                    Description = u.Description,
                                    UsestatusId = u.UsestatusId,
                                    UsetypeId = u.UsetypeId
                                })
                                .FirstOrDefaultAsync();

            if (user == null)
            {
                errors.Add("Not Found");
            }
            if(!errors.Any()){
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                errors.Add("Server Validation Errors");
            }
            response.Add("id", user.Id.ToString());
            response.Add("url", "/users/" + user.Id);
            response.Add("errors", string.Join("<br>", errors));

            return response;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
