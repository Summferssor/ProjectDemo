using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BCM.Models.BCM.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        bool Save();
        bool Save(bool acceptAllChangesOnSuccess);
        Task<bool> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
