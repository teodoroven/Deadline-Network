using System;
using Microsoft.AspNetCore.Mvc;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

[ApiController]
[Route("descipline/[action]")]
public class DesciplineCRUDController : ControllerBase
{
    public ApplicationDbContext Db { get; }
    public DesciplineCRUDController(ApplicationDbContext db)
    {
        Db = db;
    }

    //TODO:
    // use JWT token for auth
    // add test to each case.
    [HttpGet(Name = "add")]
    public async Task<JsonResult> Add(int userId, int groupId, string desciplineName, string comment = "")
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
                return new JsonResult("Only group owners can create desciplines")
                {
                    StatusCode = 403
                };
            }

            var descipline = new Descipline()
            {
                GroupId = groupId,
                Name = desciplineName,
                Comment = comment
            };

            await Db.Desciplines.AddAsync(descipline);
            var changes = await Db.SaveChangesAsync();
            if (changes > 0)
            {
                return new JsonResult(new { message = "Descipline created", descipline })
                {
                    StatusCode = 200
                };
            }
        }
        catch
        {
            return new JsonResult("Internal error /  failed to add descipline")
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
