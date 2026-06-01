using FluentAssertions;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services; 
using SOUPIShared.Exceptions;
using SOUPIShared.Extensions; 
using SOUPIShared.Resources;
using static SOUPITests.Helpers.Helpers; 


namespace SOUPITests.Core.Services
{
    public class JobSequenceServiceUnitTests 
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly JobSequenceService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new();

        public JobSequenceServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<JobSequenceService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(() => new SoupiDbContext(_options)); 

            _service = new JobSequenceService(_contextFactoryMock.Object, loggerMock.Object);
        }


        // --- Create --- 
        [Fact]
        public async Task Create_ShouldReturnJobSequenceDto_WhenJobDtoIsValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id); 
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id); 

            // Act
            var createdJobSequenceDto = await _service.Create(firstJob.Id, secondJob.Id);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                createdJobSequenceDto.Should().NotBeNull();
                var createdJobSequence = await _assertContext.JobSequences.FindAsync(createdJobSequenceDto.Id);
                createdJobSequence.Should().NotBeNull();

                createdJobSequenceDto.IsEquivalent(createdJobSequence).Should().Be(true);
                createdJobSequence.FirstJobId.Should().Be(firstJob.Id);
                createdJobSequence.SecondJobId.Should().Be(secondJob.Id);
            }
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenFirstJobDoesntExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJobId = Guid.NewGuid(); 
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            string expectedMessage = ServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(firstJobId, secondJob.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenSecondJobDoesntExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJobId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(firstJob.Id, secondJobId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenSecondJobsAreDifferentLevel()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object); 
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id, firstJob.Id);
            string expectedMessage = ServiceErrorMessages.JobsDifferentLevels; 

            // Act
            Func<Task> act = async () => await _service.Create(firstJob.Id, secondJob.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }


        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenJobSequenceAlreadyExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var jobSequence = await SeedJobSequence(_contextFactoryMock.Object, firstJob.Id, secondJob.Id); 
            string expectedMessage = ServiceErrorMessages.JobSequenceAlreadyExists;

            // Act
            Func<Task> act = async () => await _service.Create(firstJob.Id, secondJob.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenJobSequenceCyclic()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var jobSequence = await SeedJobSequence(_contextFactoryMock.Object, firstJob.Id, secondJob.Id); 
            string expectedMessage = ServiceErrorMessages.JobSequenceCyclic;

            // Act
            Func<Task> act = async () => await _service.Create(secondJob.Id, firstJob.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage); 
        }



        // --- Delete --- 
        [Fact]
        public async Task Delete_ShouldDeleteJobSequence_WhenJobSequenceExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var jobSequence = await SeedJobSequence(_contextFactoryMock.Object, firstJob.Id, secondJob.Id);

            // Act
            await _service.Delete(jobSequence.Id);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                var deleted = await _assertContext.JobSequences.FindAsync(jobSequence.Id);
                deleted.Should().BeNull();
            }
        }

        [Fact]
        public async Task Delete_ShouldLogErrorAndThrowBadRequestException_WhenJobSequenceDoesntExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            string expectedMessage = ServiceErrorMessages.JobSequenceNotFound;

            // Act
            Func<Task> act = async () => await _service.Delete(Guid.NewGuid());

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
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var jobSequence = await SeedJobSequence(_contextFactoryMock.Object, firstJob.Id, secondJob.Id);

            // force an exception
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.Delete(jobSequence.Id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- GetByProjectId ---
        [Fact]
        public async Task GetByProjectId_ShouldReturnJobSequences_WhenProjectHasJobSequences()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var jobSequence = await SeedJobSequence(_contextFactoryMock.Object, firstJob.Id, secondJob.Id);

            // Seed job sequence in another project to ensure filtering
            var otherProject = await SeedProject(_contextFactoryMock.Object, user.Id);
            var otherFirst = await SeedJob(_contextFactoryMock.Object, otherProject.Id, user.Id);
            var otherSecond = await SeedJob(_contextFactoryMock.Object, otherProject.Id, user.Id);
            await SeedJobSequence(_contextFactoryMock.Object, otherFirst.Id, otherSecond.Id);

            // Act
            var result = await _service.GetByProjectId(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var dto = result.First();
            dto.FirstJobId.Should().Be(firstJob.Id);
            dto.SecondJobId.Should().Be(secondJob.Id);
        }

        [Fact]
        public async Task GetByProjectId_ShouldLogErrorAndThrowBadRequestException_WhenNoSuchProjectExists()
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


        // --- Create exception ---
        [Fact]
        public async Task Create_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);

            // force an exception
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.Create(firstJob.Id, secondJob.Id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
