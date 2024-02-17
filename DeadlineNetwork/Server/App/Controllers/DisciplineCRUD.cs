using System;
using Microsoft.AspNetCore.Mvc;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

[ApiController]
[Route("discipline/[action]")]
public class DisciplineCRUDController : ControllerBase
{
    public IApplicationDbContext Db { get; }
    public DisciplineCRUDController(IApplicationDbContext db)
    {
        Db = db;
    }

    //TODO:
    // use JWT token for auth
    // add test to each case.
    [HttpGet(Name = "add")]
    public async Task<JsonResult> Add(int userId, int groupId, string disciplineName, string comment = "")
    {
        try
        {
            var userGroup = await Db.UserGroups.FindAsync(userId, groupId);
            if (userGroup is null)
                return new JsonResult("There is no such user or group")
                {
                    StatusCode = 401
                };
            if (!userGroup.IsOwner)
            {
                return new JsonResult("Only group owners can create disciplines")
                {
                    StatusCode = 403
                };
            }

            var discipline = new Discipline()
            {
                GroupId = groupId,
                Name = disciplineName,
                Comment = comment
            };

            await Db.Disciplines.AddAsync(discipline);
            var changes = await Db.SaveChangesAsync();
            if (changes > 0)
            {
                return new JsonResult(new { message = "Discipline created", discipline })
                {
                    StatusCode = 200
                };
            }
        }
        catch
        {
            return new JsonResult("Internal error /  failed to add discipline")
            {
                StatusCode=500
            };
        }
        return new JsonResult("Logic error - contact developer")
        {
            StatusCode=500
        };
    }


}
