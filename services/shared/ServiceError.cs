

namespace AbjjadAssignment.services.shared;
public class ServiceError
{
    public string Code { get; }
    public string Message { get; }

    public ServiceError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    // Generic error creators
    public static ServiceError FileTooLarge(string fileName, long maxSize) =>
        new("FILE_TOO_LARGE", $"File {fileName} exceeds maximum size of {maxSize} bytes");

    public static ServiceError InvalidFormat(string fileName) =>
        new("INVALID_FORMAT", $"File {fileName} has an invalid format");

    public static ServiceError ProcessingFailed(string context) =>
        new("PROCESSING_FAILED", $"Failed to process {context}");

    public static ServiceError InternalError(string context) =>
        new("INTERNAL_ERROR", $"Unexpected error occurred while {context}");
}