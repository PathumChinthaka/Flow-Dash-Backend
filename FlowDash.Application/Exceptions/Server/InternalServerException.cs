namespace FlowDash.Application.Exceptions.Server
{
    public class InternalServerException : Exception
    {
        public InternalServerException(string message)
        : base(message)
        {
        }

        public InternalServerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
