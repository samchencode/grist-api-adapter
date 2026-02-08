using GristApiAdapter.Core;

namespace Core.Tests;

public class GristScimApiTests
{
    private readonly GristApi api = TestFixture.CreateApi();

    [Fact]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        var resp = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "scimtest@example.com",
            Name: new("SCIM Test User"),
            Emails: [new("scimtest@example.com", true)],
            DisplayName: "SCIM Test User",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));

        try
        {
            Assert.Equal("scimtest@example.com", resp.UserName);
            Assert.Equal("SCIM Test User", resp.DisplayName);
            Assert.NotEmpty(resp.Id);
        }
        finally
        {
            await api.Scim.DeleteUser(resp.Id);
        }
    }

    [Fact]
    public async Task ListUsers_ReturnsUserList()
    {
        var createResp = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "listtest@example.com",
            Name: new("List Test User"),
            Emails: [new("listtest@example.com", true)],
            DisplayName: "List Test User",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));

        try
        {
            var resp = await api.Scim.ListUsers();

            Assert.NotEmpty(resp.Resources);
            Assert.Contains(resp.Resources, r => r.UserName == "listtest@example.com");
        }
        finally
        {
            await api.Scim.DeleteUser(createResp.Id);
        }
    }

    [Fact]
    public async Task ListUsers_WithCountAndStartIndex_ReturnsPage()
    {
        var user1 = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "page1@example.com",
            Name: new("Page User 1"),
            Emails: [new("page1@example.com", true)],
            DisplayName: "Page User 1",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));
        var user2 = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "page2@example.com",
            Name: new("Page User 2"),
            Emails: [new("page2@example.com", true)],
            DisplayName: "Page User 2",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));

        try
        {
            var all = await api.Scim.ListUsers();
            var page = await api.Scim.ListUsers(count: 1, startIndex: 1);

            Assert.Single(page.Resources);
            Assert.True(all.TotalResults >= 2);
            Assert.Equal(all.TotalResults, page.TotalResults);
        }
        finally
        {
            await api.Scim.DeleteUser(user1.Id);
            await api.Scim.DeleteUser(user2.Id);
        }
    }

    [Fact]
    public async Task GetUser_ReturnsCorrectUser()
    {
        var createResp = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "getuser@example.com",
            Name: new("Get User Test"),
            Emails: [new("getuser@example.com", true)],
            DisplayName: "Get User Test",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));

        try
        {
            var user = await api.Scim.GetUser(createResp.Id);

            Assert.Equal(createResp.Id, user.Id);
            Assert.Equal("getuser@example.com", user.UserName);
            Assert.Equal("Get User Test", user.DisplayName);
        }
        finally
        {
            await api.Scim.DeleteUser(createResp.Id);
        }
    }

    [Fact]
    public async Task DeleteUser_RemovesUser()
    {
        var createResp = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "deletetest@example.com",
            Name: new("Delete Test User"),
            Emails: [new("deletetest@example.com", true)],
            DisplayName: "Delete Test User",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));

        await api.Scim.DeleteUser(createResp.Id);

        var resp = await api.Scim.ListUsers();
        Assert.DoesNotContain(resp.Resources, r => r.UserName == "deletetest@example.com");
    }
}
