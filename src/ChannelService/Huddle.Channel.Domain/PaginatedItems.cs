namespace Huddle.Channel.Domain
{
    public class PaginatedItems<T>
    {
        public IEnumerable<T> Items { get; set; }
        public bool HasMore { get; set; }
        public Guid? NextCursor { get; set; }

        public PaginatedItems(IEnumerable<T> items, bool hasMore, Guid? nextCursor)
        {
            Items = items;
            HasMore = hasMore;
            NextCursor = nextCursor;
        }
    }
}
