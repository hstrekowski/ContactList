using ContactList.Application.Common.Behaviours;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactList.Application.Tests.Common.Behaviours
{
    public class LoggingBehaviourTests
    {
        public sealed record SampleRequest(string Value) : IRequest<string>;

        private readonly Mock<ILogger<SampleRequest>> _logger = new();

        [Fact]
        public async Task Handle_SuccessfulExecution_LogsHandlingAndHandled()
        {
            // Arrange
            var behaviour = new LoggingBehaviour<SampleRequest, string>(_logger.Object);

            // Act
            var result = await behaviour.Handle(
                new SampleRequest("x"),
                () => Task.FromResult("ok"),
                CancellationToken.None);

            // Assert
            result.Should().Be("ok");
            VerifyLogged(LogLevel.Information, "Handling", Times.Once());
            VerifyLogged(LogLevel.Information, "Handled", Times.Once());
        }

        [Fact]
        public async Task Handle_HandlerThrowsException_LogsHandlingOnly()
        {
            // Arrange
            var behaviour = new LoggingBehaviour<SampleRequest, string>(_logger.Object);

            var act = async () => await behaviour.Handle(
                new SampleRequest("x"),
                () => throw new InvalidOperationException("boom"),
                CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            VerifyLogged(LogLevel.Information, "Handling", Times.Once());
            VerifyLogged(LogLevel.Information, "Handled", Times.Never());
        }

        private void VerifyLogged(LogLevel level, string messageContains, Times times)
        {
            _logger.Verify(l => l.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(messageContains)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }
    }
}