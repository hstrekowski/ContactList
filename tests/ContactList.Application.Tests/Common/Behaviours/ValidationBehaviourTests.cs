using ContactList.Application.Common.Behaviours;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using ApplicationValidationException = ContactList.Application.Common.Exceptions.ValidationException;

namespace ContactList.Application.Tests.Common.Behaviours
{
    public class ValidationBehaviourTests
    {
        public sealed record SampleRequest(string Value) : IRequest<string>;

        private static RequestHandlerDelegate<string> NextReturning(string response, Action? onCalled = null) =>
            () =>
            {
                onCalled?.Invoke();
                return Task.FromResult(response);
            };

        [Fact]
        public async Task Handle_WithoutValidators_PassesThroughToNext()
        {
            // Arrange
            var behaviour = new ValidationBehaviour<SampleRequest, string>(Array.Empty<IValidator<SampleRequest>>());
            var nextCalled = false;

            // Act
            var result = await behaviour.Handle(
                new SampleRequest("anything"),
                NextReturning("ok", () => nextCalled = true),
                CancellationToken.None);

            // Assert
            result.Should().Be("ok");
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_PassingValidators_PassesThroughToNext()
        {
            // Arrange
            var validator = new Mock<IValidator<SampleRequest>>(MockBehavior.Strict);
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var behaviour = new ValidationBehaviour<SampleRequest, string>(new[] { validator.Object });
            var nextCalled = false;

            // Act
            var result = await behaviour.Handle(
                new SampleRequest("ok"),
                NextReturning("response", () => nextCalled = true),
                CancellationToken.None);

            // Assert
            result.Should().Be("response");
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_FailingValidator_ThrowsValidationExceptionAndDoesNotCallNext()
        {
            // Arrange
            var failure = new ValidationFailure("Value", "Value is required.");
            var validator = new Mock<IValidator<SampleRequest>>(MockBehavior.Strict);
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { failure }));

            var behaviour = new ValidationBehaviour<SampleRequest, string>(new[] { validator.Object });
            var nextCalled = false;

            var act = async () => await behaviour.Handle(
                new SampleRequest(""),
                NextReturning("never", () => nextCalled = true),
                CancellationToken.None);

            // Act & Assert
            var ex = await act.Should().ThrowAsync<ApplicationValidationException>();
            ex.Which.Errors.Should().ContainKey("Value");
            ex.Which.Errors["Value"].Should().Contain("Value is required.");
            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_MultipleFailingValidators_AggregatesFailures()
        {
            // Arrange
            var v1 = new Mock<IValidator<SampleRequest>>(MockBehavior.Strict);
            v1.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("A", "a-error") }));

            var v2 = new Mock<IValidator<SampleRequest>>(MockBehavior.Strict);
            v2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("B", "b-error") }));

            var behaviour = new ValidationBehaviour<SampleRequest, string>(new[] { v1.Object, v2.Object });

            var act = async () => await behaviour.Handle(
                new SampleRequest("x"),
                NextReturning("never"),
                CancellationToken.None);

            // Act & Assert
            var ex = await act.Should().ThrowAsync<ApplicationValidationException>();
            ex.Which.Errors.Should().ContainKeys("A", "B");
        }
    }
}