using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

[ApiController]
[Route("userGroup/[action]")]
public class UserGroupAccessController : ControllerBase
{
    public ApplicationDbContext Db { get; }
    public UserGroupAccessController(ApplicationDbContext db)
    {
        Db = db;
    }

    [HttpGet(Name = "addUser")]
    public async Task<JsonResult> AddUser(int groupId, string groupPassword, string userame = "", int userId = -1)
    {
        try
        {
            var group = await Db.Groups.FindAsync(groupId);
            if (group is null)
            {
                return new JsonResult("Group does not exists")
                {
                    StatusCode = 401
                };
            }

            if(group.PasswordHash != passwordHash(groupPassword))
            {
                return new JsonResult("Wrong password")
                {
                    StatusCode = 401
                };
            }

            // Если Пользователь не сущестует -> создаём пользователя
            if (userId == -1)
            {
                var user = new User()
                {
                    Name = userame
                };

                await Db.Users.AddAsync(user);
                var changes = await Db.SaveChangesAsync();
                if (changes <=0)
                {
                    return new JsonResult("Internal error, failed to add user to User table")
                    {
                        StatusCode = 500
                    };
                }
                userId = user.Id;
            }   
            
            // Проверяем существует ли пользователь
            var userExist = await Db.Users.FindAsync(userId);
            if (userExist is null)
            {
                return new JsonResult("User does not exists")
                {
                    StatusCode = 404
                };
            }
            
            // Проверяем если пользователь уже в группе 
            var userInGroup = await Db.UserGroups.FindAsync(userId, groupId);
            if (userInGroup is not null)
            {
                return new JsonResult("User already in  this group")
                {
                    StatusCode = 409
                };
            }

            // Пользователь существует или мы его уже создали
            var userGroup = new UserGroup()
            {
                UserId = userId,
                GroupId = groupId,
                IsOwner = false
            };
            
            await Db.UserGroups.AddAsync(userGroup);
            var res = await Db.SaveChangesAsync();
            
            if(res <= 0)
            {
                return new JsonResult("Internal error, failed to add user")
                {
                    StatusCode = 500
                };
            }
            // Если всё успешно
            return new JsonResult("User successfully added to the group")
            {
                StatusCode = 200
            };   
        }
        catch
        {
            return new JsonResult("Internal error /  failed to add user")
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