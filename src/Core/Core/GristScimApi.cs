using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GristApiAdapter.Core;

public class GristScimApi(HttpClient client)
{
    async public Task<GetUsersResponse.Response> GetUsers()
    {
        string uri = "/api/scim/v2/Users";
        HttpResponseMessage resp = await client.GetAsync(uri);
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return GetUsersResponse.Parse(body)!;
    }

    async public Task<CreateUserResponse.Response> CreateUser(CreateUserRequest.Request createUserRequest)
    {
        HttpRequestMessage req = new(HttpMethod.Post, "/api/scim/v2/Users");
        MediaTypeWithQualityHeaderValue contentType = new("application/scim+json");
        req.Content = JsonContent.Create(createUserRequest, mediaType: contentType);
        HttpResponseMessage resp = await client.SendAsync(req);
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return CreateUserResponse.Parse(body)!;
    }

    async public Task<GetUserResponse.Response> GetUser(string userId)
    {
        HttpResponseMessage resp = await client.GetAsync($"/api/scim/v2/Users/{userId}");
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return GetUserResponse.Parse(body)!;
    }

    async public Task DeleteUser(string userId)
    {
        HttpResponseMessage resp = await client.DeleteAsync($"/api/scim/v2/Users/{userId}");
        if (!resp.IsSuccessStatusCode)
        {
            string body = await resp.Content.ReadAsStringAsync();
            throw new GristApiException(resp.StatusCode, body);
        }
    }

    public static class CreateUserRequest
    {
        public record Request(
            [property: JsonPropertyName("userName")] string UserName,
            [property: JsonPropertyName("name")] Name Name,
            [property: JsonPropertyName("emails")] IReadOnlyList<Email> Emails,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("preferredLanguage")] string PreferredLanguage,
            [property: JsonPropertyName("locale")] string Locale,
            [property: JsonPropertyName("photos")] IReadOnlyList<Photo> Photos
        )
        {
            [JsonInclude]
            [JsonPropertyName("schemas")]
            public IReadOnlyList<string> Schemas
            {
                get;
            } =
            [
                "urn:ietf:params:scim:schemas:core:2.0:User"
            ];
        }

        public record Email(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary
        );

        public record Name(
            [property: JsonPropertyName("formatted")] string Formatted
        );

        public record Photo(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary,
            [property: JsonPropertyName("type")] string Type
        );
    }

    public static class CreateUserResponse
    {
        public static Response? Parse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json);
        }

        public record Email(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary
        );

        public record Meta(
            [property: JsonPropertyName("resourceType")] string ResourceType,
            [property: JsonPropertyName("location")] string Location
        );

        public record Name(
            [property: JsonPropertyName("formatted")] string Formatted
        );

        public record Photo(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary,
            [property: JsonPropertyName("type")] string Type
        );

        public record Response(
            [property: JsonPropertyName("meta")] Meta Meta,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("schemas")] IReadOnlyList<string> Schemas,
            [property: JsonPropertyName("userName")] string UserName,
            [property: JsonPropertyName("name")] Name Name,
            [property: JsonPropertyName("emails")] IReadOnlyList<Email> Emails,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("preferredLanguage")] string PreferredLanguage,
            [property: JsonPropertyName("locale")] string Locale,
            [property: JsonPropertyName("photos")] IReadOnlyList<Photo> Photos
        );


    }

    public static class GetUserResponse
    {
        public static Response? Parse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json);
        }

        public record Email(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary
        );

        public record Meta(
            [property: JsonPropertyName("resourceType")] string ResourceType,
            [property: JsonPropertyName("location")] string Location
        );

        public record Name(
            [property: JsonPropertyName("formatted")] string Formatted
        );

        public record Photo(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary,
            [property: JsonPropertyName("type")] string Type
        );

        public record Response(
            [property: JsonPropertyName("meta")] Meta Meta,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("schemas")] IReadOnlyList<string> Schemas,
            [property: JsonPropertyName("userName")] string UserName,
            [property: JsonPropertyName("name")] Name Name,
            [property: JsonPropertyName("emails")] IReadOnlyList<Email> Emails,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("preferredLanguage")] string PreferredLanguage,
            [property: JsonPropertyName("locale")] string Locale,
            [property: JsonPropertyName("photos")] IReadOnlyList<Photo> Photos
        );
    }

    public static class GetUsersResponse
    {

        public static Response? Parse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json);
        }

        public record Email(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary
        );

        public record Meta(
            [property: JsonPropertyName("resourceType")] string ResourceType,
            [property: JsonPropertyName("location")] string Location
        );

        public record Name(
            [property: JsonPropertyName("formatted")] string Formatted
        );

        public record Photo(
            [property: JsonPropertyName("value")] string Value,
            [property: JsonPropertyName("primary")] bool Primary,
            [property: JsonPropertyName("type")] string Type
        );

        public record Resource(
            [property: JsonPropertyName("meta")] Meta Meta,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("schemas")] IReadOnlyList<string> Schemas,
            [property: JsonPropertyName("userName")] string UserName,
            [property: JsonPropertyName("name")] Name Name,
            [property: JsonPropertyName("emails")] IReadOnlyList<Email> Emails,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("preferredLanguage")] string PreferredLanguage,
            [property: JsonPropertyName("locale")] string Locale,
            [property: JsonPropertyName("photos")] IReadOnlyList<Photo> Photos
        );

        public record Response(
            [property: JsonPropertyName("schemas")] IReadOnlyList<string> Schemas,
            [property: JsonPropertyName("totalResults")] int TotalResults,
            [property: JsonPropertyName("itemsPerPage")] int ItemsPerPage,
            [property: JsonPropertyName("startIndex")] int StartIndex,
            [property: JsonPropertyName("Resources")] IReadOnlyList<Resource> Resources
        );
    }
}