using System;
using FluentAssertions;
using Xunit;
using ContactList.Domain.Common;

namespace ContactList.Domain.Tests.Common
{
    public class BaseEntityTests
    {
        private class TestEntity : BaseEntity
        {
            public TestEntity() : base() { }
            public TestEntity(Guid id)
            {
                Id = id;
            }
        }

        private class AnotherTestEntity : BaseEntity
        {
            public AnotherTestEntity(Guid id)
            {
                Id = id;
            }
        }

        [Fact]
        public void Constructor_ShouldInitializeId_WithNonEmptyGuid()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_ShouldGenerateUniqueIds_ForDifferentInstances()
        {
            // Arrange & Act
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            // Assert
            entity1.Id.Should().NotBe(entity2.Id);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            var result = entity.Equals(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentObjectType_ShouldReturnFalse()
        {
            // Arrange
            var entity = new TestEntity();
            var someOtherObject = new object();

            // Act
            var result = entity.Equals(someOtherObject);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithSameReference_ShouldReturnTrue()
        {
            // Arrange
            var entity = new TestEntity();
            var sameEntity = entity;

            // Act
            var result = entity.Equals(sameEntity);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_WithSameTypeAndSameId_ShouldReturnTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);

            // Act
            var result = entity1.Equals(entity2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_WithSameTypeButDifferentId_ShouldReturnFalse()
        {
            // Arrange
            var entity1 = new TestEntity(Guid.NewGuid());
            var entity2 = new TestEntity(Guid.NewGuid());

            // Act
            var result = entity1.Equals(entity2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentTypeButSameId_ShouldReturnFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new AnotherTestEntity(id);

            // Act
            var result = entity1.Equals(entity2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForSameId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);

            // Act
            var hashCode1 = entity1.GetHashCode();
            var hashCode2 = entity2.GetHashCode();

            // Assert
            hashCode1.Should().Be(hashCode2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnDifferentValue_ForDifferentId()
        {
            // Arrange
            var entity1 = new TestEntity(Guid.NewGuid());
            var entity2 = new TestEntity(Guid.NewGuid());

            // Act
            var hashCode1 = entity1.GetHashCode();
            var hashCode2 = entity2.GetHashCode();

            // Assert
            hashCode1.Should().NotBe(hashCode2);
        }

        [Fact]
        public void EqualityOperator_WithSameId_ShouldReturnTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);

            // Act
            var result = entity1 == entity2;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void InequalityOperator_WithDifferentId_ShouldReturnTrue()
        {
            // Arrange
            var entity1 = new TestEntity(Guid.NewGuid());
            var entity2 = new TestEntity(Guid.NewGuid());

            // Act
            var result = entity1 != entity2;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void EqualityOperator_WithBothNull_ShouldReturnTrue()
        {
            // Arrange
            TestEntity? entity1 = null;
            TestEntity? entity2 = null;

            // Act
            var result = entity1 == entity2;

            // Assert
            result.Should().BeTrue();
        }
    }
}