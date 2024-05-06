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
        private IUnitOfWork _unitOfWork;

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
            AppConnTenant appConnTenant = null;
            try
            {
                foreach (AppConnTenant item in _unitOfWork.TSAppTenant)
                {
                    if (item.Name.Equals(appConnectionName))
                    {
                        appConnTenant = item;
                    }
                }
                return sqlTransaction(appConnTenant.Connection);
            }
            catch (TimeoutException innerException)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", innerException);
            }
        }

        protected async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> sqlTransaction, IUnitOfWork unitOfWork, AppConnNameEnum appConnectionName)
        {
            _unitOfWork = unitOfWork ?? _unitOfWork;
            AppConnTenant appConnTenant = null;
            try
            {
                foreach (AppConnTenant item in _unitOfWork.TSAppTenant)
                {
                    if (item.Name.Equals(appConnectionName))
                    {
                        appConnTenant = item;
                    }
                }
                return await sqlTransaction(appConnTenant.Connection);
            }
            catch (TimeoutException innerException)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", innerException);
            }
        }
    }

}
