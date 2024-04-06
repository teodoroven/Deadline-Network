using Server;
using Server.App.Db.Contexts;

namespace Server.App.Controllers;

public class ParticipantGroupManager : IParticipantGroupManager
{
    private readonly ApplicationDbContext Db;
    public User Participant { get; }
    public Group Group { get; }

    public ParticipantGroupManager(ApplicationDbContext db, User groupOwner, Group group)
    {
        Db = db;
        Participant = groupOwner;
        Group = group;

        // Проверка, что Participant является владельцем группы
        if (FindGroupOwner(group.Id).Id != groupOwner.Id)
        {
            throw new Exception("Только владелец группы могут обновить задачу.");
        }
    }

    // Поиск владельца группы
    public User FindGroupOwner(int groupId)
    {
        var owner = Db.UserGroups
        .Where(ug => ug.GroupId == groupId && ug.IsOwner)
        .Select(ug => ug.User)
        .FirstOrDefault();
    
        if (owner == null)
        {
            throw new Exception("Владелец группы не найден.");
        }
    
        return owner;
    }

    // Является ли пользователь участником группы
    public bool IsParticipantMemberOfGroup(int groupId, int userId)
    {
        return Db.UserGroups.Any(ug => ug.GroupId == groupId && ug.UserId == userId);
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
            throw new ArgumentException("Описание не может быть пустым.");

        // Проверка, что срок выполнения не в прошлом
        if (deadline < DateTime.UtcNow)
            throw new ArgumentException("Срок выполнения не может быть в прошлом.");

        // Поиск дисциплины по ID
        var discipline = Db.Disciplines.Find(disciplineId) ?? throw new ArgumentException("Дисциплина не найдена.");

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
        var task = Db.Tasks.Find(taskId) ?? throw new ArgumentException("Задача не найдена.");

        // Проверка на пустое описание
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Описание не может быть пустым.");

        // Проверка, что срок выполнения не в прошлом
        if (deadline < DateTime.UtcNow)
            throw new ArgumentException("Срок выполнения не может быть в прошлом.");

        task.Comment = comment;
        task.Deadline = deadline;

        Db.Tasks.Update(task);
        Db.SaveChanges();

        return task;
    }
}