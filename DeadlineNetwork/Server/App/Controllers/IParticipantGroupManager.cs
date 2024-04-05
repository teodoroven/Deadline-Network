using Server;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

/// <summary>
/// Менеджер группы для участников группы.
/// При создании экземпляра класса который будет реализовывать данный интерфейс нужно
/// будет сделать проверку что юзер Participant действительно является участником группы,
/// иначе кидаем exception
/// </summary>
public interface IParticipantGroupManager{
    /// <summary>
    /// Участник группы
    /// </summary>
    User Participant{get;}

    /// <summary>
    /// Группа
    /// </summary>
    Group Group{get;}

    /// <summary>
    /// Получает список дисциплин у текущей группы.
    /// Делаем запрос в бд в таблицу disciplines по значению group_id=groupId
    /// </summary>
    IEnumerable<Discipline> GetDisciplines(int groupId);

    /// <summary>
    /// Добавление задачи в дисциплину.
    /// Если дисциплина с данным id не найдена кидаем exception
    /// Если deadline в прошлом кидаем exception
    /// Если description пустой то кидаем exception
    /// Создаем новый объект Server.Task
    /// В качестве поля Server.Task.Created берем текущую дату
    /// Все даты преобразуем в UniversalTime (UTC)
    /// В поле Server.Task.WhoAdded пишем значение Participant.Id
    /// Добавляем таск в бд и возвращаем созданный объект
    /// </summary>
    Server.Task AddTask(int disciplineId,string description,string comment, DateTime deadline);

    /// <summary>
    /// Обновление значения созданного таска
    /// Если description пустой то кидаем exception
    /// Если deadline в прошлом кидаем exception
    /// Получаем объект таска из бд.
    /// Если таск с данным taskId не найден - кидаем exception
    /// Если Group.OwnerId == Participant.Id то
    /// делаем update запрос в бд и возвращаем обновленный таск.
    /// Иначе
    /// Проверяем чтоб поле WhoAdded у полученного из бд таска было равно Participant.Id
    /// (т.е только создатель таска может обновляет его). 
    /// Если это не таск который создал Participant кидаем exception.
    /// Если всё хорошо делаем update запрос в бд и возвращаем обновленный таск.
    /// </summary>
    Server.Task UpdateTask(int taskId, string description,string comment, DateTime deadline);
    
}

public class ParticipantGroupManager : IParticipantGroupManager
{
    private readonly ApplicationDbContext Db;
    public User Participant { get; }
    public Group Group { get; }
    public User GroupOwner { get; }

    public ParticipantGroupManager(ApplicationDbContext db, User participant, Group group)
    {
        Db = db;
        Participant = participant;
        Group = group;
    }

    // Получает дисциплины для данной группы
    public IEnumerable<Discipline> GetDisciplines(int groupId)
    {
        // Запрос к БД для получения дисциплин по ID группы
        return Db.Disciplines.Where(d => d.GroupId == groupId).ToList();
    }

    // Добавляет задачу в дисциплину
    public Server.Task AddTask(int disciplineId, string description, string comment, DateTime deadline)
    {
        // Проверка на пустое описание
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Описание не может быть пустым");

        // Проверка, что срок выполнения не в прошлом
        if (deadline < DateTime.UtcNow)
            throw new ArgumentException("Срок выполнения не может быть в прошлом");

        // Поиск дисциплины по ID
        var discipline = Db.Disciplines.Find(disciplineId) ?? throw new ArgumentException("Дисциплина не найдена");

        var task = new Server.Task
        {
            DisciplineId = disciplineId,
            WhoAdded = Participant.Id,
            Created = DateTime.UtcNow,
            Deadline = deadline,
            Comment = comment
        };

        Db.Tasks.Add(task);
        Db.SaveChanges();

        return task;
    }

    // Обновляет существующую задачу
    public Server.Task UpdateTask(int taskId, string description, string comment, DateTime deadline)
    {
        var task = Db.Tasks.Find(taskId) ?? throw new ArgumentException("Задача не найдена");

        // Проверка доступа на обновление задачи
        if (task.WhoAdded != Participant.Id && GroupOwner.Id != Participant.Id)
            throw new UnauthorizedAccessException("Только создатель задачи или владелец группы могут обновить задачу");

        // Проверка на пустое описание
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Описание не может быть пустым");

        // Проверка, что срок выполнения не в прошлом
        if (deadline < DateTime.UtcNow)
            throw new ArgumentException("Срок выполнения не может быть в прошлом");

        task.Comment = comment;
        task.Deadline = deadline;

        Db.Tasks.Update(task);
        Db.SaveChanges();

        return task;
    }
}