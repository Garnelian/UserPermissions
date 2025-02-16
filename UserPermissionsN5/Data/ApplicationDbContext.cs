using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserPermissionsN5.Models;

namespace UserPermissionsN5.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Permission> Permission { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }

    }
}
