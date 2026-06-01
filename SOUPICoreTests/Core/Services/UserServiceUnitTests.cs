using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SOUPICore;
using SOUPICore.Services;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using static SOUPITests.Helpers.Helpers;


namespace SOUPITests.Core.Services
{
    public class UserServiceUnitTests
    {
        private readonly DbContextOptions<SoupiDbContext> _options;
        private readonly UserService _service;
        private readonly Mock<IDbContextFactory<SoupiDbContext>> _contextFactoryMock = new();

        public UserServiceUnitTests()
        {
            _options = new DbContextOptionsBuilder<SoupiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<UserService>>();

            _contextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new SoupiDbContext(_options));

            _service = new UserService(_contextFactoryMock.Object, loggerMock.Object);
        }

        // --- Get --- 
        [Fact]
        public async Task Get_ShouldReturnAllUsers()
        {
            // Arrange
            var user1 = await SeedUser(_contextFactoryMock.Object);
            var user2 = await SeedUser(_contextFactoryMock.Object);

            // Act
            var users = await _service.Get();

            // Assert
            users.Should().HaveCount(x => x >= 2);
            users.Select(u => u.Id).Should().Contain(new[] { user1.Id, user2.Id });
        }


        // --- GetById --- 
        [Fact]
        public async Task GetById_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var user = await SeedUser(_contextFactoryMock.Object);

            // Act
            var result = await _service.GetById(user.Id);

            // Assert
            result.Id.Should().Be(user.Id);
            result.Login.Should().Be(user.Login);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException_WhenDoesntExist()
        {
            // Act
            Func<Task> act = async () => await _service.GetById(Guid.NewGuid());
            
            // Assert 
            await act.Should().ThrowAsync<NotFoundException>();
        }


        // --- GetByLogin --- 
        [Fact]
        public async Task GetByLogin_ShouldReturnUser_WhenExists()
        {
            // Arrange 
            var user = await SeedUser(_contextFactoryMock.Object);

            // Act
            var result = await _service.GetByLogin(user.Login);

            // Assert 
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result!.Login.Should().Be(user.Login);
        }

        [Fact]
        public async Task GetByLogin_ShouldReturnNull_WhenDoesntExist()
        {
            // Act 
            var result = await _service.GetByLogin("nonexistent");

            // Assert 
            result.Should().BeNull();
        }


        // --- Create --- 
        [Fact]
        public async Task Create_ShouldAddUserAndReturnDto_WhenDtoValid()
        {
            // Arrange 
            var newUserDto = new UserDto { Login = "newuser" };

            // Act
            var result = await _service.Create(newUserDto);

            // Assert
            result.Login.Should().Be("newuser");
            result.Id.Should().NotBe(Guid.Empty);
        }
    }
}
