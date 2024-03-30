namespace Server.App.Services;
public interface ILogin
{
    /// <summary>
    /// Принимая на вход  логин и пароль, 
    /// хеширует их, 
    /// ищет в бд есть ли такой юзер
    /// Если есть возвращает найденный объект
    /// Иначе кидает exception с ошибкой не правильный логин или пароль
    /// </summary>
    User Login(string login, string password);
}
