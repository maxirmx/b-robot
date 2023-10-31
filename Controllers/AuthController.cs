using b_robot_api.Authorization;
using b_robot_api.Data;
using b_robot_api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace b_robot_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly UserContext _context;
  private readonly IJwtUtils _jwtUtils;


  public AuthController(UserContext context, IJwtUtils jwtUtils)
  {
    _context = context;
    _jwtUtils = jwtUtils;
  }

  // POST: api/auth/login
  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<ActionResult<UserViewItem>> Login(Credentials crd)
  {
    User? user = await _context.Users.Where(u => u.Email == crd.Email).SingleOrDefaultAsync();

    if (user == null) return Unauthorized(new {message = "Неправильный адрес электронной почты или пароль" });
    if (!BCrypt.Net.BCrypt.Verify(crd.Password, user.Password)) return Unauthorized();

    UserViewItemWithJWT userViewItem = new(user) {
      Token = _jwtUtils.GenerateJwtToken(user)
    };
    return userViewItem;
  }

  // GET: api/auth/check
  [HttpGet("check")]
  public IActionResult Check()
  {
    return NoContent();
  }

  // dummy method to test the connection
  [HttpGet("hello")]
  [AllowAnonymous]
  public ActionResult Test()
  {
    return Ok(new {message = "Hello, world!" });
  }
}
