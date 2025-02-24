namespace Candle_API.Tools
{
    // Exceptions/NotFoundException.cs
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    // Exceptions/BadRequestException.cs
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
