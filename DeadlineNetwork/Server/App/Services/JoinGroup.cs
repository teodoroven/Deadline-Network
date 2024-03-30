using Server.App.Db.Contexts;
namespace Server.App.Services;

public class JoinGroup : IJoinGroup
{
    public ApplicationDbContext Db { get; }
    public IHash hashService;
    public JoinGroup(ApplicationDbContext db , IHash hashService)
    {
        Db = db;
        this.hashService = hashService;
    }

    public async Task<UserGroup> Join(int userId, int groupId, string groupPassword)
    {
        var user = await Db.Users.FindAsync(userId);
        if (user is null)
            throw new ArgumentException("No such user");

        var group = await Db.Groups.FindAsync(groupId);
        if (group is null)
            throw new ArgumentException("No such group");
        
        var userGroup = await Db.UserGroups.FindAsync(userId, groupId);
        if (userGroup is not null)
            throw new ArgumentException("User already in group");
        
        string groupPasswordHash = hashService.Hash(groupPassword);
        if (group.PasswordHash != groupPasswordHash)
            throw new ArgumentException("Wrong password");
        
        var newUserGroup = new UserGroup()
        {
            UserId = userId,
            GroupId = groupId,
            IsOwner = false
        };

        await Db.UserGroups.AddAsync(newUserGroup);
        await Db.SaveChangesAsync();
        return newUserGroup;
    }
}