using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Extensions;
using SOUPIShared.Misc;
using SOUPIShared.Models;
using SOUPIShared.Resources;
using static SOUPITests.Helpers.Helpers; 


namespace SOUPITests.Core.Services
{
    public class JobSequenceServiceUnitTests : IDisposable 
    {
        private readonly SoupiDbContext _context;
        private readonly JobSequenceService _service;
        private readonly Mock<ILogger<JobSequenceService>> _loggerMock = new();

        public JobSequenceServiceUnitTests()
        {
            var options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options; 

            _context = new SoupiDbContext(options);
            _loggerMock = new Mock<ILogger<JobSequenceService>>();

            var localizationOptions = Microsoft.Extensions.Options.Options.Create(new LocalizationOptions
            {
                ResourcesPath = ""
            });

            var factory = new ResourceManagerStringLocalizerFactory(
                localizationOptions,
                Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);

            var localizer = new StringLocalizer<JobSequenceServiceErrorMessages>(factory);

            _service = new JobSequenceService(_context, _loggerMock.Object, localizer);
        }


        // --- Create --- 
        [Fact]
        public async Task Create_ShouldReturnJobSequenceDto_WhenJobDtoIsValid()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id); 
            var secondJob = await SeedJob(_context, project.Id, user.Id); 

            // Act
            var createdJobSequenceDto = await _service.Create(firstJob.Id, secondJob.Id); 

            // Assert
            createdJobSequenceDto.Should().NotBeNull(); 
            var createdJobSequence = await _context.JobSequences.FindAsync(createdJobSequenceDto.Id);
            createdJobSequence.Should().NotBeNull();

            createdJobSequenceDto.IsEquivalent(createdJobSequence).Should().Be(true);
            createdJobSequence.FirstJobId.Should().Be(firstJob.Id); 
            createdJobSequence.SecondJobId.Should().Be(secondJob.Id); 
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenFirstJobDoesntExist()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJobId = Guid.NewGuid(); 
            var secondJob = await SeedJob(_context, project.Id, user.Id);
            string expectedMessage = JobSequenceServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(firstJobId, secondJob.Id);

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
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenSecondJobDoesntExist()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJobId = Guid.NewGuid();
            string expectedMessage = JobSequenceServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(firstJob.Id, secondJobId);

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
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenSecondJobsAreDifferentLevel()
        {
            // Arrange
            var user = await SeedUser(_context); 
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);
            string expectedMessage = JobSequenceServiceErrorMessages.JobsDifferentLevels; 

            // Act
            Func<Task> act = async () => await _service.Create(firstJob.Id, secondJob.Id);

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
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenJobSequenceAlreadyExists()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id);
            var jobSequence = await SeedJobSequence(_context, firstJob.Id, secondJob.Id); 
            string expectedMessage = JobSequenceServiceErrorMessages.JobSequenceAlreadyExists;

            // Act
            Func<Task> act = async () => await _service.Create(firstJob.Id, secondJob.Id);

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
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenJobSequenceCyclic()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id);
            var jobSequence = await SeedJobSequence(_context, firstJob.Id, secondJob.Id); 
            string expectedMessage = JobSequenceServiceErrorMessages.JobSequenceCyclic;

            // Act
            Func<Task> act = async () => await _service.Create(secondJob.Id, firstJob.Id);

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




        // --- Delete --- 





        public void Dispose()
        {
            try
            {
                _context.Database.EnsureDeleted();
            }
            catch (ObjectDisposedException) { }
            finally
            {
                _context.Dispose();
            }
        }
    }
}
