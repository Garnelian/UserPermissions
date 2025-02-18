using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UserPermissionsN5;
using UserPermissionsN5.Models;
using Xunit;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Text.Json;

public class PermissionControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PermissionControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        var scope = _factory.Services.CreateScope();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RequestPermission_ShouldReturnOk()
    {
        var permission = new Permission
        {
            EmployeeForeName = "Carlos",
            EmployeeSurName = "Velez",
            PermissionTypeId = 1,
            PermissionDate = DateTime.UtcNow
        };

        var response = await _client.PostAsJsonAsync("/api/Permission/request", permission);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/Permission/get");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        var permissions = JsonSerializer.Deserialize<IEnumerable<Permission>>(await response.Content.ReadAsStringAsync());
        permissions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ModifyPermission_ShouldReturnOk_WhenPermissionExist()
    {
        var updatedPermission = new Permission
        {
            EmployeeForeName = "Jane",
            EmployeeSurName = "Smith",
            PermissionTypeId = 1,
            PermissionDate = DateTime.UtcNow
        };

        var response = await _client.PutAsJsonAsync("/api/Permission/modify/1", updatedPermission);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ModifyPermission_ShouldReturnNotFound_WhenPermissionDoesNotExist()
    {
        var updatedPermission = new Permission
        {
            EmployeeForeName = "Jane",
            EmployeeSurName = "Smith",
            PermissionTypeId = 2,
            PermissionDate = DateTime.UtcNow
        };

        var response = await _client.PutAsJsonAsync("/api/Permission/modify/99", updatedPermission);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
