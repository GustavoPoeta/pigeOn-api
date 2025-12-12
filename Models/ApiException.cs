namespace pigeon_api.Models
{
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; }

        protected ApiException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound) { }
    }

    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message)
            : base(message, StatusCodes.Status401Unauthorized) { }
    }

    public class BadRequestException : ApiException
    {
        public BadRequestException(string message)
            : base(message, StatusCodes.Status400BadRequest) { }
    }

}