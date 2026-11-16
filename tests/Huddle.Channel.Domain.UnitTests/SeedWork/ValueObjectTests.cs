using Huddle.Channel.Domain;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.UnitTests;

public class ValueObjectTests
{
    private sealed class TestValueObject(int number, string text) : ValueObject
    {
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return number;
            yield return text;
        }
    }

    [Fact]
    public void ValueObjects_WithSameComponents_AreEqual()
    {
        var first = new TestValueObject(1, "text");
        var second = new TestValueObject(1, "text");

        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ValueObjects_WithDifferentComponents_AreNotEqual()
    {
        var first = new TestValueObject(1, "text");
        var second = new TestValueObject(2, "text");

        Assert.False(first.Equals(second));
    }

    [Fact]
    public void ValueObject_IsNotEqualToNullOrOtherType()
    {
        var valueObject = new TestValueObject(1, "text");

        Assert.False(((object)valueObject).Equals(null));
        Assert.False(valueObject.Equals("text"));
    }

    [Fact]
    public void GetCopy_ReturnsEqualInstance()
    {
        var valueObject = new TestValueObject(1, "text");

        var copy = valueObject.GetCopy();

        Assert.NotSame(valueObject, copy);
        Assert.Equal(valueObject, copy);
    }

    [Fact]
    public void MessageAttachments_AreComparedByFileIdAndPosition()
    {
        var fileId = Guid.NewGuid();
        var first = new MessageAttachment(fileId, "photo.png", "image/png", 10, true, 0);
        var same = new MessageAttachment(fileId, "other-name.png", "image/png", 999, true, 0);
        var differentPosition = new MessageAttachment(fileId, "photo.png", "image/png", 10, true, 1);

        Assert.Equal(first, same);
        Assert.NotEqual(first, differentPosition);
    }
}

public class PaginatedItemsTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var items = new[] { "a", "b" };
        var nextCursor = Guid.NewGuid();
        var prevCursor = Guid.NewGuid();

        var page = new PaginatedItems<string>(items, hasPrev: true, hasNext: false, nextCursor, prevCursor);

        Assert.Equal(items, page.Items);
        Assert.True(page.HasPrev);
        Assert.False(page.HasNext);
        Assert.Equal(nextCursor, page.NextCursor);
        Assert.Equal(prevCursor, page.PrevCursor);
    }

    [Fact]
    public void Constructor_WithEmptyItems_KeepsNullCursors()
    {
        var page = new PaginatedItems<string>([], hasPrev: false, hasNext: false, null, null);

        Assert.Empty(page.Items);
        Assert.Null(page.NextCursor);
        Assert.Null(page.PrevCursor);
    }
}
