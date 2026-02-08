using GristApiAdapter.Core;

namespace GristApiAdapter;

class Program
{
    async public static Task Main(string[] args)
    {
        string token = "0dcc78e9b7fb2840d52447aec90872ac4b655a4a";
        GristApi api = new("http://localhost:8484", token);

        var createResp = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "testuser@example.com",
            Name: new("Test User"),
            Emails: [new("testuser@example.com", true)],
            DisplayName: "Test User",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));
        Console.WriteLine($"Created user: {createResp.Id} ({createResp.DisplayName})");

        var resp = await api.Scim.GetUsers();
        if (resp is null)
        {
            throw new Exception("null response");
        }
        foreach (var r in resp.Resources)
        {
            Console.WriteLine(r.DisplayName);
        }

        await api.Scim.DeleteUser(createResp.Id);
        Console.WriteLine($"Deleted user: {createResp.Id}");
    }
}