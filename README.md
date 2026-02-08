# GristApiAdapter

A .NET client library for the [Grist](https://www.getgrist.com/) API.

API reference: [REST API](https://support.getgrist.com/api/) | [OpenAPI spec](https://raw.githubusercontent.com/gristlabs/grist-help/master/api/grist.yml)

## Getting Started

### Prerequisites

- .NET 8.0 or later

### Add to your project

```bash
dotnet add reference path/to/GristApiAdapter.csproj
```

### Initialize the client

```csharp
using GristApiAdapter.Core;

var grist = new GristApi("https://your-grist-instance.com", "your-api-key");
```

The client exposes three API groups:
- `grist.Orgs` - Organization management
- `grist.Workspaces` - Workspace management
- `grist.Scim` - User management (SCIM)

## API Reference

### Organizations

#### List organizations

```csharp
var orgs = await grist.Orgs.ListOrgs();

foreach (var org in orgs)
{
    Console.WriteLine($"{org.Id}: {org.Name} ({org.Access})");
}
```

#### Get an organization

```csharp
var org = await grist.Orgs.GetOrg("42");
Console.WriteLine($"{org.Name}, created {org.CreatedAt}");
```

#### Create an organization

```csharp
long orgId = await grist.Orgs.CreateOrg(
    new GristOrgsApi.CreateOrgRequest.Request("My Org", Domain: null)
);
```

#### Delete an organization

```csharp
await grist.Orgs.DeleteOrg("42", "My Org");
```

#### Get organization access

```csharp
var access = await grist.Orgs.GetOrgAccess("42");

foreach (var user in access.Users)
{
    Console.WriteLine($"{user.Email}: {user.Access} (member={user.IsMember})");
}
```

#### Update organization access

```csharp
await grist.Orgs.UpdateOrgAccess("42",
    new GristOrgsApi.UpdateOrgAccessRequest.Request(
        new GristOrgsApi.UpdateOrgAccessRequest.Delta(
            new Dictionary<string, AccessRole?>
            {
                ["user@example.com"] = AccessRole.Editors,
                ["removed@example.com"] = null  // remove access
            }
        )
    )
);
```

### Workspaces

#### List workspaces

```csharp
var workspaces = await grist.Workspaces.ListWorkspaces("42");

foreach (var ws in workspaces)
{
    Console.WriteLine($"{ws.Id}: {ws.Name} ({ws.Docs.Count} docs)");
}
```

#### Create a workspace

```csharp
int wsId = await grist.Workspaces.CreateWorkspace("42",
    new GristWorkspacesApi.CreateWorkspaceRequest.Request("My Workspace")
);
```

#### Delete a workspace

```csharp
await grist.Workspaces.DeleteWorkspace(123);
```

#### Get workspace access

```csharp
var access = await grist.Workspaces.GetWorkspaceAccess(123);

foreach (var user in access.Users)
{
    Console.WriteLine($"{user.Email}: {user.Access}");
}
```

#### Update workspace access

```csharp
await grist.Workspaces.UpdateWorkspaceAccess(123,
    new GristWorkspacesApi.UpdateWorkspaceAccessRequest.Request(
        new GristWorkspacesApi.UpdateWorkspaceAccessRequest.Delta(
            new Dictionary<string, AccessRole?>
            {
                ["user@example.com"] = AccessRole.Viewers
            }
        )
    )
);
```

### Users (SCIM)

#### List users

```csharp
var response = await grist.Scim.GetUsers();

foreach (var user in response.Resources)
{
    Console.WriteLine($"{user.Id}: {user.DisplayName} ({user.UserName})");
}
```

#### Create a user

```csharp
var newUser = await grist.Scim.CreateUser(
    new GristScimApi.CreateUserRequest.Request(
        UserName: "jdoe",
        Name: new GristScimApi.CreateUserRequest.Name("Jane Doe"),
        Emails: [new GristScimApi.CreateUserRequest.Email("jane@example.com", Primary: true)],
        DisplayName: "Jane Doe",
        PreferredLanguage: "en",
        Locale: "en-US",
        Photos: [new GristScimApi.CreateUserRequest.Photo("https://example.com/photo.jpg", Primary: true, Type: "photo")]
    )
);

Console.WriteLine($"Created user {newUser.Id}");
```

#### Get a user

```csharp
var user = await grist.Scim.GetUser("user-id");
Console.WriteLine($"{user.DisplayName} ({user.UserName})");
```

#### Delete a user

```csharp
await grist.Scim.DeleteUser("user-id");
```

### Access Roles

The `AccessRole` type represents permission levels:

```csharp
AccessRole.Owners
AccessRole.Editors
AccessRole.Viewers
AccessRole.Members
AccessRole.Guests
```

To revoke a user's access, pass `null` instead of an `AccessRole`:

```csharp
await grist.Orgs.UpdateOrgAccess("42",
    new GristOrgsApi.UpdateOrgAccessRequest.Request(
        new GristOrgsApi.UpdateOrgAccessRequest.Delta(
            new Dictionary<string, AccessRole?>
            {
                ["user-to-remove@example.com"] = null
            }
        )
    )
);
```

The same applies to workspace access via `UpdateWorkspaceAccess`.

### Error Handling

All API methods throw `GristApiException` on failure:

```csharp
try
{
    await grist.Orgs.GetOrg("999");
}
catch (GristApiException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
```

## Development

```bash
# Build
dotnet build

# Test
dotnet test
```
