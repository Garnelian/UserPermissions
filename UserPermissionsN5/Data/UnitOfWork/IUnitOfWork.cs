﻿using Nest;
using UserPermissionsN5.Data.Repositories;
using UserPermissionsN5.Models;

namespace UserPermissionsN5.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IPermissionRepository PermissionsRepository { get; }
        Repositories.IRepository<PermissionType> PermissionTypes { get; }
        Task<int> CompleteAsync();
    }

}
