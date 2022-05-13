using System.Net;

namespace user_service.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
}

