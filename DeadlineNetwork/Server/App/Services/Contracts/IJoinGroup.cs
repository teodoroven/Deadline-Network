namespace Server.App.Services;
public interface IJoinGroup{
    /// <summary>
    /// Если нет такого юзера кидаем exception
    /// Если нет такой группы кидаем exception
    /// Находим в бд группу с данным groupId
    /// Если юзер уже состоит в группе кидаем exception
    /// Хешируем пароль
    /// Если хеш не совпадает с хешом из бд кидаем exception
    /// Создаем объект UserGroup
    /// Добавляем значение в таблицу user_group
    /// возвращаем созданный объект
    /// </summary>
    Task<UserGroup> Join(int userId, int groupId, string groupPassword);
}
