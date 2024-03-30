using Server.App.Db.Contexts;
namespace Server.App.Services;
public class LoginService : ILogin
{
    public ApplicationDbContext db { get; }
    public IHash hashService;
    public LoginService(ApplicationDbContext db, IHash hashService)
    {
        this.db = db;
        this.hashService = hashService;
    }

    public User Login(string login, string password)
    {
        string loginHash = hashService.Hash(login);
        var user = db.Users.FirstOrDefault(p => p.LoginHash == loginHash);
        if (user is null)
            throw new ArgumentException("There is no user with such password or login");

        string passwordHash = hashService.Hash(password,user.PasswordSalt);
        if (user.PasswordHash!=passwordHash)
            throw new ArgumentException("There is no user with such password or login");
        
        return user;
    }
}