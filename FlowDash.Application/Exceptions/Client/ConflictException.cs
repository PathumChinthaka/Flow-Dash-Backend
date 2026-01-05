namespace FlowDash.Application.Exceptions.Client
{
    public class ConflictException : Exception
    {
        public ConflictException(string message)
        : base(message)
        {
        }

        public ConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
