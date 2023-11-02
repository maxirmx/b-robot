// Copyright (C) 2023 Maxim [maxirmx] Samsonov (www.sw.consulting)
// All rights reserved.
// This file is a part of b-robot applcation
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 1. Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

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
public class BTasksController : BControllerBase
{
  private readonly BTaskContext btaskContext;
  public BTasksController(IHttpContextAccessor httpContextAccessor, UserContext uContext, BTaskContext bContext): base(httpContextAccessor, uContext)
  {
    btaskContext = bContext;
  }

  // GET: api/btasks
  [HttpGet]
  public async Task<ActionResult<IEnumerable<BTask>>> GetBTasks()
  {
    var res = await btaskContext.BTasksForUser(curUserId);

    foreach(BTask btask in res) {
      btask.IsRunning = BJobs.QueryRunning(btask.Id);
      btask.HasFailed = BJobs.QueryFailed(btask.Id);
      Console.WriteLine($"{btask.Id} -- HasFailed: {btask.HasFailed}");
    }

    return res;
  }

  // GET: api/btasks/all
  [HttpGet("all")]
  public async Task<ActionResult<IEnumerable<BTask>>> GetAllBTasks()
  {
    var ch = await userContext.CheckAdmin(curUserId);
    if (ch == null || !ch.Value)  return _403();

    var res = await btaskContext.BTasks.ToListAsync();

    foreach(BTask btask in res) {
      btask.IsRunning = BJobs.QueryRunning(btask.Id);
      btask.HasFailed = BJobs.QueryFailed(btask.Id);
    }

    return res;
  }

  // GET: api/btasks/5
  [HttpGet("{id}")]
  public async Task<ActionResult<BTask>> GetBTask(int id)
  {
    var btask = await btaskContext.BTasks.FindAsync(id);
    if (btask == null)  return NotFound();

    var ch = await userContext.CheckAdminOrSameUser(btask.UserId, curUserId);
    if (ch == null ||!ch.Value)  return _403();

    btask.IsRunning = BJobs.QueryRunning(id);
    btask.HasFailed = BJobs.QueryFailed(id);
    await btaskContext.SaveChangesAsync();

    return btask;
  }

  // POST: api/btasks/add
  [HttpPost("add")]
  public async Task<ActionResult<BTask>> AddBTask(BTask btask)
  {
    if (!userContext.CheckSameUser(btask.UserId, curUserId))  return _403();

    var user = await userContext.Users.FindAsync(curUserId);
    if (user == null) return _404User(curUserId);

    btaskContext.BTasks.Add(btask);
    await btaskContext.SaveChangesAsync();

    BJob bj = new (btask, user);
    BJobs.AddBJob(bj);
    if (btask.IsRunning) bj.Start();

    var reference = new Reference(user.Id) { Id = btask.Id };
    return CreatedAtAction(nameof(GetBTask), new { id = btask.Id }, reference);
  }

  // POST: api/btasks/start/5
  [HttpPost("start/{id}")]
  public async Task<ActionResult<BTask>> StartBTask(int id)
  {
    var btask = await btaskContext.BTasks.FindAsync(id);
    if (btask == null) return _404BTask(id);

    if (!userContext.CheckSameUser(btask.UserId, curUserId))  return _403();

    btask.IsRunning = true;
    btaskContext.Entry(btask).State = EntityState.Modified;
    try {
      await btaskContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)  {
      if (!btaskContext.Exists(id)) {
        return _404BTask(id);
      }
      else {
        throw;
      }
    }

    if (BJobs.QueryRunning(id)) return _409StartBTask(id);
    // [TODO]: No BJob
    BJobs.StartBJob(id);
    return NoContent();
  }

  // POST: api/btasks/stop/5
  [HttpPost("stop/{id}")]
  public async Task<ActionResult<BTask>> StopBTask(int id)
  {
    var btask = await btaskContext.BTasks.FindAsync(id);
    if (btask == null) return _404BTask(id);

    if (!userContext.CheckSameUser(btask.UserId, curUserId))  return _403();

    btask.IsRunning = false;
    btaskContext.Entry(btask).State = EntityState.Modified;
    try {
      await btaskContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)  {
      if (!btaskContext.Exists(id)) {
        return _404BTask(id);
      }
      else {
        throw;
      }
    }

    if (!BJobs.QueryRunning(id)) return _409StopBTask(id);
    // [TODO]: No BJob
    BJobs.StopBJob(id);
    return NoContent();
  }

  // PUT: api/btasks/5
  [HttpPut("{id}")]
  public async Task<ActionResult<BTask>> UpdateBTask(int id, BTask btask)
  {
    if (id != btask.Id) return BadRequest();

    if (!userContext.CheckSameUser(btask.UserId, curUserId))  return _403();

    var user = await userContext.Users.FindAsync(curUserId);
    if (user == null) return _404User(curUserId);

    btaskContext.Entry(btask).State = EntityState.Modified;
    try {
      await btaskContext.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)  {
      if (!btaskContext.Exists(id)) {
        return _404BTask(id);
      }
      else {
        throw;
      }
    }

    try {
      BJobs.RemoveBJob(id);
    }
    catch(KeyNotFoundException) {
    }

    BJob bj = new (btask, user);
    BJobs.AddBJob(bj);
    if (btask.IsRunning) bj.Start();

    return NoContent();
  }

  // DELETE: api/btasks/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteBTask(int id)
  {
    var btask = await btaskContext.BTasks.FindAsync(id);
    if (btask == null) return _404BTask(id);

    if (!userContext.CheckSameUser(btask.UserId, curUserId))  return _403();

    btaskContext.BTasks.Remove(btask);
    await btaskContext.SaveChangesAsync();

    try {
      BJobs.RemoveBJob(id);
    }
    catch(KeyNotFoundException) {
      return _404BJob(id);
    }
    return NoContent();
  }
}
