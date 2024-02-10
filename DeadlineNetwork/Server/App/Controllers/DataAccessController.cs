using System;
using Microsoft.AspNetCore.Mvc;

namespace Server.App.Controllers;

[ApiController]
[Route("[controller]")]
public class DataAccessController : ControllerBase
{
    [HttpGet(Name = "Test")]
    public JsonResult Get()
    {
        return new(new{Text="Hello!"});
    }


}
