using Server.App.Db.Contexts;
namespace Server.App.Services;
public class RegistrationService : IRegister
{
    public ApplicationDbContext Db { get; }
    public IHash hashService;
    public RegistrationService(ApplicationDbContext db, IHash hashService)
    {
        Db = db;
        this.hashService = hashService;
    }
    
    public async Task<User> Register(string login, string password, string userName)
    {
        string loginHash = hashService.Hash(login);
        string passwordHash = hashService.Hash(password);
        var userExist = Db.Users.FirstOrDefault(p => p.LoginHash == loginHash);
        if (userExist is not null)
            throw new ArgumentException("User with this login is already exists");
        var user = new User
        {
            Name = userName,
            PasswordHash = passwordHash,
            LoginHash = loginHash,
            PasswordSalt=HashService.GenerateSalt()
        };

        await Db.Users.AddAsync(user);
        await Db.SaveChangesAsync();
        return user;
    } 
}