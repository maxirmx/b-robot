using b_robot_api.Authorization;
using b_robot_api.Data;
using b_robot_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace b_robot_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private const string _errorUserExists = "Пользователь с таким адресом электронной почты уже зарегистрирован.";
  private const string _errorUserNotFound = "Не удалось найти пользователя.";
  private const string _errorUnsufficienPriviledges = "Недостаточно прав для выполнения операции";

  private readonly UserContext userContext;
  private readonly int curUserId;

  private ObjectResult _403()
  {
    return StatusCode(StatusCodes.Status403Forbidden, new { message = _errorUnsufficienPriviledges });
  }

  private ObjectResult _404()
  {
    return StatusCode(StatusCodes.Status404NotFound, new { message = _errorUserNotFound });
  }
  public UsersController(IHttpContextAccessor httpContextAccessor, UserContext uContext)
  {
    userContext = uContext;
    var uid = httpContextAccessor.HttpContext.Items["UserId"];
    curUserId = (uid != null) ? (int)uid : 0;
  }

  // GET: api/users
  [HttpGet]
  public async Task<ActionResult<IEnumerable<UserViewItem>>> GetUsers()
  {
    var ch = await userContext.CheckAdmin(curUserId);
    if (!ch.Value)  return _403();

    return await userContext.UserViewItems();
  }

  // GET: api/users/5
  [HttpGet("{id}")]
  public async Task<ActionResult<UserViewItem>> GetUser(int id)
  {
    var ch = await userContext.CheckAdminOrSameUser(id, curUserId);
    if (!ch.Value)  return _403();

    var user = await userContext.UserViewItem(id);
    return (user == null) ? _404() : user;
  }

    // POST: api/users
  [HttpPost("add")]
  public async Task<ActionResult<Reference>> PostUser(User user)
  {
    var ch = await userContext.CheckAdmin(curUserId);
    if (!ch.Value)  return _403();

    if (userContext.Exists(user.Email)) return Conflict(new { message = _errorUserExists }) ;

    string hashToStoreInDb = BCrypt.Net.BCrypt.HashPassword(user.Password);
    user.Password = hashToStoreInDb;

    userContext.Users.Add(user);
    await userContext.SaveChangesAsync();

    var reference = new Reference(user.Id) { Id = user.Id };
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, reference);
  }

  // PUT: api/users/5
  [HttpPut("{id}")]
  public async Task<IActionResult> PutUser(int id, UserUpdateItem update)
  {
    var ch = await userContext.CheckAdminOrSameUser(id, curUserId);
    if (!ch.Value)  return _403();

    var user = await userContext.Users.FindAsync(id);
    if (user == null) return _404();

    user.FirstName = update.FirstName;
    user.LastName = update.LastName;
    user.Patronimic = update.Patronimic;
    user.Email = update.Email;
    user.ApiKey = update.ApiKey;
    user.IsEnabled = update.IsEnabled;
    user.IsAdmin = update.IsAdmin;

    if (update.Password != null) user.Password = update.Password;
    if (update.ApiSecret != null) user.ApiSecret = update.ApiSecret;

    userContext.Entry(user).State = EntityState.Modified;

    await userContext.SaveChangesAsync();
    return NoContent();
  }

  // DELETE: api/users/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    var ch = await userContext.CheckAdmin(curUserId);
    if (!ch.Value)  return _403();

    var user = await userContext.Users.FindAsync(id);
    if (user == null) return _404();

    userContext.Users.Remove(user);
    await userContext.SaveChangesAsync();

    return NoContent();
  }

}
