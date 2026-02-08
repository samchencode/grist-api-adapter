using System.Net.Http.Headers;

namespace GristApiAdapter.Core;

public class GristApi
{
    public readonly GristScimApi Scim;
    public readonly GristOrgsApi Orgs;
    public readonly GristWorkspacesApi Workspaces;
    private readonly HttpClient client;
    public readonly string HostUrl;

    public GristApi(string hostUrl, string accessToken)
    {
        HostUrl = hostUrl;
        client = PrepareClient(hostUrl, accessToken);
        Scim = new GristScimApi(client);
        Orgs = new GristOrgsApi(client);
        Workspaces = new GristWorkspacesApi(client);
    }

    private static HttpClient PrepareClient(string hostUrl, string accessToken)
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(hostUrl)
        };
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    public override string ToString()
    {
        return $"GristApi({HostUrl})";
    }
}
