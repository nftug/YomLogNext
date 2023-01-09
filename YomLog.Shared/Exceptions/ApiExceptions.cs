namespace YomLog.Shared.Exceptions;

public class ApiException : ApiExceptionBase<ApiException>
{
}

public class ForbiddenException : ApiExceptionBase<ForbiddenException>
{
}

public class NotFoundException : ApiExceptionBase<NotFoundException>
{
}

public class UnauthorizedException : ApiExceptionBase<UnauthorizedException>
{
}
