using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TownSuite.CoreDataAccess.Interfaces;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreDataAccess
{
    public abstract class BaseRepository
    {
        IUnitOfWork _unitOfWork = null;

        public BaseRepository(IUnitOfWork unitOfWork = null)
        {
            _unitOfWork = unitOfWork ?? null;
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected T WithConnection<T>(Func<IDbConnection, T> sqlTransaction, IUnitOfWork unitOfWork, AppConnNameEnum appConnectionName)
        {
            _unitOfWork = unitOfWork ?? _unitOfWork;
            AppConnTenant connectedAppTenant = null;
            try
            {
                foreach(AppConnTenant appTenant in _unitOfWork.TSAppTenant)
                {
                    if (appTenant.Name.Equals(appConnectionName))
                        connectedAppTenant = appTenant;
                }
                return sqlTransaction(connectedAppTenant.Connection);
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
        }

        protected async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> sqlTransaction, IUnitOfWork unitOfWork, AppConnNameEnum appConnectionName)
        {
            _unitOfWork = unitOfWork ?? _unitOfWork;
            AppConnTenant connectedAppTenant = null;
            try
            {
                foreach (AppConnTenant appTenant in _unitOfWork.TSAppTenant)
                {
                    if (appTenant.Name.Equals(appConnectionName))
                        connectedAppTenant = appTenant;
                }
                return await sqlTransaction(connectedAppTenant.Connection);
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
        }

    }
}
