using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
namespace Server.App.Services;
public class UserGroupManager : IUserGroup
{
    public ApplicationDbContext Db { get; }
    public UserGroupManager(ApplicationDbContext db)
    {
        Db = db;
    }

    public async Task<IEnumerable<Group>> GetUserGroups(int userId)
    {
        var groups = await Db.UserGroups.Join(Db.Groups,
            ug => ug.GroupId,
            g => g.Id,
            (ug, g) => new
            {
                userId = ug.UserId,
                group = ug.Group
            })
            .Where(u => u.userId == userId)
            .Select(p => new Group
            {
                Id = p.group.Id,
                Name = p.group.Name,
                PasswordHash = p.group.PasswordHash
            })
            .ToListAsync();
        if (groups is null)
        {
            return Enumerable.Empty<Group>();
        }
        else
        {
            return groups;
        }
    }

    public async Task<IEnumerable<User>> GetGroupUsers(int groupId)
    {
        var users = await Db.UserGroups.Include(u => u.User)
            .Where(u => u.GroupId == groupId)
            .Select(u => u.User)
            .ToListAsync();
        if (users is null)
        {
            return Enumerable.Empty<User>();
        }
        else
        {
            return users;
        }
    }


}