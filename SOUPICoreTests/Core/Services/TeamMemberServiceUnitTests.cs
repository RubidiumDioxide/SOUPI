using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Resources;
using static SOUPITests.Helpers.Helpers;


namespace SOUPITests.Core.Services
{
    public class TeamMemberServiceUnitTests
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly TeamMemberService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new();

        public TeamMemberServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<TeamMemberService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new SoupiDbContext(_options));

            _service = new TeamMemberService(_contextFactoryMock.Object, loggerMock.Object);
        }


        // --- GetById ---
        [Fact]
        public async Task GetById_ShouldReturnTeamMember_WhenIdIsValid()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var memberUser = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, memberUser.Id, project.Id, null);

            // Ensure we don't accidentally try to delete the project creator
            if (teamMember.UserId == project.CreatorId)
            {
                var anotherUser = await SeedUser(_contextFactoryMock.Object);
                teamMember = await SeedTeamMember(_contextFactoryMock.Object, anotherUser.Id, project.Id, null);
            }

            // Act
            var result = await _service.GetById(teamMember.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(teamMember.Id);
        }

        [Fact]
        public async Task GetById_ShouldLogErrorAndThrowBadRequestException_WhenTeamMemberDoesntExist()
        {
            // Arrange
            var teamMemberId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.TeamMemberNotFound;

            // Act
            Func<Task> act = async () => await _service.GetById(teamMemberId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetById_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.GetById(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- GetByJobId ---
        [Fact]
        public async Task GetByJobId_ShouldReturnTeamMembers_WhenJobExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember1 = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var user2 = await SeedUser(_contextFactoryMock.Object);
            var teamMember2 = await SeedTeamMember(_contextFactoryMock.Object, user2.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember1.Id);

            // Act
            var result = await _service.GetByJobId(job.Id);

            // Assert
            result.Should().NotBeNull();
            result.Select(tm => tm.Id).Should().Contain(new[] { teamMember1.Id, teamMember2.Id });
        }

        [Fact]
        public async Task GetByJobId_ShouldLogErrorAndThrowBadRequestException_WhenJobDoesntExist()
        {
            // Arrange
            string expectedMessage = ServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.GetByJobId(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetByJobId_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.GetByJobId(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- GetByProjectId ---
        [Fact]
        public async Task GetByProjectId_ShouldReturnTeamMembers_WhenProjectHasTeamMembers()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember1 = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var user2 = await SeedUser(_contextFactoryMock.Object);
            var teamMember2 = await SeedTeamMember(_contextFactoryMock.Object, user2.Id, project.Id, null);

            // Act
            var result = await _service.GetByProjectId(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Select(tm => tm.Id).Should().Contain(new[] { teamMember1.Id, teamMember2.Id });
        }

        [Fact]
        public async Task GetByProjectId_ShouldLogErrorAndThrowBadRequestException_WhenProjectDoesntExist()
        {
            // Arrange
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.GetByProjectId(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetByProjectId_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.GetByProjectId(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- Update ---
        [Fact]
        public async Task Update_ShouldReturnTeamMemberDto_WhenTeamMemberExists()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var memberUser = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, memberUser.Id, project.Id, null);
            var updatedDto = new TeamMemberDto(teamMember);
            updatedDto.Role = "newRole";

            // Act
            var result = await _service.Update(updatedDto);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                result.Should().NotBeNull();
                var updated = await _assertContext.TeamMembers.FirstOrDefaultAsync(tm => tm.Id == result.Id);
                updated.Should().NotBeNull();
                result.Role.Should().Be("newRole");
                updated.Role.Should().Be("newRole");
            }
        }

        [Fact]
        public async Task Update_ShouldLogErrorAndThrowBadRequestException_WhenTeamMemberDoesntExist()
        {
            // Arrange
            var dto = new TeamMemberDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Role = "role" };
            string expectedMessage = "Невозмжоно изменить роль участника команды, т.к. этого участника команды нет в системе ";

            // Act
            Func<Task> act = async () => await _service.Update(dto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Update_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new TeamMemberDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Role = "role" };
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.Update(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- Delete ---
        [Fact]
        public async Task Delete_ShouldRemoveTeamMemberAndAssociatedEntities_WhenTeamMemberExists()
        {
            // Arrange
            var creator = await SeedUser(_contextFactoryMock.Object);
            var memberUser = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, creator.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, memberUser.Id, project.Id, null);

            // create another team member to be assigned to a job created by teamMember
            var user2 = await SeedUser(_contextFactoryMock.Object);
            var teamMember2 = await SeedTeamMember(_contextFactoryMock.Object, user2.Id, project.Id, null);

            var createdJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var createdAssignment = await SeedAssignment(_contextFactoryMock.Object, teamMember2.Id, createdJob.Id);
            var createdActivity = await SeedActivity(_contextFactoryMock.Object, createdAssignment.Id);

            var otherJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember2.Id);
            var assignmentForMember = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, otherJob.Id);
            var activityForMember = await SeedActivity(_contextFactoryMock.Object, assignmentForMember.Id);

            // Act
            await _service.Delete(teamMember.Id);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                var deleted = await _assertContext.TeamMembers.FindAsync(teamMember.Id);
                deleted.Should().BeNull();

                var foundCreatedJob = await _assertContext.Jobs.FindAsync(createdJob.Id);
                foundCreatedJob.Should().BeNull();

                var foundCreatedAssignment = await _assertContext.Assignments.FindAsync(createdAssignment.Id);
                foundCreatedAssignment.Should().BeNull();

                var foundCreatedActivity = await _assertContext.Activities.FindAsync(createdActivity.Id);
                foundCreatedActivity.Should().BeNull();

                var foundAssignmentForMember = await _assertContext.Assignments.FindAsync(assignmentForMember.Id);
                foundAssignmentForMember.Should().BeNull();

                var foundActivityForMember = await _assertContext.Activities.FindAsync(activityForMember.Id);
                foundActivityForMember.Should().BeNull();
            }
        }

        [Fact]
        public async Task Delete_ShouldLogErrorAndThrowBadRequestException_WhenTeamMemberDoesntExist()
        {
            // Arrange
            string expectedMessage = "Участника команды нельзя исключить, т.к. он не найден в системе ";

            // Act
            Func<Task> act = async () => await _service.Delete(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Delete_ShouldLogErrorAndThrowBadRequestException_WhenTeamMemberIsProjectCreator()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            string expectedMessage = "Создателя пректа нельзя исключить из команды ";

            // Act
            Func<Task> act = async () => await _service.Delete(teamMember.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Delete_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.Delete(teamMember.Id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
