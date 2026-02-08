using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GristApiAdapter.Core;

public class GristApiException(HttpStatusCode statusCode, string responseBody) : Exception(ParseResponse(responseBody))
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    private static string ParseResponse(string body)
    {
        if (IsValidJson(body))
        {
            ApiErrorResponse error = JsonSerializer.Deserialize<ApiErrorResponse>(body)!;
            return error.Error;
        }
        return body;
    }

    public record ApiErrorResponse([property: JsonPropertyName("error")] string Error);

    static bool IsValidJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        try
        {
            using var _ = JsonDocument.Parse(input);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
