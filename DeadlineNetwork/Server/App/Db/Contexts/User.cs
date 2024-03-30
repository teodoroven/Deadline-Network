namespace Server;

public partial class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    /// <summary>
    /// Login must not be salted
    /// </summary>
    public required string LoginHash { get; set; }
    /// <summary>
    /// Password must be salted to ensure uniqueness among all accounts
    /// </summary>
    public required string PasswordHash { get; set; }
    public required byte[] PasswordSalt{get;set;}
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
