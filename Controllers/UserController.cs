using Data;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace b_robot_api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private const string _errorUserExists = "Пользователь с таким адресом электронной почты уже зарегистрирован.";

  private readonly UserContext _context;

  public UsersController(UserContext context)
  {
    _context = context;
  }

  // GET: api/users
  [HttpGet]
  public async Task<ActionResult<IEnumerable<UserViewItem>>> GetUsers()
  {
    return await _context.UserViewItems();
  }

  // GET: api/users/5
  [HttpGet("{id}")]
  public async Task<ActionResult<User>> GetUser(int id)
  {
    var user = await _context.Users.FindAsync(id);

    if (user == null)
    {
      return NotFound();
    }

    return user;
  }

  // POST: api/users
  [HttpPost]
  public async Task<ActionResult<Reference>> PostUser(User user)
  {
    if (_context.Exists(user.Email)) return Conflict(new { message = _errorUserExists }) ;

    string hashToStoreInDb = BCrypt.Net.BCrypt.HashPassword(user.Password);
    user.Password = hashToStoreInDb;

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    var reference = new Reference(user.Id) { Id = user.Id };
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, reference);
  }

  // PUT: api/users/5
  [HttpPut("{id}")]
  public async Task<IActionResult> PutUser(int id, UserUpdateItem update)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null) return NotFound();

    user.FirstName = update.FirstName;
    user.LastName = update.LastName;
    user.Patronimic = update.Patronimic;
    user.Email = update.Email;
    user.ApiKey = update.ApiKey;

    if (update.Password)
    _context.Entry(user).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!_context.Exists(id))
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
    if (user == null) return NotFound();

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();

    return NoContent();
  }

}
