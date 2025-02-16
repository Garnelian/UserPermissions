using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserPermissionsN5.Models
{
    public class PermissionType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<Permission> Permissions { get; set; }
    }
}
