using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerApp.Controllers;
using ServerApp;
using ServerApp.Security;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<DatabaseUtil>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 3, 0))));

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtTokenUtil>();
builder.Services.AddScoped<IUserConfirmation<User>, DefaultUserConfirmation<User>>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
})
.AddEntityFrameworkStores<DatabaseUtil>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<SignInManager<User>, MySignInManager>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, IdentityRole>>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();

SecurityConfig.Configure(builder.Services, configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SecurityConfig.UseSecurity(app);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtAuthorizationFilter>();

app.MapControllers();
app.Run();
