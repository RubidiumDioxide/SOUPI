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
    public class ProjectServiceUnitTests
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly ProjectService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new();

        public ProjectServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<ProjectService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new SoupiDbContext(_options));

            _service = new ProjectService(_contextFactoryMock.Object, loggerMock.Object);
        }


        // --- GetByUserId ---
        [Fact]
        public async Task GetByUserId_ShouldReturnProjectsForUser()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project1 = await SeedProject(_contextFactoryMock.Object, user.Id);
            var project2 = await SeedProject(_contextFactoryMock.Object, user.Id);
            
            await SeedTeamMember(_contextFactoryMock.Object, user.Id, project1.Id, null);
            await SeedTeamMember(_contextFactoryMock.Object, user.Id, project2.Id, null);

            // Act
            var result = await _service.GetByUserId(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().Contain(p => p.Id == project1.Id);
            result.Should().Contain(p => p.Id == project2.Id);
        }

        [Fact]
        public async Task GetByUserId_ShouldThrowBadRequestException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var project1 = await SeedProject(_contextFactoryMock.Object, userId);
            var project2 = await SeedProject(_contextFactoryMock.Object, userId);

            await SeedTeamMember(_contextFactoryMock.Object, userId, project1.Id, null);
            await SeedTeamMember(_contextFactoryMock.Object, userId, project2.Id, null);

            // Act
            Func<Task> act = async () => await _service.GetByUserId(userId);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                     .WithMessage(ServiceErrorMessages.UserNotFound);
        }


        // --- GetById ---
        [Fact]
        public async Task GetById_ShouldReturnProjectDisplayDto_WhenProjectExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatorId = project.CreatorId
            };

            // Act
            var result = await _service.GetById(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(project.Id);
            result.Title.Should().Be(project.Title);
        }

        [Fact]
        public async Task GetById_ShouldThrow_WhenProjectDoesNotExist()
        {
            // Act
            Func<Task> act = async () => await _service.GetById(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }


        // --- Create --- 
        [Fact]
        public async Task Create_ShouldReturnProjectDtoAndCreateTeamMember_WhenProjectIsValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var projectDto = new ProjectDto
            {
                Title = "Test Project",
                Description = "Test Description",
                CreatorId = user.Id,
                GithubRepository = null
            };

            // Act
            var result = await _service.Create(projectDto);

            // Assert
            using (var _assertContext = await _contextFactoryMock.Object.CreateDbContextAsync())
            {
                var createdTeamMember = await _assertContext.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == user.Id && tm.ProjectId == result.Id); 

                result.Should().NotBeNull();
                result.Title.Should().Be(projectDto.Title);
                result.Description.Should().Be(projectDto.Description);
                result.CreatorId.Should().Be(projectDto.CreatorId); 
                createdTeamMember.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Create_ShouldThrowBadRequestException_WhenProjectAlreadyExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var projectDto = new ProjectDto
            {
                Title = "Duplicate Project",
                Description = "Test Description",
                CreatorId = user.Id,
                GithubRepository = null
            };
            await _service.Create(projectDto);

            // Act
            Func<Task> act = async () => await _service.Create(projectDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ProjectAlreadyExists);
        }


        [Fact]
        public async Task Create_ShouldThrowBadRequestException_WhenRepositoryAlreadyLinkedToProject()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project1Dto = new ProjectDto
            {
                Title = "Project1",
                Description = "Description",
                CreatorId = user.Id,
                GithubRepository = "SOUPI"
            };
            await _service.Create(project1Dto); 
            var project2Dto = new ProjectDto
            {
                Title = "Project2",
                Description = "Description",
                CreatorId = user.Id,
                GithubRepository = "SOUPI"
            };

            // Act
            Func<Task> act = async () => await _service.Create(project2Dto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.RepositoryAlreadyLinkedToProject);
        }


        // --- Update --- 
        [Fact]
        public async Task Update_ShouldUpdateProject_WhenProjectExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var updatedDto = new ProjectDto
            {
                Id = project.Id,
                Title = "Updated Title",
                Description = "Updated Description",
                CreatorId = project.CreatorId 
            };

            // Act
            var result = await _service.Update(updatedDto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Updated Title");
            result.Description.Should().Be("Updated Description");
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenProjectDoesNotExist()
        {
            // Arrange
            var updatedDto = new ProjectDto
            {
                Id = Guid.NewGuid(),
                Title = "Nonexistent",
                Description = "Nonexistent",
                CreatorId = Guid.NewGuid()
            };

            // Act
            Func<Task> act = async () => await _service.Update(updatedDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }


        // --- SetGitHubRepository --- 
        [Fact]
        public async Task SetGitHubRepository_ShouldSetRepository_WhenValid()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);
            var repoName = "SOUPI";

            // Act
            var result = await _service.SetGitHubRepository(project.Id, repoName);

            // Assert
            result.GithubRepository.Should().Be(repoName);
        }

        [Fact]
        public async Task SetGitHubRepository_ShouldThrow_WhenGitHubRepositoryNameEmpty()
        {
            // Act
            Func<Task> act = async () => await _service.SetGitHubRepository(Guid.NewGuid(), "");

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.RepositoryNameNotValid);
        }

        [Fact]
        public async Task SetGitHubRepository_ShouldThrow_WhenProjectNotFound()
        {
            // Act
            Func<Task> act = async () => await _service.SetGitHubRepository(Guid.NewGuid(), "SOUPI");

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ProjectNotFound);
        }

        [Fact]
        public async Task SetGitHubRepository_ShouldThrow_WhenProjectAlreadyHasRepository()
        {
            // Act
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id); 
            await _service.SetGitHubRepository(project.Id, "SOUPI"); 

            Func<Task> act = async () => await _service.SetGitHubRepository(project.Id, "SOUPI2");

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ProjectAlreadyHasRepository);
        }

        [Fact]
        public async Task SetGitHubRepository_ShouldThrow_WhenRepositoryAlreadyLinkedToProject()
        {
            // Act
            var user = await SeedUser(_contextFactoryMock.Object);
            var project1 = await SeedProject(_contextFactoryMock.Object, user.Id);
            var project2Dto = new ProjectDto
            {
                Title = "Project1",
                Description = "Description",
                CreatorId = user.Id 
            };
            await _service.SetGitHubRepository(project1.Id, "SOUPI");
            var project2 = await _service.Create(project2Dto); 
            
            Func<Task> act = async () => await _service.SetGitHubRepository(project2.Id, "SOUPI");

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.RepositoryAlreadyLinkedToProject);
        }


        // --- Delete --- 
        [Fact]
        public async Task Delete_ShouldRemoveProject_WhenProjectExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);
            var project = await SeedProject(_contextFactoryMock.Object, user.Id);

            // Act
            await _service.Delete(project.Id);

            // Assert
            Func<Task> act = async () => await _service.GetById(project.Id);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenProjectDoesNotExist()
        {
            // Act
            Func<Task> act = async () => await _service.Delete(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage(ServiceErrorMessages.ProjectNotFound);
        }
    }
}
