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

using b_robot_api.Data;
using Microsoft.AspNetCore.Mvc;

namespace b_robot_api.Controllers;

public class BControllerBase : ControllerBase
{
  protected readonly UserContext userContext;
  protected readonly int curUserId;

  protected ObjectResult _403()
  {
    return StatusCode(StatusCodes.Status403Forbidden,
                      new { message = "Недостаточно прав для выполнения операции." });
  }
  protected ObjectResult _404User(int id)
  {
    return StatusCode(StatusCodes.Status404NotFound,
                      new { message = $"Не удалось найти пользователя [id={id}]." });
  }
  protected ObjectResult _404BTask(int id)
  {
    return StatusCode(StatusCodes.Status404NotFound,
                      new { message = $"Не удалось найти торгового робота [task id={id}]." });
  }
  protected ObjectResult _404BJob(int id)
  {
    return StatusCode(StatusCodes.Status404NotFound,
                      new { message = $"Не удалось найти торгового робота [job id={id}]." });
  }
  protected ObjectResult _409Email(string email)
  {
    return StatusCode(StatusCodes.Status409Conflict,
                      new { message = $"Пользователь с таким адресом электронной почты уже зарегистрирован [email = {email}]." });
  }
  protected ObjectResult _409StartBTask(int id)
  {
    return StatusCode(StatusCodes.Status409Conflict,
                      new { message = $"Этот торговый робот уже запущен [id = {id}]." });
  }
  protected ObjectResult _409StopBTask(int id)
  {
    return StatusCode(StatusCodes.Status409Conflict,
                      new { message = $"Этот торговый робот не запущен [id = {id}]." });
  }
  protected BControllerBase(IHttpContextAccessor httpContextAccessor, UserContext uContext)
  {
    userContext = uContext;
    curUserId = 0;
    var htc = httpContextAccessor.HttpContext;
    if (htc != null) {
      var uid = htc.Items["UserId"];
      if (uid != null) curUserId = (int)uid;
    }
  }
}
