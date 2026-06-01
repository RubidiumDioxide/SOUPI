using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Dtos.SOUPIDtos; 
using SOUPIShared.Exceptions;
using SOUPIShared.Misc;
using SOUPIShared.Resources;
using static SOUPIShared.Extensions.JobDtoExtensions;
using static SOUPITests.Helpers.Helpers; 


namespace SOUPITests.Core.Services
{
    public class JobServiceUnitTests 
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly JobService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new(); 

        public JobServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<JobService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new SoupiDbContext(_options));

            _service = new JobService(_contextFactoryMock.Object, loggerMock.Object);
        }


        // --- GetByProjectId ---
        [Fact]
        public async Task GetByProjectIdParentId_ShouldReturnJobs_WhenProjectHasJobs()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, project.Id, user.Id, null); 

            // Seed two jobs for this project
            await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);

            // Seed one job for a different project (to ensure filtering works)
            var otherProject = await SeedProject(_contextFactoryMock.Object, user.Id);
            var otherTeamMember = await SeedTeamMember(_contextFactoryMock.Object, otherProject.Id, user.Id, null);
            await SeedJob(_contextFactoryMock.Object, otherProject.Id, otherTeamMember.Id);

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
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);

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
        }

        [Fact]
        public async Task GetByProjectIdParentId_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            // force an exception 
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.GetByProjectIdParentId(Guid.NewGuid(), null);

            // Assert
            await act.Should().ThrowAsync<Exception>(); 
        }


        // --- GetById ---
        [Fact]
        public async Task GetById_ShouldReturnJob_WhenIdIsValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);

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
        }

        [Fact]
        public async Task GetById_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            // force an exception 
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.GetById(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- Create --- 
        [Fact]
        public async Task Create_ShouldReturnJobDto_WhenJobDtoIsValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var newJobDto = SeedJobDto(project.Id, teamMember.Id);

            // Act
            var createdJobDto = await _service.Create(newJobDto);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                createdJobDto.Should().NotBeNull();
                var createdJob = await _assertContext.Jobs.FindAsync(createdJobDto.Id);
                createdJob.Should().NotBeNull();

                // sent dto and created object should be property equivalent  
                newJobDto.AreNonKeyPropertiesEquivalent(createdJob).Should().Be(true);

                // received dto and created object should be property equivalent
                createdJobDto.IsEquivalent(createdJob).Should().Be(true);

                // sent and received dtos should be property equivalent 
                createdJobDto.AreNonKeyPropertiesEquivalent(newJobDto).Should().Be(true);
            }
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenProjectDosentExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var projectId = Guid.NewGuid(); 
            var newJobDto = SeedJobDto(projectId, user.Id);
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage); 
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenCreatorDosentExist()
        {
            // Arrange
            var userId = Guid.NewGuid(); 
            var project = await SeedProject(_contextFactoryMock.Object, userId); 
            var newJobDto = SeedJobDto(project.Id, userId);
            string expectedMessage = ServiceErrorMessages.TeamMemberNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenParentJobDosentExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object); 
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var parentJobId = Guid.NewGuid();  
            var newJobDto = SeedJobDto(project.Id, user.Id, parentJobId);
            string expectedMessage = ServiceErrorMessages.ParentJobNotFound;

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrowBadRequestException_WhenEndDateEarlierThanStartDate()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var newJobDto = new JobDto
            {
                Id = Guid.NewGuid(), 
                ProjectId = project.Id, 
                CreatorId = teamMember.Id, 
                Title = "Test Job", 
                StartDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(1)), 
                EndDateTime = DateOnly.FromDateTime(DateTime.UtcNow), 
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
        }

        [Fact]
        public async Task Create_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange 
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var newJobDto = SeedJobDto(project.Id, user.Id);

            // force an exception 
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.Create(newJobDto);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- UpdateContent ---
        [Fact]
        public async Task UpdateContent_ShouldReturnJobDto_WhenJobExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody"; 
            updatedJobDto.StartDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)); 
            updatedJobDto.EndDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(11));
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working; 

            // Act
            var newJobDto = await _service.UpdateContent(updatedJobDto);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                newJobDto.Should().NotBeNull();
                var updatedJob = await _assertContext.Jobs.FindAsync(newJobDto.Id);
                updatedJob.Should().NotBeNull();

                // sent dto and created object should be property equivalent  
                newJobDto.AreNonKeyPropertiesEquivalent(updatedJob).Should().Be(true);

                // received dto and created object should be property equivalent
                newJobDto.IsEquivalent(updatedJob).Should().Be(true);

                // sent and received dtos should be property equivalent 
                updatedJobDto.AreNonKeyPropertiesEquivalent(newJobDto).Should().Be(true);
            }
        }

        [Fact]
        public async Task UpdateContent_ShouldLogErrorAndThrowBadRequestException_WhenProjectDosentExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var projectId = Guid.NewGuid(); 
            var job = await SeedJob(_contextFactoryMock.Object, projectId, user.Id);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody";
            updatedJobDto.StartDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
            updatedJobDto.EndDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(11));
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working;
            string expectedMessage = ServiceErrorMessages.ProjectNotFound;

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedJobDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task UpdateContent_ShouldLogErrorAndThrowBadRequestException_WhenCreatorDosentExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var project = await SeedProject(_contextFactoryMock.Object, userId);
            var teamMemberId = Guid.NewGuid();
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMemberId);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody";
            updatedJobDto.StartDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
            updatedJobDto.EndDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(11));
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working;
            string expectedMessage = ServiceErrorMessages.TeamMemberNotFound;

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedJobDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task UpdateContent_ShouldLogErrorAndThrowBadRequestException_WhenEndDateEarlierThanStartDate()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object); 
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var job = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var updatedJobDto = new JobDto(job);
            updatedJobDto.Title = "newTitle";
            updatedJobDto.Body = "newBody";
            updatedJobDto.StartDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(11));
            updatedJobDto.EndDateTime = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
            updatedJobDto.Progress = 70;
            updatedJobDto.Status = JobStatus.Working;
            string expectedMessage = ServiceErrorMessages.JobIncompatibleEndStartDates;

            // Act
            Func<Task> act = async () => await _service.UpdateContent(updatedJobDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task UpdateContent_ShouldLogErrorAndThrow_WhenExceptionOccurs()
        {
            // Arrange 
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var newJobDto = SeedJobDto(project.Id, user.Id);

            // force an exception 
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.UpdateContent(newJobDto);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- UpdateParent ---
        [Fact]
        public async Task UpdateParent_ShouldReturnJobDtoAndDeleteAssociatedJobSequences_WhenUpdateValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id, null);
            var thirdJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id, null);
            var fourthJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id, null);
            var secondThirdJob = await SeedJobSequence(_contextFactoryMock.Object, secondJob.Id, thirdJob.Id); 
            var thirdFourthJob = await SeedJobSequence(_contextFactoryMock.Object, thirdJob.Id, fourthJob.Id);

            // Act
            var updatedJobDto = await _service.UpdateParent(thirdJob.Id, firstJob.Id);

            // Assert 
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                updatedJobDto.Should().NotBeNull();
                var updatedJob = await _assertContext.Jobs.FindAsync(updatedJobDto.Id);
                updatedJob.Should().NotBeNull();
                var associatedJobSequences = await _assertContext.JobSequences
                    .Where(js => js.FirstJobId == thirdJob.Id
                        || js.SecondJobId == thirdJob.Id)
                    .ToListAsync();
                associatedJobSequences.Count().Should().Be(0);

                updatedJobDto.IsEquivalent(updatedJob).Should().Be(true);
                updatedJob.ParentJobId.Should().Be(firstJob.Id);
            }
        }

        [Fact]
        public async Task UpdateParent_ShouldLogErrorAndThrowBadRequestException_WhenJobDosentExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJobId = Guid.NewGuid();
            string expectedMessage = ServiceErrorMessages.JobNotFound; 

            // Act
            Func<Task> act = async () => await _service.UpdateParent(secondJobId, firstJob.Id); 

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task UpdateParent_ShouldLogErrorAndThrowBadRequestException_WhenParentJobDosentExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object); 
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            string expectedMessage = ServiceErrorMessages.ParentJobNotFound;

            // Act
            Func<Task> act = async () => await _service.UpdateParent(firstJob.Id, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage); 
        }

        [Fact]
        public async Task UpdateParent_ShouldLogErrorAndThrowBadRequestException_WhenHierarchyCyclic()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            var teamMember = await SeedTeamMember(_contextFactoryMock.Object, user.Id, project.Id, null);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id, firstJob.Id);
            var thirdJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id, secondJob.Id);
            var fourthJob = await SeedJob(_contextFactoryMock.Object, project.Id, teamMember.Id, secondJob.Id);

            string expectedMessage = ServiceErrorMessages.JobCyclic;

            // Act
            Func<Task> act = async () => await _service.UpdateParent(firstJob.Id, fourthJob.Id);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(expectedMessage); 
        }

        [Fact]
        public async Task UpdateParent_ShouldLogErrorAndThrow_WhenExceptionOccurs()
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
            Func<Task> act = async () => await _service.UpdateParent(secondJob.Id, firstJob.Id); 

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }


        // --- Delete --- 
        [Fact]
        public async Task Delete_WithPreserveChildrenTrue_ShouldDeleteJobAndMoveChildrenToJobsParentAndDeleteAssociatedJobSequences_WhenJobExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var firstJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id);
            var secondJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id, firstJob.Id);             // target
            var thirdJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id, secondJob.Id);
            var fourthJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id, secondJob.Id);
            var fifthJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id, firstJob.Id);
            var sixthJob = await SeedJob(_contextFactoryMock.Object, project.Id, user.Id, firstJob.Id);

            var fifthSecondJob = await SeedJobSequence(_contextFactoryMock.Object, fifthJob.Id, secondJob.Id);
            var secondSixthJob = await SeedJobSequence(_contextFactoryMock.Object, secondJob.Id, sixthJob.Id);
            // Act
            await _service.Delete(secondJob.Id);

            // Assert 
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                var deletedJob = await _assertContext.Jobs.FindAsync(secondJob.Id);
                deletedJob.Should().Be(null);

                var associatedJobSequences = await _assertContext.JobSequences
                    .Where(js => js.FirstJobId == secondJob.Id
                        || js.SecondJobId == secondJob.Id)
                    .ToListAsync();
                associatedJobSequences.Count().Should().Be(0);

                var foundThirdJob = await _assertContext.Jobs.FindAsync(thirdJob.Id);
                var foundFourthJob = await _assertContext.Jobs.FindAsync(fourthJob.Id);
                foundThirdJob.Should().NotBe(null);
                foundFourthJob.Should().NotBe(null);
                foundThirdJob.ParentJobId.Should().Be(firstJob.Id);
                foundFourthJob.ParentJobId.Should().Be(firstJob.Id);
            }
        }

        [Fact]
        public async Task Delete_ShouldLogErrorAndThrowBadRequestException_WhenJobDoesntExist()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            string expectedMessage = ServiceErrorMessages.JobNotFound;

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

            // force an exception 
            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new InvalidOperationException("Database connection failed")); 

            // Act
            Func<Task> act = async () => await _service.Delete(firstJob.Id); 

            // Assert
            await act.Should().ThrowAsync<Exception>(); 
        }
    }
}