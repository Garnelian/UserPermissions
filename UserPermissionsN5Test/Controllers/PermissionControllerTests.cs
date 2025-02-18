using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserPermissionsN5.Controllers;
using UserPermissionsN5.Data.UnitOfWork;
using UserPermissionsN5.Models;
using UserPermissionsN5.Services;

public class PermissionControllerTests
{
    private readonly Mock<IQueryServerEngine<Permission>> _mockElasticsearchService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<PermissionController>> _mockLogger;
    private readonly Mock<IMessageBroker> _mockKafkaProducerService;
    private readonly PermissionController _controller;

    public PermissionControllerTests()
    {
        _mockElasticsearchService = new Mock<IQueryServerEngine<Permission>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<PermissionController>>();
        _mockKafkaProducerService = new Mock<IMessageBroker>();

        _controller = new PermissionController(
            _mockElasticsearchService.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mockKafkaProducerService.Object
        );
    }

    [Fact]
    public async Task RequestPermission_ShouldReturnOk_WhenValidPermission()
    {
        var permission = new Permission { Id = 1, EmployeeForeName = "John", EmployeeSurName = "Doe", PermissionTypeId = 1, PermissionDate = DateTime.Now };

        _mockUnitOfWork.Setup(u => u.PermissionsRepository.AddAsync(It.IsAny<Permission>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.PermissionsRepository.GetPermissionsWithTypesAsync(It.IsAny<int[]>()))
                       .ReturnsAsync(new List<Permission> { permission });

        var result = await _controller.RequestPermission(permission);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPermission = Assert.IsAssignableFrom<IEnumerable<Permission>>(okResult.Value);
        Assert.Single(returnedPermission);
    }

    [Fact]
    public async Task RequestPermission_ShouldReturnBadRequest_WhenModelInvalid()
    {
        _controller.ModelState.AddModelError("EmployeeForeName", "Required");

        var result = await _controller.RequestPermission(new Permission());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // 🟢 Prueba: ModifyPermission (Éxito)
    [Fact]
    public async Task ModifyPermission_ShouldReturnOk_WhenPermissionExists()
    {
        var permission = new Permission { Id = 1, EmployeeForeName = "John", EmployeeSurName = "Doe", PermissionTypeId = 1, PermissionDate = DateTime.Now };
        var updatedPermission = new Permission { EmployeeForeName = "Jane", EmployeeSurName = "Smith", PermissionTypeId = 2, PermissionDate = DateTime.Now };

        _mockUnitOfWork.Setup(u => u.PermissionsRepository.GetByIdAsync(1)).ReturnsAsync(permission);
        _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.PermissionsRepository.GetPermissionsWithTypesAsync(It.IsAny<int[]>()))
                       .ReturnsAsync(new List<Permission> { permission });

        var result = await _controller.ModifyPermission(1, updatedPermission);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPermission = Assert.IsAssignableFrom<IEnumerable<Permission>>(okResult.Value);
        Assert.Single(returnedPermission);
    }

    [Fact]
    public async Task ModifyPermission_ShouldReturnNotFound_WhenPermissionDoesNotExist()
    {
        _mockUnitOfWork.Setup(static u => u.PermissionsRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Permission)null);

        var result = await _controller.ModifyPermission(1, new Permission());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnOk_WhenDataExists()
    {
        var permissions = new List<Permission>
        {
            new Permission { Id = 1, EmployeeForeName = "John", EmployeeSurName = "Doe", PermissionTypeId = 1, PermissionDate = DateTime.Now },
            new Permission { Id = 2, EmployeeForeName = "Jane", EmployeeSurName = "Smith", PermissionTypeId = 2, PermissionDate = DateTime.Now }
        };

        _mockUnitOfWork
    .Setup(u => u.PermissionsRepository.GetPermissionsWithTypesAsync(It.IsAny<int[]>()))
    .ReturnsAsync(permissions);

        var result = await _controller.GetPermissions();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPermissions = Assert.IsAssignableFrom<IEnumerable<Permission>>(okResult.Value);
        Assert.Equal(2, returnedPermissions.Count());
    }
    [Fact]
    public async Task RequestPermission_ShouldReturnServerError_WhenExceptionOccurs()
    {
        _mockUnitOfWork
            .Setup(u => u.PermissionsRepository.AddAsync(It.IsAny<Permission>()))
            .ThrowsAsync(new Exception("Database error"));

        var permission = new Permission
        {
            EmployeeForeName = "John",
            EmployeeSurName = "Doe",
            PermissionTypeId = 1,
            PermissionDate = DateTime.UtcNow
        };

        var result = await _controller.RequestPermission(permission);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}
