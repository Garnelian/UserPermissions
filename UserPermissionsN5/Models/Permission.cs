using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserPermissionsN5.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string EmployeeForeName { get; set; }
        [Required]
        public string EmployeeSurName { get; set; }

        [Column("PermissionType")]
        public int PermissionTypeId { get; set; }
        public PermissionType? PermissionType { get; set; }
        [Required]
        public DateTime PermissionDate { get; set; }
    }
}
