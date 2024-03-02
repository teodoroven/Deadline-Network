using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

[ApiController]
[Route("group/[action]")]
public class GroupAccessController : ControllerBase
{
    public ApplicationDbContext Db { get; }
    public GroupAccessController(ApplicationDbContext db)
    {
        Db = db;
    }

    [HttpGet(Name = "groupList")]
    public async Task<JsonResult> GroupList(int userId)
    {
        try
        {
            var user = await Db.Users.FindAsync(userId);
            if(user is null)
            {
                return new JsonResult("There is no such user")
                {
                    StatusCode = 401
                };
            }

            var groups = await Db.UserGroups.Join(Db.Groups,
                ug => ug.GroupId,
                g => g.Id,
                (ug, g) => new
                {
                    groupId = g.Id,
                    userId = ug.UserId,
                    groupName = g.Name
                })
                .Where(u => u.userId == userId)
                .Select(p => new
                {
                    id = p.groupId,
                    name = p.groupName
                })
                .ToArrayAsync();
                
            // Пользователь не вступил ни в одну группу
            // Такое возомжно??
            if (groups is null)
            {
                return new JsonResult("User not joined any group")
                {
                    StatusCode = 200
                };
            }

            return new JsonResult(new {message ="GroupList successfully returned", groups})
            {
                StatusCode = 200
            };
        }   
        catch
        {
            return new JsonResult("Internal error /  failed to add group")
            {
                StatusCode = 500
            };
        }
    }

    [HttpGet(Name = "membersList")]
    public async Task<JsonResult> MembersList(int groupId)
    {
        try
        {   
            var group = await Db.Groups.FindAsync(groupId);
            if (group is null)
            {
                return new JsonResult("There is no such group")
                {
                    StatusCode = 401
                };
            }

            var members = await Db.UserGroups.Include(u => u.User)
                .Where(u => u.GroupId == groupId)
                .Select(u => new 
                {
                    u.User.Id,
                    u.User.Name
                })
                .ToArrayAsync();

            return new JsonResult(new {message = "Members list returned", members})
            {
                StatusCode = 200
            };
        }
        catch
        {
            return new JsonResult("Internal error /  failed to get members")
            {
                StatusCode = 500
            };
        }
    }

    [HttpGet(Name = "create")]
    public async Task<JsonResult> CreateGroup(int userId, string groupName, string password)
    {
        try
        {
            var userCredits = await Db.UserCredentials.FindAsync(userId);
            if(userCredits is null)
            {
                return new JsonResult("User is not authorized")
                {
                    StatusCode = 401
                };
            }
            
            var group = new Group()
            {
                Name = groupName,
                PasswordHash = passwordHash(password)
            };

            await Db.Groups.AddAsync(group);
            var res = await Db.SaveChangesAsync(); 
            
            if(res <= 0)
            {
                return new JsonResult("Internal error, failed to add group")
                {
                    StatusCode = 500
                };
            }
            
            var userGroup = new UserGroup()
            {
                UserId = userId,
                GroupId = group.Id,
                IsOwner = true
            };

            await Db.UserGroups.AddAsync(userGroup);
            res = await Db.SaveChangesAsync();
            if(res <= 0)
            {
                return new JsonResult("Internal error, failed to add group")
                {
                    StatusCode = 500
                };
            }

            // Если всё успешно
            return new JsonResult(new {message = "Group successfully added", group})
            {
                StatusCode = 200
            };
        }
        catch
        {
            return new JsonResult("Internal error /  failed to add group")
            {
                StatusCode = 500
            };
        }
    }

    [HttpGet(Name = "changePassword")]
    public async Task<JsonResult> ChangePassword(int userId, int groupId, string password)
    {
        try
        {
            var user = await Db.UserGroups.FindAsync(userId, groupId);
            if (user is null)
                return new JsonResult("There is no such user or group")
                {
                    StatusCode = 401
                };
            if (!user.IsOwner)
            {
                return new JsonResult("Only group owners can change password")
                {
                    StatusCode = 403
                };
            }

            var entity = Db.Groups.FirstOrDefault(item => item.Id == groupId);
            
            if (entity == null)
            {
                return new JsonResult("There is no such group")
                {
                    StatusCode = 401
                };
            }

            entity.PasswordHash = passwordHash(password);
            var res = await Db.SaveChangesAsync();
            
            if(res <= 0)
            {
                return new JsonResult("Internal error, failed to change password")
                {
                    StatusCode = 500
                };
            }
            // Если всё успешно возвращаем не хэшированный пароль
            return new JsonResult(new {message = "Password changed", password} )
            {
                StatusCode = 200
            };
        }
        catch
        {
            return new JsonResult("Internal error /  failed to change password")
            {
                StatusCode=500
            };
        }
    }

    // TODO: Заглушка
    private string passwordHash(string password)
    {
        return password;
    }
}


