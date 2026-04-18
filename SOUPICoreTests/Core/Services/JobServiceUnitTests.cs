using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Misc;
using SOUPIShared.Resources;
using static SOUPIShared.Extensions.JobDtoExtensions;
using static SOUPITests.Helpers.Helpers; 


namespace SOUPITests.Core.Services
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

            _service = new JobService(_context, _loggerMock.Object);
        }


        // --- TESTS ---
        // --- GetByProjectId ---
        [Fact]
        public async Task GetByProjectIdParentId_ShouldReturnJobs_WhenProjectHasJobs()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);

            // Seed two jobs for this project
            await SeedJob(_context, project.Id, user.Id);
            await SeedJob(_context, project.Id, user.Id);

            // Seed one job for a different project (to ensure filtering works)
            var otherProject = await SeedProject(_context, user.Id);
            await SeedJob(_context, otherProject.Id, user.Id);

            // Act
            var result = await _service.GetByProjectIdParentId(project.Id, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.All(j => j.ProjectId == project.Id).Should().BeTrue();
        }

        [Fact]
        public async Task GetByProjectIdParentId_ShouldReturnEmpty_WhenProjectHasNoJobs()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);

            // Seed no jobs 

            // Act
            var result = await _service.GetByProjectIdParentId(project.Id, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetByProjectIdParentId_ShouldLogErrorAndThrowBadRequestException_WhenNoSuchProjectExists()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.GetByProjectIdParentId(projectId, null);

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
        public async Task GetByProjectIdParentId_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            // force an exception by disposing the context before the call
            _context.Dispose();

            // Act
            Func<Task> act = async () => await _service.GetByProjectIdParentId(Guid.NewGuid(), null);

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
        [Fact]
        public async Task GetById_ShouldReturnJob_WhenIdIsValid()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var job = await SeedJob(_context, project.Id, user.Id);

            // Act
            var result = await _service.GetById(job.Id);

            // Assert
            result.Should().NotBeNull(); 
            result.Id.Should().Be(job.Id); 
        }

        [Fact]
        public async Task GetById_ShouldLogErrorAndThrowNotFoundException_WhenJobWithSpecifiedIdDoesntExist()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.GetById(jobId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
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
        public async Task GetById_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            // force an exception by disposing the context before the call
            _context.Dispose();

            // Act
            Func<Task> act = async () => await _service.GetById(Guid.NewGuid());

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


        // --- Create --- 
        [Fact]
        public async Task Create_ShouldReturnJobDto_WhenJobDtoIsValid()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var newJobDto = SeedJobDto(project.Id, user.Id);

            // Act
            var createdJobDto = await _service.Create(newJobDto); 

            // Assert
            createdJobDto.Should().NotBeNull();
            var createdJob = await _context.Jobs.FindAsync(createdJobDto.Id);
            createdJob.Should().NotBeNull();
            
            // sent dto and created object should be property equivalent  
            newJobDto.AreNonKeyPropertiesEquivalent(createdJob).Should().Be(true);

            // received dto and created object should be property equivalent
            createdJobDto.IsEquivalent(createdJob).Should().Be(true);

            // sent and received dtos should be property equivalent 
            createdJobDto.AreNonKeyPropertiesEquivalent(newJobDto).Should().Be(true);
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenProjectDosentExist()
        {
            // Arrange
            var user = await SeedUser(_context);
            var projectId = Guid.NewGuid(); 
            var newJobDto = SeedJobDto(projectId, user.Id);
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

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
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenCreatorDosentExist()
        {
            // Arrange
            var userId = Guid.NewGuid(); 
            var project = await SeedProject(_context, userId); 
            var newJobDto = SeedJobDto(project.Id, userId);
            string expectedMessage = ServiceErrorMessages.UserNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

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
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenParentJobDosentExist()
        {
            // Arrange
            var user = await SeedUser(_context); 
            var project = await SeedProject(_context, user.Id);
            var parentJobId = Guid.NewGuid();  
            var newJobDto = SeedJobDto(project.Id, user.Id, parentJobId);
            string expectedMessage = ServiceErrorMessages.ParentJobNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

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
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenEndDateEarlierThanStartDate()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var newJobDto = new JobDto
            {
                Id = Guid.NewGuid(), 
                ProjectId = project.Id, 
                CreatorId = user.Id, 
                Title = "Test Job", 
                StartDateTime = DateTime.UtcNow.AddHours(1), 
                EndDateTime = DateTime.UtcNow, 
                Progress = 0, 
                CreationDateTime = DateTime.UtcNow, 
                Status = JobStatus.New
            };

            string expectedMessage = ServiceErrorMessages.JobIncompatibleEndStartDates;

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

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
        public async Task Create_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange 
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var newJobDto = SeedJobDto(project.Id, user.Id);

            // force an exception by disposing the context before the call 
            _context.Dispose();

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

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


        // --- UpdateContent ---
        [Fact]
        public async Task UpdateContent_ShouldReturnJobDto_WhenJobExists()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var job = await SeedJob(_context, project.Id, user.Id);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody"; 
            updatedJobDto.StartDateTime = DateTime.UtcNow.AddDays(10); 
            updatedJobDto.EndDateTime = DateTime.UtcNow.AddDays(11);
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working; 

            // Act
            var newJobDto = await _service.UpdateContent(updatedJobDto);

            // Assert
            newJobDto.Should().NotBeNull();
            var updatedJob = await _context.Jobs.FindAsync(newJobDto.Id);
            updatedJob.Should().NotBeNull();

            // sent dto and created object should be property equivalent  
            newJobDto.AreNonKeyPropertiesEquivalent(updatedJob).Should().Be(true);

            // received dto and created object should be property equivalent
            newJobDto.IsEquivalent(updatedJob).Should().Be(true);

            // sent and received dtos should be property equivalent 
            updatedJobDto.AreNonKeyPropertiesEquivalent(newJobDto).Should().Be(true);
        }

        [Fact]
        public async Task UpdateContent_ShouldLogErrorAndThrowBadRequestException_WhenProjectDosentExist()
        {
            // Arrange
            var user = await SeedUser(_context);
            var projectId = Guid.NewGuid(); 
            var job = await SeedJob(_context, projectId, user.Id);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody";
            updatedJobDto.StartDateTime = DateTime.UtcNow.AddDays(10);
            updatedJobDto.EndDateTime = DateTime.UtcNow.AddDays(11);
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working;
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedJobDto);

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
        public async Task UpdateContent_ShouldLogErrorAndThrowBadRequestException_WhenCreatorDosentExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var project = await SeedProject(_context, userId); 
            var job = await SeedJob(_context, project.Id, userId);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody";
            updatedJobDto.StartDateTime = DateTime.UtcNow.AddDays(10);
            updatedJobDto.EndDateTime = DateTime.UtcNow.AddDays(11);
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working;
            string expectedMessage = ServiceErrorMessages.UserNotFound;

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedJobDto);

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
        public async Task UpdateContent_ShouldLogErrorAndThrowBadRequestException_WhenEndDateEarlierThanStartDate()
        {
            // Arrange
            var user = await SeedUser(_context); 
            var project = await SeedProject(_context, user.Id);
            var job = await SeedJob(_context, project.Id, user.Id);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody";
            updatedJobDto.StartDateTime = DateTime.UtcNow.AddDays(11);
            updatedJobDto.EndDateTime = DateTime.UtcNow.AddDays(10);
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working;
            string expectedMessage = ServiceErrorMessages.JobIncompatibleEndStartDates;

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedJobDto);

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
        public async Task UpdateContent_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange 
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var newJobDto = SeedJobDto(project.Id, user.Id);

            // force an exception by disposing the context before the call 
            _context.Dispose();

            // Act
            Func<Task> act = async () => await _service.UpdateContent(newJobDto);

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


        // --- UpdateParent ---
        [Fact]
        public async Task UpdateParent_ShouldReturnJobDtoAndDeleteAssociatedJobSequences_WhenUpdateValid()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id, null);
            var thirdJob = await SeedJob(_context, project.Id, user.Id, null);
            var fourthJob = await SeedJob(_context, project.Id, user.Id, null);
            var secondThirdJob = await SeedJobSequence(_context, secondJob.Id, thirdJob.Id); 
            var thirdFourthJob = await SeedJobSequence(_context, thirdJob.Id, fourthJob.Id);

            // Act
            var updatedJobDto = await _service.UpdateParent(thirdJob.Id, firstJob.Id);

            // Assert 
            updatedJobDto.Should().NotBeNull(); 
            var updatedJob = await _context.Jobs.FindAsync(updatedJobDto.Id);
            updatedJob.Should().NotBeNull();
            var associatedJobSequences = await _context.JobSequences
                .Where(js => js.FirstJobId == thirdJob.Id
                    || js.SecondJobId == thirdJob.Id)
                .ToListAsync(); 
            associatedJobSequences.Count().Should().Be(0); 

            updatedJobDto.IsEquivalent(updatedJob).Should().Be(true);
            updatedJob.ParentJobId.Should().Be(firstJob.Id); 
        }

        [Fact]
        public async Task UpdateParent_ShouldLogErrorAndThrowBadRequestException_WhenJobDosentExist()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJobId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.JobNotFound; 

            // Act
            Func<Task> act = async () => await _service.UpdateParent(secondJobId, firstJob.Id); 

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
        public async Task UpdateParent_ShouldLogErrorAndThrowBadRequestException_WhenParentJobDosentExist()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            string expectedMessage = ServiceErrorMessages.ParentJobNotFound;

            // Act
            Func<Task> act = async () => await _service.UpdateParent(firstJob.Id, Guid.NewGuid());

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
        public async Task UpdateParent_ShouldLogErrorAndThrowBadRequestException_WhenHierarchyCyclic()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);
            var thirdJob = await SeedJob(_context, project.Id, user.Id, secondJob.Id);
            var fourthJob = await SeedJob(_context, project.Id, user.Id, secondJob.Id);

            string expectedMessage = ServiceErrorMessages.JobCyclic;

            // Act
            Func<Task> act = async () => await _service.UpdateParent(firstJob.Id, fourthJob.Id);

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
        public async Task UpdateParent_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange 
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id); 

            // force an exception by disposing the context before the call 
            _context.Dispose();

            // Act
            Func<Task> act = async () => await _service.UpdateParent(secondJob.Id, firstJob.Id); 

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


        // --- Delete --- 
        [Fact]
        public async Task Delete_WithPreserveChildrenFalse_ShouldDeleteJobAndMoveChildrenToJobsParentAndDeleteAssociatedJobSequences_WhenJobExists()
        {
            // Arrange
            var user = await SeedUser(_context);   
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);             // target
            var thirdJob = await SeedJob(_context, project.Id, user.Id, secondJob.Id);
            var fourthJob = await SeedJob(_context, project.Id, user.Id, secondJob.Id);
            var fifthJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);
            var sixthJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);

            var fifthSecondJob = await SeedJobSequence(_context, fifthJob.Id, secondJob.Id);
            var secondSixthJob = await SeedJobSequence(_context, secondJob.Id, sixthJob.Id);

            // Act
            await _service.Delete(secondJob.Id, false);

            // Assert 
            var deletedJob = await _context.Jobs.FindAsync(secondJob.Id);
            deletedJob.Should().Be(null); 
            
            var associatedJobSequences = await _context.JobSequences
                .Where(js => js.FirstJobId == secondJob.Id
                    || js.SecondJobId == secondJob.Id)
                .ToListAsync();
            associatedJobSequences.Count().Should().Be(0);

            var foundThirdJob = await _context.Jobs.FindAsync(thirdJob.Id); 
            var foundFourthJob = await _context.Jobs.FindAsync(fourthJob.Id); 
            foundThirdJob.Should().Be(null);    
            foundFourthJob.Should().Be(null);
        }

        [Fact]
        public async Task Delete_WithPreserveChildrenTrue_ShouldDeleteJobAndMoveChildrenToJobsParentAndDeleteAssociatedJobSequences_WhenJobExists()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            var secondJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);             // target
            var thirdJob = await SeedJob(_context, project.Id, user.Id, secondJob.Id);
            var fourthJob = await SeedJob(_context, project.Id, user.Id, secondJob.Id);
            var fifthJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);
            var sixthJob = await SeedJob(_context, project.Id, user.Id, firstJob.Id);

            var fifthSecondJob = await SeedJobSequence(_context, fifthJob.Id, secondJob.Id);
            var secondSixthJob = await SeedJobSequence(_context, secondJob.Id, sixthJob.Id);

            // Act
            await _service.Delete(secondJob.Id, true);

            // Assert 
            var deletedJob = await _context.Jobs.FindAsync(secondJob.Id);
            deletedJob.Should().Be(null);

            var associatedJobSequences = await _context.JobSequences
                .Where(js => js.FirstJobId == secondJob.Id
                    || js.SecondJobId == secondJob.Id)
                .ToListAsync();
            associatedJobSequences.Count().Should().Be(0);

            var foundThirdJob = await _context.Jobs.FindAsync(thirdJob.Id);
            var foundFourthJob = await _context.Jobs.FindAsync(fourthJob.Id);
            foundThirdJob.Should().NotBe(null);
            foundFourthJob.Should().NotBe(null);
            foundThirdJob.ParentJobId.Should().Be(firstJob.Id);
            foundFourthJob.ParentJobId.Should().Be(firstJob.Id);
        }

        [Fact]
        public async Task Delete_ShouldLogErrorAndThrowBadRequestException_WhenJobDoesntExist()
        {
            // Arrange
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id); 
            string expectedMessage = ServiceErrorMessages.JobNotFound;

            // Act
            Func<Task> act = async () => await _service.Delete(Guid.NewGuid(), true); 

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
        public async Task Delete_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange 
            var user = await SeedUser(_context);
            var project = await SeedProject(_context, user.Id);
            var firstJob = await SeedJob(_context, project.Id, user.Id);
            
            // force an exception by disposing the context before the call 
            _context.Dispose();

            // Act
            Func<Task> act = async () => await _service.Delete(firstJob.Id, true); 

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