using System.Net;

namespace user_service.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
}
