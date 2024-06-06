using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServerApp;
using ServerApp.Security;

public class JwtAuthenticationFilter : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserService _userService;
    private readonly JwtTokenUtil _jwtTokenUtil;

    public JwtAuthenticationFilter(SignInManager<User> signInManager, UserService userService, JwtTokenUtil jwtTokenUtil)
    {
        _signInManager = signInManager;
        _userService = userService;
        _jwtTokenUtil = jwtTokenUtil;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RequestUserLogin loginRequest)
    {
        if (loginRequest != null)
        {
            User? user = null;
            
            try
            {
                user = (await _userService.GetUserByNameAsync(loginRequest.Username));

                if (user != null)
                {
                    bool isPasswordCorrect = false;

                    string storedSalt = user?.Salt;
                    string storedHashedPassword = user?.Password;

                    string passwordToVerify = storedSalt + loginRequest.Password;

                    isPasswordCorrect = BCrypt.Net.BCrypt.Verify(storedSalt + loginRequest.Password, storedHashedPassword);

                    if (isPasswordCorrect)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, isPersistent: false, lockoutOnFailure: true);

                        if (result.Succeeded)
                        {

                            return SuccessfulAuthentication(await _userService.LoadUserDetailsByUser(user));
                        }
                        else
                        {
                            return BadRequest("Invalid username or password.");
                        }
                    }
                    else
                    {
                        return BadRequest("Invalid username or password.");
                    }
                }
                else
                {
                    return BadRequest("User not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        else
        {
            return BadRequest("Invalid Attempt" );
        }
    }

    private IActionResult SuccessfulAuthentication(IUser user)
    {
        var token = _jwtTokenUtil.GenerateToken(user);

        HttpContext.Response.Headers.Append("Authorization", "Bearer " + token);

        HttpContext.Response.Headers.Append("Access-Control-Expose-Headers", "Authorization");

        return Ok(new { token });
    }
}
