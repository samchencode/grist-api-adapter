using GristApiAdapter.Core;

namespace Core.Tests;

public class GristOrgsApiTests
{
    private readonly GristApi api = TestFixture.CreateApi();

    [Fact]
    public async Task CreateOrg_ReturnsOrgId()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test Org Create",
            Domain: "test-org-create"
        ));

        try
        {
            Assert.True(orgId > 0);
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test Org Create");
        }
    }

    [Fact]
    public async Task ListOrgs_ContainsCreatedOrg()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test Org List",
            Domain: "test-org-list"
        ));

        try
        {
            var orgs = await api.Orgs.ListOrgs();

            Assert.NotEmpty(orgs);
            Assert.Contains(orgs, o => o.Id == orgId);
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test Org List");
        }
    }

    [Fact]
    public async Task GetOrg_ReturnsCorrectOrg()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test Org Get",
            Domain: "test-org-get"
        ));

        try
        {
            var org = await api.Orgs.GetOrg(orgId.ToString());

            Assert.Equal(orgId, org.Id);
            Assert.Equal("Test Org Get", org.Name);
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test Org Get");
        }
    }

    [Fact]
    public async Task DeleteOrg_RemovesOrg()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test Org Delete",
            Domain: "test-org-delete"
        ));

        await api.Orgs.DeleteOrg(orgId.ToString(), "Test Org Delete");

        var orgs = await api.Orgs.ListOrgs();
        Assert.DoesNotContain(orgs, o => o.Id == orgId);
    }

    [Fact]
    public async Task GetOrgAccess_ReturnsUsers()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test Org Access",
            Domain: "test-org-access"
        ));

        try
        {
            var access = await api.Orgs.GetOrgAccess(orgId.ToString());

            Assert.NotEmpty(access.Users);
            Assert.Contains(access.Users, u => u.Access == AccessRole.Owners);
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test Org Access");
        }
    }

    [Fact]
    public async Task UpdateOrgAccess_ChangesAccess()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test Org Update Access",
            Domain: "test-org-update-access"
        ));

        var userResp = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "orgaccess@example.com",
            Name: new("Org Access User"),
            Emails: [new("orgaccess@example.com", true)],
            DisplayName: "Org Access User",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));

        try
        {
            await api.Orgs.UpdateOrgAccess(orgId.ToString(), new(new(
                new Dictionary<string, AccessRole?> { ["orgaccess@example.com"] = AccessRole.Editors }
            )));

            var access = await api.Orgs.GetOrgAccess(orgId.ToString());
            Assert.Contains(access.Users, u => u.Email == "orgaccess@example.com" && u.Access == AccessRole.Editors);
        }
        finally
        {
            await api.Scim.DeleteUser(userResp.Id);
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test Org Update Access");
        }
    }
}
