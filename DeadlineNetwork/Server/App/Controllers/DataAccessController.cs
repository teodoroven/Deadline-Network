using System;
using Microsoft.AspNetCore.Mvc;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

[ApiController]
[Route("data/[action]")]
public class DataAccessController : ControllerBase
{
    public ApplicationDbContext Db { get; }
    public DataAccessController(ApplicationDbContext db)
    {
        Db = db;
    }

    [HttpGet(Name = "get")]
    public JsonResult Get()
    {
        return new(new{Text="Hello!"});
    }

    [HttpGet(Name = "users")]
    public JsonResult Users(){
        return new(new{Users=Db.Users.First()});
    }
    

}
