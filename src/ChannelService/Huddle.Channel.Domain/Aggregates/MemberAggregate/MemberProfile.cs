namespace Huddle.Channel.Domain.Aggregates.MemberAggregate
{
    public class MemberProfile
    {
        const int MAX_USERNAME_LENGTH = 20;
        const int MAX_DESCRIPTION_LENGTH = 20;

        public string SeverUsername { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        // public string ServerImage {get; private set;}

        public void ChangeServerUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException(nameof(username));
            if (username.Length > MAX_USERNAME_LENGTH)
                throw new ArgumentException(nameof(username));

            SeverUsername = username;
        }

        public void ChangeDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException(nameof(description));
            if (description.Length > MAX_DESCRIPTION_LENGTH)
                throw new ArgumentException(nameof(description));

            Description = description;
        }
    }
}