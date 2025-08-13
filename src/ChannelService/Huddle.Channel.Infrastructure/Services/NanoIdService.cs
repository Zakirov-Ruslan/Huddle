using Huddle.Channel.Application.Services;
using NanoidDotNet;

namespace Huddle.Channel.Infrastructure.Services
{
    public class NanoIdService : IShortIdService
    {
        private const int DefaultLength = 10;

        public string GetShortId() =>
           Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, DefaultLength);
    }
}
