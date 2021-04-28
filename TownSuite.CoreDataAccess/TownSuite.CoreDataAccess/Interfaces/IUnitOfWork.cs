using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreDataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }    
        IEnumerable<AppConnTenant> TSAppTenant { get; }
        IDbTransaction Transaction(AppConnNameEnum appConnName);
        void Begin(IEnumerable<AppConnNameEnum> appConnNames);
        void Commit();
        void Rollback();
    }
}
