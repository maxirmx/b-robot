using Data;
using Models;
using Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace b_robot_api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BTasksController : ControllerBase
{
  private readonly BTaskContext _context;
  public BTasksController(BTaskContext context)
  {
    _context = context;
  }

  // GET: api/btasks
  [HttpGet]
  public async Task<ActionResult<IEnumerable<BTask>>> GetBTasks()
  {
    return await _context.BTasks.ToListAsync();
  }

  // GET: api/btasks/5
  [HttpGet("{id}")]
  public async Task<ActionResult<BTask>> GetBTask(int id)
  {
    var btask = await _context.BTasks.FindAsync(id);

    if (btask == null)
    {
      return NotFound();
    }

    return btask;
  }

  // POST: api/btasks
  [HttpPost]
  public async Task<ActionResult<BTask>> PostBTask(BTask btask)
  {
    _context.BTasks.Add(btask);
    await _context.SaveChangesAsync();
    BJob _bJob = new (btask.Id, btask.ApiKey, btask.Strategy);
    BJobs.Instance.Add(btask.Id, _bJob);
    _bJob.Start();
    return CreatedAtAction(nameof(GetBTask), new { id = btask.Id }, btask);
  }

  // PUT: api/btasks/5
  [HttpPut("{id}")]
  public async Task<IActionResult> PutBTask(int id, BTask btask)
  {
    if (id != btask.Id)
    {
      return BadRequest();
    }

    _context.Entry(btask).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();
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
    var btask = await _context.BTasks.FindAsync(id);
    if (btask == null)
    {
      return NotFound();
    }

    _context.BTasks.Remove(btask);
    await _context.SaveChangesAsync();

Console.WriteLine();
foreach( KeyValuePair<int, BJob> kvp in BJobs.Instance )
{
    Console.WriteLine("Key = {0}, Value = {1}",
        kvp.Key, kvp.Value);
}

    try {
      BJob _bJob = BJobs.Instance[id];
      BJobs.Instance.Remove(id);
      _bJob.Stop();
    }
    catch(KeyNotFoundException) {
            Console.WriteLine("Did not find BJob with id = {0}", id);

    }
    return NoContent();
  }

  private bool BTaskExists(int id)
  {
    return _context.BTasks.Any(e => e.Id == id);
  }

  // dummy method to test the connection
  [HttpGet("hello")]
  public string Hello()
  {
    return "Hello, World!";
  }
}
