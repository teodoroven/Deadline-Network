using System;
using Microsoft.AspNetCore.Mvc;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

[ApiController]
[Route("data")]
public class DataAccessController : ControllerBase
{
    public ApplicationDbContext Db { get; }
    public DataAccessController(ApplicationDbContext db)
    {
        Db = db;
    }


    [HttpGet(Name = "Test")]
    [Route("hello")]
    public JsonResult Get()
    {
        return new(new{Text="Hello!"});
    }

    [HttpGet(Name = "users")]
    [Route("users")]
    public JsonResult Users(){
        return new(new{Users=Db.Users.First()});
    }

}
