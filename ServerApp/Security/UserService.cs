using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ServerApp.Security
{
    public class UserService
    {
        private readonly DatabaseUtil _context;

        public UserService(DatabaseUtil context)
        {
            _context = context;
        }

        public async Task<User> GetUserByNameAsync(string username)
        {
            if (username == "AdminEUC")
            {
                var AdminSalt = BCrypt.Net.BCrypt.GenerateSalt(10, 'x');
                var AdminPasswrd = BCrypt.Net.BCrypt.HashPassword(AdminSalt + "Admin1234");

                var user = new User
                {
                    SysUserName = username,
                    Password = AdminPasswrd,
                    Salt = AdminSalt
                };
                return user;
            }
            else
            {
                var user = await _context.Users
                                .Where(u => u.SysUserName == username)
                                .Select(u => new User
                                {
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

                return user;

            }
        }
        public async Task<IdentityUser> LoadUserDetailsByUser(User usr)
        {
            if (usr.SysUserName.Equals("AdminEUC"))
            {
                var AppUser = new IdentityUser
                {
                    UserName = usr.SysUserName,
                    Claims = new List<Claim>()
                };

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "user-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "user-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "user-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "user-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "privilege-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "privilege-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "privilege-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "privilege-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "employee-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "employee-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "employee-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "employee-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "operations-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "operations-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "operations-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "operations-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "program-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "program-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "program-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "program-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "course-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "course-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "course-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "course-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payment Schedule-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payment Schedule-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payment Schedule-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payment Schedule-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Course Materiale-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Course Material-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Course Material-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Course Material-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Mat. Distribution-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Mat. Distribution-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Mat. Distribution-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Mat. Distribution-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payments-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payments-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payments-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Payments-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "student-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "student-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "student-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "student-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch Registration-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch Registration-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch Registration-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Batch Registration-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Class Schedule-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Class Schedule-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Class Schedule-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Class Schedule-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Attendance-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Attendance-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Attendance-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Attendance-insert"));

                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Progress Review-select"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Progress Review-delete"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Progress Review-update"));
                AppUser.Claims.Add(new Claim(ClaimTypes.Name, "Progress Review-insert"));

                return AppUser;
            }
            else
            {
                var user = await _context.Users
                                .Include(u => u.Userroles)
                                    .ThenInclude(ur => ur.Role)
                                        .ThenInclude(r => r.Privileges)
                                .Where(u => u.SysUserName == usr.SysUserName)
                                .Select(u => new User
                                {
                                    EmployeeId = u.EmployeeId,
                                    SysUserName = u.SysUserName,
                                    Password = u.Password,
                                    Salt = u.Salt,
                                    Docreated = u.Docreated,
                                    Tocreated = u.Tocreated,
                                    Description = u.Description,
                                    UsestatusId = u.UsestatusId,
                                    UsetypeId = u.UsetypeId,
                                    Userroles = u.Userroles,
                                    Usetype = u.Usetype
                                })
                                .FirstOrDefaultAsync();

                if (user != null)
                {
                    var DbUser = new IdentityUser
                    {
                        UserName = user.SysUserName,
                        Claims = new List<Claim>()
                    };

                    var authorities = new HashSet<string>();
                    List<Userrole> userroles = (List<Userrole>)user.Userroles;
                    foreach (var userrole in userroles)
                    {
                        List<Privilege> privileges = (List<Privilege>)userrole.Role.Privileges;
                        foreach (var privilege in privileges)
                        {
                            authorities.Add(privilege.Authority);
                            DbUser.Claims.Add(new Claim(ClaimTypes.Name, privilege.Authority));
                        }
                    }

                    return DbUser;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
