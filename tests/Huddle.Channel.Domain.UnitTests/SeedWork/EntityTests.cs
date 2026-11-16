using Huddle.Channel.Domain.SeedWork;
using MediatR;

namespace Huddle.Channel.Domain.UnitTests.SeedWork;

public class EntityTests
{
    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid id) => Id = id;

        public TestEntity() { }

        public void Raise(INotification notification) => AddDomainEvent(notification);

        public void Drop(INotification notification) => RemoveDomainEvent(notification);
    }

    private sealed class OtherTestEntity : Entity
    {
        public OtherTestEntity(Guid id) => Id = id;
    }

    private sealed record TestNotification : INotification;

    [Fact]
    public void Entities_WithSameId_AreEqual()
    {
        var id = Guid.NewGuid();
        var first = new TestEntity(id);
        var second = new TestEntity(id);

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Entities_WithDifferentIds_AreNotEqual()
    {
        var first = new TestEntity(Guid.NewGuid());
        var second = new TestEntity(Guid.NewGuid());

        Assert.False(first.Equals(second));
        Assert.True(first != second);
    }

    [Fact]
    public void Entities_OfDifferentTypes_AreNotEqual()
    {
        var id = Guid.NewGuid();

        Assert.False(new TestEntity(id).Equals(new OtherTestEntity(id)));
    }

    [Fact]
    public void Entity_IsNotEqualToNull()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Assert.False(((object)entity).Equals(null));
    }

    [Fact]
    public void TransientEntities_AreNotEqual()
    {
        var first = new TestEntity(Guid.Empty);
        var second = new TestEntity(Guid.Empty);

        Assert.True(first.IsTransient());
        Assert.False(first.Equals(second));
    }

    [Fact]
    public void DomainEvents_CanBeAddedRemovedAndCleared()
    {
        var entity = new TestEntity(Guid.NewGuid());
        var notification = new TestNotification();
        var otherNotification = new TestNotification();

        entity.Raise(notification);
        entity.Raise(otherNotification);
        Assert.Equal(2, entity.DomainEvents!.Count);

        entity.Drop(notification);
        Assert.Same(otherNotification, Assert.Single(entity.DomainEvents!));

        entity.ClearDomainEvents();
        Assert.Empty(entity.DomainEvents!);
    }
}
