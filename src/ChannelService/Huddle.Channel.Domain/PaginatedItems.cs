namespace Huddle.Channel.Domain
{
    public class PaginatedItems<T>
    {
        public IEnumerable<T> Items { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
        public Guid? NextCursor { get; set; }
        public Guid? PrevCursor { get; set; }

        public PaginatedItems(IEnumerable<T> items, bool hasPrev, bool hasNext, Guid? nextCursor, Guid? prevCursor)
        {
            Items = items;
            HasPrev = hasPrev;
            HasNext = hasNext;
            NextCursor = nextCursor;
            PrevCursor = prevCursor;
        }
    }
}
