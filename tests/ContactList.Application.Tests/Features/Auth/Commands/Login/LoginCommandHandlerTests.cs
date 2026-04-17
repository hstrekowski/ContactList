using ContactList.Application.Contracts.Identity;
using ContactList.Application.Features.Auth.Commands.Login;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Auth.Commands.Login
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserService> _userService = new(MockBehavior.Strict);

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsResponseDto()
        {
            // Arrange
            var expected = new AuthResponseDto(Guid.NewGuid(), "user@example.com", "jwt-token", DateTime.UtcNow.AddHours(1));

            _userService
                .Setup(s => s.LoginAsync(
                    It.Is<AuthRequestDto>(r => r.Email == "user@example.com" && r.Password == "Password1!"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var handler = new LoginCommandHandler(_userService.Object);
            var command = new LoginCommand("user@example.com", "Password1!");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task Handle_ServiceReturnsNull_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userService.Setup(s => s.LoginAsync(It.IsAny<AuthRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AuthResponseDto?)null);

            var handler = new LoginCommandHandler(_userService.Object);
            var command = new LoginCommand("unknown@example.com", "wrong");

            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Act & Assert
            (await act.Should().ThrowAsync<UnauthorizedAccessException>())
                .WithMessage("Invalid email or password.");
        }
    }
}