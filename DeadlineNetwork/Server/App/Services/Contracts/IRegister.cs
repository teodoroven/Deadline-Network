namespace Server.App.Services;
public interface IRegister{
    /// <summary>
    /// Принимая на вход логин, пароль и имя
    /// хеширует логин и пароль
    /// Проверяет существование логина в базе данных
    /// Если логин есть, кидает Exception
    /// Создает новый объект User с полученными данными
    /// Добавляет созданный юзер в бд
    /// Возвращает созданный объект
    /// </summary>
    Task<User> Register(string login, string password, string name);
}
