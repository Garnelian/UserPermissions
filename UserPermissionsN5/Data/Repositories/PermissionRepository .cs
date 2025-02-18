using Microsoft.EntityFrameworkCore;
using UserPermissionsN5.Models;

namespace UserPermissionsN5.Data.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Permission>> GetPermissionsWithTypesAsync(int[]? ids = null)
        {
            if (ids != null && ids.Length > 0) return await _context.Permission.Include(p => p.PermissionType).Where(x => ids.Contains(x.Id)).ToListAsync();
            return await _context.Permission.Include(p => p.PermissionType).ToListAsync();
        }
    }
}
