using ContactList.Application.Common.Behaviours;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactList.Application.Tests.Common.Behaviours
{
    public class UnhandledExceptionBehaviourTests
    {
        public sealed record SampleRequest(string Value) : IRequest<string>;

        private readonly Mock<ILogger<SampleRequest>> _logger = new();

        [Fact]
        public async Task Handle_SuccessfulExecution_ReturnsResponseWithoutLogging()
        {
            // Arrange
            var behaviour = new UnhandledExceptionBehaviour<SampleRequest, string>(_logger.Object);

            // Act
            var result = await behaviour.Handle(
                new SampleRequest("x"),
                () => Task.FromResult("ok"),
                CancellationToken.None);

            // Assert
            result.Should().Be("ok");
            VerifyLogged(LogLevel.Error, Times.Never());
        }

        [Fact]
        public async Task Handle_HandlerThrowsException_LogsErrorAndRethrows()
        {
            // Arrange
            var behaviour = new UnhandledExceptionBehaviour<SampleRequest, string>(_logger.Object);
            var original = new InvalidOperationException("boom");

            var act = async () => await behaviour.Handle(
                new SampleRequest("x"),
                () => throw original,
                CancellationToken.None);

            // Act & Assert
            (await act.Should().ThrowAsync<InvalidOperationException>())
                .Which.Should().BeSameAs(original);
            VerifyLogged(LogLevel.Error, Times.Once());
        }

        private void VerifyLogged(LogLevel level, Times times)
        {
            _logger.Verify(l => l.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }
    }
}