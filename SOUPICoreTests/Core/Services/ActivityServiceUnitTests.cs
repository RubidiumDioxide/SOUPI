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
    public class ActivityServiceUnitTests
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly ActivityService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new();

        public ActivityServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<ActivityService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new SoupiDbContext(_options));

            _service = new ActivityService(_contextFactoryMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task GetByAssignmentId_ShouldReturnActivities_WhenAssignmentExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);

            // Act
            var result = await _service.GetByAssignmentId(assignment.Id);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByAssignmentId_ShouldThrowBadRequestException_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _service.GetByAssignmentId(assignmentId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.AssignmentNotFound);
        }

        [Fact]
        public async Task Delete_ShouldRemoveActivity_WhenActivityExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            var activity = await SeedActivity(_contextFactoryMock.Object, assignment.Id);

            // Act
            await _service.Delete(activity.Id);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                var deletedActivity = await _assertContext.Activities.FindAsync(activity.Id);
                deletedActivity.Should().BeNull();
            }
        }

        [Fact]
        public async Task Delete_ShouldThrowBadRequestException_WhenActivityDoesNotExist()
        {
            // Arrange
            var activityId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _service.Delete(activityId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ActivityNotFound);
        }

        [Fact]
        public async Task GetByTeamMemberId_ShouldReturnActivities_WhenTeamMemberExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            await SeedActivity(_contextFactoryMock.Object, assignment.Id);

            // Act
            var result = await _service.GetByTeamMemberId(teamMember.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByTeamMemberId_ShouldThrowBadRequestException_WhenTeamMemberDoesNotExist()
        {
            // Arrange
            var teamMemberId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _service.GetByTeamMemberId(teamMemberId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.TeamMemberNotFound);
        }

        [Fact]
        public async Task GetByJobId_ShouldThrowBadRequestException_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _service.GetByJobId(jobId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.JobNotFound);
        }

        [Fact]
        public async Task GetByProjectId_ShouldThrowBadRequestException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _service.GetByProjectId(projectId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ProjectNotFound);
        }

        [Fact]
        public async Task GetByJobId_ShouldReturnActivities_WhenJobExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            await SeedActivity(_contextFactoryMock.Object, assignment.Id);

            // Act
            var result = await _service.GetByJobId(job.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByProjectId_ShouldReturnActivities_WhenProjectExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            await SeedActivity(_contextFactoryMock.Object, assignment.Id);

            // Act
            var result = await _service.GetByProjectId(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task Create_ShouldAddActivity_WhenValidDataProvided()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);

            var newActivityDto = new ActivityDto
            {
                AssignmentId = assignment.Id,
                Commit = "newCommit",
                Comment = "newComment"
            };

            // Act
            var result = await _service.Create(newActivityDto);

            // Assert
            result.Should().NotBeNull();
            result.AssignmentId.Should().Be(assignment.Id);
        }

        [Fact]
        public async Task Create_ShouldThrowBadRequestException_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var newActivityDto = new ActivityDto
            {
                AssignmentId = Guid.NewGuid(),
                Commit = "newCommit",
                Comment = "newComment"
            };

            // Act
            Func<Task> act = async () => await _service.Create(newActivityDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.AssignmentNotFound);
        }

        [Fact]
        public async Task Create_ShouldThrowBadRequestException_WhenActivityAlreadyExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            await SeedActivity(_contextFactoryMock.Object, assignment.Id, "commit123", "sameComment");

            var newActivityDto = new ActivityDto
            {
                AssignmentId = assignment.Id,
                Commit = "commit123",
                Comment = "otherComment"
            };

            // Act
            Func<Task> act = async () => await _service.Create(newActivityDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ActivityAlreadyExists);
        }

        [Fact]
        public async Task UpdateContent_ShouldUpdateActivity_WhenActivityExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            var activity = await SeedActivity(_contextFactoryMock.Object, assignment.Id);

            var updatedActivityDto = new ActivityDto
            {
                Id = activity.Id,
                AssignmentId = assignment.Id, 
                Comment = "updatedComment"
            };

            // Act
            var result = await _service.UpdateContent(updatedActivityDto);

            // Assert
            result.Should().NotBeNull();
            result.Comment.Should().Be("updatedComment");
        }

        [Fact]
        public async Task UpdateContent_ShouldThrowBadRequestException_WhenActivityDoesNotExist()
        {
            // Arrange
            var updatedActivityDto = new ActivityDto
            {
                Id = Guid.NewGuid(),
                AssignmentId = Guid.NewGuid(),
                Comment = "updatedComment"
            };

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedActivityDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ActivityNotFound);
        }
    }
}