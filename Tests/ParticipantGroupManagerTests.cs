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
            var groupOwner = new User { Id = 2, Name = "Владелец группы", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "" };
            var discipline = new Discipline { Id = 1, GroupId = 1, Name = "Тестовая дисциплина" };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };

            db.Users.Add(user);
            db.Groups.Add(group);
            db.Disciplines.Add(discipline);
            db.UserGroups.Add(userGroup);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            int disciplineId = discipline.Id;
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
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };

            db.Users.Add(user);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
            var testDiscipline1 = new Discipline { GroupId = 1, Name = "Тестовая дисциплина 1" };
            var testDiscipline2 = new Discipline { GroupId = 1, Name = "Тестовая дисциплина 2" };
            db.Disciplines.AddRange(testDiscipline1, testDiscipline2);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            var disciplines = manager.GetDisciplines(group.Id).ToList();

            Assert.NotEmpty(disciplines);
            Assert.Equal(2, disciplines.Count);
            Assert.Contains(disciplines, d => d.Name == "Тестовая дисциплина 1");
            Assert.Contains(disciplines, d => d.Name == "Тестовая дисциплина 2");
        }

        [Fact]
        public void UpdateTask_ValidInput_ShouldUpdateTask()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };
            db.Users.Add(user);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
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

        [Fact]
        public void Constructor_UserNotGroupOwner_ShouldThrowException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var groupOwner = new User { Id = 2, Name = "Владелец группы", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var userGroup = new UserGroup { UserId = groupOwner.Id, GroupId = group.Id, IsOwner = true };

            db.Users.Add(user);
            db.Users.Add(groupOwner);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
            db.SaveChanges();

            var ex = Assert.Throws<Exception>(() => new ParticipantGroupManager(db, user, group));
            Assert.Equal("Только владелец группы могут обновить задачу.", ex.Message);
        }

        [Fact]
        public void FindGroupOwner_GroupOwnerNotFound_ShouldThrowException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Participant", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 2, Name = "Orphan Group", PasswordHash = "group_password_hash" };

            var ex = Assert.Throws<Exception>(() => new ParticipantGroupManager(db, user, group));
            Assert.Equal("Владелец группы не найден.", ex.Message);
        }

        [Fact]
        public void AddTask_EmptyDescription_ShouldThrowArgumentException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };
            var discipline = new Discipline { Id = 1, GroupId = 1, Name = "Тестовая дисциплина" };
            
            db.Users.Add(user);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
            db.Disciplines.Add(discipline);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            var ex = Assert.Throws<ArgumentException>(() => manager.AddTask(1, "", "Тестовый комментарий", DateTime.UtcNow.AddDays(1)));
            Assert.Equal("Описание не может быть пустым.", ex.Message);
        }

        [Fact]
        public void AddTask_PastDeadline_ShouldThrowArgumentException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };
            var discipline = new Discipline { Id = 1, GroupId = 1, Name = "Тестовая дисциплина" };
            
            db.Users.Add(user);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
            db.Disciplines.Add(discipline);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            var ex = Assert.Throws<ArgumentException>(() => manager.AddTask(1, "Тестовое описание", "Тестовый комментарий", DateTime.UtcNow.AddDays(-1)));
            Assert.Equal("Срок выполнения не может быть в прошлом.", ex.Message);
        }

        [Fact]
        public void AddTask_InvalidDiscipline_ShouldThrowArgumentException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };
            
            db.Users.Add(user);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);

            var ex = Assert.Throws<ArgumentException>(() => manager.AddTask(999, "Тестовое описание", "Тестовый комментарий", DateTime.UtcNow.AddDays(1)));
            Assert.Equal("Дисциплина не найдена.", ex.Message);
        }

        [Fact]
        public void UpdateTask_TaskNotFound_ShouldThrowArgumentException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };
            
            db.Users.Add(user);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);
            
            var ex = Assert.Throws<ArgumentException>(() => manager.UpdateTask(999, "Updated Description", "Updated Comment", DateTime.UtcNow.AddDays(1)));
            Assert.Equal("Задача не найдена.", ex.Message);
        }

        [Fact]
        public void UpdateTask_EmptyDescription_ShouldThrowArgumentException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var testTask = new Server.Task { DisciplineId = 1, Comment = "Initial Comment", Deadline = DateTime.UtcNow.AddDays(1), WhoAdded = 1 };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };
            
            db.Users.Add(user);
            db.Groups.Add(group);
            db.UserGroups.Add(userGroup);
            db.Tasks.Add(testTask);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);
            
            var ex = Assert.Throws<ArgumentException>(() => manager.UpdateTask(testTask.Id, "", "Updated Comment", DateTime.UtcNow.AddDays(1)));
            Assert.Equal("Описание не может быть пустым.", ex.Message);
        }

        [Fact]
        public void UpdateTask_PastDeadline_ShouldThrowArgumentException()
        {
            var db = GetInMemoryDbContext();
            var user = new User { Id = 1, Name = "Тестовый пользователь", PasswordSalt = new byte[0], LoginHash = "", PasswordHash = "hashed_password" };
            var group = new Group { Id = 1, Name = "Тестовая группа", PasswordHash = "group_password_hash" };
            var testTask = new Server.Task { DisciplineId = 1, Comment = "Initial Comment", Deadline = DateTime.UtcNow.AddDays(1), WhoAdded = 1 };
            var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id, IsOwner = true };
            
            db.Users.Add(user);
            db.Groups.Add(group);
            db.Tasks.Add(testTask);
            db.UserGroups.Add(userGroup);
            db.SaveChanges();

            var manager = new ParticipantGroupManager(db, user, group);
            
            var ex = Assert.Throws<ArgumentException>(() => manager.UpdateTask(testTask.Id, "Updated Description", "Updated Comment", DateTime.UtcNow.AddDays(-1)));
            Assert.Equal("Срок выполнения не может быть в прошлом.", ex.Message);
        }
    }
}