using UserPermissionsN5.Models;

namespace UserPermissionsN5.Data.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<IEnumerable<Permission>> GetPermissionsWithTypesAsync();
    }
}
