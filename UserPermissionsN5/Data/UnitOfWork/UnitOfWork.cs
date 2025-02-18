using UserPermissionsN5.Data.Repositories;
using UserPermissionsN5.Models;

namespace UserPermissionsN5.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IPermissionRepository PermissionsRepository { get; }
        public IRepository<PermissionType> PermissionTypes { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            PermissionsRepository = new PermissionRepository(_context);
            PermissionTypes = new Repository<PermissionType>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
