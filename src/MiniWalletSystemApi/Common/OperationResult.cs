namespace MiniWalletSystemApi.Common;

public class OperationResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int StatusCode { get; private set; }
        public string? SuccessMessage { get; private set; }

        public static OperationResult<T> Success(T data, int statusCode = 200)
            => new() { IsSuccess = true, Data = data, StatusCode = statusCode };

        //public static OperationResult<T> Failure(string errorMessage, int statusCode = 400)
        //    => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };

        public static OperationResult<T> SuccessMessageOnly(string message, int statusCode = 200)
          => new() { IsSuccess = true, SuccessMessage = message, StatusCode = statusCode };

        public static OperationResult<T> BadRequest(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 400 };

        public static OperationResult<T> Unauthorized(string errorMessage = "Unauthorized access.")
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 401 };

        public static OperationResult<T> Forbidden(string errorMessage = "Access denied.")
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 403 };

        public static OperationResult<T> NotFound(string errorMessage = "Resource not found.")
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 404 };

        public static OperationResult<T> Conflict(string errorMessage)
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 409 };

        public static OperationResult<T> UnprocessableEntity(string errorMessage)
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 422 };

        public static OperationResult<T> TooManyRequests(string errorMessage = "Too many requests.")
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 429 };


        public static OperationResult<T> InternalServerError(string errorMessage = "An unexpected error occurred.")
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 500 };

        public static OperationResult<T> ServiceUnavailable(string errorMessage = "Service is temporarily unavailable.")
            => new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = 503 };

    }