public interface IUserGroup{
    /// <summary>
    /// Делаем join в базу данных по userId в таблицу user_group,
    /// делаем ещё один join в таблицу groups, и возвращаем список групп в
    /// которых состоит юзер.
    /// 
    /// Если юзера с таким id нет в бд, или у него нет групп, возвращаем 
    /// Enumerable.Empty<Group>();
    /// </summary>
    IEnumerable<Group> GetUserGroups(int userId);
    /// <summary>
    /// Делаем join в бд из таблицы group по groupId в user_group,
    /// Делаем join в User - получаем список юзеров в группе.
    /// 
    /// Если группы с таким id нет в бд то кинуть исключение, 
    /// если у группы нет юзеров, возвращаем 
    /// Enumerable.Empty<User>();
    /// </summary>
    IEnumerable<User> GetGroupUsers(int groupId);
}
