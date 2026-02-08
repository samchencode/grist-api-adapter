using GristApiAdapter.Core;

namespace Core.Tests;

internal class TestFixture
{
    private const string HostUrl = "http://localhost:8484";
    private const string AccessToken = "0dcc78e9b7fb2840d52447aec90872ac4b655a4a";

    public static GristApi CreateApi()
    {
        return new GristApi(HostUrl, AccessToken);
    }
}
