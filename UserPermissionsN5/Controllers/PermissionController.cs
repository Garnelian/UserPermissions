using Elasticsearch.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System;
using UserPermissionsN5.Data;
using UserPermissionsN5.Data.UnitOfWork;
using UserPermissionsN5.Models;
using UserPermissionsN5.Services;


namespace UserPermissionsN5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IElasticsearchService<Permission> _elasticsearchService;
        private readonly KafkaProducerService _kafkaProducerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(IElasticsearchService<Permission> elasticsearchService, IUnitOfWork unitOfWork, ILogger<PermissionController> logger, KafkaProducerService kafkaProducerService)
        {
            _elasticsearchService = elasticsearchService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _kafkaProducerService = kafkaProducerService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPermission([FromBody] Permission permission)
        {
            try
            {
                _logger.LogInformation("RequestPermission operation executed.");

                await _unitOfWork.Permissions.AddAsync(permission);

                await _unitOfWork.CompleteAsync();

                await _elasticsearchService.IndexDocumentAsync(permission);

                await _kafkaProducerService.SendMessageAsync(Guid.NewGuid(), "request");

                return Ok(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mesagge:{ex.Message}, StackTrace:{ex.StackTrace}");

                return StatusCode(500, "Server Error");
            }
        }

        [HttpPut("modify/{id}")]
        public async Task<IActionResult> ModifyPermission(int id, [FromBody] Permission updatedPermission)
        {
            try
            {
                _logger.LogInformation("ModifyPermission operation executed.");

                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);

                if (permission == null) return NotFound();

                permission.EmployeeForeName = updatedPermission.EmployeeForeName;
                permission.PermissionTypeId = updatedPermission.PermissionTypeId;
                permission.PermissionDate = updatedPermission.PermissionDate;

                await _unitOfWork.CompleteAsync();

                await _elasticsearchService.IndexDocumentAsync(permission);

                await _kafkaProducerService.SendMessageAsync(Guid.NewGuid(), "modify");

                return Ok(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mesagge:{ex.Message}, StackTrace:{ex.StackTrace}");

                return StatusCode(500, "Server Error");
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                _logger.LogInformation("GetPermission operation executed.");

                var permissions = await _unitOfWork.Permissions.GetPermissionsWithTypesAsync();

                await _kafkaProducerService.SendMessageAsync(Guid.NewGuid(), "get");

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mesagge:{ex.Message}, StackTrace:{ex.StackTrace}");

                return StatusCode(500, "Server Error");
            }
        }
    }
}