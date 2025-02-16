using Microsoft.EntityFrameworkCore;
using UserPermissionsN5.Models;

namespace UserPermissionsN5.Data.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Permission>> GetPermissionsWithTypesAsync()
        {
            return await _context.Permission.Include(p=>p.PermissionType).ToListAsync();
        }
    }
}
