using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Models;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Misc;
using Xunit;


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
            _service = new JobService(_loggerMock.Object, _context);
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
            var projectId = Guid.NewGuid(); // Random ID with no data

            // Act
            var result = await _service.GetByProjectId(projectId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByProjectId_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            // We force an exception by disposing the context before the call
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
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        public void Dispose()
        {
            try
            {
                // Check if context is already disposed or if database is accessible
                _context.Database.EnsureDeleted();
            }
            catch (ObjectDisposedException)
            {
                // Silence this exception during cleanup if the test already disposed it
            }
            finally
            {
                _context.Dispose();
            }
        }
    }
}