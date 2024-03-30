using Server;

/// <summary>
/// Менеджер группы для владельев. 
/// Данным классом может пользоваться только владелец группы.
/// При созданнии экземпляра класса который будет реализовывать данный интерфейс нужно
/// будет сделать проверку что GroupOwner действительно владелец группы, иначе кидаем exception
/// </summary>
public interface IOwnerGroupManager{
    /// <summary>
    /// Владелец группы
    /// </summary>
    User GroupOwner{get;}
    /// <summary>
    /// Группа
    /// </summary>
    Group Group{get;}
    /// <summary>
    /// Добавление дисцплины в группу.
    /// Создает новый объект Discipline и добавляет его в бд,
    /// возвращает созданный объект.
    /// </summary>
    Discipline AddDiscipline(string disciplineName, string description);
    /// <summary>
    /// Удаляет дисциплину из группы.
    /// Получает список 
    /// </summary>
    Discipline RemoveDiscipline(int disciplineId);
    /// <summary>
    /// Обновляет пароль к группе.
    /// Хеширует oldPassword и newPassword.
    /// Заново получает объект группы (GroupOwner) из бд - чтоб получить обновленные данные.
    /// Проверяет совпадает ли старый пароль с текущим в GroupOwner
    /// Если совпадает, то выполняется UPDATE в базу данных с новым паролем, и текущий объект
    /// (GroupOwner) обновляется.
    /// </summary>
    void UpdateGroupPassword(string oldPassword, string newPassword);
    /// <summary>
    /// Кикает юзера из группы.
    /// Если userId это id владельца группы, то кидаем exception(не может владелец кикнуть себя самого)
    /// Если userId не найден в списке то кидаем exception
    /// Удаляем юзера из группы 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    User Kick(int userId);
}