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
        private readonly IQueryServerEngine<Permission> _elasticsearchService;
        private readonly IMessageBroker _kafkaProducerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(IQueryServerEngine<Permission> elasticsearchService
                                    , IUnitOfWork unitOfWork
                                    , ILogger<PermissionController> logger
                                    , IMessageBroker kafkaProducerService)
        {
            _elasticsearchService = elasticsearchService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _kafkaProducerService = kafkaProducerService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPermission([FromBody] Permission permission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("RequestPermission operation executed.");

                await _elasticsearchService.IndexDocumentAsync(permission);

                await _unitOfWork.PermissionsRepository.AddAsync(permission);

                await _unitOfWork.CompleteAsync();

                await _kafkaProducerService.SendMessageAsync(Guid.NewGuid(), "request");

                var storedPermission = await _unitOfWork.PermissionsRepository
                                      .GetPermissionsWithTypesAsync(new int[] { permission.Id });

                return Ok(storedPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mesagge:{ex.Message}, StackTrace:{ex.StackTrace}");

                return StatusCode(500, new { Error = "Server Error" });
            }
        }

        [HttpPut("modify/{id}")]
        public async Task<IActionResult> ModifyPermission(int id, [FromBody] Permission updatedPermission)
        {
            try
            {
                _logger.LogInformation("ModifyPermission operation executed.");

                await _elasticsearchService.IndexDocumentAsync(updatedPermission);

                var permission = await _unitOfWork.PermissionsRepository.GetByIdAsync(id);

                if (permission == null) return NotFound();

                permission.EmployeeForeName = updatedPermission.EmployeeForeName;
                permission.EmployeeSurName = updatedPermission.EmployeeSurName;
                permission.PermissionTypeId = updatedPermission.PermissionTypeId;
                permission.PermissionDate = updatedPermission.PermissionDate;

                await _unitOfWork.CompleteAsync();

                await _kafkaProducerService.SendMessageAsync(Guid.NewGuid(), "modify");

                var storedPermission = await _unitOfWork.PermissionsRepository
                                      .GetPermissionsWithTypesAsync(new int[] { id });

                return Ok(storedPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mesagge:{ex.Message}, StackTrace:{ex.StackTrace}");

                return StatusCode(500, new { Error = "Server Error" });
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                _logger.LogInformation("GetPermission operation executed.");

                var permissions = await _unitOfWork.PermissionsRepository.GetPermissionsWithTypesAsync();

                await _kafkaProducerService.SendMessageAsync(Guid.NewGuid(), "get");

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mesagge:{ex.Message}, StackTrace:{ex.StackTrace}");

                return StatusCode(500, new { Error = "Server Error" });
            }
        }
    }
}