using Xunit;
using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
using Server.App.Controllers;
using Server;

namespace Tests
{
    public class IParticipantGroupManagerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var dbName = Guid.NewGuid().ToString(); // Generate a unique name for each test database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }

        [Fact]
        public void AddTask_ValidInput_ShouldAddTask()
        {
            // Подготовка
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "" };
            var discipline = new Discipline { Id = 1, GroupId = 1, Name = "Тестовая дисциплина" }; // Ensure the discipline is added
            db.Users.Add(user);
            db.Groups.Add(group);
            db.Disciplines.Add(discipline); // Add this line
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            int disciplineId = 1;
            string description = "Тестовое описание задачи";
            string comment = "Тестовый комментарий";
            DateTime deadline = DateTime.UtcNow.AddDays(1);

            // Действие
            var task = manager.AddTask(disciplineId, description, comment, deadline);

            // Проверка
            Assert.NotNull(task);
            Assert.Equal(comment, task.Comment);
            Assert.Equal(deadline, task.Deadline);
            Assert.Equal(user.Id, task.WhoAdded);
        }

        [Fact]
        public void GetDisciplines_ValidGroupId_ShouldReturnDisciplines()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "" };
            db.Users.Add(user);
            db.Groups.Add(group);
            var testDiscipline = new Discipline { GroupId = 1, Name = "Тестовая дисциплина" };
            db.Disciplines.Add(testDiscipline);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            var disciplines = manager.GetDisciplines(group.Id).ToList();

            Assert.NotEmpty(disciplines);
            Assert.Contains(disciplines, d => d.Name == "Тестовая дисциплина");
        }

        [Fact]
        public void UpdateTask_ValidInput_ShouldUpdateTask()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "" };
            db.Users.Add(user);
            db.Groups.Add(group);
            var testTask = new Server.Task { DisciplineId = 1, Comment = "Комментарий", Deadline = DateTime.UtcNow.AddDays(1), WhoAdded = 1 };
            db.Tasks.Add(testTask);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            string updatedDescription = "Обновленное описание";
            string updatedComment = "Обновленный комментарий";
            DateTime updatedDeadline = DateTime.UtcNow.AddDays(2);

            var updatedTask = manager.UpdateTask(testTask.Id, updatedDescription, updatedComment, updatedDeadline);

            Assert.NotNull(updatedTask);
            Assert.Equal(updatedComment, updatedTask.Comment);
            Assert.Equal(updatedDeadline, updatedTask.Deadline);
        }
    }
}