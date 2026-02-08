using System.Net.Http.Headers;

namespace GristApiAdapter.Core;

class GristApi
{
    public readonly GristScimApi Scim;
    private readonly HttpClient client;
    public readonly string HostUrl;

    public GristApi(string hostUrl, string accessToken)
    {
        HostUrl = hostUrl;
        client = PrepareClient(hostUrl, accessToken);
        Scim = new GristScimApi(client);
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
