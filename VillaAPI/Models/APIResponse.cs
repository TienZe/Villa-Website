using System.Net;

namespace VillaAPI.Models;

public class APIResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public Dictionary<string, List<string>> ErrorMessages { get; set; } = new();
    public object? Result { get; set; }

    public static APIResponse CreateResponse(HttpStatusCode statusCode, object? result = null, IEnumerable<string>? errorMessages = null, Dictionary<string, List<string>>? validationErrors = null)
    {
        bool isSuccessResponse = (int)statusCode >= 200 && (int)statusCode < 300;


        var apiResponse = new APIResponse {
            StatusCode = statusCode,
            IsSuccess = isSuccessResponse,
            Result = result,
        };

        if (validationErrors is not null) {
            apiResponse.ErrorMessages = validationErrors;
        }

        if (errorMessages is not null) {
            if (apiResponse.ErrorMessages.TryGetValue("", out List<string>? modelErrors)) {
                modelErrors.AddRange(errorMessages);
            } else {
                apiResponse.ErrorMessages[""] = new List<string>(errorMessages);
            }
        }
        
        return apiResponse;
    }

    public static APIResponse Ok(object? result = null)
    {
        return CreateResponse(HttpStatusCode.OK, result);
    }

    public static APIResponse BadRequest(IEnumerable<string>? errorMessages = null, Dictionary<string, List<string>>? validationErrors = null)
    {
        return CreateResponse(HttpStatusCode.BadRequest, errorMessages: errorMessages, validationErrors: validationErrors);
    }

    public static APIResponse NotFound(IEnumerable<string>? errorMessages = null, Dictionary<string, List<string>>? validationErrors = null)
    {
        return CreateResponse(HttpStatusCode.NotFound, errorMessages: errorMessages, validationErrors: validationErrors);
    }

    public static APIResponse Created(object? result = null)
    {
        return CreateResponse(HttpStatusCode.Created, result);
    }

    public static APIResponse NoContent()
    {
        return CreateResponse(HttpStatusCode.NoContent);
    }
}