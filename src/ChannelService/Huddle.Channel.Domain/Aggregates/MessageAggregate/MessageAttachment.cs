using Huddle.Channel.Domain.SeedWork;

namespace Huddle.Channel.Domain.Aggregates.MessageAggregate
{
    public class MessageAttachment : ValueObject
    {
        public Guid FileId { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public long Size { get; private set; }
        public bool IsImage { get; private set; }
        public int Position { get; private set; }

        private MessageAttachment() { }

        public MessageAttachment(Guid fileId, string fileName, string contentType, long size, bool isImage, int position)
        {
            FileId = fileId;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            IsImage = isImage;
            Position = position;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FileId;
            yield return Position;
        }
    }
}
