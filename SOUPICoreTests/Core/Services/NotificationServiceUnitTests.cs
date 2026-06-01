using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Misc;
using SOUPIShared.Models;
using static SOUPITests.Helpers.Helpers;


namespace SOUPITests.Core.Services
{
    public class NotificationServiceUnitTests
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly NotificationService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new();

        public NotificationServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<NotificationService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new SoupiDbContext(_options));

            _service = new NotificationService(_contextFactoryMock.Object, loggerMock.Object);
        }


        // --- GetByReceiverId ---
        [Fact]
        public async Task GetByReceiverId_ShouldReturnNotifications_WhenReceiverHasNotifications()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);

            // seed two notifications for this receiver
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                var n1 = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "one",
                    SenderId = user.Id,
                    ReceiverId = user.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Info,
                    Role = "R",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                var n2 = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "two",
                    SenderId = user.Id,
                    ReceiverId = user.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Info,
                    Role = "R",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.AddRange(n1, n2);
                await _context.SaveChangesAsync();
            }

            // Act
            var result = await _service.GetByReceiverId(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.All(n => n.ReceiverId == user.Id).Should().BeTrue();
        }

        [Fact]
        public async Task GetByReceiverId_ShouldReturnEmpty_WhenReceiverHasNoNotifications()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);

            // Act
            var result = await _service.GetByReceiverId(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetByReceiverId_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.GetByReceiverId(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- Create ---
        [Fact]
        public async Task Create_ShouldReturnNotificationDto_WhenNotificationDtoIsValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);

            var newDto = new NotificationDto
            {
                Id = Guid.NewGuid(),
                Message = "hello",
                SenderId = user.Id,
                ReceiverId = user.Id,
                ProjectId = project.Id,
                NotificationType = NotificationType.Info,
                Role = "Dev",
                HasBeenViewed = false,
                CreationDateTime = DateTime.UtcNow
            };

            // Act
            var created = await _service.Create(newDto);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                created.Should().NotBeNull();
                var found = await _assertContext.Notifications.FindAsync(created.Id);
                found.Should().NotBeNull();
                found.Message.Should().Be(newDto.Message);
                found.ProjectId.Should().Be(newDto.ProjectId);
                found.ReceiverId.Should().Be(newDto.ReceiverId);
                found.SenderId.Should().Be(newDto.SenderId);
                found.NotificationType.Should().Be(newDto.NotificationType);
                found.Role.Should().Be(newDto.Role);
                found.HasBeenViewed.Should().Be(false);
            }
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var newDto = new NotificationDto
            {
                Id = Guid.NewGuid(),
                Message = "hello",
                SenderId = user.Id,
                ReceiverId = user.Id,
                ProjectId = project.Id,
                NotificationType = NotificationType.Info,
                Role = "Dev",
                HasBeenViewed = false,
                CreationDateTime = DateTime.UtcNow
            };

            // force exception
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.Create(newDto);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- AcceptInvite ---
        [Fact]
        public async Task AcceptInvite_ShouldReturnNotificationDtoAndAddTeamMember_WhenInvitationValid()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var receiver = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);
            var supervisor = await SeedTeamMember(_contextFactoryMock.Object, creator.Id, project.Id, null);

            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "invite",
                    SenderId = creator.Id,
                    ReceiverId = receiver.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Invitation,
                    Role = "Dev",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            // Act
            var result = await _service.AcceptInvite(notification.Id);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                result.Should().NotBeNull();
                var foundNotification = await _assertContext.Notifications.FindAsync(result.Id);
                foundNotification.Should().NotBeNull();
                foundNotification.HasBeenViewed.Should().BeTrue();

                var addedTm = await _assertContext.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == receiver.Id && tm.ProjectId == project.Id);
                addedTm.Should().NotBeNull();
                addedTm.Role.Should().Be(notification.Role);
            }
        }

        [Fact]
        public async Task AcceptInvite_ShouldLogErrorAndThrowBadRequestException_WhenNotificationDosentExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            string expectedMessage = "Невозмжоно добавить пользователя в команду проекта, т. к. такого приглашения не существует";

            // Act
            Func<Task> act = async () => await _service.AcceptInvite(id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task AcceptInvite_ShouldLogErrorAndThrowBadRequestException_WhenNotificationTypeWrong()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var receiver = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);

            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "invite",
                    SenderId = creator.Id,
                    ReceiverId = receiver.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Info,
                    Role = "Dev",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            string expectedMessage = "Невозмжоно добавить пользователя в команду проекта, т. к. тип найденного приглашения неверный";

            // Act
            Func<Task> act = async () => await _service.AcceptInvite(notification.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task AcceptInvite_ShouldLogErrorAndThrowBadRequestException_WhenUserOrProjectMissing()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var receiver = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);

            // create notification that references non-existing project/user
            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "bad",
                    SenderId = creator.Id,
                    ReceiverId = Guid.NewGuid(), // non-existing user
                    ProjectId = Guid.NewGuid(), // non-existing project
                    NotificationType = NotificationType.Invitation,
                    Role = "Dev",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            string expectedMessage = "Невозмжоно добавить пользователя в команду проекта, т. к. такого проекта и/или пользователя не существует";

            // Act
            Func<Task> act = async () => await _service.AcceptInvite(notification.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task AcceptInvite_ShouldLogErrorAndThrowBadRequestException_WhenUserAlreadyTeamMember()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var receiver = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);
            var supervisor = await SeedTeamMember(_contextFactoryMock.Object, creator.Id, project.Id, null);
            var existing = await SeedTeamMember(_contextFactoryMock.Object, receiver.Id, project.Id, supervisor.Id);

            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "invite",
                    SenderId = creator.Id,
                    ReceiverId = receiver.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Invitation,
                    Role = "Dev",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            string expectedMessage = $"Невозмжоно добавить в команду проекта {project.Title} пользователя {receiver.Login}, т.к. этот пользователь уже есть в команде ";

            // Act
            Func<Task> act = async () => await _service.AcceptInvite(notification.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task AcceptInvite_ShouldLogErrorAndThrowBadRequestException_WhenSupervisorMissing()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var receiver = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);

            // do NOT add supervisor team member

            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "invite",
                    SenderId = creator.Id,
                    ReceiverId = receiver.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Invitation,
                    Role = "Dev",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            string expectedMessage = $"Ошибка при вычислении руководителя в команде проекта {project.Title} для пользователя {receiver.Login} ";

            // Act
            Func<Task> act = async () => await _service.AcceptInvite(notification.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task AcceptInvite_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var receiver = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);
            var supervisor = await SeedTeamMember(_contextFactoryMock.Object, creator.Id, project.Id, null);

            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "invite",
                    SenderId = creator.Id,
                    ReceiverId = receiver.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Invitation,
                    Role = "Dev",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            // force exception
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.AcceptInvite(notification.Id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- MarkAsViewed ---
        [Fact]
        public async Task MarkAsViewed_ShouldReturnNotificationDto_WhenNotificationExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "m",
                    SenderId = user.Id,
                    ReceiverId = user.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Info,
                    Role = "R",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            // Act
            var result = await _service.MarkAsViewed(notification.Id);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                result.Should().NotBeNull();
                var found = await _assertContext.Notifications.FindAsync(result.Id);
                found.Should().NotBeNull();
                found.HasBeenViewed.Should().BeTrue();
            }
        }

        [Fact]
        public async Task MarkAsViewed_ShouldLogErrorAndThrowBadRequestException_WhenNotificationNotFound()
        {
            // Arrange
            string expectedMessage = "Уведомление не найдено";

            // Act
            Func<Task> act = async () => await _service.MarkAsViewed(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task MarkAsViewed_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            Notification notification;
            using (var _context = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Message = "m",
                    SenderId = user.Id,
                    ReceiverId = user.Id,
                    ProjectId = project.Id,
                    NotificationType = NotificationType.Info,
                    Role = "R",
                    HasBeenViewed = false,
                    CreationDateTime = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            // force exception
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.MarkAsViewed(notification.Id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
