using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Exceptions;
using SOUPIShared.Misc;
using SOUPIShared.Models;
using SOUPIShared.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options; 


namespace SOUPICoreTests
{
    public class JobServiceUnitTests : IDisposable
    {
        private readonly SoupiDbContext _context;
        private readonly JobService _service;
        private readonly Mock<ILogger<JobService>> _loggerMock = new();

        public JobServiceUnitTests()
        {
            var options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SoupiDbContext(options);
            _loggerMock = new Mock<ILogger<JobService>>();

            var localizationOptions = Microsoft.Extensions.Options.Options.Create(new LocalizationOptions
            {
                ResourcesPath = ""
            });  

            var factory = new ResourceManagerStringLocalizerFactory(
                localizationOptions,
                Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);

            var localizer = new StringLocalizer<ServiceErrorMessages>(factory);

            _service = new JobService(_context, _loggerMock.Object, localizer);
        }

        // --- HELPERS ---

        private async Task<User> SeedUser()
        {
            var user = new User { Id = Guid.NewGuid(), Login = $"user_{Guid.NewGuid().ToString()[..8]}" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<Project> SeedProject(Guid creatorId)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Test Project",
                CreatorId = creatorId,
                CreationDateTime = DateTime.UtcNow,
                StartDateTime = DateTime.UtcNow.AddDays(-10)
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        private async Task<Job> SeedJob(Guid projectId, Guid creatorId, Guid? parentId = null)
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                Title = "Test Job",
                ProjectId = projectId,
                CreatorId = creatorId,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddDays(1),
                Progress = 0,
                Status = JobStatus.New,
                ParentJobId = parentId, 
                ChildJobs = new List<Job>(),
                NextJobSequences = new List<JobSequence>(),
                PreviousJobSequences = new List<JobSequence>()
            };
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }


        // --- TESTS ---
        // --- GetByProjectId ---
        [Fact]
        public async Task GetByProjectId_ShouldReturnJobs_WhenProjectHasJobs()
        {
            // Arrange
            var user = await SeedUser();
            var project = await SeedProject(user.Id);

            // Seed two jobs for this project
            await SeedJob(project.Id, user.Id);
            await SeedJob(project.Id, user.Id);

            // Seed one job for a different project (to ensure filtering works)
            var otherProject = await SeedProject(user.Id);
            await SeedJob(otherProject.Id, user.Id);

            // Act
            var result = await _service.GetByProjectId(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.All(j => j.ProjectId == project.Id).Should().BeTrue();
        }

        [Fact]
        public async Task GetByProjectId_ShouldReturnEmpty_WhenProjectHasNoJobs()
        {
            // Arrange
            var user = await SeedUser();
            var project = await SeedProject(user.Id);

            // Seed no jobs 

            // Act
            var result = await _service.GetByProjectId(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetByProjectId_ShouldThrowBadRequestExceptionAndLogError_WhenNoSuchProjectExists()
        {
            // Arrange
            // random Id
            var projectId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.GetByProjectId(projectId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage); 
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<BadRequestException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByProjectId_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            // force an exception by disposing the context before the call
            _context.Dispose();

            // Act
            Func<Task> act = async () => await _service.GetByProjectId(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // --- GetById ---
        // --- Create --- 
        // --- UpdateContent ---
        // --- UpdateParent ---
        // --- CreateLink ---
        // --- DeleteLink --- 
        // --- Delete --- 

        public void Dispose()
        {
            try
            {
                _context.Database.EnsureDeleted();
            }
            catch (ObjectDisposedException) {}
            finally
            {
                _context.Dispose();
            }
        }
    }
}