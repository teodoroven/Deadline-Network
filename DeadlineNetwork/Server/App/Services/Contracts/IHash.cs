namespace Server.App.Services;
public interface IHash
{
    string Hash(string data, byte[]? salt = null);
}
