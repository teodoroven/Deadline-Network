public interface ILogin{
    /// <summary>
    /// Принимая на вход логин и пароль, 
    /// хеширует их, 
    /// ищет в бд есть ли такой юзер
    /// Если есть возвращает найденный объект
    /// Иначе кидает exception с ошибкой не правильный логин или пароль
    /// </summary>
    User Login(string login, string password);
}

public interface IRegister{
    /// <summary>
    /// Принимая на вход логин, пароль и имя
    /// хеширует логин и пароль
    /// Создает новый объект User с полученными данными
    /// Добавляет созданный юзер в бд
    /// Возврашает созданный объект
    /// </summary>
    User Register(string login, string password, string name);
}