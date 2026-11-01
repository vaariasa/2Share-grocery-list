namespace GroceryList.Application.Exceptions;

// Business errors thrown by the services.
// The API layer translates each one into an HTTP status code (see ExceptionHandlingMiddleware).

/// <summary>The request data is not valid. Mapped to 400 Bad Request.</summary>
public class ValidationException(string message) : Exception(message);

/// <summary>Wrong username or password. Mapped to 401 Unauthorized.</summary>
public class UnauthorizedException(string message) : Exception(message);

/// <summary>The resource does not exist (or belongs to another user). Mapped to 404 Not Found.</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>The resource already exists, e.g. a username that is taken. Mapped to 409 Conflict.</summary>
public class ConflictException(string message) : Exception(message);
