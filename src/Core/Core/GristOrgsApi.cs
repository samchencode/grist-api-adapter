using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GristApiAdapter.Core;

public class GristOrgsApi(HttpClient client)
{
    async public Task<IReadOnlyList<ListOrgsResponse.Org>> ListOrgs()
    {
        HttpResponseMessage resp = await client.GetAsync("/api/orgs");
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return ListOrgsResponse.Parse(body)!;
    }

    async public Task<GetOrgResponse.Org> GetOrg(string orgId)
    {
        HttpResponseMessage resp = await client.GetAsync($"/api/orgs/{orgId}");
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return GetOrgResponse.Parse(body)!;
    }

    async public Task<long> CreateOrg(CreateOrgRequest.Request request)
    {
        // NOTE: although not documented, route exists
        // https://community.getgrist.com/t/api-endpoint-missing-create-org/2854
        HttpResponseMessage resp = await client.PostAsJsonAsync("/api/orgs", request);
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return long.Parse(body);
    }

    async public Task DeleteOrg(string orgId, string name)
    {
        string escapedName = Uri.EscapeDataString(name);
        HttpResponseMessage resp = await client.DeleteAsync($"/api/orgs/{orgId}/{escapedName}");
        if (!resp.IsSuccessStatusCode)
        {
            string body = await resp.Content.ReadAsStringAsync();
            throw new GristApiException(resp.StatusCode, body);
        }
    }

    async public Task<GetOrgAccessResponse.Response> GetOrgAccess(string orgId)
    {
        HttpResponseMessage resp = await client.GetAsync($"/api/orgs/{orgId}/access");
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return GetOrgAccessResponse.Parse(body)!;
    }

    async public Task UpdateOrgAccess(string orgId, UpdateOrgAccessRequest.Request request)
    {
        HttpResponseMessage resp = await client.PatchAsync($"/api/orgs/{orgId}/access",
            JsonContent.Create(request));
        if (!resp.IsSuccessStatusCode)
        {
            string body = await resp.Content.ReadAsStringAsync();
            throw new GristApiException(resp.StatusCode, body);
        }
    }

    public static class CreateOrgRequest
    {
        public record Request(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("domain")] string? Domain
        );
    }

    public static class GetOrgAccessResponse
    {
        public static Response? Parse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json);
        }

        public record Response(
            [property: JsonPropertyName("users")] IReadOnlyList<User> Users
        );

        public record User(
            [property: JsonPropertyName("id")] long Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("email")] string Email,
            [property: JsonPropertyName("access")] AccessRole Access,
            [property: JsonPropertyName("isMember")] bool IsMember
        );
    }

    public static class UpdateOrgAccessRequest
    {
        public record Request(
            [property: JsonPropertyName("delta")] Delta Delta
        );

        public record Delta(
            [property: JsonPropertyName("users")] Dictionary<string, AccessRole?> Users
        );
    }

    public static class ListOrgsResponse
    {
        public static IReadOnlyList<Org>? Parse(string json)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<Org>>(json);
        }

        public record Org(
            [property: JsonPropertyName("id")] long Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("domain")] string? Domain,
            [property: JsonPropertyName("owner")] Owner? Owner,
            [property: JsonPropertyName("access")] AccessRole Access,
            [property: JsonPropertyName("createdAt")] string CreatedAt,
            [property: JsonPropertyName("updatedAt")] string UpdatedAt
        );

        public record Owner(
            [property: JsonPropertyName("id")] long Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("email")] string Email
        );
    }

    public static class GetOrgResponse
    {
        public static Org? Parse(string json)
        {
            return JsonSerializer.Deserialize<Org>(json);
        }

        public record Org(
            [property: JsonPropertyName("id")] long Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("domain")] string? Domain,
            [property: JsonPropertyName("owner")] Owner? Owner,
            [property: JsonPropertyName("access")] AccessRole Access,
            [property: JsonPropertyName("createdAt")] string CreatedAt,
            [property: JsonPropertyName("updatedAt")] string UpdatedAt
        );

        public record Owner(
            [property: JsonPropertyName("id")] long Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("email")] string Email
        );
    }
}
