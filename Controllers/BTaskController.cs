using b_robot_api.Authorization;
using b_robot_api.Data;
using b_robot_api.Jobs;
using b_robot_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace b_robot_api.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BTasksController : ControllerBase
{
  private readonly BTaskContext btaskContext;

  public BTasksController(BTaskContext context)
  {
    btaskContext = context;
  }

  // GET: api/btasks
  [HttpGet]
  public async Task<ActionResult<IEnumerable<BTask>>> GetBTasks()
  {
    return await btaskContext.BTasks.ToListAsync();
  }

  // GET: api/btasks/5
  [HttpGet("{id}")]
  public async Task<ActionResult<BTask>> GetBTask(int id)
  {
    var btask = await btaskContext.BTasks.FindAsync(id);

    if (btask == null)
    {
      return NotFound();
    }

    return btask;
  }

  // POST: api/btasks/add
  [HttpPost("add")]
  public async Task<ActionResult<BTask>> PostBTask(BTask btask)
  {
    btaskContext.BTasks.Add(btask);
    await btaskContext.SaveChangesAsync();

    var user = await btaskContext.Users.FindAsync(btask.UserId);
    if (user == null) return BadRequest(new { message = $"Не удалось найти информацию о пользователе [{btask.UserId}]" } );

    BJob bj = new (btask, user);
    BJobs.AddBJob(bj);
//    _bJob.Start();

    var reference = new Reference(user.Id) { Id = btask.Id };
    return CreatedAtAction(nameof(GetBTask), new { id = btask.Id }, reference);

  }

  // PUT: api/btasks/5
  [HttpPut("{id}")]
  public async Task<IActionResult> PutBTask(int id, BTask btask)
  {
    if (id != btask.Id)
    {
      return BadRequest();
    }

    btaskContext.Entry(btask).State = EntityState.Modified;

    try
    {
      await btaskContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!BTaskExists(id))
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

  // DELETE: api/btasks/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteBTask(int id)
  {
    var btask = await btaskContext.BTasks.FindAsync(id);
    if (btask == null)
    {
      return NotFound();
    }

    btaskContext.BTasks.Remove(btask);
    await btaskContext.SaveChangesAsync();

    try {
      BJobs.RemoveBJob(id);
    }
    catch(KeyNotFoundException) {
            Console.WriteLine("Did not find BJob with id = {0}", id);

    }
    return NoContent();
  }

  private bool BTaskExists(int id)
  {
    return btaskContext.BTasks.Any(e => e.Id == id);
  }

  // dummy method to test the connection
  [HttpGet("hello")]
  public string Hello()
  {
    return "Hello, World!";
  }
}
