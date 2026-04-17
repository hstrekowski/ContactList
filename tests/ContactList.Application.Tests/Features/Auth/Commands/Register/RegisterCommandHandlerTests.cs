using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Identity;
using ContactList.Application.Features.Auth.Commands.Register;
using FluentAssertions;
using Moq;

namespace ContactList.Application.Tests.Features.Auth.Commands.Register
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IUserService> _userService = new(MockBehavior.Strict);

        [Fact]
        public async Task Handle_ValidCommand_ReturnsResponseDto()
        {
            // Arrange
            var expected = new AuthResponseDto(Guid.NewGuid(), "user@example.com", "jwt-token", DateTime.UtcNow.AddHours(1));

            _userService
                .Setup(s => s.RegisterAsync(
                    It.Is<AuthRequestDto>(r => r.Email == "user@example.com" && r.Password == "Password1!"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var handler = new RegisterCommandHandler(_userService.Object);
            var command = new RegisterCommand("user@example.com", "Password1!");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(expected);
            _userService.VerifyAll();
        }

        [Fact]
        public async Task Handle_ServiceThrowsConflictException_ThrowsConflictException()
        {
            // Arrange
            _userService.Setup(s => s.RegisterAsync(It.IsAny<AuthRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("Email already taken."));

            var handler = new RegisterCommandHandler(_userService.Object);
            var command = new RegisterCommand("user@example.com", "Password1!");

            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task Handle_ServiceThrowsValidationException_ThrowsValidationException()
        {
            // Arrange
            _userService.Setup(s => s.RegisterAsync(It.IsAny<AuthRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException());

            var handler = new RegisterCommandHandler(_userService.Object);
            var command = new RegisterCommand("user@example.com", "Password1!");

            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}