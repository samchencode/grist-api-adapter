using System.Text.Json;
using System.Text.Json.Serialization;

namespace GristApiAdapter.Core;

class GristOrgsApi(HttpClient client)
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

    async public Task DeleteOrg(string orgId)
    {
        HttpResponseMessage resp = await client.DeleteAsync($"/api/orgs/{orgId}");
        if (!resp.IsSuccessStatusCode)
        {
            string body = await resp.Content.ReadAsStringAsync();
            throw new GristApiException(resp.StatusCode, body);
        }
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
            [property: JsonPropertyName("access")] string Access,
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
            [property: JsonPropertyName("access")] string Access,
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
