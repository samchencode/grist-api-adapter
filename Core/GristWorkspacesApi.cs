using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GristApiAdapter.Core;

class GristWorkspacesApi(HttpClient client)
{
    async public Task<IReadOnlyList<ListWorkspacesResponse.Workspace>> ListWorkspaces(string orgId)
    {
        HttpResponseMessage resp = await client.GetAsync($"/api/orgs/{orgId}/workspaces");
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return ListWorkspacesResponse.Parse(body)!;
    }

    async public Task<int> CreateWorkspace(string orgId, CreateWorkspaceRequest.Request request)
    {
        HttpResponseMessage resp = await client.PostAsJsonAsync($"/api/orgs/{orgId}/workspaces", request);
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return int.Parse(body);
    }

    async public Task DeleteWorkspace(int workspaceId)
    {
        HttpResponseMessage resp = await client.DeleteAsync($"/api/workspaces/{workspaceId}");
        if (!resp.IsSuccessStatusCode)
        {
            string body = await resp.Content.ReadAsStringAsync();
            throw new GristApiException(resp.StatusCode, body);
        }
    }

    async public Task<GetWorkspaceAccessResponse.Response> GetWorkspaceAccess(int workspaceId)
    {
        HttpResponseMessage resp = await client.GetAsync($"/api/workspaces/{workspaceId}/access");
        string body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new GristApiException(resp.StatusCode, body);
        }
        return GetWorkspaceAccessResponse.Parse(body)!;
    }

    async public Task UpdateWorkspaceAccess(int workspaceId, UpdateWorkspaceAccessRequest.Request request)
    {
        HttpResponseMessage resp = await client.PatchAsync($"/api/workspaces/{workspaceId}/access",
            JsonContent.Create(request));
        if (!resp.IsSuccessStatusCode)
        {
            string body = await resp.Content.ReadAsStringAsync();
            throw new GristApiException(resp.StatusCode, body);
        }
    }

    public static class GetWorkspaceAccessResponse
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

    public static class UpdateWorkspaceAccessRequest
    {
        public record Request(
            [property: JsonPropertyName("delta")] Delta Delta
        );

        public record Delta(
            [property: JsonPropertyName("users")] Dictionary<string, AccessRole?> Users
        );
    }

    public static class CreateWorkspaceRequest
    {
        public record Request(
            [property: JsonPropertyName("name")] string Name
        );
    }

    public static class ListWorkspacesResponse
    {
        public static IReadOnlyList<Workspace>? Parse(string json)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<Workspace>>(json);
        }

        public record Workspace(
            [property: JsonPropertyName("id")] int Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("access")] AccessRole Access,
            [property: JsonPropertyName("docs")] IReadOnlyList<Doc> Docs,
            [property: JsonPropertyName("org")] Org Org
        );

        public record Doc(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("access")] AccessRole Access,
            [property: JsonPropertyName("createdAt")] string CreatedAt,
            [property: JsonPropertyName("updatedAt")] string UpdatedAt,
            [property: JsonPropertyName("isPinned")] bool IsPinned
        );

        public record Org(
            [property: JsonPropertyName("id")] long Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("domain")] string? Domain
        );
    }
}
