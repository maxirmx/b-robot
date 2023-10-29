using Data;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace b_robot_api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly UserContext _context;

  public AuthController(UserContext context)
  {
    _context = context;
  }

  // POST: api/users
  [HttpPost]
  public IActionResult Authorize(Credentials crd)
  {
    User? user = _context.Users.Where(u => u.Email == crd.Email).SingleOrDefault();

    if (user == null) return Unauthorized();
    if (!BCrypt.Net.BCrypt.Verify(crd.Password, user.Password)) return Unauthorized();

    return NoContent();
  }

  // PUT: api/users/5
  [HttpPut("{id}")]
  public async Task<IActionResult> PutUser(int id, User user)
  {
    if (id != user.Id)
    {
      return BadRequest();
    }

    _context.Entry(user).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!UserExists(id))
      {
        return NotFound();
      }
      else
      {
        throw;
      }
    }

    return NoContent();
  }

  // DELETE: api/users/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
      return NotFound();
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool UserExists(int id)
  {
    return _context.Users.Any(e => e.Id == id);
  }

  // dummy method to test the connection
  [HttpGet("hello")]
  public string Test()
  {
    return "Hello World!";
  }
}
