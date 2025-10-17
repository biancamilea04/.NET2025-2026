namespace lab03.Middleware;

public class NotFoundException : Exception
{
        public int StatusCode { get; } = 404;
        public NotFoundException(string message) : base(message) { }
}