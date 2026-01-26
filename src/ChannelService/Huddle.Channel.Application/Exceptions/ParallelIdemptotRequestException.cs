namespace Huddle.Channel.Application.Exceptions;

public class ParallelIdempotentRequestException : Exception
{
    public ParallelIdempotentRequestException()
    {
    }

    public ParallelIdempotentRequestException(string? message) : base(message)
    {
    }

    public ParallelIdempotentRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}