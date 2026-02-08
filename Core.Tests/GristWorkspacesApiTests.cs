using GristApiAdapter.Core;

namespace Core.Tests;

public class GristWorkspacesApiTests
{
    private readonly GristApi api = TestFixture.CreateApi();

    [Fact]
    public async Task CreateWorkspace_ReturnsWorkspaceId()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test WS Create Org",
            Domain: "test-ws-create-org"
        ));

        try
        {
            var ws = await api.Workspaces.CreateWorkspace(orgId.ToString(),
                new GristWorkspacesApi.CreateWorkspaceRequest.Request("Test Workspace"));

            Assert.True(ws.Id > 0);
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test WS Create Org");
        }
    }

    [Fact]
    public async Task ListWorkspaces_ContainsCreatedWorkspace()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test WS List Org",
            Domain: "test-ws-list-org"
        ));

        try
        {
            var ws = await api.Workspaces.CreateWorkspace(orgId.ToString(),
                new GristWorkspacesApi.CreateWorkspaceRequest.Request("Test WS List"));

            var workspaces = await api.Workspaces.ListWorkspaces(orgId.ToString());

            Assert.NotEmpty(workspaces);
            Assert.Contains(workspaces, w => w.Id == ws.Id && w.Name == "Test WS List");
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test WS List Org");
        }
    }

    [Fact]
    public async Task DeleteWorkspace_RemovesWorkspace()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test WS Delete Org",
            Domain: "test-ws-delete-org"
        ));

        try
        {
            var ws = await api.Workspaces.CreateWorkspace(orgId.ToString(),
                new GristWorkspacesApi.CreateWorkspaceRequest.Request("Test WS Delete"));

            await api.Workspaces.DeleteWorkspace(ws.Id);

            var workspaces = await api.Workspaces.ListWorkspaces(orgId.ToString());
            Assert.DoesNotContain(workspaces, w => w.Id == ws.Id);
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test WS Delete Org");
        }
    }

    [Fact]
    public async Task GetWorkspaceAccess_ReturnsUsers()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test WS Access Org",
            Domain: "test-ws-access-org"
        ));

        try
        {
            var ws = await api.Workspaces.CreateWorkspace(orgId.ToString(),
                new GristWorkspacesApi.CreateWorkspaceRequest.Request("Test WS Access"));

            var access = await api.Workspaces.GetWorkspaceAccess(ws.Id);

            Assert.NotEmpty(access.Users);
        }
        finally
        {
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test WS Access Org");
        }
    }

    [Fact]
    public async Task UpdateWorkspaceAccess_ChangesAccess()
    {
        long orgId = await api.Orgs.CreateOrg(new GristOrgsApi.CreateOrgRequest.Request(
            Name: "Test WS UpdAccess Org",
            Domain: "test-ws-updaccess-org"
        ));

        var userResp = await api.Scim.CreateUser(new GristScimApi.CreateUserRequest.Request(
            UserName: "wsaccess@example.com",
            Name: new("WS Access User"),
            Emails: [new("wsaccess@example.com", true)],
            DisplayName: "WS Access User",
            PreferredLanguage: "en",
            Locale: "en_US",
            Photos: []
        ));

        try
        {
            // Add user to org first so they can be added to workspace
            await api.Orgs.UpdateOrgAccess(orgId.ToString(), new(new(
                new Dictionary<string, AccessRole?> { ["wsaccess@example.com"] = AccessRole.Members }
            )));

            var ws = await api.Workspaces.CreateWorkspace(orgId.ToString(),
                new GristWorkspacesApi.CreateWorkspaceRequest.Request("Test WS UpdAccess"));

            await api.Workspaces.UpdateWorkspaceAccess(ws.Id, new(new(
                new Dictionary<string, AccessRole?> { ["wsaccess@example.com"] = AccessRole.Editors }
            )));

            var access = await api.Workspaces.GetWorkspaceAccess(ws.Id);
            Assert.Contains(access.Users, u => u.Email == "wsaccess@example.com" && u.Access == AccessRole.Editors);
        }
        finally
        {
            await api.Scim.DeleteUser(userResp.Id);
            await api.Orgs.DeleteOrg(orgId.ToString(), "Test WS UpdAccess Org");
        }
    }
}
