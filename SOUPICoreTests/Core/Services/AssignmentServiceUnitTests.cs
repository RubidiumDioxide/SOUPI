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
    public class AssignmentServiceUnitTests
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly AssignmentService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new();

        public AssignmentServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<AssignmentService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new SoupiDbContext(_options));

            _service = new AssignmentService(_contextFactoryMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnAssignment_WhenIdIsValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);

            // Act
            var result = await _service.GetById(assignment.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(assignment.Id);
        }

        [Fact]
        public async Task GetById_ShouldThrowBadRequestException_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.AssignmentNotFound;

            // Act
            Func<Task> act = async () => await _service.GetById(assignmentId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetByProjectId_ShouldReturnAssignments_WhenProjectHasAssignments()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);

            // Act
            var result = await _service.GetByProjectId(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByProjectId_ShouldThrowBadRequestException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.GetByProjectId(projectId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Create_ShouldReturnAssignmentDto_WhenValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var newAssignmentDto = new AssignmentDto
            {
                TeamMemberId = teamMember.Id,
                JobId = job.Id,
                Comment = "Test Comment"
            };

            // Act
            var result = await _service.Create(newAssignmentDto);

            // Assert
            result.Should().NotBeNull();
            result.TeamMemberId.Should().Be(newAssignmentDto.TeamMemberId);
            result.JobId.Should().Be(newAssignmentDto.JobId);
        }

        [Fact]
        public async Task Create_ShouldThrowBadRequestException_WhenAssignmentAlreadyExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            var newAssignmentDto = new AssignmentDto
            {
                TeamMemberId = teamMember.Id,
                JobId = job.Id,
                Comment = "Test Comment"
            };
            string expectedMessage = ServiceErrorMessages.AssignmentAlreadyExists;

            // Act
            Func<Task> act = async () => await _service.Create(newAssignmentDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Delete_ShouldRemoveAssignment_WhenValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);

            // Act
            await _service.Delete(assignment.Id);

            // Assert
            using var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync();
            var deletedAssignment = await _assertContext.Assignments.FindAsync(assignment.Id);
            deletedAssignment.Should().BeNull();
        }

        [Fact]
        public async Task Delete_ShouldThrowBadRequestException_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.AssignmentNotFound;

            // Act
            Func<Task> act = async () => await _service.Delete(assignmentId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetByJobId_ShouldReturnAssignments_WhenJobHasAssignments()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);

            // Act
            var result = await _service.GetByJobId(job.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByJobId_ShouldThrowBadRequestException_WhenJobDoesNotExist()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.GetByJobId(jobId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnAssignments_WhenUserHasAssignments()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);

            // Act
            var result = await _service.GetByUserId(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByUserId_ShouldThrowBadRequestException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.UserNotFound;

            // Act
            Func<Task> act = async () => await _service.GetByUserId(userId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task UpdateContent_ShouldUpdateAssignment_WhenValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var assignment = await SeedAssignment(_contextFactoryMock.Object, teamMember.Id, job.Id);
            var updatedAssignmentDto = new AssignmentDto(assignment)
            {
                Comment = "Updated Comment"
            };

            // Act
            var result = await _service.UpdateContent(updatedAssignmentDto);

            // Assert
            result.Should().NotBeNull();
            result.Comment.Should().Be("Updated Comment");
        }

        [Fact]
        public async Task UpdateContent_ShouldThrowBadRequestException_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var updatedAssignmentDto = new AssignmentDto
            {
                Id = Guid.NewGuid(),
                Comment = "Updated Comment"
            };
            string expectedMessage = ServiceErrorMessages.AssignmentNotFound;

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedAssignmentDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }
    }
}