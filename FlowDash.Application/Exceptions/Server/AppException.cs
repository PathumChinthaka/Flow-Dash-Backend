namespace FlowDash.Application.Exceptions.Server
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
    }
}
